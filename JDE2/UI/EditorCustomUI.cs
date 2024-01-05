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

        public virtual ISleekElement ParentElement { get; }

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
            Container.PositionOffset_X = ContainerOffsetX;
            Container.PositionOffset_Y = ContainerOffsetY;
            Container.PositionScale_X = ContainerPositionScaleX;
            Container.SizeOffset_X = ContainerSizeOffsetX;
            Container.SizeOffset_Y = ContainerOffsetY;
            Container.SizeScale_X = ContainerSizeScaleX;
            Container.SizeScale_Y = ContainerSizeScaleY;
            if (ParentElement == null)
            {
                EditorUI.window.AddChild(Container);
            }
            else
            {
                ParentElement.AddChild(Container);
            }
            Active = false;
        }
    }
}
