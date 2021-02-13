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
using System.Net.Http;
using System.Collections.Specialized;


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
            ProviderManager.Instance.GlobalProvider = WindowsProvider.Create("2fc98686-0464-42a2-ae3e-7f45c8c8257d", new string[] { "User.Read", "Tasks.ReadWrite", "Files.ReadWrite.All" });


            Init();
        }

        private async void Instance_ProviderUpdated(object sender, ProviderUpdatedEventArgs e)
        {
            if (e.Reason == ProviderManagerChangedState.ProviderStateChanged)
            {
                if (ProviderManager.Instance.GlobalProvider is IProvider provider && provider.State == ProviderState.SignedIn) 
                {
                    var user = await provider.Graph.Me.Request().GetAsync();

                    var folder = await OneDriveDataSource.GetOrCreateRootFolder();
                    System.Diagnostics.Debug.WriteLine(folder.Id);
                    await TestFlow();
                    


                }
            }
        }

        private async Task TestFlow()
        {
            Microsoft.Graph.TodoTask todoTask = await CreateTodo("Do hackathon");
            Microsoft.Graph.TodoTask todoTaskCompleted = await CompleteTodo(todoTask.Id);
        }

        private async Task Init()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///test.txt"));
            var text = await FileIO.ReadTextAsync(file);

            myEditBox.Text = text;

        }

        private async Task<Microsoft.Graph.TodoTaskList> CreateTodoList()
        {
            var provider = ProviderManager.Instance.GlobalProvider;
            Microsoft.Graph.TodoTaskList list = null;
            var existingLists = await provider.Graph.Me.Todo.Lists.Request().Filter("displayName eq 'MyAwesomeNotesApp'").GetAsync();
            if (existingLists.Count == 0)
            {
                //var list = await provider.Graph.Me.Todo.Lists.Request().AddAsync(new Microsoft.Graph.TodoTaskList { DisplayName = "MyAwesomeNotesApp" });
                string requestUrl = provider.Graph.Me.Todo.Lists.Request().RequestUrl;
                string json = "{\"displayName\": \"MyAwesomeNotesApp\"}";
                HttpRequestMessage hrm = new HttpRequestMessage(HttpMethod.Post, requestUrl);
                hrm.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                await provider.Graph.AuthenticationProvider.AuthenticateRequestAsync(hrm);
                HttpResponseMessage response = await provider.Graph.HttpProvider.SendAsync(hrm);
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize into OneNotePage object.
                    var content = await response.Content.ReadAsStringAsync();
                    list = provider.Graph.HttpProvider.Serializer.DeserializeObject<Microsoft.Graph.TodoTaskList>(content);
                }
            }
            else
            {
                list = existingLists[0];
            }
            return list;
        }
        private async Task<Microsoft.Graph.TodoTask> CreateTodo(string task)
        {
            var provider = ProviderManager.Instance.GlobalProvider;
            Microsoft.Graph.TodoTaskList todoList = await CreateTodoList();
            Microsoft.Graph.TodoTask todoTask = null;
            var requestUrl = provider.Graph.Me.Todo.Lists[todoList.Id].Tasks.Request().RequestUrl;
            string taskjson = "{\"title\": \"" + task + "\"}";
            HttpRequestMessage hrm = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            hrm.Content = new StringContent(taskjson, System.Text.Encoding.UTF8, "application/json");
            await provider.Graph.AuthenticationProvider.AuthenticateRequestAsync(hrm);
            HttpResponseMessage response = await provider.Graph.HttpProvider.SendAsync(hrm);
            if (response.IsSuccessStatusCode)
            {
                // Deserialize into OneNotePage object.
                var content = await response.Content.ReadAsStringAsync();
                todoTask = provider.Graph.HttpProvider.Serializer.DeserializeObject<Microsoft.Graph.TodoTask>(content);
            }
            return todoTask;
        }

        private async Task<Microsoft.Graph.TodoTask> CompleteTodo(string taskId)
        {
            var provider = ProviderManager.Instance.GlobalProvider;
            Microsoft.Graph.TodoTask todoTask = null;
            Microsoft.Graph.TodoTaskList todoList = await CreateTodoList();
            var requestUrl = provider.Graph.Me.Todo.Lists[todoList.Id].Tasks[taskId].Request().RequestUrl;
            string updatejson = "{\"status\": \"completed\"}";
            HttpRequestMessage hrm = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            hrm.Content = new StringContent(updatejson, System.Text.Encoding.UTF8, "application/json");
            await provider.Graph.AuthenticationProvider.AuthenticateRequestAsync(hrm);
            HttpResponseMessage response = await provider.Graph.HttpProvider.SendAsync(hrm);
            if (response.IsSuccessStatusCode)
            {
                // Deserialize into OneNotePage object.
                var content = await response.Content.ReadAsStringAsync();
                todoTask = provider.Graph.HttpProvider.Serializer.DeserializeObject<Microsoft.Graph.TodoTask>(content);
            }
            return todoTask;
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
