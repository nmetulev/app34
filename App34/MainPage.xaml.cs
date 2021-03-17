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
        public MainPage()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }

    }
}
