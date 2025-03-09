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

    public class GameConfig : BaseConfig
    {
        //UI Behaviour
        public bool Language { get; set; } = false;
        public double WindowWidth { get; set; } = 1000;
        public double WindowHeight { get; set; } = 600;
        public int WindowPositionX { get; set; } = -1;
        public int WindowPositionY { get; set; } = -1;

        //public float GamePageBackGroundImageOpacity { get; set; } = 0.6f;
        //public float WorkPageBackGroundImageOpacity { get; set; } = 0.3f;
        //public float PluginPageBackGroundImageOpacity { get; set; } = 0.3f;
        //public float TexturePageBackGroundImageOpacity { get; set; } = 0.3f;

        public bool SimpleMode { get; set; } = false;

        public bool StartToWorkPage { get; set; } = false;
        public bool WindowTopMost { get; set; } = false;
        //Others
        public bool AutoCleanFrameAnalysisFolder { get; set; } = true;
        public bool AutoCleanLogFile { get; set; } = true;
        public int FrameAnalysisFolderReserveNumber { get; set; } = 1;
        public int LogFileReserveNumber { get; set; } = 3;

        // ����Mod����

        public string ModSwitchKey { get; set; } = "\"x\",\"c\",\"v\",\"b\",\"n\",\"m\",\"j\",\"k\",\"l\",\"o\",\"p\",\"[\",\"]\",\"x\",\"c\",\"v\",\"b\",\"n\",\"m\",\"j\",\"k\",\"l\",\"o\",\"p\",\"[\",\"]\",\"x\",\"c\",\"v\",\"b\",\"n\",\"m\",\"j\",\"k\",\"l\",\"o\",\"p\",\"[\",\"]\"";


        //Extract Options
        public bool DontSplitModelByMatchFirstIndex { get; set; } = false;

        //Texture Options
        public bool AutoTextureOnlyConvertDiffuseMap { get; set; } = true;
        public int AutoTextureFormat { get; set; } = 0;
        public bool AutoDetectAndMarkTexture { get; set; } = true;

        public string DBMT_Protect_ACLFolderPath { get; set; } = "";

        public string DBMT_Protect_TargetModPath { get; set; } = "";

    }


    public class MainSetting : BaseConfig
    {
        public string GameName { get; set; } = "HSR";
        public string WorkSpaceName { get; set; } = string.Empty;

        //DBMTλ��
        public string DBMTLocation { get; set; } = "";
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

        public static string Path_ModsFolder
        {
            get { return Path.Combine(Path_Base, "Mods\\"); }
        }



    }

}
