using System;
using Features.Core;
using UnityEngine;

namespace Common.Inputs
{
    public interface IInputManager
    {
        public GameplayReactiveProperty<Vector3> InputPosition { get; }
        
        public event Action<Vector3> OnInputStart;
        public event Action<Vector3> OnInputEnd;
        public event Action<Vector3> OnClick;
        public event Action<Vector3> OnStartHold;
        public event Action<Vector3> OnEndHold;
        public event Action<Vector3> OnStartDrag;
        public event Action<Vector3> OnEndDrag;
    }
}