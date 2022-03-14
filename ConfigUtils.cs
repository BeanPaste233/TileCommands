using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace TileCommands
{
   public static  class ConfigUtils
    {
        public static readonly string configDir = TShock.SavePath + "/TileCommands";
        public static readonly string configPath = configDir + "/tilecommands.json";
        public static TConfig config = new TConfig();
        public static void LoadConfig() 
        {
            if (Directory.Exists(configDir))
            {
                if (File.Exists(configPath))
                {
                    config = JsonConvert.DeserializeObject<TConfig>(File.ReadAllText(configPath));
                }
                else{
                    File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
                }
            }else
            {
                Directory.CreateDirectory(configDir);
                File.WriteAllText(configPath,JsonConvert.SerializeObject(config,Formatting.Indented));
            }
        
        }
        public static void UpdateConfig() {
            File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
        }
    }
    public class TConfig {
        public bool DisableFloatText { get; set; }

        public List<TileInfo> Tiles = new List<TileInfo>();
    }
}
