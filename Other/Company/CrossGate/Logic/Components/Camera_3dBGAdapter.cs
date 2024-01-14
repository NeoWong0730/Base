using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    //public class Camera_3dBGAdapter : MonoBehaviour
    //{
    //    private float ratio;//宽比高
    //    private Camera m_camera;

    //    private SpriteRenderer m_spriteRenderer;
    //    private Transform m_bg;

    //    public void Init(float _width, float _height)
    //    {
    //        m_camera = transform.Find("Camera").GetComponent<Camera>();

    //        m_bg = transform.Find("bg");
    //        float halfHeight = Mathf.Tan(Mathf.Deg2Rad * (m_camera.fieldOfView / 2)) * Vector3.Distance(m_bg.position, m_camera.transform.position);
    //        float height = halfHeight * 2;            
            
    //        m_spriteRenderer = transform.Find("bg").GetComponent<SpriteRenderer>();
    //        Vector3 scale = m_bg.localScale;
    //        float _ratio = height / m_spriteRenderer.bounds.size.y;
    //        m_bg.localScale = scale * _ratio * _width / 1280 * 1.2f;
    //    }
    //}
}


