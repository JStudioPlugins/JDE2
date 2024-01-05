using JDE2.Commands.Callers;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Commands
{
    public interface ICommand
    {
    }

    public static class CommandExtensions
    {
        public static ICaller GetCaller(this ICommand command)
        {
            if (command is ISingleplayerCommand)
            {
                return new SingleplayerCaller();
            }
            else
                return new EditorCaller();
        }

        public static SingleplayerCaller GetCaller(this ISingleplayerCommand command) => new SingleplayerCaller();

        public static EditorCaller GetCaller(this IEditorCommand command) => new EditorCaller();
    }
}
