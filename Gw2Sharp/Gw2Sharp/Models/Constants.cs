using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gw2Sharp.Models
{
    public class Constants
    {
        static public string ItemDBPath { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "itemDB.txt"); } }
    }
}
