using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting
{
    private PlayerController controller;
    private PlayerManager pm;
    public PlayerShooting(PlayerController controller)
    {
        this.controller = controller;
        pm = PlayerManager.Instance;
    }
    public void handleShooting()
    {
        if (controller.canShooting &&
            !controller.isShooting)
        {
            controller.StartCoroutine(PerformShooting());
        }

    }
    private IEnumerator PerformShooting()
    {

        controller.isShooting = true;

        GameObject bullet = GameObject.Instantiate(pm.MPbulletPrefab, pm.firePoint.transform.position,Quaternion.identity);
        float direction = pm.player.transform.localScale.x > 0 ? 1f : -1f;
        bullet.transform.localScale = new Vector3(direction, 1, 1);
        bullet.GetComponent<MPbullet>().direction = direction;

        yield return new WaitForSeconds(PlayerManager.Instance.shootingDuration);
        controller.isShooting = false;


    }
}
