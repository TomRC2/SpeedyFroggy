using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkullBoss : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    [SerializeField] private float moveSpeed = 2f;

    [Header("Boss Stats")]
    [SerializeField] private int maxLives = 3;
    [SerializeField] private float postHitDelay = 2f;
    [Header("Projectiles")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform orbitCenter;
    [SerializeField] private float orbitRadius = 1.5f;
    [SerializeField] private float orbitSpeed = 90f;

    [SerializeField] private Animator animator;
    private bool facingRight = true;
    private int currentLives;
    private int tripsCount = 0;
    private bool goingRight = true;
    private bool isVulnerable = false;
    private bool isActive = false;
    private List<BossProjectile> orbitingProjectiles = new();
    [SerializeField] private GameEvents gameEvents;

    private void Awake()
    {
        currentLives = maxLives;
        SpawnProjectiles(3);
    }

    private void Update()
    {
        if (!isActive) return;

        if (!isVulnerable)
        {
            Patrol();
            RotateProjectiles();
        }
    }

    private void Patrol()
    {
        Transform target = goingRight ? rightPoint : leftPoint;

        if (goingRight && !facingRight)
            Flip();
        else if (!goingRight && facingRight)
            Flip();

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            goingRight = !goingRight;
            tripsCount++;

            if (tripsCount >= 3)
            {
                EnterVulnerableState();
            }
        }
    }

    private void RotateProjectiles()
    {
        orbitCenter.Rotate(Vector3.forward * orbitSpeed * Time.deltaTime);
    }

    private void EnterVulnerableState()
    {
        isVulnerable = true;
        tripsCount = 0;
        animator.SetTrigger("Transform");
        animator.SetBool("IsVulnerable", true);

        if (orbitingProjectiles.Count > 0)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector2 playerPos = player.transform.position;

                var proyectilCazador = orbitingProjectiles
                    .Where(p => p != null)
                    .OrderBy(p => Vector2.Distance(p.transform.position, playerPos))
                    .FirstOrDefault();

                if (proyectilCazador != null)
                {
                    proyectilCazador.LaunchTowards(playerPos);
                }

                orbitingProjectiles
                    .Where(p => p != null && p != proyectilCazador)
                    .ToList()
                    .ForEach(p => p.LaunchOutward());
            }
        }
        orbitingProjectiles.Clear();
    }

    public void TakeDamage(int amount)
    {
        if (!isVulnerable) return;

        currentLives -= amount;

        animator.SetBool("IsVulnerable", false);
        animator.SetTrigger("Hit");

        if (currentLives <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(PostHitRoutine());
    }

    private void SpawnProjectiles(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = i * (360f / count);
            Vector3 pos = orbitCenter.position + Quaternion.Euler(0, 0, angle) * Vector3.right * orbitRadius;

            GameObject obj = Instantiate(projectilePrefab, pos, Quaternion.identity, orbitCenter);
            BossProjectile proj = obj.GetComponent<BossProjectile>();
            proj.SetOrbitCenter(orbitCenter);
            orbitingProjectiles.Add(proj);
        }
    }
    private System.Collections.IEnumerator PostHitRoutine()
    {
        isVulnerable = false;

        yield return new WaitForSeconds(postHitDelay);

        moveSpeed += 0.5f;
        orbitSpeed += 30f;

        int newProjectileCount =
            currentLives == 2 ? 6 : 9;

        SpawnProjectiles(newProjectileCount);
    }
    private void Die()
    {
        gameEvents?.OnWin?.Invoke();
        Destroy(gameObject);
    }

    public void ActivateBoss()
    {
        isActive = true;
        animator.Play("Idle");
    }

    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public BossProjectile ObtenerProyectilMasCercano(Vector3 posicionJugador)
    {
        return orbitingProjectiles
            .Where(p => p != null)
            .OrderBy(p => Vector2.Distance(p.transform.position, posicionJugador))
            .FirstOrDefault();
    }
}
