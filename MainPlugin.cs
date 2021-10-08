using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace TileCommands
{
    public class MainPlugin : TerrariaPlugin
    {
        public MainPlugin(Main game) : base(game)
        {
        }

        public override string Name => "TileCommands";

        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public override string Author => "豆沙";

        public override string Description => "当玩家走到某个方块上时执行指令";

        public override void Initialize()
        {
            ServerApi.Hooks.GamePostInitialize.Register(this,OnPostInitialize);
            GetDataHandlers.PlayerUpdate += OnPlayerUpdate;
        }
        private void OnPlayerUpdate(object sender,GetDataHandlers.PlayerUpdateEventArgs args) 
        {
            
        
        
        }
        private void OnPostInitialize(EventArgs args)
        {
            Commands.ChatCommands.Add(new Command("tc.admin", TC, "tc") { AllowServer = false }) ;
        }

        private void TC(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendInfoMessage("插件用法输入 /tc help 查看");
                return;
            }
            switch (args.Parameters[0])
            {
                case "help":
                    break;
                case "add":
                    if (args.Player.TempPoints[0]==Point.Zero)
                    {
                        args.Player.SendErrorMessage("未选取点");
                        return;
                    }
                    TileInfo tile = new TileInfo(ConfigUtils.config.Tiles.Count+1,new List<string>(),"",args.Player.TempPoints[0]);
                    ConfigUtils.config.Tiles.Add(tile);
                    args.Player.TempPoints[0] = Point.Zero;
                    args.Player.SendSuccessMessage($"成功添加方块 坐标:({tile.Coordinate.X},{tile.Coordinate.Y})");
                    ConfigUtils.UpdateConfig();
                    break;
                case "remove":
                    if (args.Player.TempPoints[0] == Point.Zero)
                    {
                        args.Player.SendErrorMessage("未选取点");
                        return;
                    }

                    ConfigUtils.config.Tiles.RemoveAll(t=>t.Coordinate.X==args.Player.TempPoints[0].X&&t.Coordinate.Y==args.Player.TempPoints[0].Y);
                    args.Player.SendSuccessMessage($"成功删除方块 坐标:({args.Player.TempPoints[0].X},{args.Player.TempPoints[0].Y})");
                    args.Player.TempPoints[0] = Point.Zero;
                    ConfigUtils.UpdateConfig();
                    break;
                case "select":
                    args.Player.AwaitingTempPoint = 1;
                    args.Player.SendInfoMessage("请选取一个方块");
                    break;
                case "list":
                    break;
                case "setperm":
                    break;
                case "addcmd":
                    break;
                case "debug":
                    break;
                default:
                    args.Player.SendInfoMessage("输入/tc help 查看指令帮助");
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }
            base.Dispose(disposing);
        }
    }
}
