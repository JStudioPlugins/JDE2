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

        public void LoadDefaults()
        {
            AssetsConfig = new() { LoadMaps = true, LoadSandbox = true };
            CommandsConfig = new() { Enabled = true };
        }
    }

    public class AssetsConfig
    {
        public bool LoadSandbox { get; set; }
        public bool LoadMaps { get; set; }
    }

    public class CommandsConfig
    {
        public bool Enabled { get; set; }
    }

    public interface IDefaultable
    {
        void LoadDefaults();
    }
}
