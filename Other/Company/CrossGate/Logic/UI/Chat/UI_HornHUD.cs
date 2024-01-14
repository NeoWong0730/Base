using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Table;
using System;

namespace Logic
{
    public class UI_HornHUD : UIBase
    {
        private GameObject mAnimatorRoot = null;
        private Image mImgBg = null;        
        private RectTransform rt_Viewport;
        private RectTransform rt_Viewport_Sender;
        private EmojiText txt_Content = null;
        private Text txt_Sender = null;

        private Sys_Chat.ChatContent curContent;

        //当前次数
        private int nCurTimes;
        //当前需要展示的次数
        private int nCurNeedTimes;

        private bool has = false;

        protected override void OnLoaded()
        {           
            mAnimatorRoot = transform.Find("Animator").gameObject;            
            mImgBg = transform.Find("Animator/_img_bg").GetComponent<Image>();

            Button button = mImgBg.GetComponent<Button>();
            button.onClick.AddListener(OnClick);

            rt_Viewport_Sender = transform.Find("Animator/_img_bg/_rt_Viewport_Sender") as RectTransform;
            rt_Viewport = transform.Find("Animator/_img_bg/_rt_Viewport") as RectTransform;            

            txt_Sender = transform.Find("Animator/_img_bg/_rt_Viewport_Sender/_txt_sender").GetComponent<Text>();
            txt_Content = transform.Find("Animator/_img_bg/_rt_Viewport/_txt_content").GetComponent<EmojiText>();
        }

        private void OnClick()
        {
            Sys_Chat.Instance.eChatType = Packet.ChatType.System;
            Sys_Chat.Instance.SetSystemChannelShow(Packet.ChatType.Horn);
            UIManager.OpenUI(EUIID.UI_Chat, false, null, EUIID.UI_MainInterface);
        }

        protected override void OnUpdate()
        {
            if(!has)
            {
                if(curContent == null)
                {
                    if (Sys_Chat.Instance.mChatHUD.Count > 0)
                    {
                        curContent = Sys_Chat.Instance.mChatHUD.Dequeue();                        
                        nCurTimes = 0;
                        nCurNeedTimes = Sys_Chat.Instance.nHornPlayTimes;
                    }                    
                }

                if (curContent != null)
                {
                    has = true;

                    if(curContent.mBaseInfo == null)
                    {
                        //公告
                        CSVHorn.Data hornData = CSVHorn.Instance.GetConfData(100001);

                        if (hornData != null)
                        {
                            ImageHelper.SetIcon(mImgBg, null, hornData.background);
                            TextHelper.SetText(txt_Content, curContent.sContent, CSVWordStyle.Instance.GetConfData(hornData.wordStyle));
                        }
                        else
                        {
                            TextHelper.SetText(txt_Content, curContent.sContent);
                        }
                        
                        txt_Sender.text = LanguageHelper.GetTextContent(11877);
                    }
                    else
                    {
                        CSVHorn.Data hornData = CSVHorn.Instance.GetConfData(curContent.mBaseInfo.nHornItemID);
                        if(hornData == null)
                        {
                            hornData = CSVHorn.Instance.GetConfData(100001);
                        }

                        if (hornData != null)
                        {
                            ImageHelper.SetIcon(mImgBg, null, hornData.background);
                            TextHelper.SetText(txt_Content, curContent.sContent, CSVWordStyle.Instance.GetConfData(hornData.wordStyle));
                        }
                        else
                        {
                            TextHelper.SetText(txt_Content, curContent.sContent);
                        }
                        
                        txt_Sender.text = LanguageHelper.GetTextContent(11878, curContent.mBaseInfo.sSenderName);
                    }                    
                    mAnimatorRoot.SetActive(true);                    

                    rt_Viewport.anchoredPosition = new Vector2(rt_Viewport_Sender.anchoredPosition.x + txt_Sender.preferredWidth, rt_Viewport.anchoredPosition.y);
                    rt_Viewport.sizeDelta = new Vector2(rt_Viewport_Sender.sizeDelta.x - txt_Sender.preferredWidth, rt_Viewport.sizeDelta.y);

                    float tSender = (rt_Viewport_Sender.rect.width) / Sys_Chat.Instance.nHornPlaySpeed;
                    float tContent = (txt_Sender.preferredWidth + txt_Content.preferredWidth + rt_Viewport.rect.width) / Sys_Chat.Instance.nHornPlaySpeed;                    

                    txt_Sender.rectTransform.localPosition = new Vector3(rt_Viewport_Sender.rect.width, txt_Sender.rectTransform.localPosition.y, txt_Sender.rectTransform.localPosition.z);
                    txt_Content.rectTransform.localPosition = new Vector3(rt_Viewport.rect.width + txt_Sender.preferredWidth, txt_Content.rectTransform.localPosition.y, txt_Content.rectTransform.localPosition.z);

                    txt_Sender.rectTransform.DOLocalMoveX(0, tSender, true).SetEase(Ease.Linear);

                    txt_Content.rectTransform.DOLocalMoveX(-(txt_Content.preferredWidth), tContent, true).SetEase(Ease.Linear).onComplete = HUDEnd;
                }
            }        
        }

        private void HUDEnd()
        {
            has = false;
            ++nCurTimes;
            if (nCurTimes >= nCurNeedTimes)
            {
                Sys_Chat.Instance.RemoveChatContentReference(curContent, Sys_Chat.EChatContentReference.HornHUD);
                curContent = null;
                if (Sys_Chat.Instance.mChatHUD.Count <= 0)
                {
                    mAnimatorRoot.SetActive(false);
                    ImageHelper.SetIcon(mImgBg, null);
                }
            }
        }
    }
}