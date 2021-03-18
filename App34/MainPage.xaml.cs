using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace App34
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            _ = BindTodoClient();
        }

        public async Task BindTodoClient() {
            myEditBox.TodoClient = await EditBoxTodoClient.CreateAsync();
        }

        private void SaveButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            vm.Text = myEditBox.GetTextFromContent();

            vm.SaveCommand.Execute(null);
        }
    }
}
