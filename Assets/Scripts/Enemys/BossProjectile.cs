using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 6f;
    private bool launched = false;
    private Vector2 direction;

    public void SetOrbitCenter(Transform center)
    {
        transform.parent = center;
    }

    public void LaunchOutward()
    {
        launched = true;
        transform.parent = null;
        direction = (transform.position - Camera.main.transform.position).normalized;
        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        if (launched)
        {
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            player.TakeDamage(1);
            Destroy(gameObject);
        }
    }
}