using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDPositionCorrect : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Camera c3;

    private RectTransform rectTransform
    {
        get
        {
            return gameObject.transform as RectTransform;
        }
    }

    private Vector3 baseoffest = Vector3.zero;
    private Vector2 uiRootOffest; //ui偏移
    public bool NeedCorrectAtSkillPlay;
    public bool NeedFollow = true;


    public void SetTarget(Transform transform)
    {
        target = transform;
        c3 = CameraManager.mCamera;
        CalPos_Trans();
    }

    public void SetCamera(Camera camera)
    {
        c3 = camera;
    }

    public void SetbaseOffest(Vector3 baseoffest)
    {
        this.baseoffest = baseoffest;
    }

    public void SetuiRootOffest(Vector2 uiRootOffest)
    {
        this.uiRootOffest = uiRootOffest;
    }

    public void CalOffest(Vector3 _offest)
    {
        baseoffest += _offest;
        CalPos_Trans();
    }

    public void CalPos_Trans()
    {
        if (target == null)
        {
            return;
        }
        if (!NeedCorrectAtSkillPlay || CameraManager.mSkillPlayCamera == null)
        {
            CameraManager.World2UI(gameObject, target.position + baseoffest, c3, CameraManager.mUICamera);
            rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y, CameraManager.mUICamera.transform.position.z + 100);
            rectTransform.anchoredPosition += uiRootOffest;
        }
        if (NeedCorrectAtSkillPlay && CameraManager.mSkillPlayCamera != null)
        {
            CameraManager.World2UI(gameObject, target.position + new Vector3(0, 1.5f, 0), CameraManager.mSkillPlayCamera, CameraManager.mUICamera, 800, 480,
                      CameraManager.relativePos.x, CameraManager.relativePos.y);
            rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y, CameraManager.mUICamera.transform.position.z + 100);
        }
    }

    public void CalPos_Vec(Vector3 vector3)
    {
        if (!NeedCorrectAtSkillPlay || CameraManager.mSkillPlayCamera == null)
        {
            CameraManager.World2UI(gameObject, vector3, CameraManager.mCamera, CameraManager.mUICamera);
            rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y, CameraManager.mUICamera.transform.position.z + 100);
        }
        if (NeedCorrectAtSkillPlay && CameraManager.mSkillPlayCamera != null)
        {
            CameraManager.World2UI(gameObject, vector3, CameraManager.mSkillPlayCamera, CameraManager.mUICamera, 800, 480,
                  CameraManager.relativePos.x, CameraManager.relativePos.y);
            rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y, CameraManager.mUICamera.transform.position.z + 100);
        }
    }

    private void LateUpdate()
    {
        if (!NeedFollow)
        {
            return;
        }
        CalPos_Trans();
    }

    public void Dispose()
    {
        target = null;
        uiRootOffest = Vector2.zero;
        baseoffest = Vector3.zero;
        NeedCorrectAtSkillPlay = false;
        NeedFollow = true;
    }
}
