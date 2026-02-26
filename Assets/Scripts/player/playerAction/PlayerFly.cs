using UnityEngine;

public class PlayerFly
{
    private PlayerController controller;
    private Rigidbody2D rb;
    private float originalGravity;
    private float originalDrag;

    private const float staminaCostPerSecond = 0f; // 測試階段為 0，正式上線後改為實際數值

    public PlayerFly(PlayerController controller)
    {
        this.controller = controller;
        this.rb = controller.GetComponent<Rigidbody2D>();
    }

    // 按 P 時呼叫：切換飛行開關
    public void handleFly()
    {
        if (!controller.canFly || controller.isExplode || controller.isDashing) return;

        if (!controller.isFlying)
            StartFlying();
        else
            StopFlying();
    }

    // 每幀在 PlayerController.Update 呼叫：處理飛行移動與退出條件
    public void UpdateFly()
    {
        Debug.Log("飛");
        PlayerManager pm = PlayerManager.Instance;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 moveInput = new Vector2(h, v).normalized;

        rb.AddForce(moveInput * pm.flyAcceleration);

        // 速度上限
        if (rb.velocity.magnitude > pm.flySpeed)
            rb.velocity = rb.velocity.normalized * pm.flySpeed;

        if (h != 0)
        {
            pm.playerFlip = h < 0;
            controller.transform.localScale = new Vector3(
                (h < 0 ? -1 : 1) * Mathf.Abs(controller.transform.localScale.x),
                controller.transform.localScale.y,
                controller.transform.localScale.z
            );
        }

        if (staminaCostPerSecond > 0)
            pm.UseStamina(staminaCostPerSecond * Time.deltaTime);
        Debug.Log($"氣剩餘：{pm.currentStamina}/{pm.maxStamina}");

        if (pm.currentStamina <= 0)
            StopFlying();

        if (Input.GetKeyDown(KeyCode.K))
            StopFlying();
    }

    private void StartFlying()
    {
        Debug.Log("起飛");
        controller.isFlying = true;
        originalGravity = rb.gravityScale;
        originalDrag = rb.drag;
        rb.gravityScale = 0;
        rb.drag = PlayerManager.Instance.flyDrag;
        rb.velocity = Vector2.zero;
        StoryManager.Instance.SetNoSaveFlags("isFlying", 1);
    }

    public void StopFlying()
    {
        controller.isFlying = false;
        rb.gravityScale = originalGravity;
        rb.drag = originalDrag;
        StoryManager.Instance.SetNoSaveFlags("isFlying", 0);
    }
}
