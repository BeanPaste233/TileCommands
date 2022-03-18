using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using TShockAPI;

namespace TileCommands
{
    public static class Utils
    {
        //to do
        public static Point GetPlayerFeetPoint(int tileX,int tileY) {
            return new Point(tileX,tileY+3);
        }
        public static string GetValidCmdText(List<string> texts,int startIndex) 
        {
            for (int i = 0; i < startIndex; i++)
            {
                texts.RemoveAt(i);
            }
            var realCmd = string.Join(" ",texts.ToArray());
            return realCmd;
        }
        public static void SendCombatMsg(string msg, Color color, Vector2 position)
        {
            TSPlayer.All.SendData(PacketTypes.CreateCombatTextExtended,msg,(int)color.packedValue,position.X,position.Y);
        }
    }
}
