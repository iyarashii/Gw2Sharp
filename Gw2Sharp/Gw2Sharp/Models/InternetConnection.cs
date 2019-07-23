using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net;
using Xamarin.Forms;

namespace Gw2Sharp.Models
{
    public static class InternetConnection
    {
        // initializing a readonly instance of HttpClient to give app access to http get requests
        public static readonly HttpClient client = new HttpClient();

        // field that stores the result of internet connection test
        public static bool connection;

        // method used for checking internet connection by trying to open readable stream from site
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
        /// Checks internet connection by using CheckWebResponse method and changes text of label given as parameter if no internet connection is available.
        /// </summary>
        /// <param name="labelsTextProperty">Labels text property which will be changed when no internet connection is available.</param>
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
