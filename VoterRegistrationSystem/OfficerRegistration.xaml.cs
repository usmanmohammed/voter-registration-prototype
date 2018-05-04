using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace VoterRegistrationSystem
{
    /// <summary>
    /// Interaction logic for OfficerRegistration.xaml
    /// </summary>
    public partial class OfficerRegistration : Window
    {
        DatabaseContext db = new DatabaseContext();
        public OfficerRegistration()
        {
            InitializeComponent();
        }

        private async void btnCheckUsername_Click(object sender, RoutedEventArgs e)
        {
            await IsUserNameAvailable(true);
        }

        private async Task<bool> IsUserNameAvailable(bool showMessage)
        {
            if (!string.IsNullOrEmpty(usernameTextBox.Text) && !string.IsNullOrWhiteSpace(usernameTextBox.Text.Trim()))
            {
                var match = await db.ApplicationUsers.Where(r => r.UserName.Trim() == usernameTextBox.Text.Trim()).FirstOrDefaultAsync();

                if (match != null)
                {
                    if (showMessage)
                    {
                        MessageBox.Show("Username Already Taken! Please try another one");
                    }

                    return false;
                }

                else
                {
                    if (showMessage)
                    {
                        MessageBox.Show("Valid Username! Please continue...");
                    }

                    return true;
                }
            }

            return false;
        }

        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(nameTextBox.Text) && !string.IsNullOrWhiteSpace(nameTextBox.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(usernameTextBox.Text) && !string.IsNullOrWhiteSpace(usernameTextBox.Text.Trim()))
                {
                    if (!string.IsNullOrEmpty(passwordBox.Password) && !string.IsNullOrWhiteSpace(passwordBox.Password.Trim()))
                    {
                        if (!string.IsNullOrEmpty(confirmPasswordBox.Password) && !string.IsNullOrWhiteSpace(confirmPasswordBox.Password.Trim()))
                        {
                            if (passwordBox.Password == confirmPasswordBox.Password)
                            {
                                //Check if Username is vailable
                                if (await IsUserNameAvailable(false))
                                {
                                    //Push data to db
                                    RegistrationOfficer officer = new RegistrationOfficer() { OfficerName = nameTextBox.Text, AddedBy = MainWindow.ActiveUser.UserName, DateAdded = DateTime.Now };
                                    db.RegistrationOfficers.Add(officer);
                                    await db.SaveChangesAsync();

                                    ApplicationUser appUser = new ApplicationUser()
                                    {
                                        OfficerID = officer.OfficerID,
                                        UserName = usernameTextBox.Text,
                                        Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(passwordBox.Password)),
                                        IsAdmin = IsAdminChkBox.IsChecked == true ? true : false,

                                    };
                                    db.ApplicationUsers.Add(appUser);
                                    await db.SaveChangesAsync();

                                    MessageBox.Show("Officer Successfully Added");

                                    //Clear all Selections
                                    nameTextBox.Text = usernameTextBox.Text = "";
                                    passwordBox.Clear();
                                    confirmPasswordBox.Clear();
                                    IsAdminChkBox.IsChecked = false;
                                }
                            }

                            else
                            {
                                MessageBox.Show("Passwords do not match! Please review");
                            }
                        }
                    }

                    else
                    {
                        MessageBox.Show("The field Password is Required!");
                        return;
                    }
                }

                else
                {
                    MessageBox.Show("The field Username is Required!");
                    return;
                }
            }

            else
            {
                MessageBox.Show("The field Name is Required!");
                return;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
