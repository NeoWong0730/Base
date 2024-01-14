using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BGIncreaser : MonoBehaviour
{
    [Flags]
    public enum EDIR
    {
        Left = 0x01,
        Right = 0x02,
        Up = 0x04,
        Down = 0x08,
    }

    public EDIR dir = EDIR.Down;
    
    private RectTransform bg;
    public bool auto = true;
    public bool autoRecoveryWhenClose = true;
    public float speed = 50f;
    public float time = 1.5f;

    private bool isReached = true;
    private float startTime;
    private Vector2 sizeDelta;

    private void Awake()
    {
        if (bg == null)
        {
            bg = GetComponent<RectTransform>();
            sizeDelta = bg.sizeDelta;
        }

        if (auto)
        {
            Begin(speed, time);
        }
    }

    private void OnDisable()
    {
        if(autoRecoveryWhenClose)
            bg.sizeDelta = sizeDelta;
    }

    public void Begin(float speed = 50f, float time = 1.5f)
    {
        this.speed = speed;
        this.time = time;
        
        startTime = Time.time;
        isReached = false;
    }

    private void Update()
    {
        if (!isReached)
        {
            isReached = Time.time - startTime > time;
            if ((dir & EDIR.Left) != 0)
            {
                
            }
            if ((dir & EDIR.Right) != 0)
            {
                
            }
            if ((dir & EDIR.Up) != 0)
            {
                bg.sizeDelta = new Vector2(bg.sizeDelta.x, bg.sizeDelta.y + speed * Time.deltaTime); 
            }
            if ((dir & EDIR.Down) != 0)
            {
                bg.sizeDelta = new Vector2(bg.sizeDelta.x, bg.sizeDelta.y + speed * Time.deltaTime); 
            }
        }
    }
}
