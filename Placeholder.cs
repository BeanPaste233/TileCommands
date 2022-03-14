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
        public string player = "{player}";
        public string itemInHeld = "{helditem}";
        public Placeholder() { }
        public string Replace(string text,TSPlayer plr) 
        {
            text.Replace(player,plr.Name);
            text.Replace(itemInHeld,plr.TPlayer.HeldItem.netID.ToString());
            return text;
        }
    }
}
