using DG.Tweening;
using UnityEngine;

public class DoTweenSharedMaterial : MonoBehaviour
{
    [SerializeField]
    private DoTweenSharedMaterialProfile _doTweenSharedMaterialProfile;
    public DoTweenSharedMaterialProfile doTweenSharedMaterialProfile
    {
        get { return _doTweenSharedMaterialProfile; }
        set
        {
            if (_doTweenSharedMaterialProfile != value)
            {                
                if(enabled)
                {
                    _doTweenSharedMaterialProfile?.RemoveRef();
                    _doTweenSharedMaterialProfile = value;
                    _doTweenSharedMaterialProfile?.AddRef();
                }   
                else
                {
                    _doTweenSharedMaterialProfile = value;
                }
            }
        }
    }

    private void OnEnable()
    {
        doTweenSharedMaterialProfile?.AddRef();
    }

    private void OnDisable()
    {
        doTweenSharedMaterialProfile?.RemoveRef();       
    }

    public void DOPlay()
    {        
        doTweenSharedMaterialProfile?.DOPlay();
    }

    public void DOReplay()
    {
        doTweenSharedMaterialProfile?.DOReplay();
    }

    public void DOStop()
    {
        doTweenSharedMaterialProfile?.DOStop();        
    }

#if UNITY_EDITOR
    public Tween CreateEditorPreview()
    {
        if (doTweenSharedMaterialProfile)
            return doTweenSharedMaterialProfile.CreateEditorPreview();
        return null;
    }
#endif
}