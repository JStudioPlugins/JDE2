using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JDE2.Commands.Callers
{
    public interface ICaller
    {
        public void SendChat(string message);

        public void SendChat(string message, Color color, EChatMode mode = EChatMode.SAY, string iconURL = null, bool useRichTextFormatting = false);

        public void SendHintMessage(string message);


    }
}
