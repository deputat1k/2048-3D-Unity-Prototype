using UnityEngine;
using UnityEngine.SceneManagement;
namespace Cube2048.Core
{
    public class LevelLoader : MonoBehaviour
    {
        public void ReloadCurrentLevel()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
    }
}