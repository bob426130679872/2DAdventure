using System;

[System.Serializable]
public class MetaData
{
    public string saveTime;

    // 礚把计篶JsonUtility 惠璶
    public MetaData()
    {
        saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    // 盿把计篶
    public MetaData(string saveTime)
    {
        this.saveTime = saveTime;
    }
}