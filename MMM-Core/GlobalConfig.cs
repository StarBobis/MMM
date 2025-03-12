using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MMM_Core
{
    public class ConfigLoader<T> where T : BaseConfig, new()
    {
        public string SavePath { get; set; }
        public string LoadPath { get; set; }
        public T Value { get; set; }

        public ConfigLoader(string path)
        {
            LoadPath = path;
            SavePath = path;
        }

        public ConfigLoader(string loadPath, string savePath)
        {
            LoadPath = loadPath;
            SavePath = savePath;
        }

        public void LoadConfig()
        {
            if (string.IsNullOrEmpty(LoadPath))
            {
                throw new Exception("SavePath of" + this.GetType().Name + "is null");
            }
            if (!File.Exists(LoadPath))
            {
                // ����ļ������ڣ�����һ���µ������ļ�
                Value = new T();
                SaveConfig();
                //throw new Exception("Config file not found:" + LoadPath);
            }
            try
            {
                string json = File.ReadAllText(LoadPath);
                // ��ȡ�ļ�����,��ת��ΪT����,Ȼ��ֵ����ǰ����
                Value = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                ex.ToString();
                //����û���Ϊϵͳ��������DBMT�����ļ��𻵣����Ǿ͸��������ļ�
                Value = new T();
                SaveConfig();
                //���Ǻ������¶�ȡ
                string json = File.ReadAllText(LoadPath);
                // ��ȡ�ļ�����,��ת��ΪT����,Ȼ��ֵ����ǰ����
                Value = JsonConvert.DeserializeObject<T>(json);
            }

        }

        public void SaveConfig(string SpecialSavePath = "")
        {
            if (string.IsNullOrEmpty(SavePath))
            {
                throw new Exception("SavePath of" + this.GetType().Name + "is null");
            }
            string jsonString = JsonConvert.SerializeObject(Value, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(SavePath, jsonString);

        }
    }
    public class BaseConfig { }

    public class MainSetting : BaseConfig
    {
        public string GameName { get; set; } = "GI";

        public string CurrentGameMigotoFolder { get; set; } = "";

    }

    public static class GlobalConfig
    {
        // ���ػ��洢������
        public static readonly ConfigLoader<MainSetting> SettingCfg = new ConfigLoader<MainSetting>(Path_MMMSettings);

        public static string Path_Base
        {
            get { return Directory.GetCurrentDirectory(); }
        }

        public static string Path_AppDataLocal
        {
            get
            { // �������Ҫ�����������ļ�·����AppData\Local����������������
                string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return localAppDataPath;
            }
        }

        public static string Path_MMMSettings
        {
            get { return Path.Combine(Path_AppDataLocal, "MMM-Settings.json"); }
        }

        public static string Path_ConfigsFolder
        {
            get { return Path.Combine(Path_Base, "Configs\\"); }
        }

        public static string Path_CurrentGameConfigsFolder
        {
            get { return Path.Combine(Path_ConfigsFolder, GlobalConfig.SettingCfg.Value.GameName + "\\"); }
        }


        public static string Path_ModsFolder
        {
            get { return Path.Combine(Path_Base, "Mods\\"); }
        }

        public static string Path_PluginsFolder
        {
            get { return Path.Combine(Path_Base, "Plugins\\"); }
        }

        public static string Path_7ZipExe
        {
            get { return Path.Combine(Path_PluginsFolder, "7-Zip\\7z.exe"); }
        }

        public static string Path_CurrentGameMainConfigJsonFile
        {
            get { return Path.Combine(Path_ConfigsFolder, GlobalConfig.SettingCfg.Value.GameName + "\\MainConfig.json"); }
        }

        //Path_D3DXINI
        public static string Path_D3DXINI
        {
            get { return Path.Combine(GlobalConfig.SettingCfg.Value.CurrentGameMigotoFolder, "d3dx.ini"); }
        }

        //Path_3DmigotoGameModForkFolder

        public static string Path_3DmigotoGameModForkFolder
        {
            get { return Path.Combine(Path_Base, "3Dmigoto-GameMod-Fork\\"); }
        }

        //Path_LoaderFolder
        public static string Path_LoaderFolder
        {
            get { return GlobalConfig.SettingCfg.Value.CurrentGameMigotoFolder; }
        }

    }

}
