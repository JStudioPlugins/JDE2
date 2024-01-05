using JDE2.SituationDependent;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JDE2.Managers
{
    public class EditorChatManager : IEditorDependent, IDisposable
    {
        private static EditorChatManager _instance;

        public static EditorChatManager Instance 
        { 
            get { return _instance; } 
        }

        private string[] _recentlySentMessages = new string[10];

        public string[] RecentlySentMessages
        {
            get { return _recentlySentMessages; }
        }

        private List<ReceivedChatMessage> _receivedChatHistory;

        public IReadOnlyList<ReceivedChatMessage> receivedChatHistory
        {
            get { return _receivedChatHistory.AsReadOnly(); }
        }

        public ChatMessageReceivedHandler OnChatMessageReceived;

        public delegate void SendChatRequestedHandler(string text, ref bool shouldAllow);

        public SendChatRequestedHandler OnSendChatRequested;

        public EditorChatManager() 
        {
            _instance = this;
            _receivedChatHistory = new();
        }

        ~EditorChatManager() 
        {
            Dispose();
        }

        public void AddRecentMessage(string text)
        {
            for (int num = _recentlySentMessages.Length - 1; num > 0; num--)
            {
                _recentlySentMessages[num] = _recentlySentMessages[num - 1];
            }
            _recentlySentMessages[0] = text;
        }

        public string GetRecentlySentMessage(int index)
        {
            index %= _recentlySentMessages.Length;
            return _recentlySentMessages[index];
        }

        public void ReceiveMessage(CSteamID speakerSteamID, string iconURL, EChatMode mode, Color color, bool isRich, string text)
        {
            SteamPlayer steamPlayer;
            if (speakerSteamID == CSteamID.Nil)
                steamPlayer = null;
            else
                steamPlayer = PlayerTool.getSteamPlayer(speakerSteamID);

            ReceivedChatMessage item = new ReceivedChatMessage(steamPlayer, iconURL, mode, color, isRich, text);
            _receivedChatHistory.Insert(0, item);
            if (receivedChatHistory.Count > Provider.preferenceData.Chat.History_Length)
            {
                _receivedChatHistory.RemoveAt(receivedChatHistory.Count - 1);
            }
            OnChatMessageReceived?.Invoke();
        }

        public void SendChat(string text)
        {
            bool shouldAllow = true;
            string starter = text.Substring(0, 1);
            if (starter == "@" || starter == "/")
                shouldAllow = !Commander.execute(Player.player.channel.owner.playerID.steamID, text);

            OnSendChatRequested?.Invoke(text, ref shouldAllow);

            if (shouldAllow)
                ReceiveMessage(CSteamID.Nil, null, EChatMode.GLOBAL, Color.white, true, text);
        }



        public void Dispose()
        {
            _recentlySentMessages = null;
            _receivedChatHistory = null;

            _instance = null;
            GC.SuppressFinalize(this);
        }
    }
}
