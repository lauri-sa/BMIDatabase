using BMIDatabase.ViewModels;
using System.Windows;

namespace BMIDatabase.Views
{
    /// <summary>
    /// Interaction logic for LoggedInWindow.xaml
    /// </summary>
    public partial class LoggedInWindow : Window
    {
        public LoggedInWindow()
        {
            InitializeComponent();
            this.DataContext = new LoggedInWindowViewModel();
        }
    }
}