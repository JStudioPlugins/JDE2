using DanielWillett.UITools.API.Extensions;
using DanielWillett.UITools.API.Extensions.Members;
using DanielWillett.UITools.Util;
using JDE2.Managers;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.UI
{
    [UIExtension(typeof(MenuDashboardUI))]
    public class MenuDashboardUIExtension : UIExtensionWrapper
    {
        private const string DiscordInvite = "https://discord.gg/XBSGmGTNWP";

        [ExistingMember("exitButton")]
        private readonly SleekButtonIcon _exitButton;

        [ExistingMember("container")]
        private readonly SleekFullscreenBox _container;

        private readonly ISleekButton _discordButton;

        public MenuDashboardUIExtension() : base()
        {
            _discordButton = Glazier.Get().CreateButton();

            _discordButton.CopyTransformFrom(_exitButton);
            _discordButton.PositionOffset_Y -= 60;
            _discordButton.Text = "JStudio Discord";
            _discordButton.FontSize = ESleekFontSize.Medium;
            _discordButton.OnClicked += OnClickedDiscordButton;
            _discordButton.BackgroundColor = ESleekTint.BACKGROUND;

            _container.AddChild(_discordButton);
        }

        private static void OnClickedDiscordButton(ISleekElement button)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = DiscordInvite,
                UseShellExecute = true
            });
        }

        public override void Dispose()
        {
            _discordButton.OnClicked -= OnClickedDiscordButton;
            _container.TryRemoveChild(_discordButton);
        }
    }
}
