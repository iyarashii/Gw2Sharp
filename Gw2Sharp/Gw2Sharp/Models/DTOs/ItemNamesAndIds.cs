using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Gw2Sharp.Models.DTOs
{
    public class ItemNamesAndIds
    {
        [Collation("NOCASE")]
        public string name { get; set; }

        [PrimaryKey]
        public int id { get; set; }
    }
}
