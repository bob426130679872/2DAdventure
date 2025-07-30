using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SaveSlotButton : MonoBehaviour
{
    public string slotName;

    public void OnClick()
    {
        
        SaveManager.Instance.LoadAll(slotName);
        GameManager.Instance.spawnPortalName = GameManager.Instance.saveScene+"Spawn0";
        SceneManager.LoadScene(GameManager.Instance.saveScene);
        
    }
}
