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
            Player player = PlayerTool.getPlayer(executorID);
            if (player.equipment.asset == null)
            {
                CommandWindow.LogError("NOT HOLDING ITEM!");
                return;
            }
            byte amount;
            byte.TryParse(parameter, out amount);

            ItemTool.tryForceGiveItem(player, player.equipment.itemID, amount);
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
