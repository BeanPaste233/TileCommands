using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace TileCommands
{
    public class Placeholder
    {
        internal string player = "{player}";
        internal string itemInHeld = "{helditem}";
        internal string group = "{group}";
        public Placeholder() { }
        public string Replace(string text,TSPlayer plr) 
        {
            text.Replace(player,plr.Name);
            text.Replace(itemInHeld,plr.TPlayer.HeldItem.netID.ToString());
            text.Replace(group,plr.Group.Name);
            return text;
        }
    }
}
