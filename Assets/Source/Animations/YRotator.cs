using UnityEngine;

namespace Source.Animations
{
    public class YRotator : MonoBehaviour
    {
        [SerializeField]
        private float _rotationSpeed = 90f;

        [SerializeField]
        private bool _pingPong;

        [SerializeField]
        private float _minAngle;

        [SerializeField]
        private float _maxAngle = 90f;

        [SerializeField]
        private AnimationCurve _easingCurve = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 2f),
            new Keyframe(1f, 1f, 0f, 0f));

        private float _accumulatedAngle;

        private void Update()
        {
            if (_pingPong)
            {
                // The distance (in degrees) of one sweep between the two angles.
                float range = _maxAngle - _minAngle;

                // Both angles are equal: there is nothing to sweep, and dividing by
                // the range below would produce NaN rotations.
                if (Mathf.Approximately(range, 0f))
                    return;

                // Advance the motion by the rotation speed so the overall pace is the
                // same as continuous mode; the easing curve only reshapes it.
                _accumulatedAngle += _rotationSpeed * Time.deltaTime;

                // Wrap the accumulated angle into one full back-and-forth cycle
                // (two sweeps). Unlike Mathf.PingPong, this keeps track of which
                // half we are in, which we need to mirror the easing curve below.
                float cycle = Mathf.Repeat(_accumulatedAngle, range * 2f);
                bool isMovingForward = cycle < range;

                // Normalized 0..1 progress through the current sweep.
                float progress = isMovingForward ? cycle / range : (cycle - range) / range;

                // Remap progress through the easing curve. With the default ease-out
                // curve each sweep starts fast and decelerates into its endpoint.
                float easedProgress = _easingCurve.Evaluate(progress);

                // Forward sweeps ease into the max angle, return sweeps into the min
                // angle, so easing is applied at both endpoints.
                float targetAngle = isMovingForward
                    ? Mathf.Lerp(_minAngle, _maxAngle, easedProgress)
                    : Mathf.Lerp(_maxAngle, _minAngle, easedProgress);

                // Write only the y component so any x/z rotation is preserved.
                Vector3 eulerAngles = transform.localEulerAngles;
                eulerAngles.y = targetAngle;
                transform.localEulerAngles = eulerAngles;
            }
            else
            {
                transform.Rotate(0f, _rotationSpeed * Time.deltaTime, 0f);
            }
        }
    }
}
