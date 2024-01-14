using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class EraserArea : MonoBehaviour,IPointerClickHandler,IDragHandler,IEndDragHandler
{
    private RawImage rawImage;
    private Texture2D tex;
    private Texture2D MyTex;
    private bool isStartEraser;
    private bool isEndEraser;
    private int brushSize = 8;
    private int brushLerpSize = 3;
    private int rate = 90;
    private float fate;
    private float maxColorA;
    private float colorA;
    private int mWidth;
    private int mHeight;
    private Vector2 _lastPoint;
    private Action endEraser;
    private Action startEraser;
    
    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
        tex = (Texture2D)rawImage.texture;
    }

    public void SetData( int _brushSize,int _brushLerpSize,int _rate,Action _endEraser,Action _startEraser)
    {
        brushSize = _brushSize;
        brushLerpSize = _brushLerpSize;
        rate = _rate;
        endEraser = _endEraser;
        startEraser = _startEraser;
    }

    public void InitData()
    {
        MyTex = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
        mWidth = MyTex.width;
        mHeight = MyTex.height;
        MyTex.SetPixels32(tex.GetPixels32());
        //MyTex.SetPixels(tex.GetPixels());
        MyTex.Apply();
        rawImage.texture = MyTex;
        maxColorA = mWidth * mHeight;
        colorA = 0;
        isEndEraser = false;
        isStartEraser = false;
    }

    private void Update()
    {
        if (isEndEraser)
            return;
        getTransparentPercent();
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (isEndEraser)
            return;
        LerpCheck(eventData.position);
        //_lastPoint = Vector2.zero;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _lastPoint = Vector2.zero;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isEndEraser)
            return;
        LerpCheck(eventData.position);
        _lastPoint = Vector2.zero;
    }


    private void LerpCheck(Vector2 point)
    {
        CheckPoint(point);
        if (_lastPoint == Vector2.zero)
        {
            _lastPoint = point;
            return;
        }
        float dis = Vector2.Distance(point, _lastPoint);
        if (dis > brushLerpSize)
        {
            Vector2 dir = (point - _lastPoint).normalized;
            int num = (int)(dis / brushLerpSize);
            for (int i = 0; i < num; i++)
            {
                Vector2 newPoint = _lastPoint + dir * (i + 1) * brushLerpSize;
                CheckPoint(newPoint);
            }
        }
        _lastPoint = point;
    }
    

    public void CheckPoint(Vector3 pScreenPos)
    {
        Vector3 worldPos = CameraManager.mUICamera.ScreenToWorldPoint(pScreenPos);
        Vector3 localPos = rawImage.transform.InverseTransformPoint(worldPos);
        if (localPos.x > -mWidth / 2 && localPos.x < mWidth / 2 && localPos.y > -mHeight / 2 && localPos.y < mHeight / 2)
        {
            for (int i = (int)localPos.x - brushSize; i < (int)localPos.x + brushSize; i++)
            {
                for (int j = (int)localPos.y - brushSize; j < (int)localPos.y + brushSize; j++)
                {
                    if (Mathf.Pow(i - localPos.x, 2) + Mathf.Pow(j - localPos.y, 2) > Mathf.Pow(brushSize, 2))
                        continue;
                    if (i < 0) { if (i < -mWidth / 2) { continue; } }
                    if (i > 0) { if (i > mWidth / 2) { continue; } }
                    if (j < 0) { if (j < -mWidth / 2) { continue; } }
                    if (j > 0) { if (j > mWidth / 2) { continue; } }

                    Color col = GetPixel(MyTex, i + (int)mWidth / 2, j + (int)mHeight / 2);
                    if (col.a != 0f)
                    {
                        col.a = 0.0f;
                        colorA++;
                        SetPixel(MyTex, i + (int)mWidth / 2, j + (int)mHeight / 2, col);
                    }
                }
            }
            if (!isStartEraser)
            {
                isStartEraser = true;
                startEraser?.Invoke();
            }
            MyTex.Apply();
        }
    }

    public void getTransparentPercent()
    {
        fate = (float)colorA / (float)maxColorA * 100;

        if (fate >= rate)
        {
            isEndEraser = true;
            endEraser?.Invoke();
        }
    }

    public  Color GetPixel(Texture2D texture, int x, int y)
    {
        return texture.GetPixel(x, y);
    }

    public  void SetPixel(Texture2D texture, int x, int y, Color col)
    {
        texture.SetPixel(x, y, col);
    }

    public void Dispose()
    {
        GameObject.Destroy(MyTex);
        fate = 0;
        colorA = 0;
        _lastPoint = Vector2.zero;
    }
}
