using System;

namespace BMIDatabase.Models
{
    // Luokka jonka pohjalta luodaan UserModel-olio
    internal class UserModel
    {
        public int ID { get; set; }
        public int AccountLockCounter { get; set; }
        public bool IsLocked { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double Height { get; set; }
        public DateTime LogInTime { get; set; }

        public UserModel(int id, string userName, string password, string firstName, string lastName, double height)
        {
            this.ID = id;
            this.AccountLockCounter = 3;
            this.IsLocked = false;
            this.UserName = userName;
            this.Password = password;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Height = height;
            this.LogInTime = DateTime.MinValue;
        }
    }
}
