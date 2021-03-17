using App34.Helpers.RoamingSettings;
using Microsoft.Toolkit.Graph.Providers;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace App34
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public RoamingSettingsHelper _roamingSettings;

        public MainPage()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }

        private async Task TestFlow()
        {
            Microsoft.Graph.TodoTask todoTask = await CreateTodo("Do hackathon");
            Microsoft.Graph.TodoTask todoTaskCompleted = await CompleteTodo(todoTask.Id);
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
