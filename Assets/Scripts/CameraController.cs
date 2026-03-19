// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class CameraController : MonoBehaviour
// {
//     public Rigidbody2D playerRb;         // 改為 Rigidbody2D，避免 Transform/Physics 同步問題
//     public float smoothTime = 0.2f;      
//     public float verticalDeadZone = 1f;  
//     public BoxCollider2D cameraBound;    

//     private Vector3 velocity = Vector3.zero;
//     private Vector2 minBounds;
//     private Vector2 maxBounds;
//     private float halfHeight;
//     private float halfWidth;

//     void Start()
//     {
//         Camera cam = Camera.main;
//         halfHeight = cam.orthographicSize;
//         halfWidth = halfHeight * cam.aspect;

//         Bounds bounds = cameraBound.bounds;
//         minBounds = bounds.min;
//         maxBounds = bounds.max;
//     }

//     void FixedUpdate()
//     {
//         if (playerRb == null) return;

//         Vector3 currentPos = transform.position;
//         Vector3 playerPos = playerRb.position; //  使用 Rigidbody2D 的 position
//         Vector3 targetPos = currentPos;

//         // X 軸：始終跟隨角色
//         targetPos.x = playerPos.x;

//         // Y 軸：超出死區才移動
//         float yOffset = playerPos.y - currentPos.y;
//         if (Mathf.Abs(yOffset) > verticalDeadZone)
//         {
//             targetPos.y = playerPos.y;
//         }

//         // Clamp 限制範圍
//         targetPos.x = Mathf.Clamp(targetPos.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
//         targetPos.y = Mathf.Clamp(targetPos.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);

//         // 平滑跟隨
//         transform.position = Vector3.SmoothDamp(currentPos, targetPos, ref velocity, smoothTime);
//     }
// }
