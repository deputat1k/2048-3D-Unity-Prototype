using UnityEngine;
using System;
using UnityEngine.EventSystems;
using Cube2048.Core.Interfaces; 

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
                if (EventSystem.current.IsPointerOverGameObject()) return;

                // Перевірка на тач для мобільних
                if (UnityEngine.Input.touchCount > 0 &&
                    EventSystem.current.IsPointerOverGameObject(UnityEngine.Input.GetTouch(0).fingerId)) return;

                isDragging = true;
                lastMouseX = UnityEngine.Input.mousePosition.x;
                OnTouchDown?.Invoke();
            }

            if (UnityEngine.Input.GetMouseButton(0) && isDragging)
            {
                float currentMouseX = UnityEngine.Input.mousePosition.x;
                float delta = currentMouseX - lastMouseX;

                OnDrag?.Invoke(delta);

                lastMouseX = currentMouseX;
            }

            if (UnityEngine.Input.GetMouseButtonUp(0) && isDragging)
            {
                isDragging = false;
                OnRelease?.Invoke();
            }
        }
    }
}