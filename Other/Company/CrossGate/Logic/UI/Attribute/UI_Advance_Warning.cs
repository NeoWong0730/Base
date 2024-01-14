using Logic.Core;
using Table;
using Lib.Core;
using UnityEngine.UI;
using UnityEngine;
using System;

namespace Logic
{
    public class UI_Advance_Warning : UIBase
    {
        private GameObject warningGo;
        private GameObject messageGo;
        private Timer timer;
        private Button sureBtn;

        private uint time;
        private uint curPromoteCareerId;
        private bool isAdvance;
        private Timer timerAnimator01;
        private Timer timerAnimator02;
        private Animator animator01;
        private Animator animator02;

        const int PageNum = 4;
        private GameObject[] go_PageContent = new GameObject[PageNum];
        private CP_PageDot m_CP_PageDot;
        private Button m_PageLeft;
        private Button m_PageRight;
        private Text page02_advanceNumT, page02_PVPScoreT, page02_curScoreT, page02_text4, page02_text5;
        private Text page03_PVEScoreT, page03_curScoreT, page03_text3;
        private Text page04_LevelT;
        private GameObject image01;
        private GameObject image02;
        private GameObject image01Page03;
        private GameObject image02Page03;

        protected override void OnOpen(object arg)
        {
            isAdvance =(bool) arg ;
        }

        protected override void OnLoaded()
        {
            warningGo = transform.Find("Animator/View_Warning01").gameObject;
            messageGo = transform.Find("Animator/View_Warning02").gameObject;
            animator01 = transform.Find("Animator/View_Warning01").GetComponent<Animator>();
            animator02 = transform.Find("Animator/View_Warning02").GetComponent<Animator>();
            sureBtn = transform.Find("Animator/View_Warning02/Btn_01").GetComponent<Button>();

            m_CP_PageDot = transform.Find("Animator/View_Warning02/Grid").GetComponent<CP_PageDot>();
            if (m_CP_PageDot == null)
            {
                m_CP_PageDot = transform.Find("Animator/View_Warning02/Grid").gameObject.AddComponent<CP_PageDot>();
                m_CP_PageDot.parent = m_CP_PageDot.transform;
                m_CP_PageDot.proto = m_CP_PageDot.transform.GetChild(0).gameObject;
            }
            m_PageRight = transform.Find("Animator/View_Warning02/Arrow_Left/Button_Left").GetComponent<Button>();
            m_PageLeft = transform.Find("Animator/View_Warning02/Arrow_Right/Button_Right").GetComponent<Button>();
            for (int i = 0; i < PageNum; i++)
            {
                go_PageContent[i] = transform.Find(string.Format("Animator/View_Warning02/Page0{0}", i + 1)).gameObject;
                go_PageContent[i].SetActive(false);
            }

            page02_advanceNumT = transform.Find("Animator/View_Warning02/Page02/Text1").GetComponent<Text>();
            page02_PVPScoreT = transform.Find("Animator/View_Warning02/Page02/Text2").GetComponent<Text>();
            page02_curScoreT = transform.Find("Animator/View_Warning02/Page02/Text3").GetComponent<Text>();
            page02_text4 = transform.Find("Animator/View_Warning02/Page02/Text4").GetComponent<Text>();
            page02_text5 = transform.Find("Animator/View_Warning02/Page02/Text5").GetComponent<Text>();

            page03_PVEScoreT = transform.Find("Animator/View_Warning02/Page03/Text1").GetComponent<Text>();
            page03_curScoreT = transform.Find("Animator/View_Warning02/Page03/Text2").GetComponent<Text>();
            page03_text3 = transform.Find("Animator/View_Warning02/Page03/Text3").GetComponent<Text>();
            page04_LevelT = transform.Find("Animator/View_Warning02/Page04/Text1").GetComponent<Text>();

            image01 = transform.Find("Animator/View_Warning02/Page01/Image").gameObject;
            image02 = transform.Find("Animator/View_Warning02/Page01/Image1").gameObject;
            image01Page03 = transform.Find("Animator/View_Warning02/Page03/Image").gameObject;
            image02Page03 = transform.Find("Animator/View_Warning02/Page03/Image1").gameObject;

            sureBtn.onClick.AddListener(OnSureClicked);
            m_PageLeft.onClick.AddListener(OnPageLeftClick);
            m_PageRight.onClick.AddListener(OnPageRightClick);
        }

        private void OnSureClicked()
        {
            if (isAdvance)
            {
                UIManager.OpenUI(EUIID.UI_Advance_Tips);
            }
            UIManager.CloseUI(EUIID.UI_Advance_Warning);
        }

        protected override void OnShow()
        {
            curPromoteCareerId = Sys_Advance.Instance.SetAdvanceRank();
            OnUpdatePageContent();
            messageGo.SetActive(false);
            warningGo.SetActive(true);
            if (!CSVPromoteCareer.Instance.ContainsKey(curPromoteCareerId))
            {
                Debug.LogError("进阶表不存在key：" + curPromoteCareerId);
                return;
            }
            timer?.Cancel();
            timerAnimator01?.Cancel();
            uint.TryParse(CSVParam.Instance.GetConfData(814).str_value, out time);
            timer = Timer.Register(time/1000, () =>
            {
                animator01.enabled = true;
                animator01.Play("Close", -1, 0);
                timer.Cancel();
                timerAnimator01 = Timer.Register(0.26f, () =>
                {
                    messageGo.SetActive(true);
                    warningGo.SetActive(false);
                    timerAnimator01.Cancel();
                }, null, false, false);
            }, null, false, false);    
    }

        protected override void OnHide()
        {
            timer?.Cancel();
            timerAnimator01?.Cancel();
            timerAnimator02?.Cancel();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Advance.Instance.eventEmitter.Handle(Sys_Advance.EEvents.OnUpdateCareerUpInfoEvent, OnUpdateCareerUpInfo, toRegister);
        }

        private void OnUpdateCareerUpInfo()
        {
            CSVPromoteCareer.Instance.TryGetValue(curPromoteCareerId + 1, out CSVPromoteCareer.Data csvPromoteCareerNext);
            if (Sys_Advance.Instance.careerUpData != null&& csvPromoteCareerNext!=null)
            {
                page02_advanceNumT.text = Sys_Advance.Instance.careerUpData.Count.ToString();
                uint averagePowerAdd = Sys_Advance.Instance.careerUpData.Count == 0 ? 0 : (uint)csvPromoteCareerNext.pvpAddPoint;
                page02_PVPScoreT.text = (Sys_Advance.Instance.careerUpData.AveragePower + averagePowerAdd).ToString();
            }
        }

        private void OnUpdatePageContent()
        {
            CSVPromoteCareer.Instance.TryGetValue(curPromoteCareerId + 1, out CSVPromoteCareer.Data csvPromoteCareerNext);
            page02_advanceNumT.text = page02_PVPScoreT.text = "";
            if (csvPromoteCareerNext != null)
                Sys_Role.Instance.CareerUpInfoReq((curPromoteCareerId + 1) % 10);
            page02_curScoreT.text = Sys_Attr.Instance.power.ToString();
            page03_PVEScoreT.text = csvPromoteCareerNext?.pvePoint.ToString();
            page03_curScoreT.text = Sys_Attr.Instance.power.ToString();
            page04_LevelT.text = Sys_Advance.Instance.GetCurLimiteLevel().ToString();

            string msg = LanguageHelper.GetTextContent(2005051, LanguageHelper.GetTextContent((uint)(10127 + Sys_Role.Instance.Role.CareerRank+1)));
            page02_text4.text = page02_text5.text = page03_text3.text = msg;

            m_CP_PageDot.SetMax(PageNum);
            m_CP_PageDot.Build();
            UpdatePage();
        }
        #region Page

        private int curPage = 1;
        private int beforePage = 1;
        private void OnPageLeftClick()
        {
            curPage--;
            if (curPage < 1)
            {
                curPage = 1;
            }
            UpdatePage();
        }

        private void OnPageRightClick()
        {
            curPage++;
            if (curPage > Sys_Cooking.Instance.maxCookingLevel)
            {
                curPage = Sys_Cooking.Instance.maxCookingLevel;
            }
            UpdatePage();
        }

        private void UpdatePage()
        {
            m_CP_PageDot.SetSelected(curPage - 1);

            if (curPage == 1)
            {
                m_PageLeft.gameObject.SetActive(false);
                m_PageRight.gameObject.SetActive(true);
            }
            else if (curPage == PageNum)
            {
                m_PageRight.gameObject.SetActive(false);
                m_PageLeft.gameObject.SetActive(true);
            }
            else
            {
                m_PageLeft.gameObject.SetActive(true);
                m_PageRight.gameObject.SetActive(true);
            }

            if (beforePage != 0 && beforePage != curPage)
                go_PageContent[beforePage - 1].SetActive(false);
            go_PageContent[curPage - 1].SetActive(true);

            uint nextRank = Sys_Role.Instance.Role.CareerRank + 1;
            //区分59和79的进阶内容
            if (curPage == 1 )
            {
                image01.SetActive(nextRank != 3);
                image02.SetActive(nextRank == 3);
            }
            else if (curPage == 3)
            { 
                image01Page03.SetActive(nextRank != 3);
                image02Page03.SetActive(nextRank == 3);
            }
            beforePage = curPage;

            ImageHelper.SetImageGray(sureBtn.GetComponent<Image>(), curPage != PageNum);
            sureBtn.enabled = curPage == PageNum;
        }
        #endregion
    }
}
