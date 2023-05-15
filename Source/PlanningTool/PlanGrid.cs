using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace PlanningTool
{
    public class PlanGrid
    {
        public static Dictionary<int, GameObject> PlansDict;
        public static PlanIndexer Plans;

        public static void Initialize()
        {
            // should be called once Grid has been initialized
            PlansDict = new Dictionary<int, GameObject>();
        }

        public static void Clear()
        {
            PlansDict = null;
        }

        public struct PlanIndexer
        {
            public GameObject this[int cell]
            {
                get
                {
                    GameObject go = null;
                    PlansDict.TryGetValue(cell, out go);
                    return go;
                }
                set
                {
                    if (value == null)
                    {
                        PlansDict.Remove(cell);
                    }
                    else
                    {
                        PlansDict[cell] = value;
                    }
                }
            }
        }
    }
}
