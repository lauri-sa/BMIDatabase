using System;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace BMIDatabase
{
    internal class Methods
    {
        public static bool StringTarkastus(string syöte, out string errorText)                // metodi ottaa vastaan string tyyppisen syötteen
        {

            bool result;                                                // esitellään boolean muuttuja

            if (syöte != null && syöte != "")                           // tarkastetaan onko saatu syöte null tai tyhjä
            {
                if (syöte.Length <= 15)
                {
                    syöte = syöte.Replace("-", "");

                    if (string.IsNullOrWhiteSpace(syöte))
                    {
                        result = false;
                        errorText = "Syöte ei kelpaa";
                        return result;
                    }

                    result = syöte.All(Char.IsLetter);                      // if lauseessa tarkastetaan onko syötteen kaikki merkit kirjaimia

                    errorText = result ? string.Empty : "Syöte ei kelpaa";

                    return result;
                }
                else
                {
                    result = false;
                    errorText = "pituus max 15 merkkiä";
                    return result;
                }                                                        // palautetaan result > true jos oli pelkästään kirjaimia
            }
            else
            {
                result = false;                                         // muuten palautetaan false
                errorText = "Kenttä ei voi olla tyhjä" ;
                return result;
            }

        }




        public static bool CheckIfDouble(string input)          //Metodi saa stringin parametrina
        {

            double result;


            if (double.TryParse(input, out result))             //Tarkistaa TryParsella onko string muutettavissa double muotoon
            {
                return true;                                    //Jos syöte oli mahdollista muuttaa double tyyppiseksi, palauttaa bool arvon true
            }

            else
            {
                return false;                                   //Jos syöte ei ollut mahdollista muuttaa, palauttaa bool arvon false
            }

        }

        
        
        
        public static double CalculateBMI(double height, double weight)     //Metodi joka laskee painoindeksin
        {
            
            double bmi;
            bmi = weight / (height * height);                               //Painoindeksin laskeminen
            return Math.Round(bmi, 1);                                      //Palauttaa painoindeksin yhden desimaalin tarkkuudella

        }




        public static string BMIresult(double result)
        {


            if (result < 18.5)
            {
                return "Alipainoinen";
            }
            else if (result == 18.5 || result <= 25)
            {
                return "Normaalipainoinen";
            }
            else if (result == 25.1 || result <= 30)
            {
                return "Lievä ylipaino";
            }
            else if (result == 30.1 || result <= 35)
            {
                return "Merkittävä ylipaino";
            }
            else if (result == 35.1 || result <= 40)
            {
                return "Vaikea ylipaino";
            }
            else
            {
                return "Sairaalloinen ylipaino";
            }
        }


        public static Color ResultColor(string bmiResult)
        {
            Color color;

            if (bmiResult == "Normaalipainoinen")
            {
                color = Colors.Green;
            }
            else if (bmiResult == "Alipainoinen" || bmiResult == "Lievä ylipaino")
            {
                color = Colors.Orange;
            }
            else if (bmiResult == "Merkittävä ylipaino" || bmiResult == "Vaikea ylipaino" || bmiResult == "Sairaalloinen ylipaino")
            {
                color = Colors.Red;
            }

            return color;
        }

        public static string Encrypt(string password)
        {
            return Convert.ToBase64String((Encoding.Unicode.GetBytes(password))); //Metodi joka "salaa" käyttäjän syöttämän salasanan Base64 muotoon
        }
    }
}