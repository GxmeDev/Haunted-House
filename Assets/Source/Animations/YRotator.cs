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

        private float _accumulatedAngle;

        private void Update()
        {
            if (_pingPong)
            {
                _accumulatedAngle += _rotationSpeed * Time.deltaTime;
                Vector3 eulerAngles = transform.localEulerAngles;
                eulerAngles.y = _minAngle + Mathf.PingPong(_accumulatedAngle, _maxAngle - _minAngle);
                transform.localEulerAngles = eulerAngles;
            }
            else
            {
                transform.Rotate(0f, _rotationSpeed * Time.deltaTime, 0f);
            }
        }
    }
}
