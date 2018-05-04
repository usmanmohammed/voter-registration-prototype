using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for PrintIDCard.xaml
    /// </summary>
    public partial class PrintIDCard : Window
    {
        private Voter selectedVoter;

        private DatabaseContext db = new DatabaseContext();

        private static string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Voters Registration\\Cards";
        private static string fileName = "Cards.html";
        private static string fileUrl = string.Format("{0}\\{1}", dir, fileName);

        private string appDirectory = "Fingerprint Enrollment";
        private string schoolDirectory = "Voters Registration";
        private string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public PrintIDCard(Voter selectedVoter)
        {
            this.selectedVoter = selectedVoter;
            InitializeComponent();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("<!DOCTYPE html><html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\"><head><link rel=\"stylesheet\" href=\"..//Content/bootstrap.css\" type=\"text/css\" media=\"print,screen\" /><link rel=\"stylesheet\" href=\"..//Content/site.css\" type=\"text/css\" media=\"print,screen\" /><meta charset=\"utf-8\" http-equiv=\"X-UA-Compatible\" content=\"IE=edge,chrome=1\" /></head><body class=\"body-content1 body1\"><div class=\"items1\"><div class=\"main-title1\"><p class=\"country-name1\">FEDERAL REPUBLIC OF NIGERIA</p><p>VOTER'S CARD</p><img class=\"logo1\" src=\"C:/Users/Usman/Documents/Voters Registration/Content/arm.png\" /></div><div class=\"main-body1\"><img class=\"passport1\" src=\"{0}\" /><div class=\"other-info1\"><p class=\"info1\">VIN: {1}</p><!--<p>DELIM: ANAMBRA | ILE-ILFE | SABON GARI</p>--><p class=\"name1\">{2}</p><p class=\"small-title1\">DATE OF BIRTH</p><p class=\"info1\">{3}</p><p class=\"small-title1\">OCCUPATION</p><p class=\"info1\">{4}</p><p class=\"small-title1\">ADDRESS</p><p class=\"info1\">{5}</p></div></div><div class=\"main-footer1\"></div></div></body></html>",
                SaveImageToDrive(selectedVoter),
                 selectedVoter.VoterGuid.ToUpper().Replace('-', ' '),
                  getNames(selectedVoter).ToUpper(),
                  selectedVoter.DateOfBirth.ToString("dd-MM-yyyy"),
                  selectedVoter.Occupation.ToUpper(),
                  selectedVoter.HomeAddress.ToUpper()));

            File.WriteAllText(fileUrl, builder.ToString());
            browser.Source = new Uri(fileUrl);
        }

        private static string getNames(Voter voter)
        {
            return string.Format("{0} {1} {2}", voter.FirstName, voter.Surname, voter.OtherNames);
        }

        private string SaveImageToDrive(Voter voter)
        {
            string url = string.Format("{0}\\{1}\\{2}\\{3}.jpg", appData, schoolDirectory, appDirectory, voter.VoterGuid);
            //File.WriteAllBytes(url, voter.PhotoFront);

            return url;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintFile(fileUrl);
                MessageBox.Show("File Sent to the Default Printer.");
            }
            catch (Exception)
            {
                MessageBox.Show("Please set Internet Explorer as your default .html viewer!");
                return;
            }
        }

        private void PrintFile(string fileUrl)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.Verb = "print";
            info.FileName = fileUrl;

            Process.Start(info);
        }
    }
}
