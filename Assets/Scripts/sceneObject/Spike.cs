using UnityEngine;
public class Spike : MonoBehaviour
{
    private void Start() 
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")&& !PlayerManager.Instance.isDying)
        {
            GameEvents.Player.TriggerPlayerDie(other.gameObject);
        }
    }
    
}
