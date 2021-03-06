﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App34.Common;
using Microsoft.Toolkit.Graph.Providers;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.Storage;

namespace App34.Helpers.RoamingSettings
{
    /// <summary>
    /// An enumeration of the available data storage methods for roaming data.
    /// </summary>
    public enum RoamingDataStore
    {
        /// <summary>
        /// Store data using open extensions on the Graph User.
        /// </summary>
        UserExtensions,

        /// <summary>
        /// Store data in files in the user's OneDrive AppRoot folder.
        /// </summary>
        OneDrive,
    }

    /// <summary>
    /// A helper class for syncing data to roaming data store.
    /// </summary>
    public class RoamingSettingsHelper : IRoamingSettingsDataStore
    {
        //public string Name => DataStore.Name;

        /// <summary>
        /// Gets the internal data storage helper instance.
        /// </summary>
        public IRoamingSettingsDataStore DataStore { get; private set; }

        /// <inheritdoc />
        public IDictionary<string, object> Settings => DataStore?.Settings;

        /// <summary>
        /// Creates a new RoamingSettingsHelper instance for the currently signed in user.
        /// </summary>
        /// <param name="dataStore">Which specific data store is being used.</param>
        /// <param name="autoSync">Whether the values should immediately sync or not.</param>
        /// <param name="serializer">An object serializer for serialization of objects in the data store.</param>
        /// <returns>A new instance of the RoamingSettingsHelper configured for the current user.</returns>
        public static async Task<RoamingSettingsHelper> CreateForCurrentUser(RoamingDataStore dataStore = RoamingDataStore.UserExtensions, bool autoSync = true, IObjectSerializer serializer = null)
        {
            var provider = ProviderManager.Instance.GlobalProvider;
            if (provider == null || provider.State != ProviderState.SignedIn)
            {
                throw new InvalidOperationException("The GlobalProvider must be set and signed in to create a new RoamingSettingsHelper for the current user.");
            }

            var me = await provider.Graph.Me.Request().GetAsync();
            return new RoamingSettingsHelper(me.Id, dataStore, autoSync, serializer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoamingSettingsHelper"/> class.
        /// </summary>
        /// <param name="userId">The id of the target Graph User.</param>
        /// <param name="dataStore">Which specific data store is being used.</param>
        /// <param name="autoSync">Whether the values should immediately sync or not.</param>
        /// <param name="serializer">An object serializer for serialization of objects in the data store.</param>
        public RoamingSettingsHelper(string userId, RoamingDataStore dataStore = RoamingDataStore.UserExtensions, bool autoSync = true, IObjectSerializer serializer = null)
        {
            if (serializer == null)
            {
                serializer = new JsonObjectSerializer();
            }

            // TODO: Infuse unique identifier from Graph registration into the storage name.
            string dataStoreName = "communityToolkit.roamingSettings";

            switch (dataStore)
            {
                case RoamingDataStore.UserExtensions:
                    DataStore = new UserExtensionDataStore(dataStoreName, userId, serializer);
                    break;

                case RoamingDataStore.OneDrive:
                    DataStore = new OneDriveDataStore(dataStoreName + ".json", serializer);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(dataStore));
            }

            if (autoSync)
            {
                try
                {
                    DataStore.Sync();
                }
                catch
                {
                    // Sync may fail if the storage container does not yet exist.
                }
            }
        }

        /// <summary>
        /// An indexer for easily accessing key values.
        /// </summary>
        /// <param name="key">The key for the desired value.</param>
        /// <returns>The value found for the provided key.</returns>
        public object this[string key]
        {
            get => DataStore.Read<object>(key);
            set
            {
                if (DataStore.KeyExists(key))
                {
                    DataStore.Save(key, value);
                }
            }
        }

        /// <inheritdoc />
        public Task<bool> FileExistsAsync(string filePath) => DataStore.FileExistsAsync(filePath);

        /// <inheritdoc />
        public bool KeyExists(string key) => DataStore.KeyExists(key);

        /// <inheritdoc />
        public bool KeyExists(string compositeKey, string key) => DataStore.KeyExists(compositeKey, key);

        /// <inheritdoc />
        public T Read<T>(string key, T @default = default) => DataStore.Read<T>(key, @default);

        /// <inheritdoc />
        public T Read<T>(string compositeKey, string key, T @default = default) => DataStore.Read(compositeKey, key, @default);

        /// <inheritdoc />
        public Task<T> ReadFileAsync<T>(string filePath, T @default = default) => DataStore.ReadFileAsync(filePath, @default);
        
        /// <inheritdoc />
        public Task<string> ReadFileAsync(string filePath, string @default = default) => DataStore.ReadFileAsync(filePath, @default);

        /// <inheritdoc />
        public void Save<T>(string key, T value) => DataStore.Save<T>(key, value);

        /// <inheritdoc />
        public void Save<T>(string compositeKey, IDictionary<string, T> values) => DataStore.Save<T>(compositeKey, values);

        /// <inheritdoc />
        public Task<StorageFile> SaveFileAsync<T>(string filePath, T value) => DataStore.SaveFileAsync<T>(filePath, value);

        /// <summary>
        /// Create a new storage container.
        /// </summary>
        /// <returns>A Task.</returns>
        public Task Create() => DataStore.Create();

        /// <summary>
        /// Delete the existing storage container.
        /// </summary>
        /// <returns>A Task.</returns>
        public Task Delete() => DataStore.Delete();

        /// <summary>
        /// Syncronize the internal cache with the remote storage endpoint.
        /// </summary>
        /// <returns>A Task.</returns>
        public Task Sync() => DataStore.Sync();
    }
}
