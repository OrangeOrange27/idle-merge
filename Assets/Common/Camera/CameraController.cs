using Common.GlobalServiceLocator;
using Common.Inputs;
using UnityEngine;

namespace Common.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera _camera;
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private Collider2D _boundingShape2D;

        private IInputManager _inputManager;
        private bool _canMove;

        private void Awake()
        {
            _inputManager = GlobalServices.Get<IInputManager>();
            _inputManager.OnStartDrag += OnDragStarted;
            _inputManager.OnEndDrag += OnDragEnded;
        }

        private void FixedUpdate()
        {
            if (!_canMove)
                return;

            var inputWorldPos = _camera.ScreenToWorldPoint(_inputManager.InputPosition.CurrentValue);
            var lastInputWorldPos = _camera.ScreenToWorldPoint(_inputManager.InputPosition.PreviousValue);
            var delta = inputWorldPos - lastInputWorldPos;

            _cameraTarget.position -= delta;

            KeepCameraTargetInsideFrustumBounds();
        }

        private void KeepCameraTargetInsideFrustumBounds()
        {
            if (_boundingShape2D == null)
                return;

            var bounds = _boundingShape2D.bounds;
            var vertExtent = _camera.orthographicSize;
            var horzExtent = vertExtent * _camera.aspect;
            var pos = _cameraTarget.position;

            pos.x = Mathf.Clamp(pos.x, bounds.min.x + horzExtent, bounds.max.x - horzExtent);
            pos.y = Mathf.Clamp(pos.y, bounds.min.y + vertExtent, bounds.max.y - vertExtent);

            _cameraTarget.position = pos;
        }

        private void OnDragStarted(Vector3 pos)
        {
            _canMove = true;
        }

        private void OnDragEnded(Vector3 pos)
        {
            _canMove = false;
        }

        private void OnDestroy()
        {
            if (_inputManager != null)
            {
                _inputManager.OnStartDrag -= OnDragStarted;
                _inputManager.OnEndDrag -= OnDragEnded;
            }
        }
    }
}