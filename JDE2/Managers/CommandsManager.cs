using JDE2.SituationDependent;
using JDE2.Utils;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Managers
{
    public class CommandsManager : ISingleplayerDependent
    {
        public static CommandsManager Instance;

        public CommandsManager()
        {
            Instance = this;

            LoggingManager.Log("LOADING ISingleplayerCommands");
            var types = ReflectionTool.GetTypesFromInterface(Main.Instance.Assembly, "ISingleplayerCommand");
            foreach (Type type in types)
            {
                Commander.register((Command)Activator.CreateInstance(type));
                LoggingManager.Log($"REGISTERED COMMAND {{cyan}}{type.FullName}{{_}}");
            }
        }
    }
}
