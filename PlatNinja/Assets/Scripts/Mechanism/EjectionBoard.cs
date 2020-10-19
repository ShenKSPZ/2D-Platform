using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EjectionBoard : MonoBehaviour
{

    public Sprite UnEject;
    public Sprite Eject;
    public SpriteRenderer SR;
    public Vector2 Direction;

    // Start is called before the first frame update
    void Awake()
    {
        SR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(SR.sprite == Eject)
        {
            Invoke("Recovery", 0.3f);
        }
    }

    void Recovery()
    {
        SR.sprite = UnEject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.SendMessage("Eject", Direction);
            SR.sprite = Eject;
        }
    }
}
