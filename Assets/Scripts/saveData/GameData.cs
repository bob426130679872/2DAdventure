using System;
using System.Collections.Generic;
[System.Serializable]
public class GameData
{
    public int volume; 

    public GameData()
    {
        volume = 10;
    }

    // 帶參數建構子 (把 StorySaveData 完整傳入)
    public GameData(int volume)
    {
        this.volume = volume;
    }
}
