using UnityEngine;
using System;

namespace Cube2048.Input
{
    public class InputHandler : MonoBehaviour, IInputHandler
    {
        public event Action<float> OnDrag;
        public event Action OnRelease;
        public event Action OnTouchDown;

        private float lastMouseX;
        private bool isDragging = false;

        void Update()
        { 
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                lastMouseX = UnityEngine.Input.mousePosition.x;
                OnTouchDown?.Invoke();
            }

            if (UnityEngine.Input.GetMouseButton(0) && isDragging)
            {
                float delta = UnityEngine.Input.mousePosition.x - lastMouseX;
                OnDrag?.Invoke(delta);
                lastMouseX = UnityEngine.Input.mousePosition.x;
            }

            if (UnityEngine.Input.GetMouseButtonUp(0) && isDragging)
            {
                isDragging = false;
                OnRelease?.Invoke();
            }
        }
    }
}