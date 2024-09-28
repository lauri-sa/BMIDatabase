using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BMIDatabase.Models;

namespace BMIDatabase.ViewModels
{
    internal class LoggedInWindowViewModel : BaseViewModel
    {
        public string UserWelcomeText { get; set; }
        public string LastLogInTime { get; set; }
        public string InfoText1 { get; set; }
        public string InfoText2 { get; set; }
        public string InfoText3 { get; set; }
        public bool IsEnabled { get; set; }
        public string AddButton { get; set; }
        public string LogOutButton { get; set; }

        public ICommand LogOutCommand => new DelegateCommand(LogOut);
        public ICommand SaveBMIDataCommand => new DelegateCommand(SaveBMIData);
        public ICommand FetchDatesCommand => new DelegateCommand(FetchDates);
        public ICommand ShowAddElementCommand => new DelegateCommand(ShowAddElement);
        public ICommand ShowListElementCommand => new DelegateCommand(ShowListElement);
        public ICommand ShowSearchElementCommand => new DelegateCommand(ShowSearchElement);

        private List<string> errorBoxList = new List<string>() { "DatePicker1Error", "DatePicker2Error" };

        Button listButton = (Button)App.Current.Windows[App.Current.Windows.Count - 1].FindName("List");
        Button searchButton = (Button)App.Current.Windows[App.Current.Windows.Count - 1].FindName("Search");

        Border addElement = (Border)App.Current.Windows[App.Current.Windows.Count - 1].FindName("AddElement");
        Border listElement = (Border)App.Current.Windows[App.Current.Windows.Count - 1].FindName("ListElement");
        Border searchElement = (Border)App.Current.Windows[App.Current.Windows.Count - 1].FindName("SearchElement");

        WrapPanel resultPanel = (WrapPanel)App.Current.Windows[App.Current.Windows.Count - 1].FindName("ResultPanel");
        DatePicker datePicker1 = (DatePicker)App.Current.Windows[App.Current.Windows.Count - 1].FindName("DatePicker1");
        DatePicker datePicker2 = (DatePicker)App.Current.Windows[App.Current.Windows.Count - 1].FindName("DatePicker2");

        TextBox weightTextBox = (TextBox)App.Current.Windows[App.Current.Windows.Count - 1].FindName("Weight");

        TextBlock infoText1 = (TextBlock)App.Current.Windows[App.Current.Windows.Count - 1].FindName("InfoText1");
        TextBlock infoText2 = (TextBlock)App.Current.Windows[App.Current.Windows.Count - 1].FindName("InfoText2");
        TextBlock infoText3 = (TextBlock)App.Current.Windows[App.Current.Windows.Count - 1].FindName("InfoText3");
        
        TextBlock weightError = (TextBlock)App.Current.Windows[App.Current.Windows.Count - 1].FindName("WeightError");
        TextBlock datePicker1Error = (TextBlock)App.Current.Windows[App.Current.Windows.Count - 1].FindName("DatePicker1Error");
        TextBlock datePicker2Error = (TextBlock)App.Current.Windows[App.Current.Windows.Count - 1].FindName("DatePicker2Error");

        public LoggedInWindowViewModel()
        {
            this.LastLogInTime = DataHandler.LoggedInUser.LogInTime == DateTime.MinValue ? string.Empty :
                                    $"Kirjauduit viimeksi {DataHandler.LoggedInUser.LogInTime.ToString("d.M.yyyy")}\n" +
                                    $"klo {DataHandler.LoggedInUser.LogInTime.ToString("HH.mm")}";
            this.UserWelcomeText = $"Tervetuloa {DataHandler.LoggedInUser.FirstName}\n{this.LastLogInTime}";
            this.AddButton = "Lisää";
            this.LogOutButton = "Kirjaudu Ulos";
            this.IsEnabled = IsDataBaseEmpty();
            this.InfoText1 = GetLastResultAddTime();
            this.InfoText2 = "\n\n";
            this.InfoText3 = "\n\n";
            DataHandler.UpdateLogInTime();
        }

        // Metodi joka hoitaa syötteentarkistuksen ja syötteen virheilmoituksien näyttämisen
        private bool WeightInputValidation(string weight)
        {
            bool validInput;

            if (!string.IsNullOrWhiteSpace(weight))
            {

                if (Methods.CheckIfDouble(weight))
                {
                    if (double.Parse(weight) >= 0.5 && double.Parse(weight) <= 700)
                    {
                        validInput = true;
                        weightError.Text = string.Empty;
                    }
                    else
                    {
                        validInput = false;
                        weightError.Text = "Anna arvo väliltä 0.5 ja 700";
                    }
                }
                else
                {
                    validInput = false;
                    weightError.Text = "Anna numeerinen arvo";
                }
            }
            else
            {
                validInput = false;
                weightError.Text = "Kenttä ei voi olla tyhjä";
            }

            return validInput;
        }

        private bool DateInputValidation()
        {
            bool validInput = true;

            if (!datePicker1.SelectedDate.HasValue)
            {
                validInput = false;
                datePicker1Error.Text = "Kenttä ei voi olla tyhjä";
            }
            else
            {
                datePicker1Error.Text = string.Empty;
            }

            if (!datePicker2.SelectedDate.HasValue)
            {
                validInput = false;
                datePicker2Error.Text = "Kenttä ei voi olla tyhjä";
            }
            else
            {
                datePicker2Error.Text = string.Empty;
            }

            return validInput;
        }

        // Metodi lisää käyttöliittymään Border-elementtejä, jotka sisältävät käyttäjän mittaustulokset
        private void AddResultElements()
        {
            resultPanel.Children.Clear();

            DateTime startDate;
            DateTime endDate;

            if (datePicker1.SelectedDate.HasValue && datePicker2.SelectedDate.HasValue)
            {
                startDate = DateTime.Compare(datePicker1.SelectedDate.Value, datePicker2.SelectedDate.Value) <= 0 ?
                                                datePicker1.SelectedDate.Value : datePicker2.SelectedDate.Value;

                endDate = DateTime.Compare(datePicker2.SelectedDate.Value, datePicker1.SelectedDate.Value) >= 0 ?
                                                    datePicker2.SelectedDate.Value : datePicker1.SelectedDate.Value;
            }
            else
            {
                startDate = DateTime.MinValue;
                endDate = DateTime.MaxValue;
            }
            
            var resultList = DataHandler.GetPersonalBMIDataBaseList();

            var trimmedList = new List<BMIModel>();

            for (int i = 0; i < resultList.Count; i++)
            {
                if (startDate.Date <= resultList[i].Date.Date)
                {
                    if (resultList[i].Date.Date <= endDate.Date)
                    {
                        trimmedList.Add(resultList[i]);
                    }
                }
            }

            double[] weightAverageArray = new double[trimmedList.Count];
            double[] bmiAverageArray = new double[trimmedList.Count];

            trimmedList.Reverse();

            for (int i = 0; i < trimmedList.Count; i++)
            {
                var border = new Border();

                var grid = new Grid();

                weightAverageArray[i] = trimmedList[i].Weight;
                bmiAverageArray[i] = trimmedList[i].BMI;

                for (int j = 0; j < 7; j++)
                {
                    if (j < 3)
                    {
                        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    }

                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                }

                border.Width = 75;
                border.Height = 75;
                border.Margin = new Thickness(10.0, 5.0, 10.0, 4.5);
                border.BorderThickness = new Thickness(1);
                border.Background = new SolidColorBrush(Colors.White);
                border.BorderBrush = new SolidColorBrush(Methods.ResultColor(trimmedList[i].BMIText));
                border.CornerRadius = new CornerRadius(5);
                border.Cursor = Cursors.Hand;
                border.Tag = $"{trimmedList[i].ID}";
                border.MouseEnter += Border_MouseEnter;
                border.MouseLeave += Border_MouseLeave;
                border.Child = grid;

                var dateText = new TextBlock
                {
                    Margin = new Thickness(0.0, 4.0, 0.0, 0.0),
                    TextAlignment = TextAlignment.Center,
                    FontSize = 7.5,
                    Text = $"{trimmedList[i].Date.ToString("d.M.yyyy")}\nklo {trimmedList[i].Date.ToString("HH.mm")}",
                };

                var measuresText = new TextBlock
                {
                    Margin = new Thickness(0.0, 2.0, 0.0, 2.0),
                    TextAlignment = TextAlignment.Left,
                    FontSize = 7.5,
                    Text = "Pituus\nPaino\nBMI",
                };

                var dataText = new TextBlock
                {
                    Margin = new Thickness(0.0, 2.0, 0.0, 2.0),
                    TextAlignment = TextAlignment.Right,
                    FontSize = 7.5,
                    Text = $"{trimmedList[i].Height}\n{trimmedList[i].Weight}\n{trimmedList[i].BMI}",
                };

                var resultText = new TextBlock
                {
                    Foreground = new SolidColorBrush(Methods.ResultColor(trimmedList[i].BMIText)),
                    TextAlignment = TextAlignment.Center,
                    FontSize = 7.5,
                    Text = $"{trimmedList[i].BMIText}",
                };

                var deleteIcon = new Image
                {
                    Margin = new Thickness(0.0,3.0,3.0,0.0),
                    Height = 10,
                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/incorrect-icon.png")),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Visibility = Visibility.Collapsed
                };

                deleteIcon.MouseLeftButtonDown += DeleteIcon_MouseLeftButtonDown;

                Grid.SetColumnSpan(dateText, 7);

                Grid.SetColumn(deleteIcon, 5);
                Grid.SetColumnSpan(deleteIcon, 2);

                Grid.SetRow(measuresText, 1);
                Grid.SetColumn(measuresText, 1);
                Grid.SetColumnSpan(measuresText, 3);

                Grid.SetRow(dataText, 1);
                Grid.SetColumn(dataText, 3);
                Grid.SetColumnSpan(dataText, 3);

                Grid.SetRow(resultText, 2);
                Grid.SetColumnSpan(resultText, 7);

                grid.Children.Add(dateText);
                grid.Children.Add(deleteIcon);
                grid.Children.Add(measuresText);
                grid.Children.Add(dataText);
                grid.Children.Add(resultText);

                resultPanel.Children.Add(border);
            }

            infoText1.Text = "Mittaustulosten keskiarvot";
            infoText2.Text = "Painoindeksi\nPaino";
            infoText3.Text = trimmedList.Count < 1 ? string.Empty :
                                $"{Math.Round(bmiAverageArray.Average(), 1)}\n{Math.Round(weightAverageArray.Average(), 1)}";

            trimmedList.Clear();
            resultList.Clear();
        }

        private void DeleteIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = (Image)sender;

            var grid = (Grid)image.Parent;

            var border = (Border)grid.Parent;

            DataHandler.DeleteFromPersonalBMIDataBase((string)border.Tag);

            if (!IsDataBaseEmpty())
            {
                listButton.IsEnabled = false;
                searchButton.IsEnabled = false;
                HideAllContentElements();
                Grid.SetRowSpan(infoText1, 2);
                infoText1.Text = GetLastResultAddTime();
                infoText2.Text = "\n\n";
                infoText3.Text = "\n\n";
                addElement.Visibility = Visibility.Visible;
            }
            else
            {
                AddResultElements();
            }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            var border = (Border)sender;

            var grid = (Grid)border.Child;

            border.BorderThickness = new Thickness(1);

            grid.Children[1].Visibility = Visibility.Collapsed;
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = (Border)sender;

            var grid = (Grid)border.Child;

            border.BorderThickness = new Thickness(2);

            grid.Children[1].Visibility = Visibility.Visible;
        }

        private string GetLastResultAddTime()
        {
            var resultList = DataHandler.GetPersonalBMIDataBaseList();

            return resultList.Count < 1 ? "Tietokanta on tyhjä" : $"Uusin lisäys {resultList[resultList.Count - 1].Date.ToString("d.M.yyyy")}" +
                                            $" klo {resultList[resultList.Count - 1].Date.ToString("HH.mm")}";
        }

        // Tarkistaa onko tietokanta tyhjä
        private bool IsDataBaseEmpty()
        {
            var resultList = DataHandler.GetPersonalBMIDataBaseList();

            return resultList.Count < 1 ? false : true;
        }

        // Asettaa DatePicker objektien kalentereiden ominaisuudet
        private void CalendarSettings(DatePicker datePicker)
        {
            int j;
            var resultList = DataHandler.GetPersonalBMIDataBaseList();
            var blackoutDates = new List<DateTime>();
            var resultDates = new List<DateTime>();
            var startDate = new DateTime(resultList[0].Date.Year, resultList[0].Date.Month, 1);
            var endDate = new DateTime(resultList[resultList.Count - 1].Date.Year, resultList[resultList.Count - 1].Date.Month,
                                        DateTime.DaysInMonth(resultList[resultList.Count - 1].Date.Year,
                                        resultList[resultList.Count - 1].Date.Month));

            datePicker.DisplayDateStart = startDate;
            datePicker.DisplayDateEnd = endDate;

            while (DateTime.Compare(startDate, endDate) <= 0)
            {
                blackoutDates.Add(startDate);
                startDate = startDate.AddDays(1);
            }

            for (int i = 0; i < resultList.Count; i++)
            {
                resultDates.Add(resultList[i].Date);
            }

            for (int i = 0; i < blackoutDates.Count; i++)
            {
                for (j = 0; j < resultDates.Count; j++)
                {
                    if (blackoutDates[i].Date == resultDates[j].Date)
                    {
                        break;
                    }
                }

                if (j >= resultDates.Count)
                {
                    datePicker.BlackoutDates.Add(new CalendarDateRange(blackoutDates[i]));
                }
            }

            resultList.Clear();
        }

        private void EmptySelectedDates(DatePicker datePicker)
        {
            var border = VisualTreeHelper.GetChild(datePicker, 0);
            var grid = VisualTreeHelper.GetChild(border, 0);
            DatePickerTextBox dbtb = VisualTreeHelper.GetChild(grid, 1) as DatePickerTextBox;
            datePicker.SelectedDate = null;
            ContentControl waterMark = dbtb.Template.FindName("PART_Watermark", dbtb) as ContentControl;
            waterMark.Content = "Valitse päivämäärä";
        }

        private void EmptyInfoAndErrorText()
        {
            Grid.SetRowSpan(infoText1, 1);
            infoText1.Text = string.Empty;
            infoText2.Text = string.Empty;
            infoText3.Text = string.Empty;
        }

        // Piilottaa kaikki sisältöelementit
        private void HideAllContentElements()
        {
            EmptyInfoAndErrorText();
            addElement.Visibility = Visibility.Collapsed;
            listElement.Visibility = Visibility.Collapsed;
            searchElement.Visibility = Visibility.Collapsed;
        }

        // Näyttää tuloksenkirjausikkunan
        private void ShowAddElement(object parameter)
        {
            HideAllContentElements();

            if (datePicker1.IsArrangeValid && datePicker2.IsArrangeValid)
            {
                EmptySelectedDates(datePicker1);
                EmptySelectedDates(datePicker2);
            }

            datePicker1Error.Text = string.Empty;
            datePicker2Error.Text = string.Empty;

            Grid.SetRowSpan(infoText1, 2);
            var resultList = DataHandler.GetPersonalBMIDataBaseList();
            infoText1.VerticalAlignment = VerticalAlignment.Center;
            infoText1.Text = resultList.Count < 1 ? "Tietokanta on tyhjä" : $"Uusin lisäys {resultList[resultList.Count - 1].Date.ToString("d.M.yyyy")}" +
                                $" klo {resultList[resultList.Count - 1].Date.ToString("HH.mm")}";
            infoText2.Text = "\n\n";
            infoText3.Text = "\n\n";
            addElement.Visibility = Visibility.Visible;
            resultList.Clear();
        }

        // Hoitaa tallennukseen tarvittavien metodien kutsumisen ja elementtien piilottamisen/näyttämisen
        private void SaveBMIData(object parameter)
        {
            string weight = (string)parameter;

            weight = weight.Replace(".", ",");

            if (WeightInputValidation(weight))
            {
                DataHandler.SaveToPersonalBMIDataBase(Math.Round(double.Parse(weight), 2));
                weightTextBox.Text = string.Empty;
                weightError.Text = string.Empty;
                listButton.IsEnabled = true;
                searchButton.IsEnabled = true;
                HideAllContentElements();
                AddResultElements();
                listElement.Visibility = Visibility.Visible;
            }
        }

        // Näyttää listaikkunan
        private void ShowListElement(object parameter)
        {
            HideAllContentElements();
            AddResultElements();

            if (datePicker1.IsArrangeValid && datePicker2.IsArrangeValid)
            {
                EmptySelectedDates(datePicker1);
                EmptySelectedDates(datePicker2);
            }

            weightTextBox.Text = string.Empty;
            weightError.Text = string.Empty;
            datePicker1Error.Text = string.Empty;
            datePicker2Error.Text = string.Empty;
            listElement.Visibility = Visibility.Visible;
        }

        // Näyttää hakuikkunan
        private void ShowSearchElement(object parameter)
        {
            HideAllContentElements();
            CalendarSettings(datePicker1);
            CalendarSettings(datePicker2);
            weightTextBox.Text = string.Empty;
            weightError.Text = string.Empty;
            Grid.SetRowSpan(infoText1, 2);
            infoText1.VerticalAlignment = VerticalAlignment.Center;
            infoText1.Text = "Anna haluamasi aikaväli";
            infoText2.Text = "\n\n";
            infoText3.Text = "\n\n";
            searchElement.Visibility = Visibility.Visible;
        }

        private void FetchDates(object parameter)
        {
            if (DateInputValidation())
            {
                HideAllContentElements();
                AddResultElements();
                EmptySelectedDates(datePicker1);
                EmptySelectedDates(datePicker2);
                datePicker1Error.Text = string.Empty;
                datePicker2Error.Text = string.Empty;
                listElement.Visibility = Visibility.Visible;
            }
        }

        // Poistaa käyttäjän kirjautumistiedot ja sulkee tämän ikkunan
        private void LogOut(object parameter)
        {
            DataHandler.ClearLoggedInUser();
            App.Current.Windows[App.Current.Windows.Count - 1].Close();
        }
    }
}