// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Gw2Sharp/blob/master/LICENSE for license details.

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
