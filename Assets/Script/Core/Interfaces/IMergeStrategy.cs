using System.Collections.Generic;
using Cube2048.Data; // Там де лежить CubeData

namespace Cube2048.Core.Interfaces
{
    public interface IMergeStrategy
    {
        // Метод приймає список даних і повертає "найкращу пару" (індекси A і B)
        // Якщо пари немає, повертає (-1, -1)
        (int indexA, int indexB) FindBestPair(List<CubeData> cubes);
    }
}