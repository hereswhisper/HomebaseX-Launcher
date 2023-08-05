using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HomebaseX
{
    class Updater
    {
        public static string defaultUpdateURL = "";
        HttpClient client = new HttpClient();
        public async Task<bool> requireUpdate(string updateURL = "")
        {
            HttpResponseMessage response = await client.GetAsync(updateURL);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                string responseContent = await response.Content.ReadAsStringAsync();
                if (responseContent != null)
                {
                    if (responseContent == "yes")
                    {
                        return true;
                    }
                    else if (responseContent == "no")
                    {
                        return false;
                    }
                    else
                    {
                        //MessageBox.Show("A server error has occured. Please try again in a few minutes", "HomebaseX Error");
                        return false;
                    }
                }

                //Console.WriteLine($"Response: {responseContent}");
            }
            else
            {
                // Request failed, display the status code and reason phrase
                Console.WriteLine($"Request failed: {response.StatusCode} - {response.ReasonPhrase}");
            }

            return false;
        }
    }
}
