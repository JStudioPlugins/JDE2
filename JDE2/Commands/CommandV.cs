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
    public class CommandV : Command, ISingleplayerCommand
    {
        protected override void execute(CSteamID executorID, string parameter)
        {
            string[] command = Parser.getComponentsFromSerial(parameter, '/');
            var caller = this.GetCaller();
            var ply = PlayerTool.getPlayer(executorID);

            if (command.Length < 1)
            {
                caller.SendChat("INVALID PARAMETERS");
            }

            ushort id = 0;

            string vehicleString = command[0].ToString();

            if (!ushort.TryParse(vehicleString, out id))
            {
                List<VehicleAsset> sortedAssets = new List<VehicleAsset>(SDG.Unturned.Assets.find(EAssetType.VEHICLE).Cast<VehicleAsset>());
                VehicleAsset found = sortedAssets.Where(i => i.vehicleName != null).OrderBy(i => i.vehicleName.Length).Where(i => i.vehicleName.ToLower().Contains(vehicleString.ToLower())).FirstOrDefault();
                if (found != null) id = found.id;
                if (string.IsNullOrEmpty(vehicleString.Trim()) || id == 0)
                {
                    caller.SendChat("INVALID PARAMETERS!");
                    return;
                }
            }

            VehicleAsset asset = (VehicleAsset)SDG.Unturned.Assets.find(EAssetType.VEHICLE, id);

            VehicleTool.giveVehicle(ply, id);
            caller.SendChat($"Gave you a {asset.vehicleName}.");
        }

        public CommandV()
        {
            localization = null;
            _command = "v";
            _info = "Gives you a vehicle.";
            _help = "/v <vehicle>";
        }
    }
}
