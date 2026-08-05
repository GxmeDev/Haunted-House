using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Source.UI
{
    public class Main : MonoBehaviour
    {
        [SerializeField] private float _fadeDuration = 1f;
        [SerializeField] private float _resetWaitTime = 2f;

        public VisualElement EndScreen { get; private set; }
        public VisualElement CaughtScreen { get; private set; }
        public VisualElement DialogueBox { get; private set; }
        public Label CharacterNameLabel { get; private set; }
        public Label DialogueTextLabel { get; private set; }

        private PanelRenderer _panelRendererComponent;
        private List<string> _dialogueText;
        private InputAction _interactAction;
        private int _dialogueIndex;

        private void Awake()
        {
            _panelRendererComponent = GetComponent<PanelRenderer>();
            _interactAction = InputSystem.actions.FindAction("Player/Interact");

            // PanelRenderer has no root-element property; the reload callback is
            // the documented way to get the root, and it also re-runs the queries
            // if the UI ever reloads so the references never go stale.
            _panelRendererComponent.RegisterUIReloadCallback(OnUIReload);
        }

        private void Start()
        {
            GameEvents.Caught += OnCaught;
            GameEvents.StartDialogue += OnStartDialogue;
        }

        private void OnDestroy()
        {
            _panelRendererComponent.UnregisterUIReloadCallback(OnUIReload);

            // GameEvents is static, so an un-removed handler would outlive this
            // component and throw on the next Caught after a scene reload.
            GameEvents.Caught -= OnCaught;
            GameEvents.StartDialogue -= OnStartDialogue;

            // A mid-dialogue scene teardown must not leave a dangling handler
            // on the shared Interact action; removing when absent is harmless.
            _interactAction.performed -= OnInteractPerformed;
        }

        private void OnCaught()
        {
            StartCoroutine(FadeElement(CaughtScreen));
        }

        private void OnUIReload(PanelRenderer panelRenderer, VisualElement root, int version)
        {
            EndScreen = root.Q<VisualElement>("EndScreen");
            CaughtScreen = root.Q<VisualElement>("CaughtScreen");
            DialogueBox = root.Q<VisualElement>("DialogueBox");
            CharacterNameLabel = root.Q<Label>("CharacterName");
            DialogueTextLabel = root.Q<Label>("DialogueText");
        }

        private void OnStartDialogue(string characterName, Color characterNameColor, List<string> dialogueText)
        {
            // The dialogue box ships hidden (display: none in MainUI.uxml), so
            // it's shown via display rather than opacity.
            DialogueBox.style.display = DisplayStyle.Flex;
            CharacterNameLabel.text = characterName;
            CharacterNameLabel.style.color = characterNameColor;

            // Copy the lines so advancing through them later can't be affected
            // by (or mutate) the NPC's serialized list.
            _dialogueText = new List<string>(dialogueText);
            _dialogueIndex = 0;
            DialogueTextLabel.text = _dialogueText[_dialogueIndex];

            // Advance one line per Interact press. The Input System defers
            // callback-list changes made during invocation, so subscribing here
            // (inside the same E press that started the dialogue) does not fire
            // OnInteractPerformed for that press.
            _interactAction.performed += OnInteractPerformed;
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            _dialogueIndex++;

            // All lines rendered: close the conversation and stop listening
            // until the next StartDialogue re-subscribes.
            if (_dialogueIndex >= _dialogueText.Count)
            {
                DialogueBox.style.display = DisplayStyle.None;
                _interactAction.performed -= OnInteractPerformed;
                GameEvents.RaiseExitDialogue();
                return;
            }

            DialogueTextLabel.text = _dialogueText[_dialogueIndex];
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

            // The element is now fully visible — let listeners react (e.g. respawn
            // the player) while the screen still covers the reset.
            GameEvents.RaiseFadeInComplete();

            // Hold the fully visible element on screen, then hide it instantly
            // (no fade-out) so it's ready for the next fade-in.
            yield return new WaitForSeconds(_resetWaitTime);

            element.style.opacity = 0f;

            // The screen is hidden again — let listeners restore player control
            // now that the respawn is fully over.
            GameEvents.RaiseFadeScreenReset();
        }
    }
}
