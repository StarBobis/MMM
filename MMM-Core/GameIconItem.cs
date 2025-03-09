using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMM_Core
{
    public class GameIconItem
    {
        public string GameIconImage { get; set; } = "";
        public string GameName { get; set; } = "";

        public BitmapImage GameBackGroundImage { get; set; } = new BitmapImage();
    }
}
