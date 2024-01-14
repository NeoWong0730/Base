using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "DoTweenSharedMaterialProfile.asset", menuName = "DoTween/SharedMaterialProfile")]
public class DoTweenSharedMaterialProfile : ScriptableObject
{
    public Material mMaterial;

    public string sPropertyName;
    public float delay;
    public float duration = 1;
    public Ease easeType = Ease.Linear;
    public AnimationCurve easeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    public LoopType loopType = LoopType.Restart;
    public int loops = -1;    
    public bool isIndependentUpdate = false;
    public bool autoKill = true;
    //public bool isSpeedBased;
    //public bool isRelative;
    //public bool isFrom;

    public ShaderPropertyType eShaderPropertyType;
    public Vector4 endValueVector;
    [SerializeField]
    public Gradient gradientColor = new Gradient();
    public bool useGradientColor;

    [System.NonSerialized]
    private Tween tween;
    [System.NonSerialized]
    public int RefCount = 0;

    public void AddRef()
    {
        ++RefCount;
        DOPlay();        
    }

    public void RemoveRef()
    {
        --RefCount;        
        if (RefCount < 1)
        {
            DOStop();
        }
    }

    private void CreateTween()
    {
        if (!mMaterial)
            return;

        int propertyID = Shader.PropertyToID(sPropertyName);
        switch (eShaderPropertyType)
        {
            case ShaderPropertyType.Float:
            case ShaderPropertyType.Range:
                tween = mMaterial.DOFloat(endValueVector.x, propertyID, duration);
                break;
            case ShaderPropertyType.Color:
                if(useGradientColor)
                {
                    tween = mMaterial.DOGradientColor(gradientColor, sPropertyName, duration);
                }
                else
                {
                    Color color = new Color(endValueVector.x, endValueVector.y, endValueVector.z, endValueVector.w);
                    tween = mMaterial.DOColor(color, propertyID, duration);
                }                
                break;
            case ShaderPropertyType.Vector:
                tween = mMaterial.DOVector(endValueVector, propertyID, duration);
                break;                
            default:
                break;
        }

        if (tween == null) return;

        //if (isFrom)
        //{
        //    ((Tweener)tween).From(isRelative);
        //}
        //else
        //{
        //    tween.SetRelative(isRelative);
        //}

        tween.SetDelay(delay)
            .SetLoops(loops, loopType)
            .SetAutoKill(autoKill)
            .OnKill(() => tween = null);

        //if (isSpeedBased) 
        //    tween.SetSpeedBased();

        if (easeType == Ease.INTERNAL_Custom)
            tween.SetEase(easeCurve);
        else
            tween.SetEase(easeType);

        tween.SetUpdate(isIndependentUpdate);
    }

    public void DOPlay()
    {
        if (!Application.isPlaying)
            return;        

        if (tween != null && tween.IsActive()) 
            return;

        if (RefCount > 0)
        {
            CreateTween();
        }
    }

    public void DOReplay()
    {
        if (tween != null && tween.IsActive())
        {
            tween.Rewind();
            tween.Kill();
        }

        if (RefCount > 0)
        {
            CreateTween();
        }
    }

    public void DOStop()
    {
        if (tween != null && tween.IsActive())
        {
            tween.Rewind();
            tween.Kill();
        }
        tween = null;
    }

#if UNITY_EDITOR
    public Tween CreateEditorPreview()
    {
        if (Application.isPlaying) return null;

        CreateTween();
        return tween;
    }
#endif
}
