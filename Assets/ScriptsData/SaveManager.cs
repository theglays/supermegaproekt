using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [System.Serializable]
    private class SaveData
    {
        public List<JournalEntry> unlockedEntries = new List<JournalEntry>();
        public Vector3 playerPosition;
        public string currentScene;
        // Сюда можно добавить: инвентарь, квесты, настройки и т.д.
    }

    private SaveData currentSave;
    private string savePath;

    void Awake()
    {
        // Синглтон + не уничтожать при смене сцены
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Application.persistentDataPath + "/savegame.json";
            LoadGame(); // Загружаем сохранение при старте
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 🔥 Добавить запись в дневник (если её ещё нет) 🔥
    public bool UnlockEntry(JournalEntry newEntry)
    {
        if (currentSave == null) LoadGame();

        // Проверяем, нет ли уже такой записи
        foreach (var entry in currentSave.unlockedEntries)
        {
            if (entry.entryId == newEntry.entryId)
                return false; // Уже есть, не добавляем
        }

        currentSave.unlockedEntries.Add(newEntry);
        SaveGame(); // Автосохранение
        Debug.Log($"[Save] Запись '{newEntry.title}' добавлена в дневник");
        return true;
    }

    // 🔥 Получить все открытые записи 🔥
    public List<JournalEntry> GetUnlockedEntries()
    {
        if (currentSave == null) LoadGame();
        return currentSave.unlockedEntries;
    }

    // 🔥 Сохранить игру 🔥
    public void SaveGame()
    {
        string json = JsonUtility.ToJson(currentSave, true);
        File.WriteAllText(savePath, json);
        Debug.Log("[Save] Игра сохранена");
    }

    // 🔥 Загрузить игру 🔥
    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentSave = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("[Save] Игра загружена");
        }
        else
        {
            // Новая игра — создаём пустое сохранение
            currentSave = new SaveData();
            Debug.Log("[Save] Создано новое сохранение");
        }
    }

    // 🔥 Удалить сохранение (для кнопки "Новая игра") 🔥
    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            currentSave = new SaveData();
            Debug.Log("[Save] Сохранение удалено");
        }
    }

    // 🔥 Обновить позицию игрока (вызывать при выходе из уровня) 🔥
    public void UpdatePlayerPosition(Vector3 pos, string sceneName)
    {
        if (currentSave == null) LoadGame();
        currentSave.playerPosition = pos;
        currentSave.currentScene = sceneName;
        SaveGame();
    }

    // 🔥 Получить позицию для продолжения 🔥
    public Vector3 GetSavedPosition() => currentSave?.playerPosition ?? Vector3.zero;
    public string GetSavedScene() => currentSave?.currentScene ?? "";
}