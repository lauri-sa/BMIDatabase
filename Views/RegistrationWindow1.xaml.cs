using BMIDatabase.ViewModels;
using System.Windows;

namespace BMIDatabase.Views
{
    /// <summary>
    /// Interaction logic for RegistrationWindow1.xaml
    /// </summary>
    public partial class RegistrationWindow1 : Window
    {
        public RegistrationWindow1()
        {
            InitializeComponent();
            this.DataContext = new RegistrationWindow1ViewModel();
        }
    }
}
