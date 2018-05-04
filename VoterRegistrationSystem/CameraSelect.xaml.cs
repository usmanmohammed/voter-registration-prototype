using AForge.Video.DirectShow;
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

namespace VoterRegistrationSystem
{
    /// <summary>
    /// Interaction logic for CameraSelect.xaml
    /// </summary>
    public partial class CameraSelect : Window
    {
        private FilterInfoCollection videoDevices;
        private List<string> VideoDevices;

        public CameraSelect(FilterInfoCollection videoDevices)
        {
            InitializeComponent();

            this.videoDevices = videoDevices;
        }

        private void cameraComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cameraComboBox.SelectedIndex != -1)
            {
                App.Current.Properties["SelectedCameraIndex"] = cameraComboBox.SelectedIndex;

                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            VideoDevices = new List<string>();

            for (int i = 0; i < videoDevices.Count; i++)
            {
                VideoDevices.Add(videoDevices[i].Name);
            }

            cameraComboBox.ItemsSource = VideoDevices;
        }
    }
}
