using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakPartDestory : MonoBehaviour
{

    public float LifeTime;
    public float FlashTime;
    public float FlashSpeed;

    SpriteRenderer SR;

    float cLifeTime = 0;
    float cFlashTime = 0;
    bool StartFlash = false;
    bool Flashing;

    // Start is called before the first frame update
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        cLifeTime += Time.deltaTime;
        if(cLifeTime >= LifeTime)
        {
            StartFlash = true;
        }
        if (StartFlash & cFlashTime >= FlashTime)
        {
            Destroy(gameObject);
        }
        else if (StartFlash)
        {
            cFlashTime += Time.deltaTime;
            if (!Flashing)
                StartCoroutine(IEFlash());
        }
    }

    IEnumerator IEFlash()
    {
        Flashing = true;
        if (SR.enabled == true)
            SR.enabled = false;
        else
            SR.enabled = true;

        yield return new WaitForSeconds(FlashSpeed);
        Flashing = false;
    }
}
