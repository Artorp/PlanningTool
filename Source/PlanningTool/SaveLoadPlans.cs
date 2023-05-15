using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;

namespace PlanningTool
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class SaveLoadPlans : KMonoBehaviour
    {
        [Serialize]
        public Dictionary<int, PlanData> PlanState;

        public static SaveLoadPlans Instance { get; private set; }

        public SaveLoadPlans()
        {
            PlanState = new Dictionary<int, PlanData>();
            Instance = this;
        }

        public static void DestroyInstance()
        {
            Instance = null;
        }

        [OnDeserialized]
        public void OnDeserialized()
        {
            if (PlanState == null)
            {
                Debug.LogWarning("[PlanningTool] PlanState was null after deserializing, reinitializing with no data.");
                PlanState = new Dictionary<int, PlanData>();
                return;
            }
            Debug.Log($"[PlanningTool] Loading {PlanState.Count} saved plans from save file.");
            foreach (var item in PlanState.Values)
            {
                PlanGrid.Plans[item.Cell] = PlanningToolInterface.CreatePlanTile(item.Cell);
            }
        }

        [SerializationConfig(MemberSerialization.OptIn)]
        public class PlanData
        {
            [Serialize]
            public int Cell;
        }
    }
}
