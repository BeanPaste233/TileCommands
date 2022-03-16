using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace TileCommands
{
    [ApiVersion(2,1)]
    public class MainPlugin : TerrariaPlugin
    {
        public MainPlugin(Main game) : base(game){ }

        public override string Name => "TileCommands";

        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public override string Author => "豆沙";

        public override string Description => "当玩家走到某个方块上时执行指令";
        public static int i;
        public override void Initialize()
        {
            ServerApi.Hooks.GamePostInitialize.Register(this,OnPostInitialize);
            ServerApi.Hooks.GameUpdate.Register(this,OnGameUpdate);
            GetDataHandlers.PlayerUpdate += OnPlayerUpdate;
            
            
            ConfigUtils.LoadConfig();
        }

        private void OnGameUpdate(EventArgs args)
        {
            foreach (var tile in ConfigUtils.config.Tiles)
            {
                tile.Update(args);
            }
        }

        private void OnPlayerUpdate(object sender,GetDataHandlers.PlayerUpdateEventArgs args) 
        {
            var plr = args.Player;
            var point = Utils.GetPlayerFeetPoint(plr.TileX, plr.TileY);
            var tile = ConfigUtils.config.Tiles.Find(t=>t.Coordinate.X==point.X&&t.Coordinate.Y==point.Y);
            if (tile!=null)
            {
                /*var distance = Vector2.Distance(new Vector2(tile.Coordinate.X*16,tile.Coordinate.Y*16),args.Player.TPlayer.position);
                if (distance<=250)
                {
                    Utils.SendCombatMsg(tile.ToString(), Color.MediumAquamarine, args.Player.TPlayer.position) ;
                }
                if (tile.CheckPermission(args.Player))
                    tile.ExecuteCommands(args.Player);
                else
                    args.Player.SendErrorMessage("你没有使用此指令方块的权限");*/
                if (!tile.Locked)
                {
                    //var distance = Vector2.Distance(new Vector2(tile.Coordinate.X * 16, tile.Coordinate.Y * 16), plr.TPlayer.position);
                    var distance = Math.Sqrt(Math.Pow(Math.Abs(tile.Coordinate.X-plr.TileX),2.0)+ Math.Pow(Math.Abs(tile.Coordinate.Y - plr.TileY), 2.0));
                    if (distance <= 3)
                    {
                        if (!ConfigUtils.config.DisableFloatText)//如果开启了漂浮字
                        {
                            Utils.SendCombatMsg(tile.ToString(), Color.MediumAquamarine, plr.TPlayer.position);
                        }
                        if (ConfigUtils.config.EnableChatTips)//如果开启了聊天栏提示
                        {
                            plr.SendInfoMessage(tile.ToString());
                        }
                    }
                    if (tile.CheckPermission(args.Player))
                        tile.ExecuteCommands(args.Player);
                    else
                        plr.SendErrorMessage("你没有使用此指令方块的权限");
                }
                else {
                    plr.SendInfoMessage("指令方块在冷却中...");
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
            int value,id;
            var plr = args.Player;
            StringBuilder helpText = new StringBuilder();
            switch (args.Parameters[0])
            {
                case "help":
                    helpText.AppendLine("/tc add 添加一个选择的点为指令方块");
                    helpText.AppendLine("/tc remove 删除所选中的点");
                    helpText.AppendLine("/tc sp 选中一个点");
                    helpText.AppendLine("/tc sc [方块ID] 选中方块");
                    helpText.AppendLine("/tc addtext [文本] 设置文本");
                    helpText.AppendLine("/tc setperm [权限] 设置该点的权限");
                    helpText.AppendLine("/tc addcmd [指令] 给选中的点添加指令");
                    helpText.AppendLine("/tc cancel 取消选中");
                    helpText.AppendLine("/tc addtext [文本] 设置文本");
                    helpText.AppendLine("/tc settime [方块ID] [秒数] 设置冷却时间");
                    helpText.AppendLine("/tc tp [方块ID] 传送到此方块");
                    helpText.AppendLine("/tc list 查看方块列表");
                    helpText.AppendLine("/tc reload 重载插件");
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
                    args.Player.SendSuccessMessage($"成功添加方块 坐标:({tile.Coordinate.X},{tile.Coordinate.Y}) 方块ID:{tile.ID}");
                    ConfigUtils.UpdateConfig();
                    break;
                case "remove":
                    tile = ConfigUtils.config.Tiles.Find(t => t.ID == plr.GetData<int>("tilecommands"));
                    if (tile == null)
                    {
                        args.Player.SendErrorMessage("指令方块不存在");
                        return;
                    }
                    ConfigUtils.config.Tiles.Remove(tile);
                    args.Player.SendSuccessMessage($"成功删除方块 坐标:({tile.Coordinate.X},{tile.Coordinate.Y})");
                    ConfigUtils.UpdateConfig();
                    break;
                case "sp":
                    args.Player.AwaitingTempPoint = 1;
                    args.Player.SendInfoMessage("请选取一个方块");
                    break;
                case "setperm":
                    if (args.Parameters.Count!=2)
                    {
                        args.Player.SendErrorMessage("正确用法 /tc setperm [权限]");
                        return;
                    }
                    tile = ConfigUtils.config.Tiles.Find(t => t.ID == plr.GetData<int>("tilecommands"));
                    if (tile == null)
                    {
                        args.Player.SendErrorMessage("指令方块不存在");
                        return;
                    }
                    else {
                        tile.Permission = args.Parameters[1];
                        args.Player.SendSuccessMessage("成功添加权限");
                        ConfigUtils.UpdateConfig();
                    }
                    break;
                case "addcmd":
                    if (args.Parameters.Count <1)
                    {
                        args.Player.SendErrorMessage("正确用法 /tc addcmd [权限]");
                        return;
                    }
                    tile = ConfigUtils.config.Tiles.Find(t => t.ID == plr.GetData<int>("tilecommands"));
                    if (tile == null)
                    {
                        args.Player.SendErrorMessage("指令方块不存在");
                        return;
                    }
                    tile.TCommands.Add(Utils.GetValidCmdText(args.Parameters,1));
                    args.Player.SendSuccessMessage("成功添加指令");
                    ConfigUtils.UpdateConfig();
                    break;
                case "debug":
                    args.Player.SendInfoMessage($"{args.Player.TileX},{args.Player.TileY}");
                    break;
                case "cancel":
                    args.Player.SendSuccessMessage("成功取消选择");
                    args.Player.TempPoints[0] = Point.Zero;
                    plr.SetData("tilecommands", 0);
                    break;
                case "reload":
                    ConfigUtils.LoadConfig();
                    args.Player.SendMessage("插件重载完毕",Color.MediumAquamarine);
                    break;
                case "addtext":
                    if (args.Parameters.Count < 1)
                    {
                        args.Player.SendErrorMessage("正确用法 /tc addtext [文本]");
                        return;
                    }
                    tile = ConfigUtils.config.Tiles.Find(t=>t.ID==plr.GetData<int>("tilecommands"));
                    if (tile == null)
                    {
                        args.Player.SendErrorMessage("指令方块不存在");
                        return;
                    }
                    tile.Text.Add(args.Parameters[1]);
                    args.Player.SendSuccessMessage("成功添加提示文本");
                    ConfigUtils.UpdateConfig();
                    break;
                default:
                    args.Player.SendInfoMessage("输入/tc help 查看指令帮助");
                    break;
                case "sc":
                    if (args.Parameters.Count!=2)
                    {
                        args.Player.SendInfoMessage("正确指令/tc sc [方块ID] 进行选中方块");
                        return;
                    }
                    if (int.TryParse(args.Parameters[1],out value))
                    {
                        tile = ConfigUtils.config.Tiles.Find(t=>t.ID==value);
                        if (tile!=null)
                        {
                            plr.SetData("tilecommands", value);
                            plr.SendInfoMessage($"成功选中 指令方块(ID:{value})");
                        }
                        else
                        {
                            plr.SendInfoMessage("指令方块不存在");
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage("请输入数字");
                    }
                    break;
                case "settime":
                    if (args.Parameters.Count != 2)
                    {
                        args.Player.SendInfoMessage("正确指令/tc settime [秒数]");
                        return;
                    }
                    if (int.TryParse(args.Parameters[1], out value))
                    {
                        tile = ConfigUtils.config.Tiles.Find(t => t.ID == plr.GetData<int>("tilecommands"));
                        if (tile != null)
                        {
                            tile.Seconds = value;
                            plr.SendInfoMessage($"成功设置指令方块(id:{tile.ID})的冷却时间为 {value} 秒");
                            ConfigUtils.UpdateConfig();
                        }
                        else
                        {
                            plr.SendInfoMessage("指令方块不存在");
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage("请输入数字");
                    }
                    break;
                case "list":
                    helpText.AppendLine("指令方块列表——>");
                    foreach (var temp in ConfigUtils.config.Tiles)
                    {
                        helpText.AppendLine($"[{temp.ID}] ({temp.Coordinate.X},{temp.Coordinate.Y})");
                    }
                    plr.SendInfoMessage(helpText.ToString());
                    break;
                case "tp":
                    if (args.Parameters.Count != 2)
                    {
                        args.Player.SendInfoMessage("正确指令/tc tp [方块ID]");
                        return;
                    }
                    if (int.TryParse(args.Parameters[1], out id))
                    {
                        tile = ConfigUtils.config.Tiles.Find(t => t.ID == id);
                        if (tile != null)
                        {
                            plr.Teleport(tile.Coordinate.X*16f,tile.Coordinate.Y*16f);
                        }
                        else
                        {
                            plr.SendInfoMessage("指令方块不存在");
                        }
                    }
                    else
                    {
                        args.Player.SendInfoMessage("请输入数字");
                    }
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
