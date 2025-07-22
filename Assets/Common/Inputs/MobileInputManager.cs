using System;
using Features.Core;
using UnityEngine;
using VContainer.Unity;

namespace Common.Inputs
{
    public class MobileInputManager : IFixedTickable, IInputManager
    {
        private const float DragThreshold = 10f;
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
        private int _activeFingerId = -1;

        public void FixedTick()
        {
            if (Input.touchCount == 0)
                return;

            var touch = GetPrimaryTouch();
            if (touch.fingerId != _activeFingerId && _activeFingerId != -1)
                return;
            
            InputPosition.Value = touch.position;
            
            OnInputStart?.Invoke(InputPosition.Value);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _inputStartPosition = touch.position;
                    _holdStartTime = Time.time;
                    _isHolding = true;
                    _isDragging = false;
                    _activeFingerId = touch.fingerId;
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (_isHolding && Time.time - _holdStartTime >= HoldDelay)
                    {
                        OnStartHold?.Invoke(_inputStartPosition);
                        _isHolding = false;
                    }

                    if (!_isDragging && Vector3.Distance(_inputStartPosition, touch.position) > DragThreshold)
                    {
                        _isDragging = true;
                        OnStartDrag?.Invoke(_inputStartPosition);
                    }

                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (!_isDragging && !_isHolding)
                    {
                        OnClick?.Invoke(touch.position);
                    }

                    if (_isDragging)
                    {
                        OnEndDrag?.Invoke(touch.position);
                    }

                    if (!_isHolding)
                    {
                        OnEndHold?.Invoke(touch.position);
                    }
                    
                    OnInputEnd?.Invoke(touch.position);

                    ResetInputState();
                    break;
            }
        }

        private Touch GetPrimaryTouch()
        {
            return Input.GetTouch(0);
        }

        private void ResetInputState()
        {
            _isHolding = false;
            _isDragging = false;
            _holdStartTime = 0f;
            _activeFingerId = -1;
        }
    }
}