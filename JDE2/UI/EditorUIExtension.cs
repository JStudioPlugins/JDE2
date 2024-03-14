using DanielWillett.UITools.API.Extensions;
using DanielWillett.UITools.API.Extensions.Members;
using DanielWillett.UITools.Util;
using JDE2.Managers;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JDE2.UI
{
    [UIExtension(typeof(EditorUI))]
    public class EditorUIExtension : UIExtensionWrapper
    {
        private static EditorUIExtension _instance;

        public static EditorUIExtension Get() => _instance;

        [ExistingMember("messageBox")]
        private readonly ISleekBox _messageBox;

        public void ModifyMessageBox(string text = "", bool isVisible = true)
        {
            _messageBox.Text = text;
            _messageBox.IsVisible = isVisible;
        }

        private readonly SleekFullscreenBox _container;

        private readonly ISleekScrollView _chatScrollView;

        private readonly ISleekField _chatField;

        private Queue<SleekChatEntryV2> _chatEntries;

        private readonly SleekButtonIcon _sendChatButton;

        private readonly SleekButtonState _chatModeButton;

        private EChatMode _chatMode;

        private int _repeatChatIndex;

        private bool _chatting;

        private void OnChatFieldEscaped(ISleekField field)
        {
            if (_chatting)
            {
                CloseChat();
            }
        }

        private void OnSendChatButtonClicked(ISleekElement button)
        {
            SendChatAndClose();
        }

        public void SendChatAndClose()
        {
            if (!string.IsNullOrEmpty(_chatField.Text))
            {
                EditorChatManager.Instance.SendChat(_chatField.Text);
            }
            CloseChat();

        }

        public void AddChatMessage(ReceivedChatMessage message)
        {
            if (_chatEntries.Count >= Provider.preferenceData.Chat.History_Length)
            {
                SleekChatEntryV2 child = _chatEntries.Dequeue();
                _chatScrollView.RemoveChild(child);
            }
            SleekChatEntryV2 sleekChatEntry = new SleekChatEntryV2();
            sleekChatEntry.shouldFadeOutWithAge = Glazier.Get().SupportsRichTextAlpha;
            sleekChatEntry.forceVisibleWhileBrowsingChatHistory = _chatting;
            sleekChatEntry.representingChatMessage = message;
            _chatScrollView.AddChild(sleekChatEntry);
            _chatEntries.Enqueue(sleekChatEntry);
            if (!_chatting)
            {
                _chatScrollView.ScrollToBottom();
            }
        }

        private void OnSwappedChatModeState(SleekButtonState button, int index)
        {
            _chatMode = (EChatMode)index;
        }

        public void OpenChat()
        {
            if (_chatting)
            {
                return;
            }
            _chatting = true;
            _chatField.Text = string.Empty;
            _chatField.AnimatePositionOffset(100f, _chatField.PositionOffset_Y, ESleekLerp.EXPONENTIAL, 20f);
            _chatModeButton.state = (int)PlayerUI.chat;

            _chatScrollView.VerticalScrollbarVisibility = ESleekScrollbarVisibility.Default;
            _chatScrollView.IsRaycastTarget = true;
            foreach (SleekChatEntryV2 item in _chatEntries)
            {
                item.forceVisibleWhileBrowsingChatHistory = true;
            }
            _chatScrollView.ScrollToBottom();

        }

        public void CloseChat()
        {
            if (!_chatting)
            {
                return;
            }
            _chatting = false;
            _repeatChatIndex = 0;
            if (_chatField != null)
            {
                _chatField.Text = string.Empty;
                _chatField.AnimatePositionOffset(0f - _chatField.SizeOffset_X - 50f, _chatField.PositionOffset_Y, ESleekLerp.EXPONENTIAL, 20f);
            }

            _chatScrollView.VerticalScrollbarVisibility = ESleekScrollbarVisibility.Hidden;
            _chatScrollView.IsRaycastTarget = false;
            foreach (SleekChatEntryV2 item in _chatEntries)
            {
                item.forceVisibleWhileBrowsingChatHistory = false;
            }
            _chatScrollView.ScrollToBottom();

        }

        public void RepeatChat(int delta)
        {
            if (_chatField != null)
            {
                string recentlySentMessage = EditorChatManager.Instance.GetRecentlySentMessage(_repeatChatIndex);
                if (!string.IsNullOrEmpty(recentlySentMessage))
                {
                    _chatField.Text = recentlySentMessage;
                    _repeatChatIndex += delta;
                }
            }
        }

        private void OnChatMessageReceived()
        {
            if (_chatScrollView != null && EditorChatManager.Instance.receivedChatHistory.Count > 0)
            {
                if (_chatEntries.Count >= Provider.preferenceData.Chat.History_Length)
                {
                    SleekChatEntryV2 child = _chatEntries.Dequeue();
                    _chatScrollView.RemoveChild(child);
                }
                SleekChatEntryV2 sleekChatEntryV = new SleekChatEntryV2();
                sleekChatEntryV.shouldFadeOutWithAge = Glazier.Get().SupportsRichTextAlpha;
                sleekChatEntryV.forceVisibleWhileBrowsingChatHistory = _chatting;
                sleekChatEntryV.representingChatMessage = EditorChatManager.Instance.receivedChatHistory[0];
                _chatScrollView.AddChild(sleekChatEntryV);
                _chatEntries.Enqueue(sleekChatEntryV);
                if (!_chatting)
                {
                    _chatScrollView.ScrollToBottom();
                }
            }
        }

        public EditorUIExtension()
        {
            _instance = this;
            var localization = Localization.read("/Player/PlayerLife.dat");
            var icons = Bundles.getBundle("/Bundles/Textures/Player/Icons/PlayerLife/PlayerLife.unity3d");

            _container = new SleekFullscreenBox();
            _container.PositionOffset_X = 10f;
            _container.PositionOffset_Y = 10f;
            _container.SizeOffset_X = -20f;
            _container.SizeOffset_Y = -20f;
            _container.SizeScale_X = 1f;
            _container.SizeScale_Y = 1f;
            EditorUI.window.AddChild(_container);

            _chatting = false;


            if (!Glazier.Get().SupportsAutomaticLayout)
            {
                LoggingManager.LogError("Dumbass! Glazier automatic layout not supported!");
            }

            _chatScrollView = Glazier.Get().CreateScrollView();
            _chatScrollView.SizeOffset_X = 630f;
            _chatScrollView.SizeOffset_Y = Provider.preferenceData.Chat.Preview_Length * 40;
            _chatScrollView.ScaleContentToWidth = true;
            _chatScrollView.ContentUseManualLayout = false;
            _chatScrollView.AlignContentToBottom = true;
            _chatScrollView.VerticalScrollbarVisibility = ESleekScrollbarVisibility.Hidden;
            _chatScrollView.IsRaycastTarget = false;
            _container.AddChild(_chatScrollView);
            _chatEntries = new Queue<SleekChatEntryV2>(Provider.preferenceData.Chat.History_Length);

            _chatField = Glazier.Get().CreateStringField();
            _chatField.PositionOffset_Y = Provider.preferenceData.Chat.Preview_Length * 40 + 10;
            _chatField.SizeOffset_X = 500f;
            _chatField.PositionOffset_X = 0f - _chatField.SizeOffset_X - 50f;
            _chatField.SizeOffset_Y = 30f;
            _chatField.TextAlignment = TextAnchor.MiddleLeft;
            _chatField.MaxLength = ChatManager.MAX_MESSAGE_LENGTH;
            _chatField.OnTextEscaped += OnChatFieldEscaped;
            _container.AddChild(_chatField);

            _sendChatButton = new SleekButtonIcon(icons.load<Texture2D>("SendChat"));
            _sendChatButton.PositionScale_X = 1f;
            _sendChatButton.SizeOffset_X = 30f;
            _sendChatButton.SizeOffset_Y = 30f;
            _sendChatButton.tooltip = localization.format("SendChat_Tooltip", MenuConfigurationControlsUI.getKeyCodeText(KeyCode.Return));
            _sendChatButton.iconColor = ESleekTint.FOREGROUND;
            _sendChatButton.onClickedButton += OnSendChatButtonClicked;
            _chatField.AddChild(_sendChatButton);

            _chatModeButton = new SleekButtonState();
            _chatModeButton.UseContentTooltip = true;
            _chatModeButton.setContent(new GUIContent(localization.format("Mode_Global"), localization.format("Mode_Global_Tooltip", MenuConfigurationControlsUI.getKeyCodeText(ControlsSettings.global))), new GUIContent(localization.format("Mode_Local"), localization.format("Mode_Local_Tooltip", MenuConfigurationControlsUI.getKeyCodeText(ControlsSettings.local))), new GUIContent(localization.format("Mode_Group"), localization.format("Mode_Group_Tooltip", MenuConfigurationControlsUI.getKeyCodeText(ControlsSettings.group))));
            _chatModeButton.PositionOffset_X = -100f;
            _chatModeButton.SizeOffset_X = 100f;
            _chatModeButton.SizeOffset_Y = 30f;
            _chatModeButton.onSwappedState = OnSwappedChatModeState;
            _chatField.AddChild(_chatModeButton);

            DependentManager.Instance.OnLevelDependentsLoaded += OnLevelDependentsLoaded;

            OnUpdateManager.Instance.OnGUIEvent += OnGUI;
            OnUpdateManager.Instance.OnUpdateEvent += OnUpdate;
        }

        private void OnLevelDependentsLoaded()
        {
            EditorChatManager.Instance.OnChatMessageReceived += OnChatMessageReceived;
            DependentManager.Instance.OnLevelDependentsLoaded -= OnLevelDependentsLoaded;
        }

        private void OnUpdate()
        {
            if (_chatting && Input.GetKeyDown(KeyCode.Escape))
            {
                CloseChat();
            }
        }

        private void OnGUI()
        {
            if (EditorUI.window == null)
            {
                return;
            }
            if (Event.current.isKey && Event.current.type == EventType.KeyUp && Config.Instance.Data.EditorConfig.ChatHotkeys)
            {
                if (Event.current.keyCode == KeyCode.UpArrow)
                {
                    if (_chatting)
                    {
                        RepeatChat(1);
                    }
                }
                else if (Event.current.keyCode == KeyCode.DownArrow)
                {
                    if (_chatting)
                    {
                        RepeatChat(-1);
                    }
                }
                else if ((Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter) && Glazier.Get().ShouldGameProcessInput)
                {
                    if (_chatting)
                    {
                        SendChatAndClose();
                    }
                    else
                    {
                        OpenChat();
                    }
                }
                else if (Event.current.keyCode == ControlsSettings.global)
                {

                    _chatMode = EChatMode.GLOBAL;
                    OpenChat();

                }
                else if (Event.current.keyCode == ControlsSettings.local)
                {

                    _chatMode = EChatMode.LOCAL;
                    OpenChat();

                }
                else if (Event.current.keyCode == ControlsSettings.group)
                {
                    _chatMode = EChatMode.GROUP;
                    OpenChat();
                }
            }
            if (_chatting)
            {
                _chatField.FocusControl();
            }
            MenuConfigurationControlsUI.bindOnGUI();
        }

        public override void Dispose()
        {
            OnUpdateManager.Instance.OnGUIEvent -= OnGUI;
            OnUpdateManager.Instance.OnUpdateEvent -= OnUpdate;

            _chatField.TryRemoveChild(_sendChatButton);
            _chatField.TryRemoveChild(_chatModeButton);

            _container.TryRemoveChild(_chatField);

            _container.TryRemoveChild(_chatScrollView);

            EditorUI.window.TryRemoveChild(_container);

            _chatEntries = null;

            _instance = null;
        }
    }
}
