using AForge.Video;
using AForge.Video.DirectShow;
using SourceAFIS.Simple;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
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
using ZKFPEngXControl;

namespace VoterRegistrationSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ConcurrentBag<Person> personsDatabase;
        private DatabaseContext db = new DatabaseContext();

        private byte[] userPicture;
        private byte[] left_1;
        private byte[] left_2;
        private byte[] left_3;
        private byte[] left_4;
        private byte[] left_5;
        private byte[] right_1;
        private byte[] right_2;
        private byte[] right_3;
        private byte[] right_4;
        private byte[] right_5;

        private string appDirectory = "Fingerprint Enrollment";
        private string schoolDirectory = "Voters Registration";
        private string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoDevice;
        private VideoCapabilities[] snapshotCapabilities;

        private ZKFPEngXClass FingerObject;
        private ActiveFinger activeFinger;
        AfisEngine Afis;
        private bool fingerprintMatchFound = false;


        private static ApplicationUser activeUser;
        private VoterViewModel mainViewModel;
        public MainWindow()
        {
            InitializeComponent();

            Database.SetInitializer(new DatabaseContextInitializer());
            using (var context = new DatabaseContext())
            {
                context.Database.Initialize(force: true);
            }

            mainViewModel = new VoterViewModel();
            DataContext = mainViewModel;
        }

        public static ApplicationUser ActiveUser
        {
            get { return activeUser; }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var records = await db.Voters.ToListAsync();

            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            var states = await db.States.ToListAsync();
            var sexes = await db.Sexes.ToListAsync();
            var maritalStatuses = await db.MaritalStatus.ToListAsync();

            sexComboBox.DataContext = sexes;
            maritalComboBox.DataContext = maritalStatuses;
            stateOfOriginCombobox.DataContext = states;

            Afis = new AfisEngine();
            Afis.Threshold = 25;
            //Loadup items to persons database
            personsDatabase = new ConcurrentBag<Person>();

            List<string> people = new List<string>();


            foreach (var item in await db.Fingerprints.ToListAsync())
            {
                var fingerPrints = new List<SourceAFIS.Simple.Fingerprint>();

                if (item.LeftOne != null)
                {  
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(item.LeftOne) });
                }

                if (item.LeftTwo != null)
                {
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(item.LeftTwo) });
                }

                if (item.LeftThree != null)
                {
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(item.LeftTwo) });
                }

                if (item.LeftFour != null)
                {
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(item.LeftTwo) });
                }
                if (item.LeftFive != null)
                {
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(item.LeftTwo) });
                }

                if (item.RightOne != null)
                {
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(item.RightOne) });
                }

                if (item.RightTwo != null)
                {
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(item.RightTwo) });
                }

                if (item.RightThree != null)
                {
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(item.LeftTwo) });
                }

                if (item.RightFour != null)
                {
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(item.LeftTwo) });
                }

                if (item.RightFive != null)
                {
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(item.LeftTwo) });
                }

                var person = new Person();
                person.Id = item.VoterID;
                person.Fingerprints = fingerPrints;

                Afis.Extract(person);
                personsDatabase.Add(person);
            }
        }


        private void ConnectCamera()
        {
            try
            {
                videoDevice.ProvideSnapshots = true;
                videoDevice.SnapshotResolution = videoDevice.SnapshotCapabilities[0];
                videoDevice.SnapshotFrame += new NewFrameEventHandler(videoDevice_SnapshotFrame);

                videoSourcePlayer.VideoSource = videoDevice;
                videoSourcePlayer.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("Connection to camera failed. Please try again!");
            }
        }

        private void LoadCamera(int cameraIndex)
        {
            videoDevice = new VideoCaptureDevice(videoDevices[cameraIndex].MonikerString);

            snapshotCapabilities = videoDevice.SnapshotCapabilities;
        }

        private void DisconnectCamera()
        {
            if (videoSourcePlayer.VideoSource != null)
            {
                // stop video device
                videoSourcePlayer.SignalToStop();
                videoSourcePlayer.WaitForStop();
                videoSourcePlayer.VideoSource = null;

                if (videoDevice.ProvideSnapshots)
                {
                    videoDevice.SnapshotFrame -= new NewFrameEventHandler(videoDevice_SnapshotFrame);
                }
            }
        }

        public BitmapSource GetBitmapSource(System.Drawing.Bitmap bitmap)
        {
            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap
            (
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );

            return bitmapSource;
        }

        public System.Drawing.Bitmap CropBitmap(System.Drawing.Bitmap bitmap, int cropX, int cropY, int cropWidth, int cropHeight)
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(cropX, cropY, cropWidth, cropHeight);
            System.Drawing.Bitmap cropped = bitmap.Clone(rect, bitmap.PixelFormat);
            return cropped;
        }

        public byte[] ImageToByte(System.Drawing.Bitmap img)
        {
            if (!Directory.Exists(string.Format("{0}\\{1}\\{2}", appData, schoolDirectory, appDirectory)))
            {
                Directory.CreateDirectory(string.Format("{0}\\{1}\\{2}", appData, schoolDirectory, appDirectory));
            }

            var _img = CropBitmap(img, 70, 0, 475, 475);

            byte[] byteArray = new byte[0];

            using (MemoryStream stream = new MemoryStream())
            {
                Int32Rect rect = new Int32Rect(96, 197, 246, 288);
                _img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                _img.Save(string.Format("{0}\\{1}\\{2}\\picture.jpg", appData, schoolDirectory, appDirectory), ImageFormat.Jpeg);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }

        private void videoDevice_SnapshotFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Console.WriteLine(eventArgs.Frame.Size);

                System.Drawing.Bitmap frame = ((System.Drawing.Bitmap)eventArgs.Frame.Clone());

                //Stop Camera Feed and collapse renderer
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                {

                    if (imageHost.Visibility == Visibility.Visible)
                    {
                        imageHost.Visibility = Visibility.Collapsed;
                    }

                    if (userProfileImage.Visibility == Visibility.Hidden)
                    {
                        userProfileImage.Visibility = Visibility.Visible;
                    }

                    try
                    {
                        userPicture = ImageToByte(frame);
                        userProfileImage.Source = ConvertFromBytes(userPicture);
                    }
                    catch (Exception)
                    {

                    }
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error!" + ex.ToString());

                //Log to Text file
                //Models.Logger.WriteToLogFile(ex.ToString());
            }
        }

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

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            //Perform Validation here

            if ((videoDevice != null) && (videoDevice.ProvideSnapshots))
            {
                videoDevice.SimulateTrigger();
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            //Stop Camera Feed and collapse renderer
            ResetCamera();
        }

        private void ResetCamera()
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {

                if (imageHost.Visibility == Visibility.Collapsed)
                {
                    imageHost.Visibility = Visibility.Visible;
                }

                if (userProfileImage.Visibility == Visibility.Visible)
                {
                    userProfileImage.Visibility = Visibility.Hidden;
                }
            }));

            userPicture = null;
            GC.Collect();
        }

        private void ClearAndResetFields()
        {
            //Clear Variables
            userPicture = null;
            left_1 = null;
            left_2 = null;
            left_3 = null;
            left_4 = null;
            left_5 = null;
            right_1 = null;
            right_2 = null;
            right_3 = null;
            right_4 = null;
            right_5 = null;
            activeFinger = null;
            
            GC.Collect();

            //Reset Fields
            firstNameTextBox.Text = String.Empty;
            surnameTextBox.Text = String.Empty;
            otherNamesTextBox.Text = String.Empty;
            phoneNumberTextBox.Text = String.Empty;
            addressTextBox.Text = String.Empty;
            sexComboBox.SelectedIndex = -1;
            stateOfOriginCombobox.SelectedIndex = -1;
            lgaOfOriginCombobox.SelectedIndex = -1;
            dateOfBirthPicker.Text = String.Empty;
            maritalComboBox.SelectedIndex = -1;
            ageTextBox.Text = string.Empty;
            occupationTextBox.Text = string.Empty;
            wardCombobox.SelectedIndex = -1;
            pollingUnitCombobox.SelectedIndex = -1;


            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {
                imageLeftFive.Source = null;
                imageLeftFour.Source = null;
                imageLeftThree.Source = null;
                imageLeftTwo.Source = null;
                imageLeftOne.Source = null;
                imageRightFive.Source = null;
                imageRightFour.Source = null;
                imageRightThree.Source = null;
                imageRightTwo.Source = null;
                imageRightOne.Source = null;
                userProfileImage.Source = null;
            }));
        }

        private string GetTempImageDir()
        {
            string TempImageDirectory = string.Format("{0}\\{1}\\{2}\\Temporary Files", appData, schoolDirectory, appDirectory);

            if (!Directory.Exists(TempImageDirectory))
            {
                Directory.CreateDirectory(TempImageDirectory);
            }

            return TempImageDirectory;
        }

        private void btnRightOne_Click(object sender, RoutedEventArgs e)
        {
            string templateName = Guid.NewGuid().ToString();
            string rightOne = string.Format("{0}\\{1}.bmp", GetTempImageDir(), templateName);

            //Set Active Finger as Lefthumb
            activeFinger = new ActiveFinger { Finger = "rightOne", Url = rightOne };

            FingerObject = new ZKFPEngXClass();
            FingerObject.InitEngine();
            FingerObject.ImageWidth = 35;
            FingerObject.ImageHeight = 45;
            FingerObject.EnrollCount = 1;

            FingerObject.BeginEnroll();

            FingerObject.ControlSensor(11, 0);
            FingerObject.ControlSensor(12, 1);
            FingerObject.ControlSensor(13, 1);
            FingerObject.ControlSensor(13, 0);
            FingerObject.ControlSensor(12, 0);
            FingerObject.ControlSensor(11, 1);

            FingerObject.OnEnroll += FingerObject_OnEnroll;
        }

        public byte[] ImageToByte2(System.Drawing.Bitmap img)
        {
            if (!Directory.Exists(string.Format("{0}\\{1}\\{2}", appData, schoolDirectory, appDirectory)))
            {
                Directory.CreateDirectory(string.Format("{0}\\{1}\\{2}", appData, schoolDirectory, appDirectory));
            }

            byte[] byteArray = new byte[0];

            using (MemoryStream stream = new MemoryStream())
            {
                Int32Rect rect = new Int32Rect(96, 197, 246, 288);
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                img.Save(string.Format("{0}\\{1}\\{2}\\picture.jpg", appData, schoolDirectory, appDirectory), ImageFormat.Jpeg);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }

        private void FingerObject_OnEnroll(bool ActionResult, object ATemplate)
        {
            if (activeFinger.Finger == "rightOne")
            {
                FingerObject.SaveBitmap(activeFinger.Url);

                //Save fingerprints to memory
                right_1 = ImageToByte2(new System.Drawing.Bitmap(activeFinger.Url));

                imageRightOne.Source = new BitmapImage(new Uri(activeFinger.Url));

                CheckForDuplicateFingerprint(right_1);
            }

            if (activeFinger.Finger == "rightTwo")
            {
                FingerObject.SaveBitmap(activeFinger.Url);

                //Save fingerprints to memory
                right_2 = ImageToByte2(new System.Drawing.Bitmap(activeFinger.Url));

                imageRightTwo.Source = new BitmapImage(new Uri(activeFinger.Url));
                CheckForDuplicateFingerprint(right_2);

            }

            if (activeFinger.Finger == "rightThree")
            {
                FingerObject.SaveBitmap(activeFinger.Url);

                //Save fingerprints to memory
                right_3 = ImageToByte2(new System.Drawing.Bitmap(activeFinger.Url));

                imageRightThree.Source = new BitmapImage(new Uri(activeFinger.Url));
                CheckForDuplicateFingerprint(right_3);

            }

            if (activeFinger.Finger == "rightFour")
            {
                FingerObject.SaveBitmap(activeFinger.Url);

                //Save fingerprints to memory
                right_4 = ImageToByte2(new System.Drawing.Bitmap(activeFinger.Url));

                imageRightFour.Source = new BitmapImage(new Uri(activeFinger.Url));
                CheckForDuplicateFingerprint(right_4);

            }

            if (activeFinger.Finger == "rightFive")
            {
                FingerObject.SaveBitmap(activeFinger.Url);

                //Save fingerprints to memory
                right_5 = ImageToByte2(new System.Drawing.Bitmap(activeFinger.Url));

                imageRightFive.Source = new BitmapImage(new Uri(activeFinger.Url));
                CheckForDuplicateFingerprint(right_5);

            }

            if (activeFinger.Finger == "leftOne")
            {
                FingerObject.SaveBitmap(activeFinger.Url);

                //Save fingerprints to memory
                left_1 = ImageToByte2(new System.Drawing.Bitmap(activeFinger.Url));

                imageLeftOne.Source = new BitmapImage(new Uri(activeFinger.Url));
                CheckForDuplicateFingerprint(left_1);

            }

            if (activeFinger.Finger == "leftTwo")
            {
                FingerObject.SaveBitmap(activeFinger.Url);

                //Save fingerprints to memory
                left_2 = ImageToByte2(new System.Drawing.Bitmap(activeFinger.Url));

                imageLeftTwo.Source = new BitmapImage(new Uri(activeFinger.Url));
                CheckForDuplicateFingerprint(left_2);

            }
            if (activeFinger.Finger == "leftThree")
            {
                FingerObject.SaveBitmap(activeFinger.Url);

                //Save fingerprints to memory
                left_3 = ImageToByte2(new System.Drawing.Bitmap(activeFinger.Url));

                imageLeftThree.Source = new BitmapImage(new Uri(activeFinger.Url));
                CheckForDuplicateFingerprint(left_3);

            }

            if (activeFinger.Finger == "leftFour")
            {
                FingerObject.SaveBitmap(activeFinger.Url);

                //Save fingerprints to memory
                left_4 = ImageToByte2(new System.Drawing.Bitmap(activeFinger.Url));

                imageLeftFour.Source = new BitmapImage(new Uri(activeFinger.Url));
                CheckForDuplicateFingerprint(left_4);

            }

            if (activeFinger.Finger == "leftFive")
            {
                FingerObject.SaveBitmap(activeFinger.Url);

                //Save fingerprints to memory
                left_5 = ImageToByte2(new System.Drawing.Bitmap(activeFinger.Url));

                imageLeftFive.Source = new BitmapImage(new Uri(activeFinger.Url));
                CheckForDuplicateFingerprint(left_5);
            }

            FingerObject.ControlSensor(11, 0);
        }

        private void CheckForDuplicateFingerprint(byte[] right_1)
        {
            //Check for Duplicate
            var probe = new Person();
            probe.Id = -1;
            probe.Fingerprints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = new BitmapImage(new Uri(activeFinger.Url)) });

            Afis.Extract(probe);

            Person match = (Person)(Afis.Identify(probe, personsDatabase).FirstOrDefault());

            if (match != null)
            {
                var matchVoter = db.Voters.Where(r => r.VoterID == match.Id).FirstOrDefault();
                MessageBox.Show(string.Format("Fingerprints Match found for: {0} {1} {2}", matchVoter.FirstName, matchVoter.Surname, matchVoter.OtherNames));
                fingerprintMatchFound = true;
                return;
            }

            else
            {
                fingerprintMatchFound = false;
                //personsDatabase.Add(probe);
            }
        }

        private void btnRightTwo_Click(object sender, RoutedEventArgs e)
        {
            string templateName = Guid.NewGuid().ToString();
            string rightTwo = string.Format("{0}\\{1}.bmp", GetTempImageDir(), templateName);

            //Set Active Finger as Lefthumb
            activeFinger = new ActiveFinger { Finger = "rightTwo", Url = rightTwo };

            FingerObject = new ZKFPEngXClass();
            FingerObject.InitEngine();
            FingerObject.ImageWidth = 35;
            FingerObject.ImageHeight = 45;
            FingerObject.EnrollCount = 1;

            FingerObject.BeginEnroll();

            FingerObject.ControlSensor(11, 0);
            FingerObject.ControlSensor(12, 1);
            FingerObject.ControlSensor(13, 1);
            FingerObject.ControlSensor(13, 0);
            FingerObject.ControlSensor(12, 0);
            FingerObject.ControlSensor(11, 1);

            FingerObject.OnEnroll += FingerObject_OnEnroll;
        }

        private void btnRightThree_Click(object sender, RoutedEventArgs e)
        {
            string templateName = Guid.NewGuid().ToString();
            string rightThree = string.Format("{0}\\{1}.bmp", GetTempImageDir(), templateName);

            //Set Active Finger as Lefthumb
            activeFinger = new ActiveFinger { Finger = "rightThree", Url = rightThree };

            FingerObject = new ZKFPEngXClass();
            FingerObject.InitEngine();
            FingerObject.ImageWidth = 35;
            FingerObject.ImageHeight = 45;
            FingerObject.EnrollCount = 1;

            FingerObject.BeginEnroll();

            FingerObject.ControlSensor(11, 0);
            FingerObject.ControlSensor(12, 1);
            FingerObject.ControlSensor(13, 1);
            FingerObject.ControlSensor(13, 0);
            FingerObject.ControlSensor(12, 0);
            FingerObject.ControlSensor(11, 1);

            FingerObject.OnEnroll += FingerObject_OnEnroll;
        }

        private void btnRightFour_Click(object sender, RoutedEventArgs e)
        {
            string templateName = Guid.NewGuid().ToString();
            string rightFour = string.Format("{0}\\{1}.bmp", GetTempImageDir(), templateName);

            //Set Active Finger as Lefthumb
            activeFinger = new ActiveFinger { Finger = "rightFour", Url = rightFour };

            FingerObject = new ZKFPEngXClass();
            FingerObject.InitEngine();
            FingerObject.ImageWidth = 35;
            FingerObject.ImageHeight = 45;
            FingerObject.EnrollCount = 1;

            FingerObject.BeginEnroll();

            FingerObject.ControlSensor(11, 0);
            FingerObject.ControlSensor(12, 1);
            FingerObject.ControlSensor(13, 1);
            FingerObject.ControlSensor(13, 0);
            FingerObject.ControlSensor(12, 0);
            FingerObject.ControlSensor(11, 1);

            FingerObject.OnEnroll += FingerObject_OnEnroll;
        }

        private void btnRightFive_Click(object sender, RoutedEventArgs e)
        {
            string templateName = Guid.NewGuid().ToString();
            string rightFive = string.Format("{0}\\{1}.bmp", GetTempImageDir(), templateName);

            //Set Active Finger as Lefthumb
            activeFinger = new ActiveFinger { Finger = "rightFive", Url = rightFive };

            FingerObject = new ZKFPEngXClass();
            FingerObject.InitEngine();
            FingerObject.ImageWidth = 35;
            FingerObject.ImageHeight = 45;
            FingerObject.EnrollCount = 1;

            FingerObject.BeginEnroll();

            FingerObject.ControlSensor(11, 0);
            FingerObject.ControlSensor(12, 1);
            FingerObject.ControlSensor(13, 1);
            FingerObject.ControlSensor(13, 0);
            FingerObject.ControlSensor(12, 0);
            FingerObject.ControlSensor(11, 1);

            FingerObject.OnEnroll += FingerObject_OnEnroll;
        }

        private void btnLeftOne_Click(object sender, RoutedEventArgs e)
        {
            string templateName = Guid.NewGuid().ToString();
            string leftOne = string.Format("{0}\\{1}.bmp", GetTempImageDir(), templateName);

            //Set Active Finger as Lefthumb
            activeFinger = new ActiveFinger { Finger = "leftOne", Url = leftOne };

            FingerObject = new ZKFPEngXClass();
            FingerObject.InitEngine();
            FingerObject.ImageWidth = 35;
            FingerObject.ImageHeight = 45;
            FingerObject.EnrollCount = 1;

            FingerObject.BeginEnroll();

            FingerObject.ControlSensor(11, 0);
            FingerObject.ControlSensor(12, 1);
            FingerObject.ControlSensor(13, 1);
            FingerObject.ControlSensor(13, 0);
            FingerObject.ControlSensor(12, 0);
            FingerObject.ControlSensor(11, 1);

            FingerObject.OnEnroll += FingerObject_OnEnroll;
        }

        private void btnLeftTwo_Click(object sender, RoutedEventArgs e)
        {
            string templateName = Guid.NewGuid().ToString();
            string leftTwo = string.Format("{0}\\{1}.bmp", GetTempImageDir(), templateName);

            //Set Active Finger as Lefthumb
            activeFinger = new ActiveFinger { Finger = "leftTwo", Url = leftTwo };

            FingerObject = new ZKFPEngXClass();
            FingerObject.InitEngine();
            FingerObject.ImageWidth = 35;
            FingerObject.ImageHeight = 45;
            FingerObject.EnrollCount = 1;

            FingerObject.BeginEnroll();

            FingerObject.ControlSensor(11, 0);
            FingerObject.ControlSensor(12, 1);
            FingerObject.ControlSensor(13, 1);
            FingerObject.ControlSensor(13, 0);
            FingerObject.ControlSensor(12, 0);
            FingerObject.ControlSensor(11, 1);

            FingerObject.OnEnroll += FingerObject_OnEnroll;
        }

        private void btnLeftThree_Click(object sender, RoutedEventArgs e)
        {
            string templateName = Guid.NewGuid().ToString();

            string leftThree = string.Format("{0}\\{1}.bmp", GetTempImageDir(), templateName);

            //Set Active Finger as Lefthumb
            activeFinger = new ActiveFinger { Finger = "leftThree", Url = leftThree };

            FingerObject = new ZKFPEngXClass();
            FingerObject.InitEngine();
            FingerObject.ImageWidth = 35;
            FingerObject.ImageHeight = 45;
            FingerObject.EnrollCount = 1;

            FingerObject.BeginEnroll();

            FingerObject.ControlSensor(11, 0);
            FingerObject.ControlSensor(12, 1);
            FingerObject.ControlSensor(13, 1);
            FingerObject.ControlSensor(13, 0);
            FingerObject.ControlSensor(12, 0);
            FingerObject.ControlSensor(11, 1);

            FingerObject.OnEnroll += FingerObject_OnEnroll;
        }

        private void btnLeftFour_Click(object sender, RoutedEventArgs e)
        {
            string templateName = Guid.NewGuid().ToString();

            string leftFour = string.Format("{0}\\{1}.bmp", GetTempImageDir(), templateName);

            //Set Active Finger as Lefthumb
            activeFinger = new ActiveFinger { Finger = "leftFour", Url = leftFour };

            FingerObject = new ZKFPEngXClass();
            FingerObject.InitEngine();
            FingerObject.ImageWidth = 35;
            FingerObject.ImageHeight = 45;
            FingerObject.EnrollCount = 1;

            FingerObject.BeginEnroll();

            FingerObject.ControlSensor(11, 0);
            FingerObject.ControlSensor(12, 1);
            FingerObject.ControlSensor(13, 1);
            FingerObject.ControlSensor(13, 0);
            FingerObject.ControlSensor(12, 0);
            FingerObject.ControlSensor(11, 1);

            FingerObject.OnEnroll += FingerObject_OnEnroll;
        }

        private void btnLeftFive_Click(object sender, RoutedEventArgs e)
        {
            string templateName = Guid.NewGuid().ToString();

            string leftFive = string.Format("{0}\\{1}.bmp", GetTempImageDir(), templateName);

            //Set Active Finger as Lefthumb
            activeFinger = new ActiveFinger { Finger = "leftFive", Url = leftFive };

            FingerObject = new ZKFPEngXClass();
            FingerObject.InitEngine();
            FingerObject.ImageWidth = 35;
            FingerObject.ImageHeight = 45;
            FingerObject.EnrollCount = 1;

            FingerObject.BeginEnroll();

            FingerObject.ControlSensor(11, 0);
            FingerObject.ControlSensor(12, 1);
            FingerObject.ControlSensor(13, 1);
            FingerObject.ControlSensor(13, 0);
            FingerObject.ControlSensor(12, 0);
            FingerObject.ControlSensor(11, 1);

            FingerObject.OnEnroll += FingerObject_OnEnroll;
        }

        private void btnSelectCamera_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CameraSelect CameraWindow = new CameraSelect(videoDevices);
                CameraWindow.ShowDialog();

                if (App.Current.Properties["SelectedCameraIndex"] != null)
                {
                    LoadCamera((int)(App.Current.Properties["SelectedCameraIndex"]));
                    ConnectCamera();
                }

                App.Current.Properties["SelectedCameraIndex"] = null;
            }

            catch (Exception)
            {
                MessageBox.Show("Error Connecting Camera Device. Please select a valid camera");
            }
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {

            //Validate Entries
            if (!ValidateUI())
            {
                return;
            }

            try
            {
                if (fingerprintMatchFound)
                {
                    MessageBox.Show("Duplicate fingerprints not rectified! Please re-enroll");
                    return;
                }

                if ((DateTime.Now.Year - dateOfBirthPicker.SelectedDate.Value.Year) < 18)
                {
                    MessageBox.Show("Voter Under Age! Minimum of 18-year-olds can vote.");
                    return;
                }

                Voter voter = new Voter()
                {
                    FirstName = firstNameTextBox.Text,
                    Surname = surnameTextBox.Text,
                    OtherNames = otherNamesTextBox.Text,
                    PhoneNumber = phoneNumberTextBox.Text,
                    VoterGuid = Guid.NewGuid().ToString(),
                    DateOfBirth = DateTime.Parse(dateOfBirthPicker.Text.ToString()),
                    SexID = ((Sex)sexComboBox.SelectedItem).SexID,
                    StateID = ((State)stateOfOriginCombobox.SelectedItem).StateID,
                    LgaID = ((Lga)lgaOfOriginCombobox.SelectedItem).LgaID,
                    WardID = ((Ward)wardCombobox.SelectedItem).WardID,
                    PollingUnitID = ((PollingUnit)pollingUnitCombobox.SelectedItem).PollingUnitID,
                    MaritalStatusID = ((MaritalStatus)maritalComboBox.SelectedItem).MaritalStatusID,
                    NIN = ninTextBox.Text,
                    Occupation = occupationTextBox.Text,
                    Age = (DateTime.Now.Year - dateOfBirthPicker.SelectedDate.Value.Year),
                    PassportID = passportTextBox.Text,
                    AddedBy = ActiveUser.UserName,
                    DateAdded = DateTime.Now,
                    HomeAddress = addressTextBox.Text,
                    PhotoFront = userPicture
                };

                db.Voters.Add(voter);
                await db.SaveChangesAsync();

                Models.Fingerprint voterFingerPrints = new Models.Fingerprint()
                {
                    VoterID = voter.VoterID,
                    LeftOne = left_1,
                    LeftTwo = left_2,
                    LeftThree = left_3,
                    LeftFour = left_4,
                    LeftFive = left_5,
                    RightOne = right_1,
                    RightTwo = right_2,
                    RightThree = right_3,
                    RightFour = right_4,
                    RightFive = right_5
                };

                db.Fingerprints.Add(voterFingerPrints);
                await db.SaveChangesAsync();

                MessageBox.Show("Entry Successfully Submited");

                var fingerPrints = new List<SourceAFIS.Simple.Fingerprint>();

                if (voterFingerPrints.LeftOne != null)
                {
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(voterFingerPrints.LeftOne) });
                }

                if (voterFingerPrints.LeftTwo != null)
                {
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(voterFingerPrints.LeftTwo) });
                }

                if (voterFingerPrints.LeftThree != null)
                {
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(voterFingerPrints.LeftTwo) });
                }

                if (voterFingerPrints.LeftFour != null)
                {
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(voterFingerPrints.LeftTwo) });
                }
                if (voterFingerPrints.LeftFive != null)
                {
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(voterFingerPrints.LeftTwo) });
                }

                if (voterFingerPrints.RightOne != null)
                {
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(voterFingerPrints.RightOne) });
                }

                if (voterFingerPrints.RightTwo != null)
                {
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(voterFingerPrints.RightTwo) });
                }

                if (voterFingerPrints.RightThree != null)
                {
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(voterFingerPrints.LeftTwo) });
                }

                if (voterFingerPrints.RightFour != null)
                {
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(voterFingerPrints.LeftTwo) });
                }

                if (voterFingerPrints.RightFive != null)
                {
                    fingerPrints.Add(new SourceAFIS.Simple.Fingerprint() { AsBitmapSource = ConvertFromBytes(voterFingerPrints.LeftTwo) });
                }

                var person = new Person();
                person.Id = voterFingerPrints.VoterID;
                person.Fingerprints = fingerPrints;

                Afis.Extract(person);
                personsDatabase.Add(person);

                ResetCamera();
                ClearAndResetFields();
            }
            catch (Exception)
            {

            }
        }

        private bool ValidateUI()
        {
            //if (userPicture == null)
            //{
            //    MessageBox.Show("Please capture a photograph!");
            //    return false;
            //}

            if (left_5 == null)
            {
                MessageBox.Show("Please Enroll Left Little Finger!");
                return false;
            }

            if (left_4 == null)
            {
                MessageBox.Show("Please Enroll Left Ring Finger!");
                return false;
            }

            if (left_3 == null)
            {
                MessageBox.Show("Please Enroll Middle Left Finger!");
                return false;
            }

            if (left_2 == null)
            {
                MessageBox.Show("Please Enroll Left Index Finger!");
                return false;
            }

            if (left_1 == null)
            {
                MessageBox.Show("Please Enroll Left Thumb!");
                return false;
            }

            if (right_5 == null)
            {
                MessageBox.Show("Please Enroll Right Little Finger!");
                return false;
            }

            if (right_4 == null)
            {
                MessageBox.Show("Please Enroll Right Ring Finger!");
                return false;
            }

            if (right_3 == null)
            {
                MessageBox.Show("Please Enroll Middle Right Finger!");
                return false;
            }

            if (right_2 == null)
            {
                MessageBox.Show("Please Enroll Right Index Finger!");
                return false;
            }

            if (right_1 == null)
            {
                MessageBox.Show("Please Enroll Right Thumb!");
                return false;
            }

            if (string.IsNullOrEmpty(firstNameTextBox.Text) || string.IsNullOrWhiteSpace(firstNameTextBox.Text))
            {
                MessageBox.Show("The field First Name is Required!");
                return false;
            }

            if (string.IsNullOrEmpty(surnameTextBox.Text) || string.IsNullOrWhiteSpace(surnameTextBox.Text))
            {
                MessageBox.Show("The field Surname is Required!");
                return false;
            }

            if (string.IsNullOrEmpty(addressTextBox.Text) || string.IsNullOrWhiteSpace(addressTextBox.Text))
            {
                MessageBox.Show("The field Home Address is Required!");
                return false;
            }

            if (string.IsNullOrEmpty(occupationTextBox.Text) || string.IsNullOrWhiteSpace(occupationTextBox.Text))
            {
                MessageBox.Show("The field Occupation is Required!");
                return false;
            }

            if (string.IsNullOrEmpty(ageTextBox.Text) || string.IsNullOrWhiteSpace(ageTextBox.Text))
            {
                MessageBox.Show("The field Date of Birth is Required!");
                return false;
            }

            if (dateOfBirthPicker.SelectedDate == null)
            {
                MessageBox.Show("The field Date of Birth is Required!");
                return false;
            }

            if (string.IsNullOrEmpty(phoneNumberTextBox.Text) || string.IsNullOrWhiteSpace(phoneNumberTextBox.Text))
            {
                MessageBox.Show("The field Phone Number is Required!");
                return false;
            }

            if (pollingUnitCombobox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Polling Unit");
                return false;
            }

            if (lgaOfOriginCombobox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a LGA");
                return false;
            }

            if (stateOfOriginCombobox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a State of Origin");
                return false;
            }

            if (wardCombobox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Ward");
                return false;
            }

            if (maritalComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Marital Status");
                return false;
            }

            if (sexComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Gender");
                return false;
            }

            return true;
        }

        private async void stateOfOriginCombobox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedItem = (State)stateOfOriginCombobox.SelectedItem;
                lgaOfOriginCombobox.DataContext = await db.LGAs.Where(r => r.StateID == selectedItem.StateID).ToListAsync();
            }
            catch (Exception)
            {

            }
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            //Give Admin Explicit Access
            string userName = txtUserName.Text;
            string password = txtPassword.Password;

            activeUser = new ApplicationUser();

            //Check for Username in the Database;

            if (userName.Trim() == "admin" && password == "password")
            {
                //Give access by Default!
                activeUser = new ApplicationUser() { UserName = "admin", IsAdmin = true };
            }

            else
            {
                var match = await db.ApplicationUsers.Where(user => user.UserName.Trim() == userName.Trim()).FirstOrDefaultAsync();

                if (match == null)
                {
                    MessageBox.Show("Username Invalid or Password! Please try again.");
                    return;
                }

                if (Convert.ToBase64String(Encoding.UTF8.GetBytes((password.Trim()))) != match.Password)
                {
                    MessageBox.Show("Password Invalid or Password! Please try again.");
                    return;
                }

                activeUser = match;
            }

            //If we got here, the password and username are both correct.
            //Load up the Active Application user tracker
            App.Current.Properties["CurrentLogin"] = activeUser;

            //Also Hide the border and clear textboxes;
            txtUserName.Clear();
            txtPassword.Clear();

            MessageBox.Show("Welcome. Thank you for Login in");

            CloseBorder();
        }

        private void CloseBorder()
        {
            if (mainBorder.Visibility == Visibility.Visible)
            {
                mainBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void OpenBorder()
        {
            if (mainBorder.Visibility == Visibility.Collapsed)
            {
                mainBorder.Visibility = Visibility.Visible;
            }
        }

        private void addOfficerMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (activeUser.IsAdmin)
                {
                    OpenModalBorder();
                    OfficerRegistration officerRegistrationWindow = new OfficerRegistration();
                    officerRegistrationWindow.ShowDialog();
                    CloseModalBorder();
                }

                else
                {
                    MessageBox.Show("Administrative Previledges Required!");
                    return;
                }

            }
            catch (Exception)
            {
                CloseModalBorder();
            }
        }

        private void logoutMenu_Click(object sender, RoutedEventArgs e)
        {
            //Remove active user from Settings
            //Bring back canvas

            OpenBorder();
            Window_Loaded(null, null);
        }

        private void OpenModalBorder()
        {
            if (modalBorder.Visibility == Visibility.Collapsed)
            {
                modalBorder.Visibility = Visibility.Visible;
            }
        }

        private void CloseModalBorder()
        {
            if (modalBorder.Visibility == Visibility.Visible)
            {
                modalBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void viewVoter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenModalBorder();
                AllVotersWindow allVotersWindow = new AllVotersWindow();
                allVotersWindow.ShowDialog();
                CloseModalBorder();

            }
            catch (Exception)
            {
                CloseModalBorder();
            }
        }

        private void phoneNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (phoneNumberTextBox.Text.Length > 11)
            {
                MessageBox.Show("Phone Number Cannot be less than 11 chracters");
                phoneNumberTextBox.Text = phoneNumberTextBox.Text.Substring(0, 11);
                return;
            }
        }

        private async void lgaOfOriginCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedItem = (Lga)lgaOfOriginCombobox.SelectedItem;
                wardCombobox.DataContext = await db.Wards.Where(r => r.LgaID == selectedItem.LgaID).ToListAsync();
            }
            catch (Exception)
            {

            }
        }

        private void printVoters_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenModalBorder();
                PreviewVotersWindow preview = new PreviewVotersWindow();
                preview.ShowDialog();
                CloseModalBorder();

            }
            catch (Exception)
            {
                CloseModalBorder();
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void wardCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedItem = (Ward)wardCombobox.SelectedItem;
                pollingUnitCombobox.DataContext = await db.PollingUnits.Where(r => r.WardID == selectedItem.WardID).ToListAsync();
            }
            catch (Exception)
            {

            }
        }

        private void dateOfBirthPicker_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ageTextBox.Text = (DateTime.Now.Year - dateOfBirthPicker.SelectedDate.Value.Year).ToString();
            }
            catch (Exception)
            {
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            fingerprintMatchFound = false;
            ClearAndResetFields();
        }
    }
}
