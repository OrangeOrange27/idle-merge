using System.Linq;
using Common.GlobalServiceLocator;
using Common.Inputs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera _camera;
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private Collider2D _boundingShape2D;

        private IInputManager _inputManager;
        private bool _isDragging;
        private bool _isMovementBlocked;

        private bool CanMove => _isDragging && !_isMovementBlocked;

        public UnityEngine.Camera Camera => _camera;

        private void Awake()
        {
            _inputManager = GlobalServices.Get<IInputManager>();
            _inputManager.OnInputStart += OnInputStart;
            _inputManager.OnInputEnd += OnInputEnd;
            _inputManager.OnStartDrag += OnDragStarted;
            _inputManager.OnEndDrag += OnDragEnded;
        }

        private void FixedUpdate()
        {
            if (!CanMove)
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
            _isDragging = true;
        }

        private void OnDragEnded(Vector3 pos)
        {
            _isDragging = false;
        }

        private void OnInputStart(Vector3 position)
        {
            _isMovementBlocked = ClickedOnUIWithIPointerHandler(position) || ClickedOnWorldIPointerHandler(position);
        }

        private void OnInputEnd(Vector3 position)
        {
            _isMovementBlocked = false;
        }

        private void OnDestroy()
        {
            if (_inputManager != null)
            {
                _inputManager.OnStartDrag -= OnDragStarted;
                _inputManager.OnEndDrag -= OnDragEnded;
                _inputManager.OnInputStart -= OnInputStart;
                _inputManager.OnInputEnd -= OnInputEnd;
            }
        }

        //TODO: refactor

        #region Tools

        private bool ClickedOnUIWithIPointerHandler(Vector3 position)
        {
            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = position
            };

            var raycastResults = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            return raycastResults.Any(result => result.gameObject.GetComponent<IPointerDownHandler>() != null);
        }

        private bool ClickedOnWorldIPointerHandler(Vector3 position)
        {
            var ray = Camera.ScreenPointToRay(position);
            if (Physics.Raycast(ray, out var hit, 100f, Physics.DefaultRaycastLayers))
            {
                return hit.collider.GetComponent<IPointerDownHandler>() != null;
            }

            return false;
        }

        #endregion
    }
}