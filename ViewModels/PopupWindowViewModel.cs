using System.Windows;
using System.Windows.Input;

namespace BMIDatabase.ViewModels
{
    // Popup-ikkunan kontrolliluokka
    internal class PopupWindowViewModel : BaseViewModel
    {
        public string PopupMessage1 { get; set; }
        public string PopupMessage2 { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Button { get; set; }

        public ICommand OKButtonCommand => new DelegateCommand(OKButton);

        public PopupWindowViewModel()
        {
            this.Button = "OK";
            this.Width = 200;
            this.Height = 30.62;
            this.PopupMessage1 = "Käyttäjä lisätty";
            this.PopupMessage2 = "onnistuneesti";
        }

        // Sulkee kaikki paitsi pääikkunan
        private void OKButton(object parameter)
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window != App.Current.MainWindow)
                {
                    window.Close();
                }
            }
        }
    }
}