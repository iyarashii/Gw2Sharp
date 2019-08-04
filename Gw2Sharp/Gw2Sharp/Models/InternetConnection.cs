using System.Net.Http;
using System.Net;

namespace Gw2Sharp.Models
{
    // responsible for checking application internet connection
    public static class InternetConnection
    {
        // initializing a read-only instance of HttpClient to give the app access to HTTP GET requests
        public static readonly HttpClient client = new HttpClient();

        // field that stores the result of the internet connection test
        public static bool connection;

        // checks internet connection by trying to open readable stream from the site
        public static bool CheckWebResponse()
        {
            
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://www.google.com/"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Checks internet connection by using the CheckWebResponse method and changes the text property given as parameter if no internet connection is available.
        /// </summary>
        /// <param name="labelsTextProperty">Text property of a label which will be changed when no internet connection is available.</param>
        /// <returns></returns>
        public static string CheckForInternetConnection(string labelsTextProperty)
        {
            if (!CheckWebResponse())
            {
                connection = false;
                return "No internet connection!";
            }
            connection = true;
            return labelsTextProperty;
        }
    }
}
