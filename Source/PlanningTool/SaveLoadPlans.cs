using System;
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

        [Serialize] private float _activeFloat;
        public event Action<float> OnActiveAlphaChange;

        public float ActiveFloat
        {
            get => _activeFloat;
            set
            {
                if (Math.Abs(_activeFloat - value) < 0.001f) return;
                _activeFloat = value;
                OnActiveAlphaChange.Signal(value);
            }
        }

        [Serialize] private bool _hidePlans;
        public event Action<bool> OnHidePlansChanged;

        public bool HidePlans
        {
            get => _hidePlans;
            set
            {
                if (value == _hidePlans) return;
                _hidePlans = value;
                OnHidePlansChanged.Signal(value);
            }
        }

        public static SaveLoadPlans Instance { get; private set; }

        public SaveLoadPlans()
        {
            PlanState = new Dictionary<int, PlanData>();
            Instance = this;
            _activeFloat = 0.2f;
            _hidePlans = false;
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
                var gameObject = PlanningToolInterface.CreatePlanTile(item);
                PlanGrid.Plans[item.Cell] = gameObject;
                gameObject.SetActive(!HidePlans);
            }
        }

        [SerializationConfig(MemberSerialization.OptIn)]
        public class PlanData
        {
            [Serialize] public int Cell;
            [Serialize] public PlanShape Shape;
            [Serialize] public PlanColor Color;

            public bool IsEquivalentTo(PlanData other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Cell == other.Cell && Shape == other.Shape && Color == other.Color;
            }

            public static bool IsEquivalent(PlanData first, PlanData second)
            {
                if (ReferenceEquals(first, second)) return true;
                if (first is null) return false;
                return first.IsEquivalentTo(second);
            }
        }
    }
}
