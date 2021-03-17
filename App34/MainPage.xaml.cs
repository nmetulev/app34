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
using App34.Helpers.RoamingSettings;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

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
            this.InitializeComponent();

            ProviderManager.Instance.ProviderUpdated += Instance_ProviderUpdated;
            ProviderManager.Instance.GlobalProvider = WindowsProvider.Create("2fc98686-0464-42a2-ae3e-7f45c8c8257d", new string[] { "User.Read", "Tasks.ReadWrite", "Files.ReadWrite" });

            Init();
        }

        private async void Instance_ProviderUpdated(object sender, ProviderUpdatedEventArgs e)
        {
            if (e.Reason == ProviderManagerChangedState.ProviderStateChanged)
            {
                if (ProviderManager.Instance.GlobalProvider is IProvider provider && provider.State == ProviderState.SignedIn) 
                {
<<<<<<< HEAD
                    var user = await provider.Graph.Me.Request().GetAsync();
                    await TestFlow();
=======
                    _roamingSettings = await RoamingSettingsHelper.CreateForCurrentUser(RoamingDataStore.OneDrive);

                    //await TestFlow();
>>>>>>> f5e861c663acf65b1799eab2de15c5cf4c59e774
                }
            }
        }

        private async Task TestFlow()
        {
            Microsoft.Graph.TodoTaskList todoTaskList = await CreateTodoList();
            Microsoft.Graph.TodoTask todoTask = await CreateTodo("Do hackathon", todoTaskList.Id);
            Microsoft.Graph.TodoTask todoTaskCompleted = await CompleteTodo(todoTask.Id, todoTaskList.Id);
            Microsoft.Graph.TodoTask todoTaskUncompleted = await UncompleteTodo(todoTask.Id, todoTaskList.Id);
            Microsoft.Graph.TodoTask todoTaskTitleChanged = await UpdateTodoTitle("Changed title", todoTask.Id, todoTaskList.Id);
            DeleteTodo(todoTask.Id, todoTaskList.Id);
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
            Microsoft.Graph.TodoTaskList todoTaskList = null;
            var existingLists = await provider.Graph.Me.Todo.Lists.Request().Filter("displayName eq 'MyAwesomeNotesApp'").GetAsync();
            if (existingLists.Count == 0)
            {
                var list = new Microsoft.Graph.TodoTaskList
                {
                    ODataType = null,
                    DisplayName = "MyAwesomeNotesApp"
                };

                todoTaskList = await provider.Graph.Me.Todo.Lists
                    .Request()
                    .AddAsync(list);
            }
            else
            {
                todoTaskList = existingLists[0];
            }
            return todoTaskList;
        }
        private async Task<Microsoft.Graph.TodoTask> CreateTodo(string task, string todoListId)
        {
            var provider = ProviderManager.Instance.GlobalProvider;
 
            var todoTask = new Microsoft.Graph.TodoTask
            {
                ODataType = null,
                Title = task,         
            };
            todoTask = await provider.Graph.Me.Todo.Lists[todoListId].Tasks
            .Request()
            .AddAsync(todoTask);
            return todoTask;
        }

        private async Task<Microsoft.Graph.TodoTask> CompleteTodo(string taskId, string todoListId)
        {
            var provider = ProviderManager.Instance.GlobalProvider;
            var todoTask = new Microsoft.Graph.TodoTask
            {
                ODataType = null,
                Status = Microsoft.Graph.TaskStatus.Completed
            };
            todoTask = await provider.Graph.Me.Todo.Lists[todoListId].Tasks[taskId].Request().UpdateAsync(todoTask);
            return todoTask;
        }

        private async Task<Microsoft.Graph.TodoTask> UncompleteTodo(string taskId, string todoListId)
        {
            var provider = ProviderManager.Instance.GlobalProvider;
            var todoTask = new Microsoft.Graph.TodoTask
            {
                ODataType = null,
                Status = Microsoft.Graph.TaskStatus.NotStarted
            };
            todoTask = await provider.Graph.Me.Todo.Lists[todoListId].Tasks[taskId].Request().UpdateAsync(todoTask);
            return todoTask;
        }

        private async Task<Microsoft.Graph.TodoTask> UpdateTodoTitle(string title, string taskId, string todoListId)
        {
            var provider = ProviderManager.Instance.GlobalProvider;
            var todoTask = new Microsoft.Graph.TodoTask
            {
                ODataType = null,
                Title = title
            };
            todoTask = await provider.Graph.Me.Todo.Lists[todoListId].Tasks[taskId].Request().UpdateAsync(todoTask);
            return todoTask;
        }

        private async void DeleteTodo(string taskId, string todoListId)
        {
            var provider = ProviderManager.Instance.GlobalProvider;
            await provider.Graph.Me.Todo.Lists[todoListId].Tasks[taskId]
            .Request()
            .DeleteAsync();

        }
    }
}
