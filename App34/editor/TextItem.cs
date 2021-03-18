// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App34
{
    public class TextItem
    {
        private string _text;

        public virtual string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
