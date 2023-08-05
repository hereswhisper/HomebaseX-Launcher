// HomebaseX Updater

using HomebaseXUpdater;
using SharpCompress.Archives.Rar;

string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
string exeDirectory = System.IO.Path.GetDirectoryName(exePath);

string BASE_URL = "http://147.185.221.16:15316";

Console.WriteLine("Download starting in 3 seconds...");

Thread.Sleep(3000);
Console.Clear();

Console.WriteLine("Installing HomebaseX Update, please wait...");

using (HttpClient client = new HttpClient())
{
    using (HttpResponseMessage response = await client.GetAsync(BASE_URL + "/ot/download/win.rar", HttpCompletionOption.ResponseHeadersRead))
    {
        response.EnsureSuccessStatusCode();

        using (var fileStream = new FileStream(exeDirectory + "/win.rar", FileMode.Create, FileAccess.Write))
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
                    //UpdateProgress(progressPercentage);
                }
            }
        }

        Console.WriteLine("Extracting HomebaseX Update...");

        RarExtractor.ExtractRARFile(exeDirectory + "/win.rar", exeDirectory);
        File.Delete(exeDirectory + "/win.rar"); // delete file
    }
}