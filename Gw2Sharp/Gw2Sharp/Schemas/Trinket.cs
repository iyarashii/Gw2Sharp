using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2Sharp.Schemas
{
    class Trinket
    {
        public class InfusionSlot
        {
            public List<string> flags { get; set; }
        }

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
            public List<InfusionSlot> infusion_slots { get; set; }
            public string secondary_suffix_item_id { get; set; }
            public List<int> stat_choices { get; set; }
        }

        public class RootObject
        {
            public string name { get; set; }
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
