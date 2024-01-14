using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimatorControl : MonoBehaviour
{
    private static string kEnterStateName = "Open";
    private static string kExitStateName = "Close";

    [SerializeField]
    private List<Animator> mAnimators;
    [SerializeField]
    private float fEnterTime = 0;
    [SerializeField]
    private float fExitTime = 0;

    public float EnterTime { get { return fEnterTime; } }
    public float ExitTime { get { return fExitTime; } }

    public void PlayEnter()
    {
        if (mAnimators == null || fEnterTime <= 0)
            return;

        for (int i = 0; i < mAnimators.Count; ++i)
        {
            Animator animator = mAnimators[i];
            if (animator != null && animator.isActiveAndEnabled)
            {
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;                
                animator.Play(kEnterStateName);
            }
        }
    }

    public void PlayExit()
    {
        if (mAnimators == null || fExitTime <= 0)
            return;

        for (int i = 0; i < mAnimators.Count; ++i)
        {
            Animator animator = mAnimators[i];
            if (animator != null && animator.isActiveAndEnabled)
            {
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                animator.Play(kExitStateName, -1, 0);
            }
        }
    }

    public void DisableAnimator()
    {
        if (mAnimators == null || fEnterTime <= 0)
            return;        

        for (int i = 0; i < mAnimators.Count; ++i)
        {            
            Animator animator = mAnimators[i];            
            if (animator != null && animator.isActiveAndEnabled)
            {
                animator.Play(kEnterStateName, -1, 1);
                animator.Update(0);
                animator.cullingMode = AnimatorCullingMode.CullCompletely;
            }
        }
    }

#if UNITY_EDITOR

    [ContextMenu("bake")]
    private void Bake()
    {
        if(mAnimators == null)
        {
            mAnimators = new List<Animator>();
        }
        else
        {
            mAnimators.Clear();
        }

        Animator animator = gameObject.GetComponent<Animator>();
        Animator[] animators = gameObject.GetComponentsInChildren<Animator>();

        List<Animator> animatorList = new List<Animator>(animators.Length + 1);
        animatorList.AddRange(animators);
        if(animator)
        {
            animatorList.Add(animator);
        }

        for(int animatorIndex = 0; animatorIndex < animatorList.Count; ++animatorIndex)
        {
            animator = animatorList[animatorIndex];
            if (animator == null || animator.runtimeAnimatorController == null)
                continue;

            if (animator.HasState(0, Animator.StringToHash(kEnterStateName)) && animator.HasState(0, Animator.StringToHash(kExitStateName)))
            {              
                AnimationClip[] animationClips = animator.runtimeAnimatorController.animationClips;
                AnimationClip enterClip = null;
                AnimationClip exitClip = null;

                for (int i = 0; i < animationClips.Length; ++i)
                {
                    string clipName = animationClips[i].name;
                    if (clipName.EndsWith(kEnterStateName))
                    {
                        enterClip = animationClips[i];
                    }
                    else if (clipName.EndsWith(kExitStateName))
                    {
                        exitClip = animationClips[i];
                    }

                    if (enterClip != null && exitClip != null)
                    {
                        break;
                    }
                }

                if (enterClip != null && exitClip != null)
                {
                    fEnterTime = fEnterTime < enterClip.length ? enterClip.length : fEnterTime;
                    fExitTime = fExitTime < exitClip.length ? exitClip.length : fExitTime;
                    
                    mAnimators.Add(animator);
                }                
            }            
        }
        UnityEditor.EditorUtility.DisplayDialog("生成动画备份", "完成！！！", "好的");
    }
#endif
}
