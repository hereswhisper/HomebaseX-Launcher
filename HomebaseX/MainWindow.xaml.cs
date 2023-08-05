using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HomebaseX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HttpClient client = new HttpClient();
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Check for updates
            Updater updater = new Updater();
            bool a = await updater.requireUpdate(Globals.BASE_URL + "/ot/checkupdate?version=" + Globals.LVersion);

            if (!a) return;

            // Update Client
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string exeDirectory = System.IO.Path.GetDirectoryName(exePath);

            MessageBox.Show("HomebaseX requires a update, restarting");
            Process process3 = ProcessHelper.StartProcess(exeDirectory + "\\HomebaseXUpdater.exe", false, "");
            Environment.Exit(0); // close
        }

        private async void LaunchClicked(object sender, RoutedEventArgs e)
        {
            string URL1 = Globals.BASE_URL + "/ot/authenticate?otkey=" + OTKey.Text;
            string URL2 = Globals.BASE_URL + "/ot/info?otkey=" + OTKey.Text;

            // Make the GET request
            HttpResponseMessage response = await client.GetAsync(URL1);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                string responseContent = await response.Content.ReadAsStringAsync();
                if (responseContent != null)
                {
                    if (responseContent == "statusfailure")
                    {
                        MessageBox.Show("This OT Key is invalid. This could because of a mistake in typing it. If you don't have one please go to " + Globals.BASE_URL + " and request one!", "HomebaseX Error");
                        return;
                    }
                    else if (responseContent == "statussuccess")
                    {

                    }
                    else
                    {
                        MessageBox.Show("A server error has occured. Please try again in a few minutes", "HomebaseX Error");
                        return;
                    }
                }

                //Console.WriteLine($"Response: {responseContent}");
            }
            else
            {
                // Request failed, display the status code and reason phrase
                Console.WriteLine($"Request failed: {response.StatusCode} - {response.ReasonPhrase}");
            }

            HttpResponseMessage response2 = await client.GetAsync(URL2);

            // Check if the request was successful
            if (response2.IsSuccessStatusCode)
            {
                // Read the response content as a string
                string responseContent = await response2.Content.ReadAsStringAsync();
                if (responseContent != null)
                {
                    if(responseContent == "statusfailure")
                    {
                        MessageBox.Show("This OT Key is invalid. This could because of a mistake in typing it. If you don't have one please go to " + Globals.BASE_URL + " and request one!", "HomebaseX Error");
                        return;
                    } else
                    {
                        String[] info = responseContent.Split("ɐ", 4, StringSplitOptions.RemoveEmptyEntries);
                        Globals.Email = info[0];
                        Globals.Password = info[1];
                        Globals.Username = info[2];
                        Globals.Role = info[3];
                        //MessageBox.Show(Globals.Email);
                        //MessageBox.Show(Globals.Password);
                        //MessageBox.Show(Globals.Username);
                    }
                }

                //Console.WriteLine($"Response: {responseContent}");
            }
            else
            {
                // Request failed, display the status code and reason phrase
                Console.WriteLine($"Request failed: {response2.StatusCode} - {response2.ReasonPhrase}");
            }

            MainMenuProd window = new MainMenuProd();
            window.Show();
        }

        private void DiscordJoin(object sender, RoutedEventArgs e)
        {
            string URL = "https://discord.gg/hMw32VDPwt";
            Process.Start(new ProcessStartInfo
            {
                FileName = URL,
                UseShellExecute = true
            });
        }
    }
}

