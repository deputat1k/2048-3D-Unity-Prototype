using System.Collections.Generic;
using Cube2048.Data;
namespace Cube2048.Core.Interfaces
{
    public interface IMergeStrategy
    {
        (int indexA, int indexB) FindBestPair(List<CubeData> cubes);
    }
}