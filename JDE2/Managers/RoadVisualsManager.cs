using JDE2.SituationDependent;
using SDG.Framework.Utilities;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JDE2.Managers
{
    public class RoadVisualsManager : IEditorDependent, IDisposable
    {
        

        public RoadVisualsManager()
        {
            if (!Config.Instance.Data.EditorConfig.RoadVisuals)
                return;

            TimeUtility.updated += OnUpdateGizmos;
        }

        ~RoadVisualsManager()
        {
            Dispose();
        }

        public void Dispose()
        {
            TimeUtility.updated -= OnUpdateGizmos;
            GC.SuppressFinalize(this);
        }

        private void OnUpdateGizmos()
        {
            if (!EditorRoads.isPaving)
                return;

            if (EditorRoads.road == null)
                return;

            foreach (var joint in EditorRoads.road.joints)
            {
                RuntimeGizmos.Get().Label(joint.vertex, $"Ignore Terrain: {joint.ignoreTerrain}");

                RuntimeGizmos.Get().Label(new Vector3(joint.vertex.x, joint.vertex.y - 1.5f, joint.vertex.z), $"Offset: {joint.offset}");

                RuntimeGizmos.Get().Label(new Vector3(joint.vertex.x, joint.vertex.y - 3.0f, joint.vertex.z), $"Mode: {joint.mode}");
            }
        }
    }
}
