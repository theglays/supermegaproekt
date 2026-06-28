using UnityEngine;

public class LevelMusicController : MonoBehaviour
{
    [Header("Настройки музыки уровня")]
    [Tooltip("Имя трека, как оно указано в массиве musicTracks у AudioManager")]
    public string trackName = "DefaultTheme";
    
    [Tooltip("Зацикливать трек?")]
    public bool loop = true;
    
    [Tooltip("Время плавного перехода (кроссфейда) в секундах")]
    public float fadeTime = 2f;

    [Tooltip("Если галочка стоит, музыка НЕ будет играть повторно, если этот трек уже звучит")]
    public bool skipIfAlreadyPlaying = true;

    void Start()
    {
    //         DontDestroyOnLoad(gameObject);
        // Ждем 1 кадр, чтобы AudioManager точно успел инициализироваться
        StartCoroutine(PlayLevelMusic());
    }

    System.Collections.IEnumerator PlayLevelMusic()
    {
        yield return null;

        if (AudioManager.Instance != null)
        {
            // Проверка: если трек уже играет, не перезапускаем его (чтобы не было скачков)
            if (skipIfAlreadyPlaying)
            {
                var activeClip = AudioManager.Instance.GetActiveMusicClipName();
                if (activeClip == trackName)
                {
                    Debug.Log($"[Music] Трек '{trackName}' уже играет, пропускаем запуск.");
                    yield break;
                }
            }
            
            Debug.Log($"[Music] Запуск трека уровня: {trackName}");
            AudioManager.Instance.PlayMusic(trackName, loop, fadeTime);
        }
        else
        {
            Debug.LogWarning("[Music] AudioManager не найден на сцене!");
        }
    }
}