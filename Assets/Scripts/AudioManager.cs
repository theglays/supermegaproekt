using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public string name;           // Имя для вызова в коде (напр. "Footstep")
        public AudioClip clip;        // Файл .wav или .ogg
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public bool loop = false;     // Зацикливание (для фоновых шумов/музыки)
        public bool is3D = false;     // false = 2D (клики UI, меню), true = 3D (шаги, двери)
        [HideInInspector] public AudioSource source;
    }

    [Header("Пул звуков (SFX)")]
    [Tooltip("Количество одновременных звуков. 10 хватает для point & click")]
    public int sfxPoolSize = 10;
    private List<AudioSource> sfxSources;

    [Header("Звуки окружения и действий")]
    public Sound[] sounds;

    [Header("Музыка")]
    public Sound[] musicTracks;
    private AudioSource musicSourceA;
    private AudioSource musicSourceB;
    private AudioSource activeMusicSource;

    private float sfxVolume = 1f;
    private float musicVolume = 1f;

    void Awake()
    {
        // 1. Синглтон
      if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else
    {
        Destroy(gameObject);
    }

        // 2. Загрузка сохранённой громкости
        sfxVolume = PlayerPrefs.GetFloat("SFX_Volume", 1f);
        musicVolume = PlayerPrefs.GetFloat("Music_Volume", 1f);

        // 3. Создаём пул AudioSource для SFX
        sfxSources = new List<AudioSource>();
        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject go = new GameObject($"SFX_Pool_{i}");
            go.transform.SetParent(transform);
            AudioSource src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.spatialBlend = 0f; // По умолчанию 2D, меняется при вызове
            sfxSources.Add(src);
        }

        // 4. Создаём 2 источника для кроссфейда музыки
        GameObject mA = new GameObject("Music_A"); mA.transform.SetParent(transform);
        musicSourceA = mA.AddComponent<AudioSource>(); musicSourceA.playOnAwake = false; musicSourceA.spatialBlend = 0f;
        GameObject mB = new GameObject("Music_B"); mB.transform.SetParent(transform);
        musicSourceB = mB.AddComponent<AudioSource>(); musicSourceB.playOnAwake = false; musicSourceB.spatialBlend = 0f;
        activeMusicSource = musicSourceA;

        // 5. Применяем громкость
        ApplyVolumes();
    }

    /// <summary>
    /// Воспроизвести звук. position = null → 2D звук. position = точка → 3D звук с затуханием.
    /// </summary>
    public void PlaySFX(string name, Vector3? position = null)
    {
        Sound s = Array.Find(sounds, x => x.name == name);
        if (s == null || s.clip == null) { Debug.LogWarning($"[Audio] Звук '{name}' не найден!"); return; }

        AudioSource source = null;
        foreach (var src in sfxSources) { if (!src.isPlaying) { source = src; break; } }
        if (source == null) source = sfxSources[0]; // Если все заняты, перезаписываем первый

        source.clip = s.clip;
        source.volume = s.volume * sfxVolume;
        source.pitch = s.pitch;
        source.loop = s.loop;
        source.spatialBlend = position.HasValue ? 1f : (s.is3D ? 1f : 0f);

        if (position.HasValue)
        {
            source.transform.position = position.Value;
            source.transform.SetParent(null); // Отвязываем, чтобы звук остался в точке мира
        }
        else
        {
            source.transform.position = transform.position;
            source.transform.SetParent(transform);
        }
        source.Play();
    }

    /// <summary>
    /// Включить музыку с плавным переходом (кроссфейд)
    /// </summary>
    public void PlayMusic(string trackName, bool loop = true, float fadeTime = 1f)
    {
        Sound track = Array.Find(musicTracks, x => x.name == trackName);
        if (track == null || track.clip == null) { Debug.LogWarning($"[Audio] Трек '{trackName}' не найден!"); return; }

        AudioSource nextSource = (activeMusicSource == musicSourceA) ? musicSourceB : musicSourceA;
        nextSource.clip = track.clip;
        nextSource.volume = 0f;
        nextSource.loop = loop;
        nextSource.Play();

        StartCoroutine(FadeMusic(activeMusicSource, nextSource, fadeTime, track.volume * musicVolume));
        activeMusicSource = nextSource;
    }

    private IEnumerator FadeMusic(AudioSource fadeOut, AudioSource fadeIn, float duration, float targetVol)
    {
        float elapsed = 0f;
        float startOut = fadeOut.volume;
        float startIn = fadeIn.volume;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeOut.volume = Mathf.Lerp(startOut, 0f, elapsed / duration);
            fadeIn.volume = Mathf.Lerp(startIn, targetVol, elapsed / duration);
            yield return null;
        }
        fadeOut.Stop();
    }

    public void SetSFXVolume(float vol)
    {
        sfxVolume = Mathf.Clamp01(vol);
        PlayerPrefs.SetFloat("SFX_Volume", sfxVolume);
        ApplyVolumes();
    }

    public void SetMusicVolume(float vol)
    {
        musicVolume = Mathf.Clamp01(vol);
        PlayerPrefs.SetFloat("Music_Volume", musicVolume);
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        // Обновляем громкость играющих SFX
        foreach (var src in sfxSources)
        {
            if (src.isPlaying && src.clip != null)
            {
                Sound s = Array.Find(sounds, x => x.clip == src.clip);
                if (s != null) src.volume = s.volume * sfxVolume;
            }
        }
        // Обновляем текущую музыку
        if (activeMusicSource != null && activeMusicSource.isPlaying)
        {
            Sound t = Array.Find(musicTracks, x => x.clip == activeMusicSource.clip);
            if (t != null) activeMusicSource.volume = t.volume * musicVolume;
        }
    }

    // 👇 👇 👇  НОВЫЙ МЕТОД (ДОБАВЬ ЕГО В САМЫЙ НИЗ, ПЕРЕД ПОСЛЕДНЕЙ } ) 👇 👇 👇
    
    /// <summary>
    /// Возвращает имя клипа, который сейчас играет в активном источнике музыки.
    /// Нужно для того, чтобы LevelMusicController не перезапускал тот же трек.
    /// </summary>
    public string GetActiveMusicClipName()
    {
        if (activeMusicSource != null && activeMusicSource.clip != null)
            return activeMusicSource.clip.name;
        return null;
    }
    
    // 👆 👆 👆  КОНЕЦ НОВОГО МЕТОДА  👆 👆 👆
}