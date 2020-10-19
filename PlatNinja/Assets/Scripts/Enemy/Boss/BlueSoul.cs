using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueSoul : MonoBehaviour
{

    public Animator Anim;
    public SpriteRenderer SR;

    public GameObject SoulBullet;

    private void Awake()
    {
        Anim = GetComponent<Animator>();
        SR = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        Anim.Play("BlueSoul", 0);
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }

    void InstantiateBarrage()
    {
        for(int i = 0; i < 8; i++)
        {
            Instantiate(SoulBullet, transform.position, Quaternion.Euler(0, 0, i * 45));
        }
    }
}
