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
    private State prevState = State.Patrol;

    // ── 狀態機 ───────────────────────────────────────────

    void Update()
    {
        if (isDead) return;

        if (isPlayerInAtkRange)
            state = State.Attack;
        else if (seePlayer)
            state = State.Chase;
        else
            state = State.Patrol;

        if (state != prevState)
        {
            OnStateEnter(state);
            prevState = state;
        }

        switch (state)
        {
            case State.Patrol: HandlePatrol(); break;
            case State.Chase: HandleChase(); break;
            case State.Attack: HandleAttack(); break;
        }
    }

    void OnStateEnter(State newState)
    {
        switch (newState)
        {
            case State.Chase:
                currentMoveSpeed = 4f;
                break;
            case State.Patrol:
                currentMoveSpeed = data.baseMoveSpeed;
                patrolDirection = isFacingRight ? 1f : -1f;
                break;
            case State.Attack:
                break;
        }
    }

    // ── 移動邏輯 ──────────────────────────────────────────

    void HandlePatrol()
    {
        bool blocked = !isGroundAhead || isWallAhead;
        if (blocked && !wasBlocked)
        {
            patrolDirection *= -1f;
            Flip();
        }
        wasBlocked = blocked;

        rb.velocity = new Vector2(patrolDirection * currentMoveSpeed, rb.velocity.y);
    }

    void HandleChase()
    {
        var player = PlayerManager.Instance?.player;
        if (player == null) return;
        float dirX = Mathf.Sign(player.transform.position.x - transform.position.x);
        if (dirX > 0 && !isFacingRight || dirX < 0 && isFacingRight)
            Flip();
        rb.velocity = new Vector2(dirX * currentMoveSpeed, rb.velocity.y);
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

        var pattern = GetRandomAttack();
        if (pattern != null)
        {
            if (pattern.mode == AttackMode.Spawn && pattern.attackColliderPrefab != null)
            {
                Instantiate(pattern.attackColliderPrefab, transform.position, Quaternion.identity);
            }
            else if (pattern.mode == AttackMode.ChildTrigger)
            {
                var zone = FindAttackZone(pattern.attackName);
                if (zone != null)
                {
                    zone.Activate();
                    yield return new WaitForSeconds(pattern.activeDuration);
                    zone.Deactivate();
                }
            }
        }

        yield return new WaitForSeconds(pattern?.cooldown ?? 1f);
        attackOnCooldown = false;
    }

    // ── 受擊 / 死亡 ──────────────────────────────────────

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        if (other.CompareTag("Player"))
        {
            OnContactWithPlayer();
            return;
        }
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
