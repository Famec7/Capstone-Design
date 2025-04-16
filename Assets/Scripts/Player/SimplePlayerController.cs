using UnityEngine;

public class SimplePlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            other.gameObject.GetComponent<MonsterAI>()?.OnDamagedByPlayer();
        }
    }
}