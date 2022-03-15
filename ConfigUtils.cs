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
        [JsonProperty("关闭方块漂浮字提示")]
        public bool DisableFloatText { get; set; }
        [JsonProperty("开启聊天窗口提示")]
        public bool EnableChatTips { get; set; }
        [JsonProperty("指令方块列表")]
        public List<TileInfo> Tiles = new List<TileInfo>();
    }
}
