using System;

namespace Cube2048.Core.Interfaces
{
    public interface IScoreService
    {
        int CurrentScore { get; }
        void AddScore(int amount);

      
        event Action<int> OnScoreChanged;
    }
}