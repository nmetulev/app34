using App34.Helpers.RoamingSettings;
using Microsoft.Toolkit.Graph.Providers;
using System.Threading.Tasks;
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

            ProviderManager.Instance.ProviderUpdated += Instance_ProviderUpdated;
        }

        private async void Instance_ProviderUpdated(object sender, ProviderUpdatedEventArgs e)
        {
            if (e.Reason == ProviderManagerChangedState.ProviderStateChanged)
            {
                if (ProviderManager.Instance.GlobalProvider is IProvider provider && provider.State == ProviderState.SignedIn) 
                {
                    await TestFlow();
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
