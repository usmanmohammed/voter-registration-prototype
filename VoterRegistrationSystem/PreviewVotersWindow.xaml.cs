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
using System.IO;
using System.Diagnostics;

namespace VoterRegistrationSystem
{
    /// <summary>
    /// Interaction logic for PreviewVotersWindow.xaml
    /// </summary>
    public partial class PreviewVotersWindow : Window
    {
        private DatabaseContext db = new DatabaseContext();

        private static string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Voters Registration\\Cards";
        private static string fileName = "Preview.html";
        private static string fileUrl = string.Format("{0}\\{1}", dir, fileName);

        private string appDirectory = "Fingerprint Enrollment";
        private string schoolDirectory = "Voters Registration";
        private string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public PreviewVotersWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("<html><head><link rel=\"stylesheet\" href=\"..//Content/bootstrap.css\" type=\"text/css\" media=\"print,screen\"><link rel=\"stylesheet\" href=\"..//Content/site.css\" type=\"text/css\" media=\"print,screen\"><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge,chrome=1\"></head><body class=\"body-content\"><div class=\"row\"><div class=\"col-md-offset-3 col-md-7\"><img src=\"..//Content/arm.png\" class=\"logo\" /><div class=\"title\"><h4 style=\"font-weight: 600;\">FEDERAL REPUBLIC OF NIGERIA</h4><h4>Valid Registrants</h4></div></div></div><hr /><div class=\"row\">");

                foreach (var voter in await db.Voters.ToListAsync())
                {
                    builder.Append(string.Format("<div class=\"col-md-6\"><div class=\"col-md-4\"><img src=\"{0}\" class=\"passport\" /></div><div class=\"col-md-8\"><p><strong>VIN: </strong>{1}</p><p><strong>NAME: </strong>{2}</p><p><strong>OCCUPATION: </strong>{3}</p><p><strong>GENDER: </strong>{4}</p><p><strong>AGE: </strong>{5}</p></div></div>",
                        SaveImageToDrive(voter),
                        voter.VoterGuid.Replace('-', ' ').ToUpper(),
                        getNames(voter),
                        voter.Occupation,
                        voter.Sex.SexTitle,
                        (DateTime.Now.Year - voter.DateOfBirth.Year)));
                }

                builder.Append("</div></body></html>");

                File.WriteAllText(fileUrl, builder.ToString());
                browser.Source = new Uri(fileUrl);
            }
            catch (Exception)
            {
            }
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
