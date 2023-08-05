using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.IO;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using SharpCompress.Archives;
using System.Diagnostics;

namespace HomebaseX
{
    /// <summary>
    /// Interaction logic for MainMenuProd.xaml
    /// </summary>
    public partial class MainMenuProd : Window
    {
        public MainMenuProd()
        {
            InitializeComponent();
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Hide Stuff
            ProgressTitleText.Visibility = Visibility.Hidden;
            progressBar.Visibility = Visibility.Hidden;
            ProgressPrecentText.Visibility = Visibility.Hidden;

            // Username
            UsernameText.Content = "@" + Globals.Username;

            // Role Names
            switch (Globals.Role)
            {
                case "Owner":
                    RoleText.Foreground = Brushes.Gold;
                    RoleText.Content = "Owner";
                    break;
                case "Moderator":
                    RoleText.Foreground = Brushes.Aqua;
                    RoleText.Content = "Moderator";
                    break;
                case "Contributor":
                    RoleText.Foreground = Brushes.Green;
                    RoleText.Content = "Contributor";
                    break;
                case "Member":
                    RoleText.Foreground = Brushes.White;
                    RoleText.Content = "Player";
                    break;
                case "OT1 Tester":
                    RoleText.Foreground = Brushes.Orange;
                    RoleText.Content = "OT1 Tester";
                    break;
                case "Partner":
                    RoleText.Foreground = Brushes.LightBlue;
                    RoleText.Content = "Partner";
                    break;
                case "Verified":
                    RoleText.Foreground = Brushes.Cyan;
                    RoleText.Content = "Verified";
                    break;
                case "ThatDefault":
                    RoleText.Foreground = Brushes.Red;
                    RoleText.Content = "ThatDefault";
                    break;
                default:
                    RoleText.Foreground = Brushes.White;
                    RoleText.Content = "Player";
                    break;
            }

            // Version Tag
            VersionTag.Content = Globals.Version;

            // News
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(Globals.BASE_URL + "/l/content");
                response.EnsureSuccessStatusCode(); // Throws an exception if the request fails

                string jsonString = await response.Content.ReadAsStringAsync();
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);

                // Now you can access the JSON data and use it as needed
                string title = data.Title;
                string description = data.Description;

                // Print the retrieved data
                TitleText.Content = title;
                DescriptionText.Content = description;

                string ImageURL = data.ImageURL;

                // Create a BitmapImage and set the UriSource to the URL
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(ImageURL);
                bitmap.EndInit();

                // Assign the BitmapImage to the Image control
                NewsPicture.Source = bitmap;
            }

            // Checks if Fortnite exists
            string GameFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HomebaseX", "Fortnite");

            if (Directory.Exists(GameFolder))
            {

            }
            else
            {
                DownloadBtn.Content = "Install";
            }

            // Profile Icon
            // TODO ...
        }

        private void SettingsPushed(object sender, RoutedEventArgs e)
        {

        }

        private async void DownloadClicked(object sender, RoutedEventArgs e)
        {
            if (DownloadBtn.Content == "Install")
            {
                //MessageBox.Show("a");
                ProgressTitleText.Visibility = Visibility.Visible;
                progressBar.Visibility = Visibility.Visible;
                ProgressPrecentText.Visibility = Visibility.Visible;
                string FileUrl = "https://cdn.fnbuilds.services/1.8.rar";

                string destinationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HomebaseX", "Fortnite");

                try
                {
                    if (!Directory.Exists(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }

                    string fileName = Path.GetFileName(FileUrl);
                    string filePath = Path.Combine(destinationDirectory, fileName);

                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage response = await client.GetAsync(FileUrl, HttpCompletionOption.ResponseHeadersRead))
                        {
                            response.EnsureSuccessStatusCode();

                            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                            using (var downloadStream = await response.Content.ReadAsStreamAsync())
                            {
                                long? fileSize = response.Content.Headers.ContentLength;

                                byte[] buffer = new byte[8192];
                                int bytesRead;
                                long totalRead = 0;

                                while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                                    totalRead += bytesRead;

                                    // Report progress
                                    if (fileSize.HasValue)
                                    {
                                        int progressPercentage = (int)((double)totalRead / fileSize.Value * 100);
                                        UpdateProgress(progressPercentage);
                                    }
                                }
                            }
                        }

                        ProgressTitleText.Content = "Extracting Fortnite 1.8... *DO NOT CLOSE THE WINDOW TILL COMPLETED*";
                        progressBar.Visibility = Visibility.Hidden;
                        ProgressPrecentText.Visibility = Visibility.Hidden;

                        MessageBox.Show($"Download completed! Please wait as we extract", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        

                        // Extract the downloaded RAR file
                        await Task.Run(() => ExtractRARFile(filePath, destinationDirectory)); // hopefully on different thread

                        File.Delete(destinationDirectory + "\\1.8.rar"); // at the very end
                        ProgressTitleText.Visibility = Visibility.Hidden;
                        DownloadBtn.Content = "Play";
                    }
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show("Error downloading the file: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            } else
            {
                //MessageBox.Show("b");
                // Get the path to the local AppData directory
                string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                // Combine the path with the HomebaseX and Fortnite directories
                string fortniteFolderPath = Path.Combine(localAppDataPath, "HomebaseX", "Fortnite", "Fortnite", "Fortnite");

                //Process process2 = ProcessHelper.StartProcess(fortniteFolderPath + "\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping_BE.exe", true, "");
                Process process3 = ProcessHelper.StartProcess(fortniteFolderPath + "\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe", false, "-AUTH_TYPE=epic -AUTH_LOGIN=\"" + Globals.Email + "\" -AUTH_PASSWORD=\"" + Globals.Password + "\" -SKIPPATCHCHECK");
                process3.WaitForInputIdle();
                process3.WaitForExit();
                //process2.Close();
                process3.Close();
            }
        }

        private void ExtractRARFile(string filePath, string destinationDirectory)
        {
            using (var archive = RarArchive.Open(filePath))
            {
                foreach (var entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        entry.WriteToDirectory(destinationDirectory, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                    }
                }
            }

            MessageBox.Show($"Extraction completed! Fortnite has been successfully installed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateProgress(int progressPercentage)
        {
            // Dispatcher is required to update UI elements from a background thread
            Dispatcher.Invoke(() =>
            {
                progressBar.Value = progressPercentage;
                ProgressPrecentText.Content = $"{progressPercentage}%";
            });
        }
    }
}
