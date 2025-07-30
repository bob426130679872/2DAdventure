
[System.Serializable]
public class GameData
{
    public string gameProcess;

    // 無參數建構子（JsonUtility 需要）
    public GameData()
    {
        gameProcess = "init";
    }

    // 帶參數建構子
    public GameData(string gameProcess)
    {
        this.gameProcess = gameProcess;
    }
}
