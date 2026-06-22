using UnityEngine;
using UnityEngine.UI;  // 🔥 ЭТОГО НЕ ХВАТАЕТ!
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Объекты")]
    public GameObject player;
    public GameObject mainCamera;
    public GameObject gameCanvas;

    private bool isGameStarted = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        if (isGameStarted) return;
        isGameStarted = true;

        Debug.Log("[GameManager] 🎮 Игра началась!");
    }

    public void RegisterPlayer(GameObject playerObj)
    {
        player = playerObj;
        DontDestroyOnLoad(player);
    }

    public void RegisterCamera(GameObject cameraObj)
    {
        mainCamera = cameraObj;
        DontDestroyOnLoad(cameraObj);
    }

    public void RegisterCanvas(GameObject canvasObj)
    {
        gameCanvas = canvasObj;
        DontDestroyOnLoad(canvasObj);
        
        // 🔥 СБРАСЫВАЕМ UI ПРИ РЕГИСТРАЦИИ
        ResetUI();
    }

    /// <summary>
    /// Сбросить все UI элементы в начальное состояние
    /// </summary>
    void ResetUI()
    {
        if (gameCanvas == null) return;

        // 1. Закрываем дневник
        JournalUI journalUI = gameCanvas.GetComponentInChildren<JournalUI>();
        if (journalUI != null)
        {
            // Принудительно закрываем книгу
            journalUI.HideJournal();
        }

        // 2. Скрываем FadeImage (делаем прозрачным)
        Image fadeImage = gameCanvas.GetComponentInChildren<Image>();
        if (fadeImage != null && fadeImage.name == "FadeImage")
        {
            fadeImage.color = new Color(0, 0, 0, 0); // Полностью прозрачный
        }

        // 3. Скрываем плашку задач (если есть)
        QuestUI questUI = gameCanvas.GetComponentInChildren<QuestUI>();
        if (questUI != null)
        {
            questUI.HideQuest();
        }

        Debug.Log("[GameManager] ✅ UI сброшен");
    }

    public void HideGameUI()
    {
        if (gameCanvas != null)
            gameCanvas.SetActive(false);
    }

    public void ShowGameUI()
    {
        if (gameCanvas != null)
            gameCanvas.SetActive(true);
    }
}