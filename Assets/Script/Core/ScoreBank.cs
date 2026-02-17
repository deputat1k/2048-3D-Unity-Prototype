using UnityEngine;
using System; // Для Action
using Cube2048.Core.Interfaces;

namespace Cube2048.Core // Перевір, чи правильний namespace у тебе
{
    public class ScoreBank : MonoBehaviour, IScoreService
    {
        public int CurrentScore { get; private set; }

        public event Action<int> OnScoreChanged;

        public void AddScore(int amount)
        {
            CurrentScore += amount;

           
            OnScoreChanged?.Invoke(CurrentScore);
        }
    }
}