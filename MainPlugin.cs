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
            Commands.ChatCommands.Add(new Command("tc.admin",TC,"tc"));
        }

        private void TC(CommandArgs args)
        {
           
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
