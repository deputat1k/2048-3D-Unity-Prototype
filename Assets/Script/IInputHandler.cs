using System;

public interface IInputHandler
{
    event Action<float> OnDrag; // Подія перетягування
    event Action OnRelease;     // Подія відпускання 
}