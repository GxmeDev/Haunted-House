using UnityEngine;
using UnityEngine.Splines;

namespace Source.Enemy
{
    public class LookTowardSpline : MonoBehaviour
    {
        // How far ahead of the current progress (in normalized 0..1 spline
        // time) to sample the path when deciding which way to face.
        private const float LookAheadTime = 0.01f;

        [SerializeField]
        private float _rotationSpeed = 300f;

        private SplineAnimate _splineAnimateComponent;

        private void Awake()
        {
            _splineAnimateComponent = GetComponent<SplineAnimate>();
        }

        private void Update()
        {
            // NormalizedTime's integer part counts completed loops; keep only
            // the fractional 0..1 progress along the spline.
            float currentTime = Mathf.Repeat(_splineAnimateComponent.NormalizedTime, 1f);

            // Sample the spline slightly ahead (wrapping past the end so
            // looping paths keep working) to find where we are heading.
            float aheadTime = Mathf.Repeat(currentTime + LookAheadTime, 1f);
            Vector3 aheadPosition = _splineAnimateComponent.Container.EvaluatePosition(aheadTime);

            // Direction from here to the look-ahead point, flattened onto the
            // ground plane so only the y-axis rotation is affected.
            Vector3 direction = aheadPosition - transform.position;
            direction.y = 0f;

            // Too close to the look-ahead point to get a meaningful direction
            // (e.g. the animation hasn't started); keep the current facing.
            if (direction.sqrMagnitude < 0.0001f)
                return;

            // Turn toward the travel direction at most _rotationSpeed degrees
            // per second instead of snapping, so corners look natural.
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }
}
