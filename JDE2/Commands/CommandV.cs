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

            if (command.Length < 1)
            {
                CommandWindow.LogError("INVALID PARAMETERS");
            }

            ushort id = 0;

            string vehicleString = command[0].ToString();

            if (!ushort.TryParse(vehicleString, out id))
            {
                List<VehicleAsset> sortedAssets = new List<VehicleAsset>(Assets.find(EAssetType.VEHICLE).Cast<VehicleAsset>());
                VehicleAsset found = sortedAssets.Where(i => i.vehicleName != null).OrderBy(i => i.vehicleName.Length).Where(i => i.vehicleName.ToLower().Contains(vehicleString.ToLower())).FirstOrDefault();
                if (found != null) id = found.id;
                if (string.IsNullOrEmpty(vehicleString.Trim()) || id == 0)
                {
                    CommandWindow.LogError("INVALID PARAMETERS!");
                    return;
                }
            }

            VehicleAsset asset = (VehicleAsset)Assets.find(EAssetType.VEHICLE, id);

            VehicleTool.giveVehicle(PlayerTool.getPlayer(executorID), id);
            CommandWindow.Log($"Gave you a {asset.vehicleName}.");
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
