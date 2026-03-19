using Unity.VisualScripting;
using UnityEngine;

public class MPbullet : MonoBehaviour
{
    [Header("數值設定")]
    public float speed = 15f;
    public float lifeTime = 3f; // 3秒後沒撞到東西就自動消失
    public int damage = 10;
    public float direction;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
    void Start()
    {
        
        rb.velocity = new Vector2(direction * speed,0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 3. 碰撞邏輯判斷
        if (other.CompareTag("Enemy"))
        {
            // 這裡呼叫敵人的受傷邏輯
            Debug.Log("擊中敵人！");
            // other.GetComponent<Enemy>().TakeDamage(damage);
            HandleImpact();
        }

    }

    private void HandleImpact()
    {
        // 4. 擊中後的處理：生成爆炸效果、音效，最後銷毀
        // Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}