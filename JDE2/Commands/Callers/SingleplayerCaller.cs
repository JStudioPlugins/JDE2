using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JDE2.Commands.Callers
{
    public class SingleplayerCaller : ICaller
    {
        public Player Player
        {
            get
            {
                return Player.player;
            }
        }

        public void SendChat(string message)
        {
            ChatManager.serverSendMessage(message, Color.white);
        }

        public void SendChat(string message, Color color, EChatMode mode = EChatMode.SAY, string iconURL = null, bool useRichTextFormatting = false)
        {
            ChatManager.serverSendMessage(message, color, null, null, mode, iconURL, useRichTextFormatting);
        }

        public void SendHintMessage(string message)
        {
            PlayerUI.message(EPlayerMessage.NPC_CUSTOM, Assembly.GetCallingAssembly().GetName().Name + ": " + message);
        }
    }
}
