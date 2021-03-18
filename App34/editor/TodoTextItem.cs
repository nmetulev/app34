// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

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

        public async Task<bool> Update()
        {
            // todo: semaphore
            if (this._state == TodoItemState.NotCreated)
            {
                this.CreateGraphTodoItem();
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

        public async Task<bool> CreateGraphTodoItem()
        {
            this._state = TodoItemState.Loading;
            Debug.WriteLine("need to create todo");
            this._state = TodoItemState.Created;
            this._isDirty = false;
            return false;
        }

        public string ToDoId { get; set; }

        public override string ToString()
        {
            var json = JsonSerializer.Serialize<MiniTodoModel>(new MiniTodoModel()
            {
                title = this.Text,
                id = this.ToDoId
            });


            return "todo:" + json;
        }

        public static TodoTextItem Create(string text)
        {
            // there should probably be some validation here
            var json = text.Trim().Substring(5);
            var item = JsonSerializer.Deserialize<MiniTodoModel>(json);

            //todo: take id and fetch from graph if there, set state to loading while fetching

            return new TodoTextItem()
            {
                Text = item.title
            };
        }

        struct MiniTodoModel
        {
            public string title { get; set; }
            public string id { get; set; }
        }
    }


}
