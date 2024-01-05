using JDE2.Managers;
using SDG.Framework.Utilities;
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
    public class CommandRegionDebug : Command, ISingleplayerCommand
    {
        public static bool Active;

        protected override void execute(CSteamID executorID, string parameter)
        {
            string[] command = Parser.getComponentsFromSerial(parameter, '/');
            var caller = this.GetCaller();

            if (command.Length > 1)
            {
                if (command[0].ToLower() == "clear" && Active)
                {
                    TimeUtility.updated -= OnUpdateGizmos;
                    Active = false;
                    return;
                }
            }

            Regions.tryGetCoordinate(caller.Player.transform.position, out byte x, out byte y);
            LoggingManager.Log($"Structure region {x}, {y} has {StructureManager.regions[x, y].drops.Count}");
            LoggingManager.Log($"Barricade region {x}, {y} has {BarricadeManager.regions[x, y].drops.Count}");

            if (!Active)
            {
                TimeUtility.updated += OnUpdateGizmos;
                Active = true;
            }
        }

        private void OnUpdateGizmos()
        {
            for (byte b = 0; b < Regions.WORLD_SIZE; b++)
            {
                for (byte b2 = 0; b2 < Regions.WORLD_SIZE; b2++)
                {
                    Regions.tryGetPoint(b, b2, out Vector3 center);
                    RuntimeGizmos.Get().Cube(center, Regions.REGION_SIZE, Color.blue);
                }
            }
        }

        public CommandRegionDebug()
        {
            localization = null;
            _command = "regiondebug";
            _info = "Debugs the region and sends it to logs.";
            _help = "";
        }
    }
}
