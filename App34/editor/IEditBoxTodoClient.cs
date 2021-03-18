using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App34
{
    public interface IEditBoxTodoClient
    {
        Task<string> CreateTodoAsync(string title);
        Task CompleteTodoAsync(string taskId);
        Task UncompleteTodoAsync(string taskId);
        Task UpdateTodoTitleAsync(string taskId, string title);
        Task DeleteTodoAsync(string taskId);

        Task<TodoTask> GetTaskAsync(string taskId);
    }
}
