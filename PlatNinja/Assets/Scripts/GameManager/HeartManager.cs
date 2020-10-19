using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{

    public float MaxHP;
    public float currentHP;
    public Sprite HasHeart;
    public Sprite NoHeart;
    public Image[] Heart;

    MainController MC;

    private void Awake()
    {
        MC = FindObjectOfType<MainController>();
    }

    // Update is called once per frame
    void Update()
    {
        currentHP = MC.currentHP;
        MaxHP = MC.currentHP;
        for(int index = 0; index < Heart.Length; index++)
        {
            if(index <= currentHP - 1)
            {
                Heart[index].sprite = HasHeart;
            }
            else
            {
                Heart[index].sprite = NoHeart;
            }
        }
    }
}
