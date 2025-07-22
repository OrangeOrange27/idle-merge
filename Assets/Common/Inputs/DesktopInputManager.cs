using System;
using Features.Core;
using UnityEngine;
using VContainer.Unity;

namespace Common.Inputs
{
    public class DesktopInputManager : IFixedTickable, IInputManager
    {
        private const float DragThreshold = 50f;
        private const float HoldDelay = 0.2f;

        public GameplayReactiveProperty<Vector3> InputPosition { get; } = new();
        
        public event Action<Vector3> OnInputStart;
        public event Action<Vector3> OnInputEnd;
        public event Action<Vector3> OnClick;
        public event Action<Vector3> OnStartHold;
        public event Action<Vector3> OnEndHold;
        public event Action<Vector3> OnStartDrag;
        public event Action<Vector3> OnEndDrag;

        private bool _isHolding;
        private bool _isDragging;
        private float _holdStartTime;
        private Vector3 _inputStartPosition;

        public void FixedTick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _inputStartPosition = Input.mousePosition;
                _holdStartTime = Time.time;
                _isHolding = true;
                
                OnInputStart?.Invoke(_inputStartPosition);
            }

            if (Input.GetMouseButton(0))
            {
                InputPosition.Value = Input.mousePosition;
                
                if (_isHolding && Time.time - _holdStartTime >= HoldDelay)
                {
                    OnStartHold?.Invoke(_inputStartPosition);
                    _isHolding = false;
                }

                var distance = Vector3.Distance(_inputStartPosition, Input.mousePosition);
                if (!_isDragging && distance > DragThreshold)
                {
                    _isDragging = true;
                    OnStartDrag?.Invoke(_inputStartPosition);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!_isDragging && !_isHolding)
                {
                    OnClick?.Invoke(Input.mousePosition);
                }

                if (_isDragging)
                {
                    OnEndDrag?.Invoke(Input.mousePosition);
                }

                if (!_isHolding)
                {
                    OnEndHold?.Invoke(Input.mousePosition);
                }
                
                OnInputEnd?.Invoke(Input.mousePosition);

                ResetInputState();
            }
        }

        private void ResetInputState()
        {
            _isHolding = false;
            _isDragging = false;
            _holdStartTime = 0f;
        }
    }
}