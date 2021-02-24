using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Instancia a llamar desde otras clases
    public static GameManager instance;

    /// <summary>
    /// Simple Singleton Patter
    /// </summary>
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Load next scene according to build settings
    /// </summary>
    public void LoadNextScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        LoadScene(currentScene + 1);
    }

    /// <summary>
    /// Load index defined scene
    /// </summary>
    /// <param name="index"></param>
    public void LoadScene(int index)
    {
        SceneManager.LoadSceneAsync(index);
    }


    /// <summary>
    /// Load name defines scene
    /// </summary>
    /// <param name="name"></param>
    public void LoadScene(string name)
    {
        SceneManager.LoadSceneAsync(name);
    }

    /// <summary>
    /// Ends the game
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}
