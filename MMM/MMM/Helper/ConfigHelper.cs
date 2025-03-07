using MMM_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMM.Helper
{
    public static class ConfigHelper
    {

        public static List<CharacterItem> GetGICharacterItemList()
        {

            List<CharacterItem> characterItemList = new List<CharacterItem>();

            characterItemList.Add(new CharacterItem
            {
                CharacterImage = "Assets/GI/HeroPicture/Funingna.png",
                BackgroundImage = "Assets/GI/HeroBackground/Gold.png",
                Title = "芙宁娜",
                ModNumber = "0"
            });


            return characterItemList;
        }

    }
}
