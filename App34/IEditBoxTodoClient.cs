using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App34
{
    interface IEditBoxTodoClient
    {
        Task<string> CreateTodoAsync(string title);
        Task CompleteTodoAsync(string taskId);
        Task UncompleteTodoAsync(string taskId);
        Task UpdateTodoTitleAsync(string taskId, string title);
        Task DeleteTodoAsync(string taskId);
    }
}
