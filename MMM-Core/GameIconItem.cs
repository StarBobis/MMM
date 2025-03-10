using Microsoft.UI.Xaml.Media.Imaging;
using Newtonsoft.Json.Linq;
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

        public string MigotoFolder { get; set; } = "";

        /// <summary>
        /// 这个是后续初始化时动态分配的字段，用于缓存在内存中防止每次都new新的导致视觉延迟。
        /// </summary>
        public BitmapImage GameBackGroundImage { get; set; } = new BitmapImage();


        public void SaveToJson(string JsonFilePath)
        {
            JObject jObject = MMMJsonUtils.CreateJObject();

            jObject["GameIconImage"] = GameIconImage;
            jObject["GameName"] = GameName;
            jObject["MigotoFolder"] = MigotoFolder;

            MMMJsonUtils.SaveJObjectToFile(jObject, JsonFilePath);

        }

    }
}
