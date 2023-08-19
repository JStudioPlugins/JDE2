using HarmonyLib;
using JDE2.SituationDependent;
using JDE2.Tools;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Managers
{
    public class PatchManager : IJDEDependent
    {
        public static PatchManager Instance { get; private set; }
        
        public PatchManager() 
        {
            Instance = this;
            Init();
            Main.Instance.OnUnload += CleanUp;
        }

        public static Harmony harmony;
        public const string harmonyId = "Jdance.JDE2";

        public void Init()
        {
            try
            {
                harmony = new Harmony(harmonyId);
                harmony.PatchAll();
            }
            catch (Exception e)
            {
                UnturnedLog.exception(e);
            }
        }

        public void CleanUp()
        {
            try
            {
                harmony.UnpatchAll(harmonyId);

            }
            catch (Exception e)
            {
                UnturnedLog.exception(e);
            }
            Instance = null;
        }
    }
}
