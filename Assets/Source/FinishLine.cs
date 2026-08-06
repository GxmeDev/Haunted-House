using UnityEngine;

namespace Source
{
    [RequireComponent(typeof(AudioSource))]
    public class FinishLine : MonoBehaviour
    {
        private Collider _colliderComponent;

        // Plays the escape sting when the player reaches the finish line.
        private AudioSource _audioSourceComponent;

        private void Awake()
        {
            _colliderComponent = GetComponent<Collider>();
            _audioSourceComponent = GetComponent<AudioSource>();

            // The finish line must not be reachable until the NPC
            // conversation has been completed.
            _colliderComponent.enabled = false;
        }

        private void OnEnable()
        {
            GameEvents.ExitDialogue += OnExitDialogue;
            GameEvents.Caught += OnCaught;
        }

        private void OnDisable()
        {
            GameEvents.ExitDialogue -= OnExitDialogue;
            GameEvents.Caught -= OnCaught;
        }

        private void OnExitDialogue()
        {
            _colliderComponent.enabled = true;
        }

        private void OnCaught()
        {
            // Being caught resets the level: the finish line becomes
            // unreachable until the NPC dialogue is completed again.
            _colliderComponent.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Only the player escaping counts; ignore enemies or props
            // wandering into the finish zone.
            if (!other.CompareTag("Player"))
                return;

            _audioSourceComponent.Play();
            GameEvents.RaiseEscaped();
        }
    }
}
