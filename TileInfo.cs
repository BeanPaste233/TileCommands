using Microsoft.Xna.Framework;
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
        public int ID { get; set; }
        public List<string> TCommands { get; set; }
        public string Permission { get; set; }
        public Point Coordinate { get; set; }
        public TileInfo(int id,List<string> cmds,string permission,Point coordinate) {
            ID = id;
            TCommands = cmds;
            Permission = permission;
            Coordinate = coordinate;
        }
        public bool ExecuteCommands(TSPlayer plr) 
        {
            if (plr == null|| TCommands == null||Coordinate==null) return false;
            foreach (var cmd in TCommands)
            {
                Commands.HandleCommand(plr,cmd);
            }
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
    }
}
