using JDE2.SituationDependent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JDE2.Managers
{
    public class OnUpdateManager : MonoBehaviour
    {
        public static OnUpdateManager Instance { get; private set; }

        public event Action OnUpdateEvent;

        public event Action OnGUIEvent;

        void Awake()
        {
            Instance = this;
        }

        void Update()
        {
            OnUpdateEvent?.Invoke();
        }

        void OnGUI()
        {
            OnGUIEvent?.Invoke();
        }
    }
}
