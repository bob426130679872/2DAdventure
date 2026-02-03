using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerColliderController : MonoBehaviour
{
    public SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = transform.parent.gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        var player = transform.parent.GetComponent<PlayerController>();
        if (player == null) return;
        if (other.tag == "Tile")
        {
            switch (gameObject.name)
            {
                case "groundCheck":
                    player.collisionWithGround = true;
                    break;
                case "headCheck":
                    player.collisionWithCeil = true;
                    break;
                case "wallCheckFront":
                if(transform.parent.localScale.x<0)
                    player.collisionWithLeftWall = true;
                else
                    player.collisionWithRightWall = true;                   
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
        if (other.tag == "Tile")
        {
            switch (gameObject.name)
            {
                case "groundCheck":
                    player.collisionWithGround = false;
                    break;
                case "headCheck":
                    player.collisionWithCeil = false;
                    break;
                case "wallCheckFront":
                    if(player.collisionWithRightWall)
                    player.collisionWithRightWall = false;
                else
                    player.collisionWithLeftWall = false;                   
                    break;
                default:
                    break;
            }
        }

    }
}