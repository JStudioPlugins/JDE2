using JDE2.Managers;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Commands
{
    public class CommandMore : Command, ISingleplayerCommand
    {

        protected override void execute(CSteamID executorID, string parameter)
        {
            var caller = this.GetCaller();
            if (caller.Player.equipment.asset == null)
            {
                caller.SendChat("NOT HOLDING ITEM!");
                return;
            }
            byte amount;
            byte.TryParse(parameter, out amount);

            ItemTool.tryForceGiveItem(caller.Player, caller.Player.equipment.itemID, amount);
            caller.SendChat($"Gave {amount} {caller.Player.equipment.asset.itemName}!");
        }

        public CommandMore()
        {
            localization = null;
            _command = "more";
            _info = "Gives you more of the item you are holding.";
            _help = "/more <amount>";
        }
    }
}
