[System.Serializable]
public class PlayerData
{
    public string playerName;
    public float playerHealth;
    public string playerPosition;

    
    public PlayerData()
    {
        playerName = "defaultPlayer";
        playerHealth = 3f;
        playerPosition = "InitialVillage";
    }

    
    public PlayerData(string name, float health, string pos)
    {
        playerName = name;
        playerHealth = health;
        playerPosition = pos;
    }
}
