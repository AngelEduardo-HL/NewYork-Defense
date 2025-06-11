// SceneController.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void LoadVictoryScene()
    {
        SceneManager.LoadScene("Victoria");
    }

    public void LoadDefeatScene()
    {
        SceneManager.LoadScene("Derrota");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
