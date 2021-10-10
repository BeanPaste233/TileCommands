using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;

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
            NetMessage.SendData((int)PacketTypes.CreateCombatTextExtended, -1, -1, NetworkText.FromLiteral(msg), (int)color.PackedValue, position.X, position.Y, 0.0f, 0, 0, 0);
        }
    }
}
