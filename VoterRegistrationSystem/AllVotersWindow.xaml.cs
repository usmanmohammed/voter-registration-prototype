using System;
using System.Collections.Generic;
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

namespace VoterRegistrationSystem
{
    /// <summary>
    /// Interaction logic for AllVotersWindow.xaml
    /// </summary>
    public partial class AllVotersWindow : Window
    {
        private DatabaseContext _db = new DatabaseContext();
        public AllVotersWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            statecomboBox.DataContext = await _db.States.ToListAsync();
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NavigateToDetails();
        }

        private void NavigateToDetails()
        {
            if (dataGrid.SelectedItem != null)
            {
                var selectedItem = (Voter)dataGrid.SelectedItem;
                //Navigate to view Window
                try
                {
                    ViewVoterWindow viewVoterWindow = new ViewVoterWindow(selectedItem.VoterGuid);
                    viewVoterWindow.Show();
                }
                catch (Exception)
                {

                }
            }
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //Validate 

            var selectedState = (State)statecomboBox.SelectedItem;
            var selectedLga = (Lga)lgacomboBox.SelectedItem;
            var selectedWard = (Ward)wardcomboBox.SelectedItem;
            var selectedPollingUnit = (PollingUnit)pollingunitcomboBox.SelectedItem;


            var items = await _db.Voters.ToListAsync();
            dataGrid.DataContext = items.Where(g => getVoterNames(g).Contains(nametextBox.Text) || g.PhoneNumber.Contains(phoneNumbertextBox.Text) ||
            g.HomeAddress.Contains(addresstextBox.Text) &&
            g.StateID == selectedState.StateID && g.LgaID == selectedLga.LgaID && g.WardID == selectedWard.WardID && g.PollingUnitID == selectedPollingUnit.PollingUnitID);
        }

        private string getVoterNames(Voter r)
        {
            return string.Format("{0} {1} {2}", r.FirstName, r.Surname, r.OtherNames);
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void statecomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedItem = (State)statecomboBox.SelectedItem;
                lgacomboBox.DataContext = await _db.LGAs.Where(r => r.StateID == selectedItem.StateID).ToListAsync();
            }
            catch (Exception)
            {
            }
        }

        private async void wardcomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedItem = (Ward)wardcomboBox.SelectedItem;
                pollingunitcomboBox.DataContext = await _db.PollingUnits.Where(r => r.WardID == selectedItem.WardID).ToListAsync();
            }
            catch (Exception)
            {
            }
        }

        private async void lgacomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedItem = (Lga)lgacomboBox.SelectedItem;
                wardcomboBox.DataContext = await _db.Wards.Where(r => r.LgaID == selectedItem.LgaID).ToListAsync();
            }
            catch (Exception)
            {
            }
        }
    }
}
