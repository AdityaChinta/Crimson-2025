using UnityEngine;

public class RangedSlash : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject player;
    public GameObject valk;

    private float timer;
    public float slashSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        
        Vector3 direction = player.transform.position - transform.position;
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * slashSpeed;

        float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > 6) { Destroy(gameObject); }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collision occurs");
            PlayerControl pS = player.GetComponent<PlayerControl>();
            pS.TakeDamage(20);
            Destroy(gameObject);
        }
    }
}
