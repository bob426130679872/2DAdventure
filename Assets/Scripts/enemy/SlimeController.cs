using System.Collections;
using UnityEngine;

/// <summary>
/// 史萊姆：巡邏 → 追擊 → 進入攻擊範圍時停下發動攻擊，生成攻擊碰撞體
/// </summary>
public class SlimeController : EnemyController
{
    private enum State { Patrol, Chase, Attack }
    private State state = State.Patrol;

    private float patrolDirection = 1f;
    private bool wasBlocked = false;
    private bool attackOnCooldown = false;

    // ── 狀態機 ───────────────────────────────────────────

    void Update()
    {
        if (isDead) return;

        if (isPlayerInAtkRange)
            state = State.Attack;
        else if (isPlayerDetected)
            state = State.Chase;
        else
            state = State.Patrol;

        switch (state)
        {
            case State.Patrol: HandlePatrol(); break;
            case State.Chase:  HandleChase();  break;
            case State.Attack: HandleAttack(); break;
        }
    }

    // ── 移動邏輯 ──────────────────────────────────────────

    void HandlePatrol()
    {
        bool blocked = !isGroundAhead || isWallAhead;
        if (blocked && !wasBlocked)
            patrolDirection *= -1f;
        wasBlocked = blocked;

        rb.velocity = new Vector2(patrolDirection * currentMoveSpeed * 0.5f, rb.velocity.y);
        Flip(patrolDirection);
    }

    void HandleChase()
    {
        var player = PlayerManager.Instance?.player;
        if (player == null) return;
        float dirX = Mathf.Sign(player.transform.position.x - transform.position.x);
        rb.velocity = new Vector2(dirX * currentMoveSpeed, rb.velocity.y);
        Flip(dirX);
    }

    // ── 攻擊邏輯 ──────────────────────────────────────────

    void HandleAttack()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        if (!attackOnCooldown)
            StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        attackOnCooldown = true;

        var pattern = data.attackPatterns.Count > 0 ? data.attackPatterns[0] : null;
        if (pattern?.attackColliderPrefab != null)
            Instantiate(pattern.attackColliderPrefab, transform.position, Quaternion.identity, transform);

        yield return new WaitForSeconds(pattern?.cooldown ?? 1f);
        attackOnCooldown = false;
    }

    // ── 受擊 / 死亡 ──────────────────────────────────────

    void OnTriggerEnter2D(Collider2D other)
    {
        var bullet = other.GetComponent<MPbullet>();
        if (bullet != null)
            StartCoroutine(HandleHit(bullet.damage));
    }

    IEnumerator HandleHit(int damage)
    {
        TakeDamage(damage);

        if (isDead)
        {
            yield return StartCoroutine(DeathRoutine());
            Destroy(gameObject);
        }
        else
        {
            yield return StartCoroutine(HitRoutine());
        }
    }

    IEnumerator HitRoutine()
    {
        // TODO：受擊動畫
        yield break;
    }

    IEnumerator DeathRoutine()
    {
        // TODO：死亡動畫
        yield break;
    }
}
