using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net;
using Xamarin.Forms;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;


namespace Gw2Sharp.Pages
{
    public class InternetConnection
    {
        public static readonly HttpClient client = new HttpClient();

        public static bool CheckWebResponse()
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
        public bool CheckForInternetConnection(Label textLabel)
        {
            if (!CheckWebResponse())
            {
                textLabel.Text = "No internet connection!";
                textLabel.IsVisible = true;
                textLabel.BindingContext = this;
                return false;
            }
            return true;
        }
        public void GrantAccess(string fullPath)
        {
            FileInfo dInfo = new FileInfo(fullPath);
            FileSecurity dSecurity = dInfo.GetAccessControl();
            //FileSystemAccessRule fsRule = new FileSystemAccessRule()
            dSecurity.SetAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.Delete, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
        }
    }
}
