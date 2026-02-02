using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExplode
{
    private PlayerController controller;

    public PlayerExplode(PlayerController controller)
    {
        this.controller = controller;
    }
    public void handleExplode()
    {
        if (controller.canExplode &&
            !controller.isExplode &&
            controller.collisionWithGround)
        {
            controller.StartCoroutine(PerformExplode());
        }
        
    }
    private IEnumerator PerformExplode()
    {
        
        controller.lockJump = true;
        controller.isExplode = true;
        controller.lockControl = true;
        PlayerManager.Instance.player.transform.GetChild(4).gameObject.SetActive(true);
        
        controller.rb.velocity = new Vector2(0f, 0f);
        controller.allowChangeHorizonSpeed = false;

        yield return new WaitForSeconds(PlayerManager.Instance.explodeDuration);
        controller.lockJump = false;
        controller.allowChangeHorizonSpeed = true;
        controller.lockControl = false;
        controller.isExplode = false;
        PlayerManager.Instance.player.transform.GetChild(4).gameObject.SetActive(false);

    }
}
