using Framework;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;
using Table;
using UnityEngine.EventSystems;
using static Packet.CmdPetGetHandbookRes.Types;
using System.Collections.Generic;
using Lib.Core;

namespace Logic
{
    public class UI_Pet_Story : UIBase
    {
        private uint petStoryScrollId;
        private Text scrollNameText;
        private Text storyContests;
        private Text storyUnlock;
        //private Text unlockRewardText;
        private Button blankBtn;
        private ScrollRect textScroll;
        //private Button goTask;
        private GameObject lockGo;
        private Animator unlockAnimator;
        private bool isAni = false;
        private Timer unlockTime;
        protected override void OnLoaded()
        {
            unlockAnimator = transform.Find("Animator").GetComponent<Animator>();
            textScroll = transform.Find("Animator/Message/View_01/Scroll_Text").GetComponent<ScrollRect>();
            scrollNameText = transform.Find("Animator/Message/Text_Level").GetComponent<Text>();
            storyContests = transform.Find("Animator/Message/View_01/Scroll_Text/Text_story").GetComponent<Text>();
            storyUnlock = transform.Find("Animator/Message/View_Lock/Image_Titlebg (1)/Condition/Text_Condition").GetComponent<Text>();
            //unlockRewardText = transform.Find("Animator/Message/View_02/Text").GetComponent<Text>();
            blankBtn = transform.Find("Animator/Message/View_Lock/Image_Titlebg (1)/UnLock/Light").GetComponent<Button>();
            blankBtn.onClick.AddListener(UnlockTask);
            //goTask = transform.Find("Animator/Message/View_02/Btn_01").GetComponent<Button>();
            //goTask.onClick.AddListener(GoTaskClicked);
            lockGo = transform.Find("Animator/Message/View_Lock").gameObject;
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(transform.Find("Blank"));
            eventListener.AddEventListener(EventTriggerType.PointerClick, OnBlankClicked);
        }

        private void OnBlankClicked(BaseEventData baseEventData)
        {
            CloseSelf();
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnPetStroyClose);            
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnPetActivateStory, OnPetActivateStory, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnReceived, OnReceived, toRegister);
        }

        private void OnReceived(int type, uint taskId, TaskEntry taskEntry)
        {
            ResetData();
        }

        private void OnPetActivateStory(uint stroyid)
        {
            if (!isAni)
            {
                isAni = true;
                unlockAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                unlockAnimator.Play("Activate", -1, 0);
                AnimationClip ani = unlockAnimator.runtimeAnimatorController.animationClips[0];
                unlockTime?.Cancel();
                unlockTime = Timer.Register(ani.averageDuration, () =>
                {
                    ResetData();
                }, null, false, false);
            }
        }

        protected override void OnHide()
        {
            unlockTime?.Cancel();
        }

        private void GoTaskClicked()
        {
            CSVBackgroundStory.Data cSVBackgroundStoryData = CSVBackgroundStory.Instance.GetConfData(petStoryScrollId);
            if (Sys_Task.Instance.receivedTasks.TryGetValue(12, out SortedDictionary<uint, TaskEntry> petStoryDic))
            {
                if (null != cSVBackgroundStoryData.mission)
                {
                    for (int i = 0; i < cSVBackgroundStoryData.mission.Count; i++)
                    {
                        uint taskid = cSVBackgroundStoryData.mission[i];
                        if (petStoryDic.ContainsKey(taskid))
                        {
                            Sys_Task.Instance.TryDoTask(petStoryDic[taskid], true, false, true);
                            TryCloseBook();
                        }
                    }
                }
            }
        }

        private void TryCloseBook()
        {
            if (UIManager.IsOpen(EUIID.UI_Pet_Story))
                UIManager.CloseUI(EUIID.UI_Pet_Story);
            if (UIManager.IsOpen(EUIID.UI_Pet_BookReview))
                UIManager.CloseUI(EUIID.UI_Pet_BookReview);
            if (UIManager.IsOpen(EUIID.UI_Pet_Message))
                UIManager.CloseUI(EUIID.UI_Pet_Message, needDestroy: false);
            if (UIManager.IsOpen(EUIID.UI_GodPet))
                UIManager.CloseUI(EUIID.UI_GodPet);
            if (UIManager.IsOpen(EUIID.UI_GodPetReview))
                UIManager.CloseUI(EUIID.UI_GodPetReview);
        }

        private void UnlockTask()
        {
            Sys_Pet.Instance.OnPetActivateStoryReq(petStoryScrollId);
        }

        protected override void OnOpen(object arg)
        {
            petStoryScrollId = (uint)arg;
        }

        protected override void OnShow()
        {
            ResetData();
        }

        private bool IsHave12Task()
        {
            CSVBackgroundStory.Data cSVBackgroundStoryData = CSVBackgroundStory.Instance.GetConfData(petStoryScrollId);
            if (Sys_Task.Instance.receivedTasks.TryGetValue(12, out SortedDictionary<uint, TaskEntry> petStoryDic))
            {
                return true;
            }
            return false;
        }

        private bool IsHaveTask()
        {
            CSVBackgroundStory.Data cSVBackgroundStoryData = CSVBackgroundStory.Instance.GetConfData(petStoryScrollId);
            if (Sys_Task.Instance.receivedTasks.TryGetValue(12, out SortedDictionary<uint, TaskEntry> petStoryDic))
            {
                if (null != cSVBackgroundStoryData.mission)
                {
                    for (int i = 0; i < cSVBackgroundStoryData.mission.Count; i++)
                    {
                        if (petStoryDic.ContainsKey(cSVBackgroundStoryData.mission[i]))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void ResetData()
        {            
            CSVPetNewLoveUp.Data cSVBackgroundStoryData = CSVPetNewLoveUp.Instance.GetConfData(petStoryScrollId);
            if (null != cSVBackgroundStoryData)
            {
                uint level = petStoryScrollId - (uint)(petStoryScrollId / 1000) * 1000;
                //scrollNameText.text = (petStoryScrollId - (uint)(petStoryScrollId / 10) * 10).ToString();
                scrollNameText.text = "";
                storyContests.text = LanguageHelper.GetTextContent(cSVBackgroundStoryData.contests);
                storyUnlock.text = LanguageHelper.GetTextContent(cSVBackgroundStoryData.txt, level.ToString());
                

                HandbookData handbookData = Sys_Pet.Instance.GetPetBookData((uint)(petStoryScrollId / 1000));
                bool isUnlock = handbookData != null && handbookData.ClickStory.Contains(level);

                bool isLevelActive = handbookData != null && level <= handbookData.LoveLevel;
                bool ishaveTask = IsHaveTask();
                //需要补充获取故事是否解锁标记
                textScroll.gameObject.SetActive(isUnlock);
                lockGo.SetActive(!isUnlock);
                storyUnlock.transform.parent.gameObject.SetActive(!isUnlock && !isLevelActive);//是否已经解封故事
                blankBtn.transform.parent.gameObject.SetActive(!isUnlock && isLevelActive);//解锁
                //goTask.transform.parent.gameObject.SetActive(ishaveTask); // 领取了任务
                //unlockRewardText.transform.parent.gameObject.SetActive(!isUnlock && cSVBackgroundStoryData.last == 1);
                textScroll.normalizedPosition = new Vector2(0, 1);
            }

        }

    }
}
