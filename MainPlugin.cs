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
    [ApiVersion(2,1)]
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
            
            ConfigUtils.LoadConfig();
        }
        private void OnPlayerUpdate(object sender,GetDataHandlers.PlayerUpdateEventArgs args) 
        {
            var point = Utils.GetPlayerFeetPoint(args.Player.TileX,args.Player.TileY);
            var tile = ConfigUtils.config.Tiles.Find(t=>t.Coordinate.X==point.X&&t.Coordinate.Y==point.Y);
            if (tile!=null)
            {
                int absInstance = Math.Abs(args.Player.TileX-tile.Coordinate.X);
                if (absInstance<=4)
                {
                    Utils.SendCombatMsg(tile.Text,Color.MediumAquamarine,new Vector2(tile.Coordinate.X*16,tile.Coordinate.Y*16));
                }
                if (tile.CheckPermission(args.Player))
                {
                    tile.ExecuteCommands(args.Player);
                    
                }else
                {
                    args.Player.SendErrorMessage("你没有使用此指令方块的权限");
                }
            }
        
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
            TileInfo tile = null;
            switch (args.Parameters[0])
            {
                case "help":
                    StringBuilder helpText = new StringBuilder();
                    helpText.AppendLine("/tc add 添加一个选择的点为指令方块");
                    helpText.AppendLine("/tc remove 删除所选中的点");
                    helpText.AppendLine("/tc select 选中一个点");
                    helpText.AppendLine("/tc setperm [权限] 设置该点的权限");
                    helpText.AppendLine("/tc addcmd [指令] 给选中的点添加指令");
                    helpText.AppendLine("/tc cancel 取消选中");
                    helpText.AppendLine("/tc settext [文本] 设置文本");
                    args.Player.SendMessage(helpText.ToString(),Color.MediumAquamarine);
                    break;
                case "add":
                    if (args.Player.TempPoints[0]==Point.Zero)
                    {
                        args.Player.SendErrorMessage("未选取点");
                        return;
                    }
                    tile = new TileInfo(ConfigUtils.config.Tiles.Count+1,new List<string>(),"",args.Player.TempPoints[0]);
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
                    if (ConfigUtils.config.Tiles.Exists(t => t.Coordinate.X == args.Player.TempPoints[0].X && t.Coordinate.Y == args.Player.TempPoints[0].Y))
                    {
                        ConfigUtils.config.Tiles.RemoveAll(t => t.Coordinate.X == args.Player.TempPoints[0].X && t.Coordinate.Y == args.Player.TempPoints[0].Y);
                        args.Player.SendSuccessMessage($"成功删除方块 坐标:({args.Player.TempPoints[0].X},{args.Player.TempPoints[0].Y})");
                        args.Player.TempPoints[0] = Point.Zero;
                        ConfigUtils.UpdateConfig();
                    }
                    else {
                        args.Player.SendErrorMessage($"该方块不是指令方块");
                    }
                    
                    break;
                case "select":
                    args.Player.AwaitingTempPoint = 1;
                    args.Player.SendInfoMessage("请选取一个方块");
                    break;
                case "setperm":
                    if (args.Player.TempPoints[0] == Point.Zero)
                    {
                        args.Player.SendErrorMessage("未选取点");
                        return;
                    }
                    if (args.Parameters.Count!=2)
                    {
                        args.Player.SendErrorMessage("正确用法 /tc setperm [权限]");
                        return;
                    }
                    tile = ConfigUtils.config.Tiles.Find(t => t.Coordinate.X == args.Player.TempPoints[0].X && t.Coordinate.Y == args.Player.TempPoints[0].Y);
                    if (tile == null)
                    {
                        args.Player.SendErrorMessage("方块不存在");
                        return;
                    }
                    else {
                        tile.Permission = args.Parameters[1];
                        args.Player.SendSuccessMessage("成功添加权限");
                        args.Player.TempPoints[0] = Point.Zero;
                        ConfigUtils.UpdateConfig();
                    }
                    break;
                case "addcmd":
                    if (args.Parameters.Count <1)
                    {
                        args.Player.SendErrorMessage("正确用法 /tc addcmd [权限]");
                        return;
                    }
                    if (args.Player.TempPoints[0] == Point.Zero)
                    {
                        args.Player.SendErrorMessage("未选取点");
                        return;
                    }
                    tile = ConfigUtils.config.Tiles.Find(t => t.Coordinate.X == args.Player.TempPoints[0].X && t.Coordinate.Y == args.Player.TempPoints[0].Y);
                    if (tile==null)
                    {
                        args.Player.SendErrorMessage("该方块不是一个指令方块");
                        return;
                    }
                    tile.TCommands.Add(Utils.GetValidCmdText(args.Parameters,1));
                    args.Player.SendSuccessMessage("成功添加指令");
                    args.Player.TempPoints[0] = Point.Zero;
                    ConfigUtils.UpdateConfig();
                    break;
                case "debug":
                    args.Player.SendInfoMessage($"{args.Player.TileX},{args.Player.TileY}");
                    break;
                case "cancel":
                    args.Player.SendSuccessMessage("成功取消选择");
                    args.Player.TempPoints[0] = Point.Zero;
                    break;
                case "reload":
                    ConfigUtils.LoadConfig();
                    args.Player.SendMessage("插件重载完毕",Color.MediumAquamarine);
                    break;
                case "settext":
                    if (args.Parameters.Count < 1)
                    {
                        args.Player.SendErrorMessage("正确用法 /tc settext [文本]");
                        return;
                    }
                    if (args.Player.TempPoints[0] == Point.Zero)
                    {
                        args.Player.SendErrorMessage("未选取点");
                        return;
                    }
                    tile = ConfigUtils.config.Tiles.Find(t => t.Coordinate.X == args.Player.TempPoints[0].X && t.Coordinate.Y == args.Player.TempPoints[0].Y);
                    if (tile == null)
                    {
                        args.Player.SendErrorMessage("该方块不是一个指令方块");
                        return;
                    }
                    tile.Text = args.Parameters[1];
                    args.Player.SendSuccessMessage("成功设置提示文本");
                    args.Player.TempPoints[0] = Point.Zero;
                    ConfigUtils.UpdateConfig();
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
                ServerApi.Hooks.GamePostInitialize.Register(this, OnPostInitialize);
                GetDataHandlers.PlayerUpdate += OnPlayerUpdate;
            }
            base.Dispose(disposing);
        }
    }
}
