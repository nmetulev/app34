using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.Threading.Tasks;
using Markdig;
using Windows.UI.Text.Core;
using Windows.UI.Core;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Xaml.Documents;
using Microsoft.Toolkit.Graph.Providers.Uwp;
using Microsoft.Toolkit.Graph.Providers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App34
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        

        public MainPage()
        {
            this.InitializeComponent();

            ProviderManager.Instance.ProviderUpdated += Instance_ProviderUpdated;
            ProviderManager.Instance.GlobalProvider = WindowsProvider.Create("2fc98686-0464-42a2-ae3e-7f45c8c8257d", new string[] { "User.Read", "Tasks.ReadWrite", "Files.ReadWrite.All", "Sites.ReadWrite.All" });


            Init();
        }

        private async void Instance_ProviderUpdated(object sender, ProviderUpdatedEventArgs e)
        {
            if (e.Reason == ProviderManagerChangedState.ProviderStateChanged)
            {
                if (ProviderManager.Instance.GlobalProvider is IProvider provider && provider.State == ProviderState.SignedIn) 
                {
                    var user = await provider.Graph.Me.Request().GetAsync();

                    var folder = await OneDriveDataSource.GetRootFolder();
                    System.Diagnostics.Debug.WriteLine(folder.Id);
                }
            }
        }

        private async Task Init()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///test.txt"));
            var text = await FileIO.ReadTextAsync(file);

            myEditBox.Text = text;
        }

        private void CreateTodo()
        {

        }

        private void CompleteTodo()
        {

        }

        private void UncompleteTodo()
        {

        }

        private void UpdateTodoTitle()
        {

        }

        private void DeleteTodo()
        {

        }
    }
}
