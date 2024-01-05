using DanielWillett.UITools.API.Extensions.Members;
using DanielWillett.UITools.API.Extensions;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DanielWillett.UITools.Util;
using Steamworks;

namespace JDE2.UI
{
    [UIExtension(typeof(MenuWorkshopSubmitUI))]
    public class MenuWorkshopSubmitUIExtension : UIExtensionWrapper
    {
        [ExistingMember("publishedButtons")]
        private readonly List<ISleekButton> _publishedButtons;

        [ExistingMember("container")]
        private readonly SleekFullscreenBox _container;

        public MenuWorkshopSubmitUIExtension() : base()
        {  
        }

        public override void Dispose()
        {

        }
    }
}
