using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour 
{
    [Tooltip("Ссылка на IntroManager")]
    public IntroSequence introSequence;

    public void StartGame()
    {
        Debug.Log("[MainMenu] Запускаем интро...");
        
        if (introSequence != null)
        {
            introSequence.StartIntro();
        }
        else
        {
            Debug.LogError("[MainMenu] IntroSequence не назначен!");
            SceneManager.LoadScene("Level1");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}