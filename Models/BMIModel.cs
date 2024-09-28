using System;

namespace BMIDatabase.Models
{
    // Luokka jonka pohjalta luodaan BMIModel-olio
    internal class BMIModel
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public double BMI { get; set; }
        public string BMIText { get; set; }

        public BMIModel(int id, DateTime date, double height, double weight, double bmi, string bmiText)
        {
            this.ID = id;
            this.Date = date;
            this.Height = height;
            this.Weight = weight;
            this.BMI = bmi;
            BMIText = bmiText;
        }
    }
}