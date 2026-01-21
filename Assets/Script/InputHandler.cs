using UnityEngine;
using System;

public class InputHandler : MonoBehaviour
{
    
    public event Action<float> OnDrag;  
    public event Action OnRelease;     
    public event Action OnTouchDown;    

    private float lastMouseX;
    private bool isDragging = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMouseX = Input.mousePosition.x;
            OnTouchDown?.Invoke(); // Повідомляємо всіх хто слухає
        }

   
        if (Input.GetMouseButton(0) && isDragging)
        {
            float delta = Input.mousePosition.x - lastMouseX;
            OnDrag?.Invoke(delta); // Передаємо різницю руху
            lastMouseX = Input.mousePosition.x;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            OnRelease?.Invoke(); // Повідомляємо про постріл
        }
    }
}