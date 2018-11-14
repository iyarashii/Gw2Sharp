﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2Sharp.Source
{
    class Bag
    {
        public class Details
        {
            public bool no_sell_or_sort { get; set; }
            public int size { get; set; }
        }

        public class RootObject
        {
            public string name { get; set; }
            public string description { get; set; }
            public string type { get; set; }
            public int level { get; set; }
            public string rarity { get; set; }
            public int vendor_value { get; set; }
            public List<string> game_types { get; set; }
            public List<string> flags { get; set; }
            public List<object> restrictions { get; set; }
            public int id { get; set; }
            public string chat_link { get; set; }
            public string icon { get; set; }
            public Details details { get; set; }
        }
    }
}
