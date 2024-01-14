using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

namespace Logic
{
    public class UI_Qa : UIBase
    {
        private ScrollViewAttach scrollViewAttach;
        private CP_PageDot cP_PageDot;
        private Transform parent;
        private Transform content;
        private int line;
        private int dataCount;
        private int extra;
        private Button closeButton;
        private Animator animator;

        protected override void OnLoaded()
        {
            scrollViewAttach = transform.Find("Animator/View_Map/Scroll View").gameObject.AddComponent<ScrollViewAttach>().SetVaild(true);
            scrollViewAttach.BindGameObject(transform.Find("Animator/View_Map/Scroll View").gameObject);
            scrollViewAttach.BindEventListener(OnGridSelect, OnGridCreated, OnDragLeft, OnDragRight);
            scrollViewAttach.SetScale(1, 1);
            parent = transform.Find("Animator/View_Map/View_Map01/View_Island/Grid");
            content = transform.Find("Animator/View_Map/Scroll View/Viewport/Content");
            cP_PageDot = transform.Find("Animator/View_Map/View_Bottom").GetComponent<CP_PageDot>();
            closeButton = transform.Find("Animator/View_Title/Btn_Close").GetComponent<Button>();
            animator = transform.Find("Animator/View_Map").GetComponent<Animator>();


            closeButton.onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_Qa);
            });
            line = int.Parse(CSVParam.Instance.GetConfData(820).str_value);
        }

        protected override void OnShow()
        {
            Create();
        }

        private void Create()
        {
            dataCount = Sys_Qa.Instance.Questionnaires.Count / line;
            extra = Sys_Qa.Instance.Questionnaires.Count % line > 0 ? 1 : 0;
            dataCount += extra;
            scrollViewAttach.SetData(dataCount);
            cP_PageDot.SetMax(dataCount);
            cP_PageDot.Build();
            scrollViewAttach.Attach(0);
        }


        private void OnGridSelect(int index)
        {
            cP_PageDot.SetSelected(index);
        }


        private void OnGridCreated(int index)
        {
            Transform parent = content.GetChild(index).Find("GameObject");
            //int needCount = index == dataCount - 1 ? Sys_Qa.Instance.Questionnaires.Count % line : line;
            //index为页数，Count一定大于0 一页最多显示line个
            int needCount = 0;
            if (Sys_Qa.Instance.Questionnaires.Count % line == 0)
            {
                needCount = line;
            }
            else
            {
                needCount = index == dataCount - 1 ? Sys_Qa.Instance.Questionnaires.Count % line : line;
            }
            //int needCount = Sys_Qa.Instance.Questionnaires.Count >= line ? line : Sys_Qa.Instance.Questionnaires.Count;
            FrameworkTool.CreateChildList(parent, needCount);
            for (int i = 0; i < needCount; i++)
            {
                GameObject gameObject = parent.GetChild(i).gameObject;
                Text tips = gameObject.transform.Find("Text_Tips").GetComponent<Text>();
                Text name = gameObject.transform.Find("Text_Name").GetComponent<Text>();
                uint qa_id = Sys_Qa.Instance.Questionnaires[index * line + i];
                CSVQuestionnaire.Data cSVQuestionnaireData = CSVQuestionnaire.Instance.GetConfData(qa_id);
                TextHelper.SetText(tips, LanguageHelper.GetTextContent(cSVQuestionnaireData.tips));
                TextHelper.SetText(name, LanguageHelper.GetTextContent(cSVQuestionnaireData.title));
                CSVMail.Data cSVMailData = CSVMail.Instance.GetConfData(cSVQuestionnaireData.MailId);
                int _needCount = cSVMailData.Enclosure.Count;

                Transform itemParent = gameObject.transform.Find("GameObject/Grid").transform;
                FrameworkTool.CreateChildList(itemParent, _needCount);
                for (int j = 0; j < _needCount; j++)
                {
                    Transform transform = itemParent.GetChild(j);
                    Image icon = transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();
                    Image mQuality = transform.Find("Btn_Item/Image_BG").GetComponent<Image>();
                    CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData((uint)cSVMailData.Enclosure[j][0]);
                    ImageHelper.SetIcon(icon, cSVItemData.icon_id);
                    ImageHelper.GetQualityColor_Frame(mQuality, (int)cSVItemData.quality);

                    Text text = transform.Find("Text_Number").GetComponent<Text>();
                    text.gameObject.SetActive(true);
                    text.text = string.Format($"x{(uint)cSVMailData.Enclosure[j][1]}");

                    Button button = transform.Find("Btn_Item").GetComponent<Button>();

                    button.onClick.AddListener(() =>
                    {
                        UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Chat, new PropIconLoader.ShowItemData(cSVItemData.id, 0, true, false, false, false, false)));
                    });
                }

                Button reqButton = gameObject.transform.Find("Btn_01_Small").GetComponent<Button>();
                reqButton.onClick.AddListener(() =>
                {
                    string url1 = cSVQuestionnaireData.QuestionURL;
                    string url2 = string.Format("?sojumpparm={0};{1};{2};{3}&userid={4}", Sys_Role.Instance.RoleId, cSVQuestionnaireData.id,
                    Sys_Login.Instance.selectedServerId, SDKManager.GetUID(), SDKManager.GetUID());
                    SDKManager.SDKOpenH5Questionnaire(url1 + url2);
                    DebugUtil.Log(ELogType.eQa, url1 + url2);
                    UIManager.CloseUI(EUIID.UI_Qa, false);
                });
            }
        }

        private void OnDragLeft()
        {
            animator.enabled = true;
            animator.Play("UI_Qa_Animator_View_Map_Left_Open", -1, 0);
        }

        private void OnDragRight()
        {
            animator.enabled = true;
            animator.Play("UI_Qa_Animator_View_Map_Left_Open", -1, 0);
        }

    }
}


