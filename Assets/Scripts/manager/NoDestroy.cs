using UnityEngine;
using System.Collections.Generic;

public class NoDestroy : MonoBehaviour
{
    public static NoDestroy Instance;

    private void Awake()
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
}