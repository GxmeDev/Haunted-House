using UnityEngine;
using UnityEngine.UIElements;

namespace Source.UI
{
    public class Main : MonoBehaviour
    {
        public VisualElement EndScreen { get; private set; }
        public VisualElement CaughtScreen { get; private set; }

        private PanelRenderer _panelRendererComponent;

        private void Awake()
        {
            _panelRendererComponent = GetComponent<PanelRenderer>();

            // PanelRenderer has no root-element property; the reload callback is
            // the documented way to get the root, and it also re-runs the queries
            // if the UI ever reloads so the references never go stale.
            _panelRendererComponent.RegisterUIReloadCallback(OnUIReload);
        }

        private void OnDestroy()
        {
            _panelRendererComponent.UnregisterUIReloadCallback(OnUIReload);
        }

        private void OnUIReload(PanelRenderer panelRenderer, VisualElement root, int version)
        {
            EndScreen = root.Q<VisualElement>("EndScreen");
            CaughtScreen = root.Q<VisualElement>("CaughtScreen");

            // Verification logging: null here means the ID doesn't exist in the UXML.
            Debug.Log($"EndScreen: {EndScreen}");
            Debug.Log($"CaughtScreen: {CaughtScreen}");
        }
    }
}
