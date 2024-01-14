using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;
using Packet;
using Lib.AssetLoader;
using DG.Tweening;

namespace Logic
{
    public class UI_CommonTip : UIBase 
    {
        private GameObject aniGo;

        private Button btn;
        private Image imgIcon;
        private Text textTitle;
        private Text textContent;
        private Transform achievementItem;

        private CSVPromptBox.Data _data;
        private Timer _timer;

        private Sys_CommonTip.CommonTipParam tipParam;

        AchievementIconCell achievementIconCell;
        protected override void OnLoaded()
        {
            aniGo = transform.Find("Animator").gameObject;

            btn = transform.Find("Animator/Image_BG2/Image_BG").GetComponent<Button>();
            imgIcon = transform.Find("Animator/Image_Bottom/Image_Icon").GetComponent<Image>();
            textTitle = transform.Find("Animator/Text_Title").GetComponent<Text>();
            textContent = transform.Find("Animator/Text").GetComponent<Text>();

            achievementItem = transform.Find("Animator/Image_Bottom/Achievement_Item");
            achievementIconCell = new AchievementIconCell();
            achievementIconCell.Init(achievementItem);
            achievementItem.gameObject.SetActive(false);

            btn.onClick.AddListener(OnClick);
        }

        protected override void OnOpen(object arg)
        {
            tipParam = null;
            if (arg != null)
                tipParam = arg as Sys_CommonTip.CommonTipParam;
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        protected override void OnHide()
        {

        }

        protected override void OnDestroy()
        {
            StopTimer();
        }

        private void UpdateInfo()
        {
            StopTimer();
            if (tipParam.tipId == 1008 && !Sys_Achievement.Instance.isCanShow)
                aniGo.SetActive(false);
            else
                aniGo.SetActive(true);
            _data = CSVPromptBox.Instance.GetConfData(tipParam.tipId);
            if (_data != null)
            {
                uint titleId = _data.Title;
                if (tipParam.tipId == 1008)
                {
                    imgIcon.gameObject.SetActive(false);
                    achievementItem.gameObject.SetActive(true);
                    AchievementDataCell dataCell = Sys_Achievement.Instance.GetAchievementByTid(tipParam.knowledgeId);
                    achievementIconCell.SetData(dataCell);
                    if (_data.Title == 0)
                        titleId = dataCell.csvAchievementData.Achievement_Title;
                }
                else
                {
                    imgIcon.gameObject.SetActive(true);
                    achievementItem.gameObject.SetActive(false);
                    ImageHelper.SetIcon(imgIcon, _data.Icon);
                }
                textTitle.text = LanguageHelper.GetTextContent(titleId);
                textContent.text = tipParam.strContent;

                _timer = Timer.Register(_data.Time, () =>
                {
                    aniGo.SetActive(false);
                    this.CloseSelf();
                });
            }
        }

        private void OnClick()
        {
            if (_data != null && _data.Jump)
            {
                if (tipParam.tipId == 1008)
                {
                    if (!Sys_FunctionOpen.Instance.IsOpen(22090, true))
                        return;
                    UIManager.OpenUI(EUIID.UI_Achievement);
                    OpenAchievementMenuParam param = new OpenAchievementMenuParam()
                    {
                        tid = tipParam.knowledgeId,
                    };
                    UIManager.OpenUI((EUIID)_data.UIId, false, param);
                }
                else
                    UIManager.OpenUI((EUIID)_data.UIId);
            }

            this.CloseSelf();
        }

        private void StopTimer()
        {
            aniGo?.SetActive(false);
            _timer?.Cancel();
            _timer = null;
        }
    }
}


