using Newtonsoft.Json.Serialization;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.UI
{
    public abstract class EditorCustomUI
    {
        public SleekFullscreenBox Container { get; private set; }

        public bool Active { get; private set; }

        public abstract int ContainerOffsetX { get; }

        public abstract int ContainerOffsetY { get; }

        public abstract float ContainerPositionScaleX { get; }

        public abstract int ContainerSizeOffsetX { get; }

        public abstract int ContainerSizeOffsetY { get; }

        public abstract float ContainerSizeScaleX { get; }

        public abstract float ContainerSizeScaleY { get; }

        public delegate void onOpenHandler();

        public event onOpenHandler onOpen;

        public delegate void onCloseHandler();

        public event onCloseHandler onClose;

        public void Open()
        {
            if (!Active)
            {
                Active = true;
                Container.AnimateIntoView();
                onOpen?.Invoke();
            }
        }

        public void Close()
        {
            if (Active)
            {
                Active = false;
                Container.AnimateOutOfView(1f, 0f);
                onClose?.Invoke();
            }
        }

        public EditorCustomUI()
        {
            Container = new SleekFullscreenBox();
            Container.positionOffset_X = ContainerOffsetX;
            Container.positionOffset_Y = ContainerOffsetY;
            Container.positionScale_X = ContainerPositionScaleX;
            Container.sizeOffset_X = ContainerSizeOffsetX;
            Container.sizeOffset_Y = ContainerOffsetY;
            Container.sizeScale_X = ContainerSizeScaleX;
            Container.sizeScale_Y = ContainerSizeScaleY;
            EditorUI.window.AddChild(Container);
            Active = false;
        }
    }
}
