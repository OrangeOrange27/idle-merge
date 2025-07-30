using System;
using Features.Core;
using UnityEngine;
using VContainer.Unity;

namespace Common.Inputs
{
    public class DesktopInputManager : ITickable, IInputManager
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

        private bool IsDragging
        {
            get => _isDragging;
            set
            {
                if (value == _isDragging)
                    return;

                if (value)
                    OnStartDrag?.Invoke(_inputStartPosition);
                else
                    OnEndDrag?.Invoke(Input.mousePosition);

                _isDragging = value;
            }
        }

        private bool IsHolding
        {
            get => _isHolding;
            set
            {
                if (value == _isHolding)
                    return;

                if (value)
                    OnStartHold?.Invoke(_inputStartPosition);
                else
                    OnEndHold?.Invoke(Input.mousePosition);

                _isHolding = value;
            }
        }

        public void Tick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _inputStartPosition = Input.mousePosition;
                _holdStartTime = Time.time;

                OnInputStart?.Invoke(_inputStartPosition);
            }

            if (Input.GetMouseButton(0))
            {
                InputPosition.Value = Input.mousePosition;

                if (!IsHolding && Time.time - _holdStartTime >= HoldDelay)
                {
                    IsHolding = true;
                }

                var distance = Vector3.Distance(_inputStartPosition, Input.mousePosition);
                if (!IsDragging && distance > DragThreshold)
                {
                    IsDragging = true;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!IsDragging && !IsHolding)
                {
                    OnClick?.Invoke(Input.mousePosition);
                }

                OnInputEnd?.Invoke(Input.mousePosition);

                ResetInputState();
            }
        }

        private void ResetInputState()
        {
            IsHolding = false;
            IsDragging = false;
            _holdStartTime = 0f;
        }
    }
}