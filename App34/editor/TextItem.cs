// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace App34
{
    public class TextItem : ObservableObject
    {
        private string _text = string.Empty;

        public virtual string Text
        {
            get { return _text; }
            set => SetProperty(ref _text, value);
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
