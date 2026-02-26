using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFly : MonoBehaviour
{
    private PlayerController controller;
    private Rigidbody2D rb;
    private float originalGravity;

    public PlayerFly(PlayerController controller)
    {
        this.controller = controller;
        this.rb = controller.GetComponent<Rigidbody2D>();
    }
    public void handleFly()
    {
        if (controller.canFly &&
            !controller.isExplode &&
            !controller.isDashing &&
            controller.collisionWithGround)
        {
           if (!controller.isFlying)
            {
                StartFlying();
            }
            else
            {
                StopFlying();
            }
            if (controller.isFlying)
        {
            Flying();
        }
        }

    }
    private void StartFlying()
    {
        controller.isFlying = true;
        originalGravity = rb.gravityScale;
        rb.gravityScale = 0; // 關閉重力
        rb.velocity = Vector2.zero; // 啟動瞬間停頓，手感較準
    }

    private void Flying()
    {
        // 抓取 8 方向輸入
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 moveInput = new Vector2(h, v).normalized;

        // 執行飛行移動
        rb.velocity = moveInput * PlayerManager.Instance.flySpeed;

        // 處理翻轉
        if (h != 0)
        {
            PlayerManager.Instance.playerFlip = h < 0;
            controller.transform.localScale = new Vector3(
                (h < 0 ? -1 : 1) * Mathf.Abs(controller.transform.localScale.x), 
                controller.transform.localScale.y, 
                controller.transform.localScale.z
            );
        }

        // 消耗能量
        PlayerManager.Instance.UseStamina(Time.deltaTime * 10f);

        // 檢查自動退出條件：能量用完
        if (PlayerManager.Instance.currentStamina <= 0)
        {
            StopFlying();
        }
        
        // 檢查主動退出條件：按下跳躍 (K)
        if (Input.GetKeyDown(KeyCode.K))
        {
            StopFlying();
        }
    }

    private void StopFlying()
    {
        controller.isFlying = false;
        rb.gravityScale = originalGravity; // 還原重力
    }
}
