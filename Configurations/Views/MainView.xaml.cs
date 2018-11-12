using System.Windows;

using Configurations.Services;
using Configurations.ViewModel;


namespace Configurations.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            this.DataContext = new MainViewViewModel(this, new ConfigurationService());
        }
    }
}
