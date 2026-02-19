using Cysharp.Threading.Tasks;
using System;

namespace Cube2048.Core.Interfaces
{
    public interface IAutoMergeService
    {
        bool HasPair { get; }
        bool IsMerging { get; }
        bool IsOnCooldown { get; }
        UniTask TriggerMerge();

        event Action<bool> OnStatusChanged;
        void SetPaused(bool isPaused);
    }
}