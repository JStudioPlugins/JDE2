using JDE2.Assets;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Commands
{
    public class CommandTest : Command, IEditorCommand, ISingleplayerCommand
    {
        protected override void execute(CSteamID executorID, string parameter)
        {
            string[] command = Parser.getComponentsFromSerial(parameter, '/');
            var caller = ((ICommand)(this)).GetCaller();

            List<TestAsset> assets = new();
            SDG.Unturned.Assets.find(assets);

            foreach (TestAsset asset in assets)
            {
                caller.SendChat($"{asset.TestValue}");
            }
        }

        public CommandTest()
        {
            localization = null;
            _command = "test";
            _info = "Tests.";
            _help = "what";
        }
    }
}
