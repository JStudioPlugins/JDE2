using JDE2.SituationDependent;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SDG.Framework.Utilities;
using System.Drawing;

namespace JDE2.Managers
{
    /*public class RoadParallelTestManager
    {
        public RoadParallelTestManager()
        {
            OnUpdateManager.Instance.OnUpdateEvent += OnUpdate;
        }

        public float Distance = 2f;

        public List<Tuple<Vector3, Vector3>> Lines;

        public void OnUpdate()
        {
            if (Level.isLoaded && Level.isEditor)
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.J))
                {
                    Lines = new();

                    var road = EditorRoads.road;

                    var jointPos = road.joints.ConvertAll(x => x.vertex).ToArray();

                    for (int i = 0; i < jointPos.Length - 1; i++)
                    {
                        Vector3 point1 = jointPos[i];
                        Vector3 point2 = jointPos[i + 1];

                        // Find the direction vector between the two points
                        Vector3 lineDirection = (point2 - point1).normalized;

                        // Find a vector perpendicular to the lineDirection
                        Vector3 perpendicularDir = new Vector3(lineDirection.y, lineDirection.x, -lineDirection.z).normalized;

                        // Calculate points for the parallel line
                        Vector3 startPoint = point1 + perpendicularDir * Distance;
                        Vector3 endPoint = point2 + perpendicularDir * Distance;

                        Lines.Add(new(startPoint, endPoint));
                    }

                    TimeUtility.updated += OnUpdateGizmos;
                }
                else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.K))
                {
                    TimeUtility.updated -= OnUpdateGizmos;
                }

            }
        }

        private void OnUpdateGizmos()
        {
            foreach (var tuple in Lines)
            {
                RuntimeGizmos.Get().Line(tuple.Item1, tuple.Item2, Palette.ADMIN);
            }
        }
    }*/
}
