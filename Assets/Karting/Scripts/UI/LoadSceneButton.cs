using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KartGame.UI
{
    public class LoadSceneButton: MonoBehaviour
    {
        public void LoadTargetScene(string name) 
        {
            SceneManager.LoadSceneAsync(name);
        }

        public void LoadTargetScene(int index)
        {
            SceneManager.LoadSceneAsync(index);
        }

        public void LoadNextScene()
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            LoadTargetScene(index + 1);
        }

        public void RestartScene()
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            LoadTargetScene(index);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
