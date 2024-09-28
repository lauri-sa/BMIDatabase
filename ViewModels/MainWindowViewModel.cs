using BMIDatabase.Views;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

namespace BMIDatabase.ViewModels
{
    // Pääikkunan ns. kontrolliluokka joka huolehtii esim. käyttöliittymään syötetyn datan
    // sekä eventtien käsittelystä
    internal class MainWindowViewModel : BaseViewModel
    {
        public string Label1 { get; set; }
        public string Label2 { get; set; }
        public string Label3 { get; set; }
        public string Button1 { get; set; }
        public string Button2 { get; set; }
        public string Button3 { get; set; }
        public string UserName { get; set; }
        public bool RememberMe { get; set; }

        private List<string> textBoxList = new List<string>() { "UserName", "Password" };
        private List<string> errorBoxList = new List<string>() { "UserNameError", "PasswordError" };

        public ICommand ExitProgramCommand => new DelegateCommand(ExitProgram);
        public ICommand ToRegistrationWindow1Command => new DelegateCommand(ToRegistrationWindow1);
        public ICommand LogInCommand => new DelegateCommand(LogIn);

        public MainWindowViewModel()
        {
            this.Label1 = "Käyttäjätunnus";
            this.Label2 = "Salasana";
            this.Label3 = "Muista minut";
            this.Button1 = "Kirjaudu Sisään";
            this.Button2 = "Luo Käyttäjätunnus";
            this.Button3 = "Sulje Ohjelma";
            SetRememberMeValues();
        }

        // Hoitaa syötteentarkistuksen
        private bool InputValidation(object[] values)
        {
            int i;
            int counter = 0;
            bool validInput = true;
            TextBlock errorBox = new TextBlock();

            for (i = 0; i < values.Length; i++)
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
                if (!DataHandler.CheckUserFromDataBase((string)values[0]))
                {
                    validInput = false;
                    errorBox = (TextBlock)App.Current.Windows[App.Current.Windows.Count - 1].FindName(errorBoxList[0]);
                    errorBox.Text = "Käyttäjätunnusta ei löydy";
                }
                else if (!DataHandler.CheckUserPasswordFromDataBase((string)values[0], (string)values[1], ref counter))
                {
                    validInput = false;
                    errorBox = (TextBlock)App.Current.Windows[App.Current.Windows.Count - 1].FindName(errorBoxList[1]);

                    if (DataHandler.IsAccountLocked((string)values[0]))
                    {
                        errorBox.Text = "Tilisi on lukittu";
                    }
                    else
                    {
                        string message = counter > 1 ? "yritystä" : "yritys";
                        errorBox.Text = $"Väärin, {counter} {message} jäljellä";
                    }
                }
            }

            return validInput;
        }

        // Tallentaa "muista minut" checkboxin arvon ja sen perusteella myös käyttätunnuksen arvon
        private void SaveRememberMeValues(string userName)
        {
            if (RememberMe)
            {
                if (userName != string.Empty)
                {
                    Properties.Settings.Default.UserName = userName;
                    Properties.Settings.Default.RememberMe = true;
                }
            }
            else
            {
                Properties.Settings.Default.UserName = string.Empty;
                Properties.Settings.Default.RememberMe = false;
            }

            Properties.Settings.Default.Save();
        }

        // Asettaa "muista minut" arvot oikeiksi ikkunan luontihetkellä
        private void SetRememberMeValues()
        {
            if (File.Exists("UserDatabase.json"))
            {
                this.UserName = Properties.Settings.Default.UserName;
                this.RememberMe = Properties.Settings.Default.RememberMe;
            }
            else
            {
                Properties.Settings.Default.UserName = string.Empty;
                Properties.Settings.Default.RememberMe = false;
            }

            Properties.Settings.Default.Save();
        }

        // Tyhjentää tekstikentät
        private void EmptyTextAndErrorFields()
        {
            int i;

            for(i = RememberMe ? 1 : 0; i < textBoxList.Count; i++)
            {
                var textBox = (TextBox)App.Current.Windows[App.Current.Windows.Count - 1].FindName(textBoxList[i]);
                textBox.Text = string.Empty;
            }

            for (i = 0; i < errorBoxList.Count; i++)
            {
                var errorBox = (TextBlock)App.Current.Windows[App.Current.Windows.Count - 1].FindName(errorBoxList[i]);
                errorBox.Text = string.Empty;
            }
        }

        // Sulkee ohjelman
        private void ExitProgram(object parameter)
        {
            SaveRememberMeValues((string)parameter);
            App.Current.Shutdown();
        }

        // Luo seuraavan ikkunan ja aukaisee luodun ikkunan
        private void ToRegistrationWindow1(object parameter)
        {
            EmptyTextAndErrorFields();
            var regWin1 = new RegistrationWindow1();
            regWin1.ShowDialog();
        }

        // Luo käyttäjän henkilökohtaisen ikkunan, tekee virheen tarkastukset syötteisiin,
        // ja sulkee tämän ikkunan
        private void LogIn(object parameter)
        {
            var values = (object[])parameter;

            values[1] = Methods.Encrypt((string)values[1]);

            if (InputValidation(values))
            {
                SaveRememberMeValues((string)values[0]);
                EmptyTextAndErrorFields();
                var loggedInWindow = new LoggedInWindow();
                loggedInWindow.ShowDialog();
            }
        }
    }
}