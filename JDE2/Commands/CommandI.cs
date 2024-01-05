using JDE2.Commands.Callers;
using JDE2.Managers;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JDE2.Commands
{
    public class CommandI : Command, ISingleplayerCommand
    {
        protected override void execute(CSteamID executorID, string parameter)
        {
            string[] command = Parser.getComponentsFromSerial(parameter, '/');
            var caller = this.GetCaller();

            if (command.Length < 1)
            {
                caller.SendChat("INVALID PARAMETERS");
            }

            ushort id = 0;
            byte amount = 1;

            string itemString = command[0].ToString();

            if (!ushort.TryParse(itemString, out id))
            {
                List<ItemAsset> sortedAssets = new List<ItemAsset>(SDG.Unturned.Assets.find(EAssetType.ITEM).Cast<ItemAsset>());
                ItemAsset found = sortedAssets.Where(i => i.itemName != null).OrderBy(i => i.itemName.Length).Where(i => i.itemName.ToLower().Contains(itemString.ToLower())).FirstOrDefault();
                if (found != null) id = found.id;
                if (string.IsNullOrEmpty(itemString.Trim()) || id == 0)
                {
                    caller.SendChat("INVALID PARAMETERS");
                    return;
                }
            }

            ItemAsset asset = (ItemAsset)SDG.Unturned.Assets.find(EAssetType.ITEM, id);


            if (command.Length > 1)
            {
                if (!byte.TryParse(command[1], out amount))
                {
                    caller.SendChat("INVALID PARAMETERS");
                    return;
                }
            }

            ItemTool.tryForceGiveItem(caller.Player, id, amount);
            caller.SendChat($"Gave you {amount} {asset.itemName}.");
        }

        public CommandI()
        {
            localization = null;
            _command = "i";
            _info = "Gives you an item.";
            _help = "/i <item>/<amount>";
        }
    }
}
