using System.Collections;
using UnityEngine;

public class PlayerDash
{
    private PlayerController controller;
    private float originalGravityScale;

    public PlayerDash(PlayerController controller)
    {
        this.controller = controller;
        originalGravityScale = controller.rb.gravityScale;
    }

    public void HandleDash()
    {


        if (controller.canDash &&
            !controller.isDashing &&
            (controller.collisionWithGround || controller.dashCount < PlayerManager.Instance.maxDashCount))
        {
            controller.StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        
        controller.lockJump = true;
        controller.dashCount++;
        controller.isDashing = true;

        controller.lockHorizonMove = true;
        controller.rb.gravityScale = 0f;

        float direction = 0f;

        if (controller.collisionWithLeftWall && controller.isWallSliding)
        {
            direction = 1f;
            controller.transform.localScale = new Vector3(Mathf.Abs(controller.transform.localScale.x), controller.transform.localScale.y, controller.transform.localScale.z);
        }
        else if (controller.collisionWithRightWall && controller.isWallSliding)
        {
            direction = -1f;
            controller.transform.localScale = new Vector3(-Mathf.Abs(controller.transform.localScale.x), controller.transform.localScale.y, controller.transform.localScale.z);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            direction = -1f;
            controller.transform.localScale = new Vector3(-Mathf.Abs(controller.transform.localScale.x), controller.transform.localScale.y, controller.transform.localScale.z);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            direction = 1f;
            controller.transform.localScale = new Vector3(Mathf.Abs(controller.transform.localScale.x), controller.transform.localScale.y, controller.transform.localScale.z);
        }
        else
        {
            direction = controller.transform.localScale.x > 0 ? 1f : -1f;
        }

        controller.rb.velocity = new Vector2(direction * PlayerManager.Instance.dashSpeed, 0f);
        controller.allowChangeHorizonSpeed = false;

        yield return new WaitForSeconds(PlayerManager.Instance.dashDuration);

        controller.rb.gravityScale = originalGravityScale;
        controller.lockJump = false;
        controller.allowChangeHorizonSpeed = true;
        controller.lockHorizonMove = false;
        yield return new WaitForSeconds(PlayerManager.Instance.dashCooldown);

        controller.isDashing = false;

    }
}
