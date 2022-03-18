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
        [JsonIgnore]
        public bool Locked { get; set; }
        [JsonProperty("执行指令")]
        public List<string> TCommands { get; set; }
        [JsonProperty("显示文本")]
        public List<string> Text { get; set; }
        [JsonProperty("冷却时间")]
        public int Seconds { get; set; }
        [JsonIgnore]
        private int tick;
        [JsonIgnore]
        private int seconds;
        public TileInfo(int id,List<string> cmds,string permission,Point coordinate) {
            ID = id;
            TCommands = cmds;
            Permission = permission;
            Coordinate = coordinate;
            Text = new List<string>();
            Locked = false;
        }
        public TileInfo() 
        {
            Locked = false;
        }
        public bool ExecuteCommands(TSPlayer plr) 
        {
            if (plr == null|| TCommands == null||Coordinate==null) return false;
            plr.tempGroup = TShock.Groups.GetGroupByName("superadmin");
            foreach (var cmd in TCommands)
            {
                var handledCmd = PlaceholderAPI.PlaceholderAPI.Instance.placeholderManager.GetText(cmd,plr);
                if (!handledCmd.StartsWith("/")) handledCmd = "/" + handledCmd;
                Commands.HandleCommand(plr,handledCmd);
            }
            plr.tempGroup = null;
            Locked = true;
            return true;
        }
        public bool CheckPermission(TSPlayer plr) 
        {
            if (plr == null ) return false;
            return plr.HasPermission(Permission);
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
        public void Update(EventArgs args) 
        {
            if (Locked)
            {
                if (tick != 60)
                {
                    tick++;
                }
                else
                {
                    seconds++;
                    tick = 0;
                }
                if (seconds >= Seconds)
                {
                    Locked = false;
                    seconds = 0;
                }
            }
        }
    }
}
