using Microsoft.Toolkit.Graph.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App34
{
    class EditBoxTodoClient : IEditBoxTodoClient
    {
        private string _listId;

        private EditBoxTodoClient() { }

        public static async Task<EditBoxTodoClient> CreateAsync()
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

            return new EditBoxTodoClient()
            {
                _listId = todoTaskList.Id
            };
        }

        public async Task CompleteTodoAsync(string taskId)
        {
            var provider = ProviderManager.Instance.GlobalProvider;
            var todoTask = new Microsoft.Graph.TodoTask
            {
                ODataType = null,
                Status = Microsoft.Graph.TaskStatus.Completed
            };
            await provider.Graph.Me.Todo.Lists[this._listId].Tasks[taskId].Request().UpdateAsync(todoTask);
        }

        public async Task<string> CreateTodoAsync(string title)
        {
            var provider = ProviderManager.Instance.GlobalProvider;

            var todoTask = new Microsoft.Graph.TodoTask
            {
                ODataType = null,
                Title = title,
            };
            todoTask = await provider.Graph.Me.Todo.Lists[this._listId].Tasks
            .Request()
            .AddAsync(todoTask);

            return todoTask.Id;
        }

        public async Task DeleteTodoAsync(string taskId)
        {
            var provider = ProviderManager.Instance.GlobalProvider;
            await provider.Graph.Me.Todo.Lists[this._listId].Tasks[taskId]
                .Request()
                .DeleteAsync();
        }

        public async Task UncompleteTodoAsync(string taskId)
        {
            var provider = ProviderManager.Instance.GlobalProvider;
            var todoTask = new Microsoft.Graph.TodoTask
            {
                ODataType = null,
                Status = Microsoft.Graph.TaskStatus.NotStarted
            };
            await provider.Graph.Me.Todo.Lists[this._listId].Tasks[taskId].Request().UpdateAsync(todoTask);
        }

        public async Task UpdateTodoTitleAsync(string taskId, string title)
        {
            var provider = ProviderManager.Instance.GlobalProvider;
            var todoTask = new Microsoft.Graph.TodoTask
            {
                ODataType = null,
                Title = title
            };
            await provider.Graph.Me.Todo.Lists[this._listId].Tasks[taskId].Request().UpdateAsync(todoTask);
        }
    }
}
