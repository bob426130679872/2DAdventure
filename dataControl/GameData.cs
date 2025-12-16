using System;
using System.Collections.Generic;
[System.Serializable]
public class GameData
{
    public string gameProcess;
    public List<BrokenSceneObject> brokenSceneObjectList = new();

    // 無參數建構子（JsonUtility 需要）
    public GameData()
    {
        gameProcess = "init";
        brokenSceneObjectList = new();
    }

    // 帶參數建構子
    public GameData(string gameProcess, List<BrokenSceneObject> brokenSceneObjectList )
    {
        this.gameProcess = gameProcess;
        this.brokenSceneObjectList = brokenSceneObjectList;
    }
   
}
 [Serializable]
public class BrokenSceneObject
{
    // 鍵 (Key)
    public string sceneName;
    // 值 (Value)
    public List<string> ids;

    public BrokenSceneObject(string sceneName, List<string> ids)
    {
        this.sceneName = sceneName;
        this.ids = ids;
    }
}