using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;
using Fiddler;

namespace HomebaseX
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {

        private int currentIndex = 0; // Index of the current image being displayed
        private DispatcherTimer timer; // Timer for automatic image rotation


        public MainMenu()
        {
            InitializeComponent();
            // Initialize the timer with 3 seconds interval
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += Timer_Tick;

            // Display the first image (TabItem)
            ShowTabItem();

            timer.Start();

            
        }

        private void ShowTabItem()
        {
            // Check if the currentIndex is within bounds
            if (currentIndex >= 0 && currentIndex < imageTabControl.Items.Count)
            {
                // Select the current TabItem to display the corresponding image
                imageTabControl.SelectedIndex = currentIndex;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Move to the next image when the timer ticks
            currentIndex++;
            if (currentIndex >= imageTabControl.Items.Count)
                currentIndex = 0;

            ShowTabItem();
        }

        private void imageTabControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Make all TabItems invisible by setting their Visibility to Collapsed
            foreach (TabItem tabItem in imageTabControl.Items)
            {
                tabItem.Visibility = Visibility.Collapsed;
            }
        }

        private void LaunchClicked(object sender, RoutedEventArgs e)
        {

            if(MessageBox.Show("You are about to launch in the HomebaseX OT1 Testing (DO NOT CLOSE THIS LAUNCHER WHILE PLAYING)","HomebaseX",MessageBoxButton.YesNo) ==  MessageBoxResult.Yes)
            {

                Process process2 = ProcessHelper.StartProcess(FortnitePath.Text + "\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping_BE.exe", true, "");
                Process process3 = ProcessHelper.StartProcess(FortnitePath.Text + "\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe", false, "-AUTH_TYPE=epic -AUTH_LOGIN=\"" + Globals.Email + "\" -AUTH_PASSWORD=\"" + Globals.Password + "\" -SKIPPATCHCHECK");
                process3.WaitForInputIdle();
                process3.WaitForExit();
                process2.Close();
                process3.Close();

            }
        }

        // Event handler to inspect and modify outgoing requests
        private static void FiddlerApplication_BeforeRequest(Session session)
        {
            // You can perform various tasks here, such as:
            // - Logging the request information
            // - Modifying the request headers or body
            // - Blocking specific requests

            // For example, to modify the User-Agent header:
            session.oRequest["User-Agent"] = "CustomUserAgent";
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            UsernameText.Content = Globals.Username;
            VersionText.Content = Globals.Version;

            bool APIStatus = await API.pingAPI();

            if(APIStatus)
            {
                APIStatusText.Content = "ONLINE";
                APIStatusText.Foreground = Brushes.LightGreen;
                return;
            } else
            {
                APIStatusText.Content = "OFFLINE";
                APIStatusText.Foreground = Brushes.Red;
            }
        }
    }
}
