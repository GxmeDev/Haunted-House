using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Source.UI
{
    [RequireComponent(typeof(AudioSource))]
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] private float _fadeOutDuration = 1f;

        public Button StartButton { get; private set; }

        private PanelRenderer _panelRendererComponent;

        // Fades the looping menu music out before the game scene loads.
        private AudioSource _audioSourceComponent;

        // Set on the first click so the start sequence can never run twice,
        // even if a UI reload delivers a fresh button mid-fade.
        private bool _hasStarted;

        private void Awake()
        {
            _panelRendererComponent = GetComponent<PanelRenderer>();
            _audioSourceComponent = GetComponent<AudioSource>();

            // PanelRenderer has no root-element property; the reload callback is
            // the documented way to get the root, and it also re-runs the query
            // if the UI ever reloads so the reference never goes stale.
            _panelRendererComponent.RegisterUIReloadCallback(OnUIReload);
        }

        private void OnDestroy()
        {
            _panelRendererComponent.UnregisterUIReloadCallback(OnUIReload);

            // The button is only assigned once the UI has loaded, so it can
            // still be null if the component is destroyed before that.
            if (StartButton != null)
                StartButton.clicked -= OnStartClicked;
        }

        private void OnUIReload(PanelRenderer panelRenderer, VisualElement root, int version)
        {
            StartButton = root.Q<Button>("StartButton");

            // A reload that arrives after the game has started must not make
            // the fresh button clickable again — keep it dead until the scene
            // transition finishes.
            if (_hasStarted)
            {
                StartButton.SetEnabled(false);
                return;
            }

            // Every reload delivers a fresh visual tree (and thus a fresh
            // button instance), so the click handler must be re-attached here.
            StartButton.clicked += OnStartClicked;
        }

        private void OnStartClicked()
        {
            if (_hasStarted)
                return;

            _hasStarted = true;

            // One-shot: stop listening and grey the button out immediately so
            // repeat clicks during the fade can't restart the sequence.
            StartButton.clicked -= OnStartClicked;
            StartButton.SetEnabled(false);

            StartCoroutine(FadeOutAndLoad());
        }

        private IEnumerator FadeOutAndLoad()
        {
            // Fade from whatever the mixer/user volume currently is rather
            // than assuming full volume.
            float startVolume = _audioSourceComponent.volume;
            float elapsed = 0f;

            while (elapsed < _fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                _audioSourceComponent.volume = Mathf.Lerp(startVolume, 0f, Mathf.Clamp01(elapsed / _fadeOutDuration));
                yield return null;
            }

            // The music is now silent — the game scene is expected to sit
            // right after this one in the Build Profiles scene list.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
