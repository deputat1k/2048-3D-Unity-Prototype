using System;

namespace Cube2048.Input
{
    public interface IInputHandler
    {
        event Action<float> OnDrag;
        event Action OnRelease;     
    }
}