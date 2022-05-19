// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Gw2Sharp/blob/master/LICENSE for license details.

namespace Gw2Sharp.Models.DTOs
{
    class ItemTpPrice
    {
        public class Buys
        {
            public int quantity { get; set; }
            public int unit_price { get; set; }
        }

        public class Sells
        {
            public int quantity { get; set; }
            public int unit_price { get; set; }
        }

        public class RootObject
        {
            public int id { get; set; }
            public bool whitelisted { get; set; }
            public Buys buys { get; set; }
            public Sells sells { get; set; }
        }
    }
}
