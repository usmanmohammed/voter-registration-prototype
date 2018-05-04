using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VoterRegistrationSystem.Models;
using System.Data.Entity;
using System.IO;

namespace VoterRegistrationSystem
{
    /// <summary>
    /// Interaction logic for ViewVoterWindow.xaml
    /// </summary>
    public partial class ViewVoterWindow : Window
    {
        private VoterViewModel mainViewModel;
        private List<Voter> records;
        private DatabaseContext db = new DatabaseContext();
        private string voterGuid;
        private Voter selectedVoter;

        public ViewVoterWindow(string voterGuid)
        {
            InitializeComponent();

            mainViewModel = new VoterViewModel();
            DataContext = mainViewModel;
   
            this.voterGuid = voterGuid;
        }

        //private void filteredlistBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    //Perform validation first!
        //    if (filteredlistBox.SelectedIndex == -1)
        //    {
        //        return;
        //    }

        //    mainViewModel.SelectedVoter = searchResults[filteredlistBox.SelectedIndex];
        //    mainViewModel.ImageSource = ConvertFromBytes(searchResults[filteredlistBox.SelectedIndex].PhotoFront);

        //    CloseCanvas();
        //}
        public BitmapImage ConvertFromBytes(byte[] array)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(array))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    return image;
                }
            }

            catch (Exception)
            {
                return null;
            }
        }
        //private void findPerson_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    //Make canvas Visible
        //    DisplayCanvas();

        //    try
        //    {
        //        //Prepare search query
        //        string query = findPersonTextBox.Text.ToLower();

        //        searchResults = new ObservableCollection<Voter>(records.Where(x =>
        //            x.FirstName.ToLower().Contains(query) ||
        //            x.Surname.ToLower().Contains(query) ||
        //            x.OtherNames.ToLower().Contains(query) ||
        //            x.VoterGuid.ToLower().Contains(query) ||
        //            string.Format("{0} {1}", x.FirstName, x.Surname).ToLower().Contains(query) ||
        //            string.Format("{0} {1}", x.Surname, x.FirstName).ToLower().Contains(query) ||
        //            string.Format("{0} {1} {2}", x.FirstName, x.OtherNames, x.Surname).ToLower().Contains(query) ||
        //            string.Format("{0} {1} {2}", x.OtherNames, x.Surname, x.FirstName).ToLower().Contains(query) ||
        //            string.Format("{0} {1} {2}", x.Surname, x.FirstName, x.OtherNames).ToLower().Contains(query)).ToList());

        //        //Bind Datacontexts
        //        mainViewModel.SearchResults = new ObservableCollection<Voter>(searchResults);
        //    }

        //    catch (Exception)
        //    {
        //        CloseCanvas();
        //    }
        //}

        //private void DisplayCanvas()
        //{
        //    if (filteredCanvas.Visibility == Visibility.Collapsed)
        //    {
        //        filteredCanvas.Visibility = Visibility.Visible;
        //    }
        //}

        //private void CloseCanvas()
        //{
        //    if (filteredCanvas.Visibility == Visibility.Visible)
        //    {
        //        filteredCanvas.Visibility = Visibility.Collapsed;
        //    }
        //}

        //private void filteredCanvas_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    //Close canvas
        //    CloseCanvas();
        //}

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            records = await db.Voters.ToListAsync();
            selectedVoter = records.Where(r => r.VoterGuid == voterGuid).FirstOrDefault();

            mainViewModel.SelectedVoter = selectedVoter;
            mainViewModel.ImageSource = ConvertFromBytes(selectedVoter.PhotoFront);

            mainViewModel.RightOneImageSource = ConvertFromBytes(selectedVoter.VoterFingerprint.RightOne);
            mainViewModel.RightTwoImageSource = ConvertFromBytes(selectedVoter.VoterFingerprint.RightTwo);
            mainViewModel.RightThreeImageSource = ConvertFromBytes(selectedVoter.VoterFingerprint.RightThree);
            mainViewModel.RightFourImageSource = ConvertFromBytes(selectedVoter.VoterFingerprint.RightFour);
            mainViewModel.RightFiveImageSource = ConvertFromBytes(selectedVoter.VoterFingerprint.RightFive);

            mainViewModel.LeftOneImageSource = ConvertFromBytes(selectedVoter.VoterFingerprint.LeftOne);
            mainViewModel.LeftTwoImageSource = ConvertFromBytes(selectedVoter.VoterFingerprint.LeftTwo);
            mainViewModel.LeftThreeImageSource = ConvertFromBytes(selectedVoter.VoterFingerprint.LeftThree);
            mainViewModel.LeftFourImageSource = ConvertFromBytes(selectedVoter.VoterFingerprint.LeftFour);
            mainViewModel.LeftFiveImageSource = ConvertFromBytes(selectedVoter.VoterFingerprint.LeftFive);

            mainViewModel.Sexes = new ObservableCollection<Sex>(await db.Sexes.ToListAsync());
            mainViewModel.MaritalStatuses = new ObservableCollection<MaritalStatus>(await db.MaritalStatus.ToListAsync());
            mainViewModel.PollingUnits = new ObservableCollection<PollingUnit>(await db.PollingUnits.ToListAsync());
            mainViewModel.States = new ObservableCollection<State>(await db.States.ToListAsync());
            mainViewModel.Lgas = new ObservableCollection<Lga>(await db.LGAs.ToListAsync());
        }

        private void btnPrintCard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintIDCard print = new PrintIDCard(selectedVoter);
                print.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
