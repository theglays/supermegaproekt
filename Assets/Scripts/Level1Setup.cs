using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1Setup : MonoBehaviour
{
    void Start()
    {
        Debug.Log("[Level1Setup] Регистрация объектов...");
        
        // Регистрируем объекты
        GameObject player = GameObject.Find("Player");
        if (player != null && GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPlayer(player);
        }

        GameObject camera = GameObject.Find("Main Camera");
        if (camera != null && GameManager.Instance != null)
        {
            GameManager.Instance.RegisterCamera(camera);
        }

        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null && GameManager.Instance != null)
        {
            GameManager.Instance.RegisterCanvas(canvas);
            GameManager.Instance.ShowGameUI();
        }

        // 🔥 ВЫГРУЖАЕМ СЦЕНУ МЕНЮ
        StartCoroutine(UnloadMenuScene());
    }

    System.Collections.IEnumerator UnloadMenuScene()
    {
        yield return new WaitForSeconds(0.5f); // Ждём полсекунды
        
        Scene menuScene = SceneManager.GetSceneByName("Menu");
        if (menuScene.IsValid())
        {
            SceneManager.UnloadSceneAsync(menuScene);
            Debug.Log("[Level1Setup] Меню выгружено");
        }
    }
}