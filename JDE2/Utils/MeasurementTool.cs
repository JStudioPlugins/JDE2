using SDG.Framework.Utilities;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JDE2.Utils
{
    public class MeasurementTool : MonoBehaviour
    {
        private static MeasurementTool _instance;

        private static bool _active;

        public static bool Active = _active;

        private bool _switch;

        private Vector3 _start;
        
        private Vector3 _end;

        private float distance;

        public static MeasurementTool Get()
        {
            return _instance;
        }

        public static void Activate()
        {
            if (!_active)
            {
                _instance = Main.BackingObj.AddComponent<MeasurementTool>();
            }
            _active = true;
        }

        public static void Deactivate()
        {
            if (!_active)
                return;
            _instance.CleanUp();
            Destroy(_instance);
            _instance = null;
            _active = false;
        }

        internal void Awake()
        {
            _switch = false;
            TimeUtility.updated += OnUpdateGizmos;
        }

        internal void CleanUp()
        {
            TimeUtility.updated -= OnUpdateGizmos;
        }

        internal void FixedUpdate()
        {
            if (!_active)
                return;

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && InputEx.GetKeyDown(ControlsSettings.primary))
            {
                if (_switch)
                {
                    _end = GetCursorHit().point;
                    _switch = false;
                }
                else
                {
                    _start = GetCursorHit().point;
                    _switch = true;
                }
            }

            if ((_start -  _end).sqrMagnitude != distance * distance)
                distance = Vector3.Distance(_start, _end);
        }

        private void OnUpdateGizmos()
        {
            if (_start != null)
                RuntimeGizmos.Get().Sphere(_start, 0.25f, Color.red);
            if (_end != null)
                RuntimeGizmos.Get().Sphere(_end, 0.25f, Color.blue);
            if (_start != null && _end != null)
            {
                RuntimeGizmos.Get().Line(_start, _end, Color.cyan);
                RuntimeGizmos.Get().Label((_start + _end) / 2f, distance.ToString() + " m");
            }
        }

        private RaycastHit GetCursorHit()
        {
            var ray = MainCamera.instance.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit worldHit, 2048f, RayMasks.EDITOR_WORLD);
            return worldHit;
        }
    }
}
