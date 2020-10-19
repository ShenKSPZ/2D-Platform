using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSoul : MonoBehaviour
{
    public float StartFollowTime;
    public float MoveSpeed;
    public bool Flip;
    public float rotateSpeed = 10f;
    public GameObject Explosion;

    float FollowTime;
    float AngleVelocity;
    public Transform Player;

    // Start is called before the first frame update
    void Awake()
    {
        Player = FindObjectOfType<MainController>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(FollowTime < StartFollowTime)
        {
            FollowTime += Time.deltaTime;
        }
        else
        {
            //float angle = GetAngle(transform.position, Player.position);
            float angle = Mathf.Atan2(Player.position.y - transform.position.y, Player.position.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetAngels = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetAngels, rotateSpeed * Time.deltaTime);
            if (Quaternion.Angle(targetAngels, transform.rotation) < 1)
            {
                transform.rotation = targetAngels;
            }
        }

        transform.Translate(new Vector3(MoveSpeed * Time.deltaTime, 0, 0));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Ground") || other.CompareTag("Player"))
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
