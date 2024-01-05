using JDE2.Managers;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JDE2.Commands.Callers
{
    public class EditorCaller : ICaller
    {
        public void SendChat(string message)
        {
            EditorChatManager.Instance.ReceiveMessage(CSteamID.Nil, null, EChatMode.GLOBAL, Color.white, true, message);
        }

        public void SendChat(string message, Color color, EChatMode mode = EChatMode.SAY, string iconURL = null, bool useRichTextFormatting = false)
        {
            EditorChatManager.Instance.ReceiveMessage(CSteamID.Nil, iconURL, mode, color, useRichTextFormatting, message);
        }

        public void SendHintMessage(string message)
        {
            EditorUI.hint(EEditorMessage.FOCUS, message);
        }
    }
}
