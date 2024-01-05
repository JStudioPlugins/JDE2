using JDE2.Commands;
using JDE2.SituationDependent;
using JDE2.UI;
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
    public class CommandsManager : IEditorDependent, ISingleplayerDependent, IDisposable
    {
        private static List<ICommand> _registeredCommands;

        public static IReadOnlyList<ICommand> RegisteredCommands
        {
            get
            {
                return _registeredCommands.AsReadOnly();
            }
        }

        public static void RegisterCommandsForAssembly(Assembly assembly)
        {
            if (Level.isEditor)
            {
                LoggingManager.Log($"LOADING IEditorCommands for {{{{cyan}}}}{assembly.FullName}{{{{_}}}}");
                var typesEditor = ReflectionTool.GetTypesFromInterface(assembly, typeof(IEditorCommand).Name);
                foreach (Type type in typesEditor)
                {
                    IEditorCommand command = (IEditorCommand)Activator.CreateInstance(type);
                    if (command is not Command baseCommand)
                    {
                        LoggingManager.LogError($"{{{{cyan}}}}{command.GetType().FullName}{{{{_}}}} DOES NOT INHERIT BASE CLASS {{{{cyan}}}}{typeof(Command).FullName}{{{{_}}}}!");
                        continue;
                    }
                    _registeredCommands.Add(command);
                    Commander.register(baseCommand);
                    LoggingManager.Log($"REGISTERED COMMAND {{{{cyan}}}}{type.FullName}{{{{_}}}}");
                }
                return;
            }

            LoggingManager.Log($"LOADING ISingleplayerCommands for {{{{cyan}}}}{assembly.FullName}{{{{_}}}}");
            var typesSingleplayer = ReflectionTool.GetTypesFromInterface(assembly, typeof(ISingleplayerCommand).Name);
            foreach (Type type in typesSingleplayer)
            {
                ISingleplayerCommand command = (ISingleplayerCommand)Activator.CreateInstance(type);
                if (command is not Command baseCommand)
                {
                    LoggingManager.LogError($"{{{{cyan}}}}{command.GetType().FullName}{{{{_}}}} DOES NOT INHERIT BASE CLASS {{{{cyan}}}}{typeof(Command).FullName}{{{{_}}}}!");
                    continue;
                }
                _registeredCommands.Add(command);
                Commander.register(baseCommand);
                LoggingManager.Log($"REGISTERED COMMAND {{{{cyan}}}}{type.FullName}{{{{_}}}}");
            }
        }

        private void EditorCommanderInit()
        {
            Commander.commands = new List<Command>();
            Commander.register(new CommandReload(Localization.read("/Server/ServerCommandReload.dat")));
        }

        public CommandsManager()
        {
            _registeredCommands = new();
            if (Level.isEditor)
                EditorCommanderInit();
            RegisterCommandsForAssembly(Main.Instance.Assembly);
        }

        ~CommandsManager()
        {
            Dispose();
        }

        public void Dispose()
        {
            _registeredCommands = null;
            GC.SuppressFinalize(this);
        }
    }

    public class PluginCommandsManager : IEditorDependent, ISingleplayerDependent
    {
        public PluginCommandsManager()
        {
            if (!PluginManager.Enabled)
            {
                LoggingManager.Log("SKIPPED LOADING PLUGIN COMMANDS.");
            }

            foreach (Assembly assembly in PluginManager.Instance.Plugins)
            {
                CommandsManager.RegisterCommandsForAssembly(assembly);
            }
        }
    }
}
