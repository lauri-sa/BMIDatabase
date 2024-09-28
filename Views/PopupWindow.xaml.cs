using BMIDatabase.ViewModels;
using System.Windows;

namespace BMIDatabase.Views
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {
        public PopupWindow()
        {
            InitializeComponent();
            this.DataContext = new PopupWindowViewModel();
        }
    }
}
