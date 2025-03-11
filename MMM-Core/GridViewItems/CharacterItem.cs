using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMM_Core
{
    public class CharacterItem
    {
        public string CharacterName { get; set; } = "";
        public string Category { get; set; } = "";

        public string CharacterImage { get; set; } = "";
        public string BackgroundImage { get; set; } = "";

        /// <summary>
        /// 实时读取对应文件夹下面的数量
        /// </summary>
        public string ModNumber { get; set; } = "";


    }

}
