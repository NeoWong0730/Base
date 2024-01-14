using Logic.Core;
using UnityEngine.UI;
using UnityEngine;
using Lib.Core;
using Table;
using DG.Tweening;

using UnityEngine.ResourceManagement.AsyncOperations;
namespace Logic
{
    public class UI_Chapter : UIBase
    {
        public enum EChapterState
        {
            EOpen,
            EStop,
            Eend,
            EClose,
        }
        private Image chapterImage;
        private Text chapterName;
        private Text chapterDesc;
        private Transform bgRoot;
        private float timer;
        private Animator animator;
        private Animator animator2;
        private float time;
        Tweener fade;
        CSVChapter.Data cSVChapterData;

        private EChapterState eChapter = EChapterState.EClose;
        private AsyncOperationHandle<GameObject> mHandle;
        protected override void OnLoaded()
        {
            bgRoot = gameObject.transform.Find("Animator/BGRoot");
            chapterImage = gameObject.transform.Find("Animator/Image_Background_Chapter").GetComponent<Image>();
            chapterName = gameObject.transform.Find("Animator/Text_Name").GetComponent<Text>();
            chapterDesc = gameObject.transform.Find("Animator/mask_image/Text_Desc").GetComponent<Text>();
            animator2 = gameObject.transform.Find("Animator").GetComponent<Animator>();
        }

        protected override void OnOpen(object arg)
        {
            cSVChapterData = arg as CSVChapter.Data;
            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterCutScene);
        }

        protected override void OnShow()
        {
            SetChapter();
        }

        protected override void OnUpdate()
        {
            if(eChapter != EChapterState.EClose)
            {
                if(eChapter == EChapterState.EOpen && timer > 0)
                {
                    timer -= deltaTime;
                    if (timer <= 0)
                    {
                        eChapter = EChapterState.EStop;
                        AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                        timer = time;
                    }
                }
                else if(eChapter == EChapterState.EStop && timer > 0)
                {
                    timer -= deltaTime;
                    if (timer <= 0)
                    {
                        animator.Play("Close", -1, 0);
                        animator2.Play("Close", -1, 0);
                        eChapter = EChapterState.Eend;
                        AnimationClip an = animator.runtimeAnimatorController.animationClips[1];
                        AnimationClip an2 = animator2.runtimeAnimatorController.animationClips[1];
                        timer = Mathf.Max(an.averageDuration, an2.averageDuration);
                    }
                }
                else if(eChapter == EChapterState.Eend && timer > 0)
                {
                    timer -= deltaTime;
                    if (timer <= 0)
                    {
                        timer = 0;
                        eChapter = EChapterState.EClose;
                        GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
                        UIManager.CloseUI(EUIID.UI_Chapter);
                    }
                }
            }       
        }

        private void SetChapter()
        {
            string chapterBackgroundPath = cSVChapterData.picModel;
            
            if (chapterBackgroundPath != null)
            {
                AddressablesUtil.InstantiateAsync(ref mHandle, chapterBackgroundPath, MHandle_Completed);
            }           
        }

        protected override void OnHide()
        {
            eChapter = EChapterState.EClose;
            timer = 0;
            chapterName.text = "";
            chapterDesc.text = "";
            bgRoot.gameObject.DestoryAllChildren();

            AudioUtil.PlayMapBGM();

            if (mHandle.IsValid())
            {
                AddressablesUtil.Release<GameObject>(ref mHandle, MHandle_Completed);
            }
        }
        private void MHandle_Completed(AsyncOperationHandle<GameObject> handle)
        {
            GameObject bgGameobject = handle.Result;

            bgGameobject.transform.SetParent(bgRoot);
            RectTransform rectTransform = bgGameobject.transform as RectTransform;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localEulerAngles = Vector3.zero;
            rectTransform.localScale = Vector3.one;

            animator = bgGameobject.transform.Find("Image_Background_Chapter").GetComponent<Animator>();
            UILocalSorting[] layoutControlls =  gameObject.GetComponentsInChildren<UILocalSorting>();
            for (int i = 0; i < layoutControlls.Length; i++)
            {
                layoutControlls[i].SetRootSorting(nSortingOrder);
            }

            AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);

            AnimationClip an = animator.runtimeAnimatorController.animationClips[0];
            AnimationClip an2 = animator2.runtimeAnimatorController.animationClips[0];
            timer = Mathf.Max(an.averageDuration, an2.averageDuration) + 0.5f;
            eChapter = EChapterState.EOpen;
            time = cSVChapterData.time / 1000.0f;
            TextHelper.SetText(chapterName, CSVLanguage.Instance.GetConfData(cSVChapterData.chapterName)?.words);
            TextHelper.SetText(chapterDesc, CSVLanguage.Instance.GetConfData(cSVChapterData.targetType)?.words);
            AudioUtil.PlayAudio(cSVChapterData.musicModel);
        }
    }
}

