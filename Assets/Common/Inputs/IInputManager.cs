using System;
using UnityEngine;

namespace Common.Inputs
{
    public interface IInputManager
    {
        public event Action<Vector3> OnClick;
        public event Action<Vector3> OnStartHold;
        public event Action<Vector3> OnEndHold;
        public event Action<Vector3> OnStartDrag;
        public event Action<Vector3> OnEndDrag;
    }
}