using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Commands
{
    public class CommandHeal : Command, ISingleplayerCommand
    {
        protected override void execute(CSteamID executorID, string parameter)
        {
            var ply = PlayerTool.getPlayer(executorID).life;
            ply.askHeal(100, true, true);
            ply.askDisinfect(100);
            ply.askDrink(100);
            ply.askEat(100);
        }


        public CommandHeal()
        {
            localization = null;
            _command = "heal";
            _info = "Fully heal yourself.";
            _help = "/heal";
        }
    }
}
