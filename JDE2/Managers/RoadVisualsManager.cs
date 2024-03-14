using JDE2.SituationDependent;
using SDG.Framework.Utilities;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using JDE2.Utils;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Drawing;
using System.IO;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using System.Reflection;

namespace JDE2.Managers;

    public class RoadVisualsManager : IEditorDependent
    {
        public RoadVisualsManager Instance { get; private set; }
        
        private List<Tuple<int, RoadPath>> _selectedVertexes = new();

        public IReadOnlyList<Tuple<int, RoadPath>> SelectedVertexes => _selectedVertexes.AsReadOnly();

        public RoadVisualsManager()
        {
            //if (!Config.Instance.Data.EditorConfig.RoadVisuals)
            //return;
            Instance = this;
            TimeUtility.updated += OnUpdateGizmos;
            OnUpdateManager.Instance.OnUpdateEvent += OnUpdate;
        }

        private void OnUpdate()
        {   
        }

        ~RoadVisualsManager()
        {
            Dispose(); 
        }

        private void OnUpdateGizmos()
        {
            if (EditorRoads.road == null)
                return;

            var material = LevelRoads.getRoadMaterial(EditorRoads.road.road);

            foreach (var joint in EditorRoads.road.joints)
            {
                RuntimeGizmos.Get().Sphere(new Vector3(joint.vertex.x, joint.vertex.y, joint.vertex.z), (material.width / 2f), Color.green);
            }
            
        }

        public void Dispose()
        {
            TimeUtility.updated -= OnUpdateGizmos;
            OnUpdateManager.Instance.OnUpdateEvent -= OnUpdate;
            _selectedVertexes = null;
            Instance = null;
            GC.SuppressFinalize(this);
        }
    }

