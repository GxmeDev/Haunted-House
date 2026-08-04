using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Source.UI
{
    public class Main : MonoBehaviour
    {
        [SerializeField] private float _fadeDuration = 1f;
        [SerializeField] private float _resetWaitTime = 2f;

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

        private void Start()
        {
            GameEvents.Caught += OnCaught;
        }

        private void OnDestroy()
        {
            _panelRendererComponent.UnregisterUIReloadCallback(OnUIReload);

            // GameEvents is static, so an un-removed handler would outlive this
            // component and throw on the next Caught after a scene reload.
            GameEvents.Caught -= OnCaught;
        }

        private void OnCaught()
        {
            StartCoroutine(FadeElement(CaughtScreen));
        }

        private void OnUIReload(PanelRenderer panelRenderer, VisualElement root, int version)
        {
            EndScreen = root.Q<VisualElement>("EndScreen");
            CaughtScreen = root.Q<VisualElement>("CaughtScreen");
        }

        private IEnumerator FadeElement(VisualElement element)
        {
            // Ramp the opacity up each frame so the element fades into full
            // visibility over _fadeDuration seconds.
            float elapsed = 0f;

            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                element.style.opacity = Mathf.Clamp01(elapsed / _fadeDuration);
                yield return null;
            }

            // Hold the fully visible element on screen, then hide it instantly
            // (no fade-out) so it's ready for the next fade-in.
            yield return new WaitForSeconds(_resetWaitTime);

            element.style.opacity = 0f;
        }
    }
}
