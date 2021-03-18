// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using Microsoft.Graph;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

namespace App34
{

    public enum TodoItemState
    {
        NotCreated,
        Created,
        Loading
    }

    public class TodoTextItem : TextItem
    {
        private TodoItemState _state = TodoItemState.NotCreated;
        private bool _isCompleted = false;
        private bool _isDirty = false;

        public override string Text
        {
            get { return base.Text; }
            set
            {
                _isDirty = true;
                Debug.WriteLine("isDirty");
                base.Text = value;

            }
        }

        public TodoItemState GetState()
        {
            return _state;
        }

        public bool GetIsCompleted()
        {
            return _isCompleted;
        }

        public async Task<bool> Update(IEditBoxTodoClient editBoxTodoClient)
        {
            // todo: semaphore
            if (this._state == TodoItemState.NotCreated)
            {
                this.CreateGraphTodoItem(editBoxTodoClient);
            }
            else if (this._isDirty)
            {
                Debug.WriteLine("need to update");
            }

            return false;
        }

        public async Task<bool> InitFromGraphId(string id)
        {
            return false;
        }

        public string ToDoId { get; set; }

        public async Task<bool> CreateGraphTodoItem(IEditBoxTodoClient editBoxTodoClient)
        {
            this._state = TodoItemState.Loading;
            Debug.WriteLine("need to create todo");
            ToDoId = await editBoxTodoClient.CreateTodoAsync(this.Text);
            this._state = TodoItemState.Created;
            this._isDirty = false;
            return false;
        }
        public override string ToString()
        {
            return "todo:" + this.ToDoId;
        }

        //Get TODO by ID and create TodoTextItem
        public static TodoTextItem Create(string text, IEditBoxTodoClient TodoClient)
        {
            // there should probably be some validation here
            var id = text.Trim().Substring(5);
            TodoTextItem todoTextItem = new TodoTextItem 
            {
                Text = "Loading",
                _state = TodoItemState.Loading
            };

            var TodoItem = new Task(async () => 
            { 
                TodoTask todoTask = await TodoClient.GetTaskAsync(id);
                todoTextItem.Text = todoTask.Title;
                todoTextItem._state = TodoItemState.Created;
            });
            TodoItem.Start();
            //TodoClient.GetTaskAsync(id);

            //todo: take id and fetch from graph if there, set state to loading while fetching

            return todoTextItem;
        }

        struct MiniTodoModel
        {
            public string title { get; set; }
            public string id { get; set; }
        }
    }


}
