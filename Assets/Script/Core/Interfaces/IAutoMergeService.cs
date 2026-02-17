using Cysharp.Threading.Tasks;

namespace Cube2048.Core.Interfaces
{
    public interface IAutoMergeService
    {
        bool HasPair { get; }
        bool IsMerging { get; }
        UniTask TriggerMerge();
    }
}