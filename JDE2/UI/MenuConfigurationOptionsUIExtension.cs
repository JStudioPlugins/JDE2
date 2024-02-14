using DanielWillett.UITools.API.Extensions;
using DanielWillett.UITools.API.Extensions.Members;
using DanielWillett.UITools.Util;
using JDE2.Managers;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.UI
{
    [UIExtension(typeof(MenuConfigurationOptionsUI))]
    public class MenuConfigurationOptionsUIExtension : UIExtensionWrapper
    {
        [ExistingMember("optionsBox")]
        private readonly ISleekScrollView _optionsBox;

        private List<ISleekElement> _elements;

        public override void Dispose()
        {
            foreach (var element in _elements)
            {
                _optionsBox.TryRemoveChild(element);
            }
        }

        public MenuConfigurationOptionsUIExtension() : base()
        {
            OptionManager.SetupOptions(_optionsBox, out List<ISleekElement> activeElements);
            _elements = activeElements;
        }
    }
}
