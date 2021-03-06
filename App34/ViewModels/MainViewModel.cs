﻿using App34.Helpers.RoamingSettings;
using Microsoft.Toolkit.Graph.Providers;
using Microsoft.Toolkit.Graph.Providers.Uwp;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace App34
{
    // TODO: Move to .NET Standard project?
    public class MainViewModel : ObservableObject
    {
        const string NotesFileName = "my_notes.json";

        public ICommand SaveCommand { get; }

        private string _text;
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        private RoamingSettingsHelper _roamingSettings;

        public MainViewModel()
        {
            SaveCommand = new RelayCommand(Save);

            // TODO: Abstract to a service injected into view model
            ProviderManager.Instance.ProviderUpdated += OnProviderUpdated;
            ProviderManager.Instance.GlobalProvider = WindowsProvider.Create("2fc98686-0464-42a2-ae3e-7f45c8c8257d", new string[] { "User.Read", "Tasks.ReadWrite", "Files.ReadWrite" });
        }

        private async void Save()
        {
            if (_roamingSettings != null)
            {
                await _roamingSettings.SaveFileAsync(NotesFileName, Text);
            }
        }

        private void OnProviderUpdated(object sender, ProviderUpdatedEventArgs e)
        {
            if (sender is ProviderManager providerManager && providerManager.GlobalProvider?.State == ProviderState.SignedIn)
            {
                Load();
            }
            else
            {
                Clear();
            }
        }

        private async void Load()
        {
            _roamingSettings = await RoamingSettingsHelper.CreateForCurrentUser(RoamingDataStore.OneDrive);

            bool notesExist = await _roamingSettings.FileExistsAsync(NotesFileName);

            Text = notesExist
                ? await _roamingSettings.ReadFileAsync(NotesFileName)
                : string.Empty;
        }

        private void Clear()
        {
            _roamingSettings = null;
            Text = null;
        }
    }
}
