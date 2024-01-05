using JDE2.SituationDependent;
using Newtonsoft.Json;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDE2
{
    public class Config
    {
        public static Config Instance { get; private set; }

        public ConfigModel Data { get; set; }

        public string DataPath
        {
            get
            {
                string path = UnturnedPaths.RootDirectory.FullName;
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path + "/config.json";
            }
        }

        public Config()
        {
            Instance = this;

            Main.Instance.OnUnload += HandleOnUnload;
            Read();
            Save();
        }

        private void HandleOnUnload()
        {
            Instance = null;
        }

        public void Save()
        {
            string objData = JsonConvert.SerializeObject(Data, Formatting.Indented);

            using (StreamWriter stream = new StreamWriter(DataPath, false))
            {
                stream.Write(objData);
            }
        }

        public void Reset()
        {
            ConfigModel data = new ConfigModel();
            data.LoadDefaults();
            Data = data;
            Save();
        }

        public void Read()
        {
            if (File.Exists(DataPath))
            {
                string dataText;
                using (StreamReader stream = File.OpenText(DataPath))
                {
                    dataText = stream.ReadToEnd();
                }
                Data = JsonConvert.DeserializeObject<ConfigModel>(dataText);
            }
            else
            {
                ConfigModel data = new ConfigModel();
                data.LoadDefaults();
                Data = data;
            }
        }
    }

    public class ConfigModel : IDefaultable
    {
        public AssetsConfig AssetsConfig { get; set; }
        public CommandsConfig CommandsConfig { get; set; }
        public EditorConfig EditorConfig { get; set; }
        public PluginConfig PluginConfig { get; set; }
        public DeveloperConfig DeveloperConfig { get; set; }
        

        public void LoadDefaults()
        {
            AssetsConfig = new()
            {
                LoadMaps = true,
                LoadSandbox = true,
                LoadWorkshop = true
            };
            CommandsConfig = new() { Enabled = true };
            EditorConfig = new() 
            {
                AutoSave = false,
                RoadVisuals = true
            };
            PluginConfig = new() { Enabled = true };
            DeveloperConfig = new() { DebuggingEnabled = true, ConsoleEnabled = true, DisabledTypes = new[] { "JDE2.Managers.TestManager", "JDE.UI.EditorCustomTestUI" } };
        }
    }

    public class AssetsConfig
    {
        public bool LoadSandbox { get; set; }
        public bool LoadMaps { get; set; }
        public bool LoadWorkshop { get; set; }
    }

    public class CommandsConfig
    {
        public bool Enabled { get; set; }
    }

    public class EditorConfig
    {
        public bool AutoSave { get; set; }
        public bool RoadVisuals { get; set; }
    }

    public class PluginConfig
    {
        public bool Enabled { get; set; }
    }

    public class DeveloperConfig
    {
        public bool DebuggingEnabled { get; set; }
        public bool ConsoleEnabled { get; set; }
        public string[] DisabledTypes { get; set; }
    }


    public interface IDefaultable
    {
        void LoadDefaults();
    }
}
