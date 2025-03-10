using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMM_Core
{
    public class ModItem
    {
        public string ModImage { get; set; } = "";
        public string ModName { get; set; } = "";
        public string ModAuthor { get; set; } = "";

        public bool Enable { get; set; } = false;

        public string Readme { get; set; } = "";

        public string SponserWebsite { get; set; } = "";

        public bool Deprecated { get; set; } = false;

        public string ModLoaction { get; set; } = "";

    }
}
