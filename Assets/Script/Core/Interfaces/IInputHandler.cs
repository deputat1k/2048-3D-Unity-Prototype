using System;


namespace Cube2048.Core.Interfaces
{
    public interface IInputHandler
    {
        event Action<float> OnDrag;
        event Action OnRelease;
    }
}