using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenObjectController : MonoBehaviour
{
    public string requiredState = "god_look";
    private Renderer myRenderer;
    private Collider2D myCollider;
    private bool lastState = false; // 用來紀錄上一次的狀態

    void Awake()
    {
        myRenderer = GetComponent<Renderer>();
        myCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        bool shouldBeVisible = StoryManager.Instance.GetNoSaveFlags(requiredState) >= 1;

        // 只有當狀態「發生變化」時才執行開關，省下大量效能
        if (shouldBeVisible != lastState)
        {
            if (myRenderer != null) myRenderer.enabled = shouldBeVisible;
            if (myCollider != null) myCollider.enabled = shouldBeVisible;
            
            lastState = shouldBeVisible;
            Debug.Log($"{gameObject.name} 視覺狀態切換為: {shouldBeVisible}");
        }
    }
}
