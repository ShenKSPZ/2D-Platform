using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulBullet : MonoBehaviour
{

    public float MoveSpeed;
    public GameObject Explosion;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(MoveSpeed * Time.deltaTime, 0, 0));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("Player"))
        {
            StartCoroutine(Explode());
        }
    }

    IEnumerator Explode()
    {
        Instantiate(Explosion, transform.position, transform.rotation);
        FindObjectOfType<AudioManager>().PlayExplosion();
        yield return new WaitForSecondsRealtime(0f);
        Destroy(gameObject);
    }
}
