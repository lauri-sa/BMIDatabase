using BMIDatabase.ViewModels;
using System.Windows;

namespace BMIDatabase.Views
{
    /// <summary>
    /// Interaction logic for RegistrationWindow2.xaml
    /// </summary>
    public partial class RegistrationWindow2 : Window
    {
        public RegistrationWindow2()
        {
            InitializeComponent();
            this.DataContext = new RegistrationWindow2ViewModel();
        }
    }
}
