using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Ссылки на UI")]
    [Tooltip("Панель паузы (тёмный экран с кнопками)")]
    public GameObject pausePanel;

    private bool isPaused = false;

    void Start()
    {
            // DontDestroyOnLoad(gameObject);
        // Убеждаемся, что панель паузы скрыта в начале
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        // Пауза по клавише Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    /// <summary> Поставить игру на паузу </summary>
    public void PauseGame()
    {
        isPaused = true;
        if (pausePanel != null)
            pausePanel.SetActive(true);
        
        Time.timeScale = 0f; // Останавливаем время
    }

    /// <summary> Продолжить игру </summary>
    public void ResumeGame()
    {
        isPaused = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        Time.timeScale = 1f; // Возобновляем время
    }

    /// <summary> Кнопка "Продолжить" </summary>
    public void OnContinueButton()
    {
        ResumeGame();
    }

    /// <summary> Кнопка "Настройки" </summary>
    public void OnSettingsButton()
    {
        Debug.Log("Открыть настройки");
        // Здесь можно открыть панель настроек
    }

    /// <summary> Кнопка "Выйти в меню" </summary>
    public void OnExitToMenuButton()
    {
        Time.timeScale = 1f; // Сначала возобновляем время
        
        // Загружаем сцену главного меню
        // Замени "MainMenu" на точное название твоей сцены меню!
        SceneManager.LoadScene("Menu");
        
        Debug.Log("Выход в главное меню");
    }

    /// <summary> Кнопка "Выход из игры" (опционально) </summary>
    public void OnQuitGameButton()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    public void OnNewGameButton()
    {
        // Сначала возобновляем время, чтобы сцена загрузилась нормально
        Time.timeScale = 1f; 

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.StartNewGame();
        }
        else
        {
            // Если SaveManager по какой-то причине не найден, просто перезагружаем сцену
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}