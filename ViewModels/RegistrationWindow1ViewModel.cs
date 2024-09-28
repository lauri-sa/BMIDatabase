using BMIDatabase.Views;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace BMIDatabase.ViewModels
{
    // Ensimmäisen rekisteröinti-ikkunan ns. kontrolliluokka joka huolehtii esim. käyttöliittymään syötetyn datan
    // sekä eventtien käsittelystä
    internal class RegistrationWindow1ViewModel : BaseViewModel
    {
        public string Label1 { get; set; }
        public string Label2 { get; set; }
        public string Label3 { get; set; }
        public string Button1 { get; set; }
        public string Button2 { get; set; }

        private List<string> textBoxList = new List<string>() { "Password1", "Password2" };
        private List<string> errorBoxList = new List<string>() { "UserNameError", "Password1Error", "Password2Error"};

        public ICommand ReturnToMainWindowCommand => new DelegateCommand(ReturnToMainWindow);
        public ICommand ToRegistrationWindow2Command => new DelegateCommand(ToRegistrationWindow2);

        public RegistrationWindow1ViewModel()
        {
            this.Label1 = "Käyttäjätunnus";
            this.Label2 = "Salasana";
            this.Label3 = "Toista salasana";
            this.Button1 = "Seuraava";
            this.Button2 = "Palaa Takaisin";
        }

        // Metodi joka hoitaa syötteentarkistuksen ja syötteen virheilmoituksien näyttämisen
        private bool InputValidation(object[] values)
        {
            bool validInput = true;
            TextBlock errorBox = new TextBlock();      

            for (int i = 0; i < values.Length; i++)
            {
                errorBox = (TextBlock)App.Current.Windows[App.Current.Windows.Count - 1].FindName(errorBoxList[i]);

                if (string.IsNullOrWhiteSpace((string)values[i]))
                {
                    validInput = false;
                    errorBox.Text = "Kenttä ei voi olla tyhjä";
                }
                else
                {
                    errorBox.Text = string.Empty;
                }
            }

            if (validInput)
            {
                if ((string)values[1] != (string)values[2])
                {
                    validInput = false;
                    errorBox.Text = "Salasana on väärin";
                }
                else if (DataHandler.CheckUserFromDataBase((string)values[0]))
                {
                    validInput = false;
                    errorBox = (TextBlock)App.Current.Windows[App.Current.Windows.Count - 1].FindName(errorBoxList[0]);
                    errorBox.Text = "Varattu käyttäjätunnus";
                }
            }

            return validInput;
        }

        // Tyhjentää tekstikentät
        private void EmptyTextFields()
        {
            for (int i = 0; i < textBoxList.Count; i++)
            {
                var textBox = (TextBox)App.Current.Windows[App.Current.Windows.Count - 1].FindName(textBoxList[i]);
                textBox.Text = string.Empty;
            }
        }

        //Kutsuu syötteentarkistusmetodia, luo seuraavan ikkunan,
        //kutsuu metodia joka tallentaa tämän ikkunan datan väliaikaiseen listaan
        //ja aukaisee luodun uuden ikkunan
        private void ToRegistrationWindow2(object parameter)
        {
            var values = (object[])parameter;

            values[1] = Methods.Encrypt((string)values[1]);
            values[2] = Methods.Encrypt((string)values[2]);

            if (InputValidation(values))
            {
                DataHandler.UserRegistrationHandler1(values);
                EmptyTextFields();
                var regWin2 = new RegistrationWindow2();
                regWin2.ShowDialog();
            }
        }

        // Sulkee tämän ikkunan
        private void ReturnToMainWindow(object parameter)
        {
            App.Current.Windows[App.Current.Windows.Count - 1].Close();
        }
    }
}