using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace TileCommands
{
    public class TileInfo
    {
        [JsonProperty("方块ID")]
        public int ID { get; set; }
        [JsonProperty("使用权限")]
        public string Permission { get; set; }
        [JsonProperty("方块坐标")]
        public Point Coordinate { get; set; }
        [JsonProperty("执行指令")]
        public List<string> TCommands { get; set; }
        [JsonProperty("显示文本")]
        public List<string> Text { get; set; }
        public TileInfo(int id,List<string> cmds,string permission,Point coordinate) {
            ID = id;
            TCommands = cmds;
            Permission = permission;
            Coordinate = coordinate;
            Text = new List<string>();
        }
        public bool ExecuteCommands(TSPlayer plr) 
        {
            if (plr == null|| TCommands == null||Coordinate==null) return false;
            plr.tempGroup = TShock.Groups.GetGroupByName("superadmin");
            foreach (var cmd in TCommands)
            {
                Commands.HandleCommand(plr,"/"+cmd);
            }
            plr.tempGroup = null;
            return true;
        }
        public bool CheckPermission(TSPlayer plr) 
        {
            if (plr == null ) return false;
            if (plr.HasPermission(Permission))
            {
                return true;
            }
            else {
                return false;
            }
        }
        public override string ToString()
        {
            StringBuilder text = new StringBuilder();
            foreach (var str in Text)
            {
                text.AppendLine(str);
            }
            return text.ToString();
        }
    }
}
