using System;

[System.Serializable]
public class MetaData
{
    public string saveTime;

    // �L�Ѽƫغc�l�]JsonUtility �ݭn�^
    public MetaData()
    {
        saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    // �a�Ѽƫغc�l
    public MetaData(string saveTime)
    {
        this.saveTime = saveTime;
    }
}