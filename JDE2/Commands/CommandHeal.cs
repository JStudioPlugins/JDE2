using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JDE2.Managers;

namespace JDE2.Commands
{
    public class CommandHeal : Command, ISingleplayerCommand
    {
        protected override void execute(CSteamID executorID, string parameter)
        {
            Player ply = this.GetCaller().Player;
            ply.life.askHeal(100, true, true);
            ply.life.askDisinfect(100);
            ply.life.askDrink(100);
            ply.life.askEat(100);
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
