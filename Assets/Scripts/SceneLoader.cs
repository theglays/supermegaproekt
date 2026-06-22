using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Tooltip("Имя сцены, которая загрузится поверх Bootstrap (например, Level1)")]
    public string firstLevelName = "Level1";

    void Start()
    {
        Debug.Log($"[SceneLoader] Начинаем загрузку сцены: {firstLevelName}");
        
        // Запускаем корутину для загрузки
        StartCoroutine(LoadLevelAdditively());
    }

    IEnumerator LoadLevelAdditively()
    {
        // Начинаем асинхронную загрузку
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(firstLevelName, LoadSceneMode.Additive);
        
        Debug.Log("[SceneLoader] Загрузка началась...");
        
        // Ждём пока сцена загрузится
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        Debug.Log("[SceneLoader] ✅ Сцена загружена, делаем активной...");
        
        // Теперь делаем её активной
        Scene loadedScene = SceneManager.GetSceneByName(firstLevelName);
        if (loadedScene.IsValid())
        {
            SceneManager.SetActiveScene(loadedScene);
            Debug.Log($"[SceneLoader] ✅ Сцена {firstLevelName} теперь активна!");
            
            // Проверяем, нашёлся ли Player
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                Debug.Log($"[SceneLoader] ✅ Player найден: {player.name}");
            }
            else
            {
                Debug.LogWarning("[SceneLoader] ⚠️ Player не найден!");
            }
        }
        else
        {
            Debug.LogError($"[SceneLoader] ❌ Сцена {firstLevelName} невалидна!");
        }
    }
}