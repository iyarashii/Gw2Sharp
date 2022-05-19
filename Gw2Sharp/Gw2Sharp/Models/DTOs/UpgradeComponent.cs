// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Gw2Sharp/blob/master/LICENSE for license details.

using System.Collections.Generic;

namespace Gw2Sharp.Models.DTOs
{
    class UpgradeComponent
    {
        public class Buff
        {
            public int skill_id { get; set; }
            public string description { get; set; }
        }

        public class Attribute
        {
            public string attribute { get; set; }
            public int modifier { get; set; }
        }

        public class InfixUpgrade
        {
            public int id { get; set; }
            public Buff buff { get; set; }
            public List<Attribute> attributes { get; set; }
        }

        public class Details
        {
            public string type { get; set; }
            public List<string> flags { get; set; }
            public List<string> infusion_upgrade_flags { get; set; }
            public InfixUpgrade infix_upgrade { get; set; }
            public string suffix { get; set; }
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
