using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SequenceFrame : MonoBehaviour
{
    public Image image;
    public List<Sprite> sprites;
    private int index = 0;
    public float frameRate = 20f;
    private float dt;
    public bool useTimeScale = false;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (sprites == null || sprites.Count <= 0)
            return;

        dt += useTimeScale ? Time.deltaTime : Time.unscaledDeltaTime;

        if (dt >= 1 / frameRate)
        {
            dt = 0;
            ++index;
            if (index >= sprites.Count)
            {
                index = 0;
            }
            image.sprite = sprites[index];
        }
    }

    public void StopIn(int index)
    {
        enabled = false;
        index = Mathf.Clamp(index, 0, sprites.Count - 1);
        image.sprite = sprites[index];
    }

    public void Play(int index)
    {
        enabled = true;
        index = Mathf.Clamp(index, 0, sprites.Count - 1);
        image.sprite = sprites[index];
    }
}
