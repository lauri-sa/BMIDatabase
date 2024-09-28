using System.Windows.Media;
using System.Windows;

namespace BMIDatabase.ViewModels
{
    // Kantaluokka josta muut ViewModel-luokat perivät kaikille yhteisiä ominaisuuksia
    internal class BaseViewModel
    {
        public string Title { get; set; }
        public FontFamily FontFamily { get; set; }
        public int FontSize { get; set; }
        public ResizeMode ResizeMode { get; set; }

        public BaseViewModel()
        {
            this.Title = "Paino Oy - painoindeksitietokanta";
            this.FontFamily = new FontFamily("Segoe UI");
            this.FontSize = 14;
            this.ResizeMode = ResizeMode.CanMinimize;
        }
    }
}