using PeterHan.PLib.UI;
using UnityEngine;

namespace PlanningTool
{
    /// <summary>
    /// Simple wrapper around an actual gameObject. Will not copy the gameObject on realization.
    /// </summary>
    public class UIComponentWrapper : IUIComponent
    {
        private GameObject _gameObject;

        public UIComponentWrapper(GameObject gameObject)
        {
            _gameObject = gameObject;
            Name = gameObject.name;
        }

        public GameObject Build()
        {
            OnRealize?.Invoke(_gameObject);
            return _gameObject;
        }

        public string Name { get; }

        public event PUIDelegates.OnRealize OnRealize;
    }
}
