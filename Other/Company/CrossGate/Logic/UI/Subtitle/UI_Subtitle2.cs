using Logic.Core;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Lib.Core;
using Table;
using UnityEngine.ResourceManagement.AsyncOperations;
using DG.Tweening;
using System.Collections.Generic;

namespace Logic
{
    public class UI_Subtitle2 : UIBase
    {
        private Button skipBtn;
        private Image subtitleImage;
        private GameObject subtitleDesc;
        private GameObject subtitleContent;
        private Transform bgRoot;
        private Timer timer_2;
        private Timer timer_1;
        private float timer0;
        private float timer;
        private float timer2;
        private float timer3;
        private bool isCanCuttimr = false;
        private bool bskipshow = false;
        private bool bautoclose = false;
        private int currentIndex;
        private Animator animator;
        private Animator animator2;
        CSVSubtitle.Data subtitleData = null;
        private AsyncOperationHandle<GameObject> mHandle;
        private GameObject volumeGo;

        private float textOpenTime = 0f;
        private float textCloseTime = 0f;

        private static readonly float MaxTextLength = 22f;
        private static readonly float MaxEffectScale = 5.5f;
        private static readonly float minEffectScale = 0.6f;
        protected override void OnLoaded()
        {
            bgRoot = gameObject.transform.Find("Animator/BGRoot");
            subtitleImage = gameObject.transform.Find("Animator/Image_Background_Subtitle").GetComponent<Image>();
            subtitleDesc = gameObject.transform.Find("Animator/Text").gameObject;
            subtitleContent = gameObject.transform.Find("Animator/Scroll_SubtitleGroup/view").gameObject;
            animator2 = gameObject.transform.Find("Animator").GetComponent<Animator>();
            skipBtn = gameObject.transform.Find("Animator/SkipButton").GetComponent<Button>();
            skipBtn.onClick.AddListener(OnSkipBtnClicked);
            volumeGo = transform.Find("Animator/Volume").gameObject;
            AnimationClip[] clips = subtitleDesc.GetComponent<Animator>().runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                AnimationClip tempClip = clips[i];
                if (tempClip.name == "UI_Subtitle2_Animator_Open")
                {
                    textOpenTime = tempClip.averageDuration;
                }
                else if(tempClip.name == "UI_Subtitle2_Animator_Close")
                {
                    textCloseTime = tempClip.averageDuration;
                }
            }
        }

        protected override void OnOpen(object arg)
        {
            subtitleData = arg as CSVSubtitle.Data;
            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterCutScene);
        }

        protected override void OnShow()
        {
            timer0 = 0;
            volumeEnd = false;
            if (subtitleData.lens)
            {
                volumeGo.SetActive(true);
                Animator volumeAni = volumeGo.GetComponent<Animator>();
                volumeAni.Play("Open", -1, 0);
                timer0 = volumeAni.runtimeAnimatorController.animationClips[0].averageDuration;
            }
            SetSubtitle();
        }

        bool volumeEnd = false;
        protected override void OnUpdate()
        {
            timer0 -= deltaTime;
            if (timer0 <= 0)
            {
                if (!volumeEnd && loadEnd)
                {
                    volumeEnd = true;
                    AnimationClip an = animator.runtimeAnimatorController.animationClips[0];
                    AnimationClip an2 = animator2.runtimeAnimatorController.animationClips[0];
                    float _timer = Mathf.Max(an.averageDuration, an2.averageDuration);
                    animator.Play("Open", -1, 0);
                    animator2.Play("Open", -1, 0);

                    timer_1?.Cancel();
                    timer_1 = Timer.Register(_timer, () =>
                    {
                        isCanCuttimr = true;
                        currentIndex = 0;
                        timer = 2.0f;
                        SetSubtitleText();
                        AudioUtil.PlayAudio(subtitleData.musicModel);
                        audioEntry = AudioUtil.PlayAudio(subtitleData.voiceModel);
                    });
                }
                if (isCanCuttimr)
                {
                    if (!bskipshow)
                    {
                        timer -= deltaTime;
                        if (timer <= 0)
                        {
                            bskipshow = true;
                            timer = 0;
                            skipBtn.gameObject.SetActive(true);
                        }
                    }

                    if (bautoclose)
                    {
                        timer2 -= deltaTime;

                        if (timer2 <= 0)
                        {
                            bautoclose = false;
                            timer2 = 0;
                            animator.Play("Close", -1, 0);
                            animator2.Play("Close", -1, 0);
                            AnimationClip an = animator.runtimeAnimatorController.animationClips[1];
                            AnimationClip an2 = animator2.runtimeAnimatorController.animationClips[1];
                            timer3 = Mathf.Max(an.averageDuration, an2.averageDuration);
                        }
                    }
                    if (timer3 > 0)
                    {
                        timer3 -= deltaTime;

                        if (timer3 <= 0)
                        {
                            bautoclose = false;
                            timer3 = 0;
                            Sys_Narrate.Instance.eventEmitter.Trigger<uint>(Sys_Narrate.EEvents.OnNarrateEnd, 1);
                            CloseUI();

                        }
                    }
                }
            }
        }

        protected override void OnHide()
        {
            textTimer?.Cancel();
            timer_2?.Cancel();
            timer_1?.Cancel();
            if (bskipshow)
            {
                bskipshow = false;
                skipBtn.gameObject.SetActive(false);
            }
            isCanCuttimr = false;
            bautoclose = false;
            loadEnd = false;
            volumeEnd = false;
            bgRoot.gameObject.DestoryAllChildren();
            AddressablesUtil.ReleaseInstance(ref mHandle, MHandle_Completed);

            audioEntry?.Stop();
            AudioUtil.PlayMapBGM();
        }

        Timer textTimer = null;
        List<Animator> showTextAnis = new List<Animator>(2);
        private void SetSubtitleText()
        {
            if (null != subtitleContent)
            {
                if (subtitleContent.transform.childCount >= 2)
                {
                    if (showTextAnis.Count >= 2)
                    {
                        for (int i = 0; i < showTextAnis.Count; i++)
                        {
                            showTextAnis[i].Play("Close", -1, 0);
                        }
                    }
                    textTimer?.Cancel();
                    textTimer = Timer.Register(textCloseTime, () =>
                    {
                        if (null != subtitleContent)
                        {
                            subtitleContent.DestoryAllChildren();
                            showTextAnis.Clear();
                            GameObject subtitleDescGo = FrameworkTool.CreateGameObject(subtitleDesc, subtitleContent);
                            SetSubtitleEffect(subtitleDescGo);
                        }
                    });
                }
                else
                {
                    GameObject subtitleDescGo = FrameworkTool.CreateGameObject(subtitleDesc, subtitleContent);
                    SetSubtitleEffect(subtitleDescGo);
                }
            }
        }

        private void SetSubtitleEffect(GameObject gameObject)
        {
            string subText = LanguageHelper.GetTextContent(subtitleData.content[currentIndex]);
            TextHelper.SetText(gameObject.transform.Find("Mask_Image/Text_Subtitle").GetComponent<Text>(), subtitleData.content[currentIndex]);
            gameObject.transform.Find("Mask_Image/Text_Subtitle/Fx_UI_Subtile2_02").localScale = new Vector3(GetEffectScale(subText.Length), 1, 1);
            showTextAnis.Add(gameObject.GetComponent<Animator>());
            gameObject.SetActive(true);
            textTimer?.Cancel();
            textTimer = Timer.Register(textOpenTime, () =>
            {
                timer_2?.Cancel();
                timer_2 = Timer.Register(0.2f, () =>
                {
                    currentIndex++;
                    CheckNextSubtitle();
                });
            });
        }

        private float GetEffectScale(int length)
        {
            float b = MaxEffectScale / MaxTextLength * length;
            if (b > MaxEffectScale)
            {
                b = MaxEffectScale;
            }
            else if(b < minEffectScale)
            {
                b = minEffectScale;
            }
            return b;
        }

        private void SetSubtitle()
        {
            loadEnd = false;
            timer3 = 0;
            timer2 = subtitleData.retainTime / 1000.0f;
            string subtitleBackgroundPath = subtitleData.picModel;
            subtitleContent.DestoryAllChildren();
            if (subtitleBackgroundPath != null)
            {
                AddressablesUtil.InstantiateAsync(ref mHandle, subtitleBackgroundPath, MHandle_Completed);
            }
        }

        private void CloseUI()
        {
            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
            CloseSelf();
        }

        private void CheckNextSubtitle()
        {
            if (currentIndex < subtitleData.content.Count)
            {
                SetSubtitleText();
            }
            else
            {
                if(null != subtitleContent)
                {
                    if (subtitleContent.transform.childCount >= 1)
                    {
                        if (showTextAnis?.Count >= 1)
                        {
                            for (int i = 0; i < showTextAnis?.Count; i++)
                            {
                                showTextAnis[i].Play("Close", -1, 0);
                            }
                        }
                        textTimer?.Cancel();
                        textTimer = Timer.Register(textCloseTime, () =>
                        {
                            if (null != subtitleContent)
                            {
                                subtitleContent.DestoryAllChildren();
                                showTextAnis?.Clear();
                                bautoclose = true;
                            }
                        });
                    }
                }
                    
            }
        }

        private void OnSkipBtnClicked()
        {
            CloseUI();
        }

        private AudioEntry audioEntry;

        bool loadEnd = false;
        private void MHandle_Completed(AsyncOperationHandle<GameObject> handle)
        {
            GameObject bgGameobject = handle.Result;
            if(null != bgGameobject)
            {
                bgGameobject.transform.SetParent(bgRoot);
                RectTransform rectTransform = bgGameobject.transform as RectTransform;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localEulerAngles = Vector3.zero;
                rectTransform.localScale = Vector3.one;

                animator = bgGameobject.transform.Find("Image_Background_Subtitle").GetComponent<Animator>();

                UILocalSorting[] layoutControlls = gameObject.GetComponentsInChildren<UILocalSorting>();
                for (int i = 0; i < layoutControlls.Length; i++)
                {
                    layoutControlls[i].SetRootSorting(nSortingOrder);
                }
                loadEnd = true;
            }
        }
    }
}