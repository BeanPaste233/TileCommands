using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCommands
{
    public static class Utils
    {
        //to do
        public static Point GetPlayerFeetPoint(int tileX,int tileY) {
            return new Point(tileX,tileY+4);
        }
    }
}
