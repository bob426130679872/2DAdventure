using UnityEngine;

/// <summary>
/// 史萊姆：巡邏 → 發現玩家追擊 → 進入攻擊範圍時攻擊
/// 繼承 EnemyController，只需實作移動 / 攻擊邏輯
/// </summary>
public class SlimeController : EnemyController
{
    private enum State { Patrol, Chase, Attack }
    private State state = State.Patrol;

    [Header("巡邏設定")]
    public float patrolDuration = 2f;

    [Header("攻擊設定")]
    public GameObject attackZoneObject;   // 拖入攻擊範圍子物件
    public float attackActiveDuration = 0.2f;
    public float attackRange = 1.5f;

    private float patrolTimer = 0f;
    private float patrolDirection = 1f;
    private float attackTimer = 0f;

    void Update()
    {
        if (isDead) return;

        attackTimer -= Time.deltaTime;

        Transform playerTransform = PlayerManager.Instance?.player?.transform;
        float dist = playerTransform != null
            ? Vector2.Distance(transform.position, playerTransform.position)
            : float.MaxValue;

        if (dist <= attackRange)
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
        patrolTimer += Time.deltaTime;
        if (patrolTimer >= patrolDuration)
        {
            patrolTimer = 0f;
            patrolDirection *= -1f;
        }
        rb.velocity = new Vector2(patrolDirection * currentMoveSpeed * 0.5f, rb.velocity.y);
        Flip(patrolDirection);
    }

    void HandleChase()
    {
        Transform playerTransform = PlayerManager.Instance?.player?.transform;
        if (playerTransform == null) return;
        Vector2 dir = ((Vector2)(playerTransform.position - transform.position)).normalized;
        rb.velocity = new Vector2(dir.x * currentMoveSpeed, rb.velocity.y);
        Flip(dir.x);
    }

    // ── 攻擊邏輯 ──────────────────────────────────────────

    void HandleAttack()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        if (attackTimer > 0f) return;

        var pattern = data.attackPatterns.Count > 0 ? data.attackPatterns[0] : null;
        attackTimer = pattern?.cooldown ?? 1f;
        if (attackZoneObject != null)
            StartCoroutine(ActivateAttackZone(attackZoneObject, attackActiveDuration));
    }

    protected override void OnHit()
    {
        // TODO：受擊特效 / 閃爍
    }

    protected override void OnDie()
    {
        // TODO：死亡特效 / 掉落物品
    }
}
