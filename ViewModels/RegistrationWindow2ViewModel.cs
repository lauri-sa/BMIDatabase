using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace BMIDatabase.ViewModels
{
    // Toisen rekisteröinti-ikkunan ns. kontrolliluokka joka huolehtii esim. käyttöliittymään syötetyn datan
    // sekä eventtien käsittelystä
    internal class RegistrationWindow2ViewModel : BaseViewModel
    {
        public string Label1 { get; set; }
        public string Label2 { get; set; }
        public string Label3 { get; set; }
        public string Button1 { get; set; }
        public string Button2 { get; set; }

        private List<string> errorBoxList = new List<string>() { "FirstNameError", "LastNameError", "HeightError" };

        public ICommand ReturnToRegistrationWindow1Command => new DelegateCommand(ReturnToRegistrationWindow1);
        public ICommand RegisterUserCommand => new DelegateCommand(RegisterUser);

        public RegistrationWindow2ViewModel()
        {
            this.Label1 = "Etunimi";
            this.Label2 = "Sukunimi";
            this.Label3 = "Pituus (cm)";
            this.Button1 = "Rekisteröidy";
            this.Button2 = "Palaa Takaisin";
        }

        // Metodi joka hoitaa syötteentarkistuksen ja syötteen virheilmoituksien näyttämisen
        private bool InputValidation(object[] values)
        {
            int i;
            bool validInput = true;
            TextBlock errorBox = new TextBlock();

            for (i = 0; i < values.Length - 1; i++)
            {
                string errorText = string.Empty;
                errorBox = (TextBlock)App.Current.Windows[App.Current.Windows.Count - 1].FindName(errorBoxList[i]);

                if (!Methods.StringTarkastus((string)values[i], out errorText))
                {
                    validInput = false;
                    errorBox.Text = errorText;
                }
                else
                {
                    errorBox.Text = string.Empty;
                }
            }

            errorBox = (TextBlock)App.Current.Windows[App.Current.Windows.Count - 1].FindName(errorBoxList[i]);

            if ((string)values[i] == string.Empty)
            {
                validInput = false;
                errorBox.Text = "Kenttä ei voi olla tyhjä";
            }
            else if (!int.TryParse((string)values[i], out int result))
            {
                validInput = false;
                errorBox.Text = "Pituus kokonaislukuna";
            }
            else if (int.Parse((string)values[i]) < 30 || int.Parse((string)values[i]) > 250)
            {
                validInput = false;
                errorBox.Text = "Pituus väliltä 30 ja 250";
            }
            else
            {
                errorBox.Text = string.Empty;
            }

            return validInput;
        }

        // Tekee syötteentarkistuksen ja kutsuu metodia joka tallentaa käyttäjän tietokantaan
        private void RegisterUser(object parameter)
        {
            var values = (object[])parameter;

            if (InputValidation(values))
            {
                DataHandler.UserRegistrationHandler2(values);
            }
        }

        // Sulkee tämän ikkunan
        private void ReturnToRegistrationWindow1(object parameter)
        {
            App.Current.Windows[App.Current.Windows.Count - 1].Close();
        }
    }
}