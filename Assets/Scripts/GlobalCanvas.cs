using UnityEngine;

public class GlobalCanvas : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}