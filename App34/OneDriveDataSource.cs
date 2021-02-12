using Microsoft.Graph;
using Microsoft.Toolkit.Graph.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App34
{

    /// <summary>
    /// CRUD operations for interacting with OneDrive
    /// </summary>
    public static class OneDriveDataSource
    {
        private static DriveItem defaultRootFolder = new DriveItem
        {
            Name = "My Awesome Notes App",
            Folder = new Folder(),
            AdditionalData = new Dictionary<string, object>()
            {
                { "@microsoft.graph.conflictBehavior", "rename" }
            }
        };


        private static GraphServiceClient _graph => ProviderManager.Instance.GlobalProvider.Graph;


        //public static async Task<DriveItem> GetRootFolder()
        //{
        //    DriveItem rootFolder;
        //    try
        //    {
        //        rootFolder = await _graph.Me.Drive.Root.Children[rootFolderId].Request().GetAsync();
        //    }
        //    catch
        //    {
        //        // TODO: Create the appRootFolder
        //        rootFolder = await CreateRootFolder(defaultRootFolder);
        //    }

        //    return rootFolder;
        //}

        public static async Task<DriveItem> GetOrCreateRootFolder()
        {
            DriveItem rootFolder;

            try
            {
                rootFolder = await _graph.Me.Drive.Root.Children.Request().AddAsync(defaultRootFolder);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Failed to get or create root folder" + e.Message);
                rootFolder = null;
            }

            return rootFolder;

        }

        public static async Task<DriveItem> CreateFileInRoot(DriveItem newItem, string fileId)
        {
            //DriveItem item = await _graph.Me.Drive.Special.AppRoot.Children[fileId].Request().CreateAsync(newItem);
            //return item;
            return null;
        }

        public static async void GetFileFromAppRoot(string fileId)
        {
            //string folderName = "";
            //var appRoot = await _graph.Me.Drive.Special.AppRoot.Children[folderName].Children[fileId];

            
        }
    }
}
