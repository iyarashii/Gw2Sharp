using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net;

namespace Gw2Sharp.Source
{
    class InternetConnection
    {
        public static readonly HttpClient client = new HttpClient();
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
