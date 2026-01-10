using UnityEngine;
using System; // Потрібно для Events

public class InputHandler : MonoBehaviour
{
    // Події, на які можуть підписатися інші скрипти (Observer Pattern)
    public event Action<float> OnDrag;  // Передаємо, наскільки зсунулась мишка
    public event Action OnRelease;      // Сигнал, що палець відпустили
    public event Action OnTouchDown;    // Сигнал, що натиснули

    private float lastMouseX;
    private bool isDragging = false;

    void Update()
    {
        // 1. Натискання
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMouseX = Input.mousePosition.x;
            OnTouchDown?.Invoke(); // Повідомляємо всіх, хто слухає
        }

        // 2. Утримання (Рух)
        if (Input.GetMouseButton(0) && isDragging)
        {
            float delta = Input.mousePosition.x - lastMouseX;
            OnDrag?.Invoke(delta); // Передаємо різницю руху
            lastMouseX = Input.mousePosition.x;
        }

        // 3. Відпускання
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            OnRelease?.Invoke(); // Повідомляємо про постріл
        }
    }
}