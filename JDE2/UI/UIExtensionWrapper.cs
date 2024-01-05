using DanielWillett.UITools.API.Extensions;
using JDE2.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.UI
{
    public abstract class UIExtensionWrapper : UIExtension, IDisposable
    {
        public abstract void Dispose();

        public UIExtensionWrapper()
        {
            LoggingManager.Log($"STARTED UI EXTENSION {{{{cyan}}}}{this.GetType().FullName}{{{{_}}}}", true);

            if (Config.Instance.Data.DeveloperConfig.DisabledTypes.Contains(this.GetType().FullName))
            {
                Dispose();
            }
        }
    }
}
