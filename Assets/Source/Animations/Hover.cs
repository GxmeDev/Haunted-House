using UnityEngine;

namespace Source.Animations
{
    public class Hover : MonoBehaviour
    {
        [SerializeField]
        private float _hoverDistance = 0.25f;

        [SerializeField]
        private float _speed = 1f;

        [SerializeField]
        private AnimationCurve _easingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private Vector3 _startPosition;

        private float _accumulatedTime;

        private void Start()
        {
            // The hover oscillates around wherever the object was placed when
            // the game started.
            _startPosition = transform.localPosition;
        }

        private void Update()
        {
            // Advance the animation; at speed 1 a full up-or-down sweep takes
            // one second.
            _accumulatedTime += _speed * Time.deltaTime;

            // Triangle wave: progress runs 0 -> 1 (rising) then 1 -> 0
            // (falling), so the object floats back and forth forever.
            float progress = Mathf.PingPong(_accumulatedTime, 1f);

            // The ease-in-out curve is flat at both ends, so the motion
            // decelerates into the top and bottom of the hover on both the
            // rising and falling halves.
            float easedProgress = _easingCurve.Evaluate(progress);

            transform.localPosition = _startPosition + Vector3.up * (_hoverDistance * easedProgress);
        }
    }
}
