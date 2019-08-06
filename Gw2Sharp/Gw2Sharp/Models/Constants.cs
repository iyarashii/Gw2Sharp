using System;
using System.IO;

namespace Gw2Sharp.Models
{
    // utility class with constants
    public static class Constants
    {
        // property that stores path to the local item names and ids database
        public static string ItemDBPath { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "itemDB.db3"); } }
    }
}
