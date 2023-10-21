using JDE2.SituationDependent;
using JDE2.Utils;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JDE2.Managers
{
    public class AutoSaveManager : IEditorDependent
    {
        public static AutoSaveManager Instance { get; private set; }

        public AutoSaveManager()
        {
            if (Config.Instance.Data.EditorConfig.AutoSave)
            {
                Instance = this;


                ThreadPool.QueueUserWorkItem(async (o) => await AutoSave());
                Level.onLevelExited += CleanUp;
            }
        }

        public void CleanUp()
        {
            Instance = null;
        }

        public static async Task AutoSave()
        {
            while (true)
            {
                if (Instance == null) break;
                await Task.Delay(300000);

                DispatcherTool.QueueOnMainThread(() => Level.save());
            }
        }
    }
}
