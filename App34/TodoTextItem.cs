// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using System.Diagnostics;
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
            return false;
        }

        public async Task<bool> InitFromGraphId(string id)
        {
            return false;
        }

        public async Task<bool> CreateGraphTodoItem()
        {
            return false;
        }

        public string ToDoId { get; set; }

        public Microsoft.Graph.TodoTask GraphTask { get; set; }
    }
}
