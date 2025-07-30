using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        var player = transform.parent.GetComponent<PlayerController>();
        if (player == null) return;
        if (other.tag == "tile")
        {
            switch (gameObject.name)
            {
                case "groundCheck":
                    player.collisionWithGround = true;
                    break;
                case "headCheck":
                    player.collisionWithCeil = true;
                    break;
                case "wallCheckLeft":
                case "wallCheckRight":
                    player.collisionWithWall = true;
                    break;
                default:
                    break;
            }
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        var player = transform.parent.GetComponent<PlayerController>();
        if (player == null) return;
        if (other.tag == "tile")
        {
            switch (gameObject.name)
            {
                case "groundCheck":
                    player.collisionWithGround = false;
                    break;
                case "headCheck":
                    player.collisionWithCeil = false;
                    break;
                case "wallCheckLeft":
                case "wallCheckRight":
                    player.collisionWithWall = false;
                    break;
                default:
                    break;
            }
        }

    }
}
