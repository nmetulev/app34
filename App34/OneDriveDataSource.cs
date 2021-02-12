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
        private static readonly string AppRootFolderId = "foo";
        
        private static GraphServiceClient _graph => ProviderManager.Instance.GlobalProvider.Graph;


        public static async Task<DriveItem> GetAppRootFolder()
        {
            try
            {
                DriveItem appRootFolder = await _graph.Me.Drive.Special.AppRoot.Children[AppRootFolderId].Request().GetAsync();
            }
            catch
            {
                // TODO: Create the appRootFolder
            }

            return null;
        }

        public static async Task<DriveItem> CreateFileInAppRoot(DriveItem newItem, string fileId)
        {
            DriveItem item = await _graph.Me.Drive.Special.AppRoot.Children[fileId].Request().CreateAsync(newItem);
            return item;
        }

        public static async void GetFileFromAppRoot(string fileId)
        {
            //string folderName = "";
            //var appRoot = await _graph.Me.Drive.Special.AppRoot.Children[folderName].Children[fileId];

            
        }
    }
}
