using BMIDatabase.Models;
using BMIDatabase.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BMIDatabase
{
    // Luokka joka hoitaa tiedostoon kirjoittamisen ja käyttäjän datan hallinnoimisen
    internal class DataHandler
    {
        internal static UserModel LoggedInUser { get; set; }
        private static List<BMIModel> personalBMIDataBase = new List<BMIModel>();
        private static List<UserModel> userDataBase = new List<UserModel>();
        private static List<string> userRegData = new List<string>();

        // Hoitaa ensimmäisen rekisteröinti-ikkunan datan käsittelyn
        internal static void UserRegistrationHandler1(object[] values)
        {
            if (userRegData.Count > 0)
            {
                ClearUserRegistrationData();
            }

            for (int i = 0; i < values.Length - 1; i++)
            {
                userRegData.Add((string)values[i]);
            }
        }

        // Hoitaa toisen rekisteröinti-ikkunan datan käsittelyn
        internal static void UserRegistrationHandler2(object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                userRegData.Add((string)values[i]);
            }

            ReadFromUserDataBase();

            userDataBase.Add(new UserModel(userDataBase.Count + 1, userRegData[0], userRegData[1],
                                           userRegData[2], userRegData[3], (double)int.Parse(userRegData[4])/100));

            SaveToUserDataBase();

            ClearUserDataBaseList();
        }

        // Tarkistaa löytyykö käyttäjänimeä tietokannasta
        internal static bool CheckUserFromDataBase(string userName)
        {
            ReadFromUserDataBase();

            for (int i = 0; i < userDataBase.Count; i++)
            {
                if (userDataBase[i].UserName == userName)
                {
                    ClearUserDataBaseList();
                    return true;
                }
            }

            ClearUserDataBaseList();
            return false;
        }

        // Tarkistaa onko tili lukittu
        internal static bool IsAccountLocked(string userName)
        {
            ReadFromUserDataBase();

            for (int i = 0; i < userDataBase.Count; i++)
            {
                if (userDataBase[i].UserName == userName && userDataBase[i].IsLocked)
                {
                    ClearUserDataBaseList();
                    return true;
                }
            }

            ClearUserDataBaseList();
            return false;
        }

        // Tarkistaa onko salasana oikein ja hoitaa tilin lukituslaskurin sekä lukituksen
        internal static bool CheckUserPasswordFromDataBase(string userName, string password, ref int counter)
        {
            ReadFromUserDataBase();

            for (int i = 0; i < userDataBase.Count; i++)
            {
                if (userDataBase[i].UserName == userName && userDataBase[i].Password != password)
                {
                    if (userDataBase[i].AccountLockCounter > 1)
                    {
                        userDataBase[i].AccountLockCounter--;
                        counter = userDataBase[i].AccountLockCounter;
                        UpdateUserDataBase();
                        ClearUserDataBaseList();
                    }
                    else
                    {
                        userDataBase[i].AccountLockCounter = 0;
                        userDataBase[i].IsLocked = true;
                        UpdateUserDataBase();
                        ClearUserDataBaseList();
                    }

                    return false;
                }
                else if (userDataBase[i].UserName == userName && userDataBase[i].Password == password && !userDataBase[i].IsLocked)
                {
                    userDataBase[i].AccountLockCounter = 3;
                    UpdateUserDataBase();
                    PullUserDataFromDataBase(userDataBase[i]);
                    ClearUserDataBaseList();
                    return true;
                }
            }

            return false;
        }

        // Tallentaa UserModel-oliolistan JSON-muodossa sekä avaa popup-ikkunan
        private static void SaveToUserDataBase()
        {
            PopupWindow popupWindow = new PopupWindow();

            string jsonString = JsonSerializer.Serialize(userDataBase);

            File.WriteAllText("UserDatabase.json", jsonString);

            ClearUserRegistrationData();

            ClearUserDataBaseList();

            popupWindow.ShowDialog();
        }

        // Tallentaa BMIModel-oliolistan JSON-muodossa
        internal static void SaveToPersonalBMIDataBase(double weight)
        {
            Directory.CreateDirectory("PersonalBMIDataBases");

            ReadFromPersonalBMIDataBase();

            var bmi = Methods.CalculateBMI(LoggedInUser.Height, weight);
            
            personalBMIDataBase.Add(new BMIModel(personalBMIDataBase.Count + 1, DateTime.Now,
                                        LoggedInUser.Height, weight, bmi, Methods.BMIresult(bmi)));

            string jsonString = JsonSerializer.Serialize(personalBMIDataBase);

            File.WriteAllText($"PersonalBMIDataBases\\{LoggedInUser.ID}_{LoggedInUser.UserName}.json", jsonString);

            ClearPersonalBMIDataBaseList();
        }

        // Päivittää käyttäjätietokannan
        private static void UpdateUserDataBase()
        {
            string jsonString = JsonSerializer.Serialize(userDataBase);

            File.WriteAllText("UserDatabase.json", jsonString);
        }

        // Päivittää bmi-tietokannan
        private static void UpdatePersonalBMIDataBase()
        {
            string jsonString = JsonSerializer.Serialize(personalBMIDataBase);

            File.WriteAllText($"PersonalBMIDataBases\\{LoggedInUser.ID}_{LoggedInUser.UserName}.json", jsonString);
        }


        // Sijoittaa kirjautuneen käyttäjän tiedot UserModel olioon
        private static void PullUserDataFromDataBase(UserModel loggedInUser)
        {
            LoggedInUser = new UserModel(loggedInUser.ID, loggedInUser.UserName, "",
                                            loggedInUser.FirstName, loggedInUser.LastName, loggedInUser.Height);

            LoggedInUser.LogInTime = loggedInUser.LogInTime;
        }

        // Päivittää kirjautuneen käyttäjän kirjautumisajan
        internal static void UpdateLogInTime()
        {
            ReadFromUserDataBase();

            for(int i = 0; i < userDataBase.Count; i++)
            {
                if (userDataBase[i].UserName == LoggedInUser.UserName)
                {
                    userDataBase[i].LogInTime = DateTime.Now;
                }
            }

            UpdateUserDataBase();

            ClearUserDataBaseList();
        }

        // Tyhjentää listan joka sisältää käyttäjän syöttämät tiedot väliaikaisesti
        private static void ClearUserRegistrationData()
        {
            userRegData.Clear();
        }

        // Tyhjentää kirjautuneen käyttäjän muistissa olevat tiedot
        internal static void ClearLoggedInUser()
        {
            LoggedInUser = new UserModel(0, "", "", "", "", 0);
            LoggedInUser.LogInTime = DateTime.MinValue;
        }

        // Tyhjentää userDataBase listan
        internal static void ClearUserDataBaseList()
        {
            userDataBase.Clear();
        }

        // Tyhjentää personalBMIDataBase listan
        internal static void ClearPersonalBMIDataBaseList()
        {
            personalBMIDataBase.Clear();
        }

        // Lukee tietokannan listaan ja palauttaa listan kutsujalle
        internal static List<BMIModel> GetPersonalBMIDataBaseList()
        {
            ReadFromPersonalBMIDataBase();

            return personalBMIDataBase;
        }

        // Poistaa alkion listasta ja päivittää listan alkioiden ID:t
        internal static void DeleteFromPersonalBMIDataBase(string id)
        {
            ReadFromPersonalBMIDataBase();

            for (int i = 0; i < personalBMIDataBase.Count; i++)
            {
                if (personalBMIDataBase[i].ID.ToString() == id)
                {
                    personalBMIDataBase.RemoveAt(i);
                }
            }

            for (int i = 0; i < personalBMIDataBase.Count; i++)
            {
                personalBMIDataBase[i].ID = i + 1;
            }

            UpdatePersonalBMIDataBase();

            ClearPersonalBMIDataBaseList();
        }

        // Lukee tietokannasta JSON-muodossa olevan listan ja muuttaa sen takaisin UserModel-oliolistaksi
        private static void ReadFromUserDataBase()
        {
            if (File.Exists("UserDataBase.json"))
            {
                string jsonString = File.ReadAllText("UserDataBase.json");
                userDataBase = JsonSerializer.Deserialize<List<UserModel>>(jsonString);
            }
        }

        // Lukee BMI-tietokannasta JSON-muodossa olevan listan ja muuttaa sen takaisin BMIModel-oliolistaksi
        private static void ReadFromPersonalBMIDataBase()
        {
            if (File.Exists($"PersonalBMIDataBases\\{LoggedInUser.ID}_{LoggedInUser.UserName}.json"))
            {
                string jsonString = File.ReadAllText($"PersonalBMIDataBases\\{LoggedInUser.ID}_{LoggedInUser.UserName}.json");
                personalBMIDataBase = JsonSerializer.Deserialize<List<BMIModel>>(jsonString);
            }
        }
    }
}