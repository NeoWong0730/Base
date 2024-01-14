using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;
using Table;
using Lib.Core;

namespace Logic
{
    public class CommonCoursePrama
    {
        public uint courseId;
        public uint FirstTitleId;
        public uint SecondTitleId;
    }
    public class UI_CommonCourse : UIBase
    {
        private uint courseId = 0;    //教程表ID
        private uint selectFirstTileId = 0;   
        private uint selectSecondTileId = 0;
        private string strInputValue = "";

        private Button btnClose;
        private Text txtTitle;
        private Text txtSecondTitle;
        private Text txtDesc;
        private RawImage imgPage;
        private Button btnLeft;
        private Button btnRight;
        private GameObject goPointParent;
        private GameObject goMenuParent;
        private GameObject goFirstMenu;
        private GameObject goSecondMenu;
        private InputField inputText;
        private Button btnClearInput;
        private GameObject goRightView;
        private GameObject goNone;
        private Text txtNoneText;

        private List<CSVTutorialFirstHeading.Data>  listFirstTitles;
        private List<UI_CommonCourse_FirstMenuCell> listFirstMenus = new List<UI_CommonCourse_FirstMenuCell>();
        private List<uint> listPage;
        private int pageIndex = 0; //页签下标
        private List<UI_CommonCourse_PagePointCell> listPagePoint = new List<UI_CommonCourse_PagePointCell>();
        #region 系统函数

        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                if (arg.GetType() == typeof(CommonCoursePrama))
                {
                    CommonCoursePrama prama = arg as CommonCoursePrama;

                    courseId = prama.courseId;
                    selectFirstTileId = prama.FirstTitleId;
                    selectSecondTileId = prama.SecondTitleId;
                }
                else if (arg.GetType() == typeof(Tuple<uint, object>))
                {
                    Tuple<uint, object> tuple = arg as Tuple<uint, object>;
                    if (tuple != null)
                    {
                        courseId = tuple.Item2 == null ? 0 : Convert.ToUInt32(tuple.Item2);
                    }
                }
                else if (arg.GetType() == typeof(uint))
                {
                    courseId = (uint)arg;
                }
            }
            Sys_CommonCourse.Instance.ParseCourseData(courseId);
        }
        protected override void OnLoaded()
        {
            Parse();
        }
        protected override void OnShow()
        {
            UpdateView();
        }
        protected override void OnDestroy()
        {

        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {

        }
        #endregion

        #region func
        private void Parse()
        {
            btnClose = transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            txtTitle = transform.Find("Animator/View_Right/Text_Talk").GetComponent<Text>();
            txtSecondTitle = transform.Find("Animator/View_Right/Text_Name").GetComponent<Text>();
            txtDesc = transform.Find("Animator/View_Right/Text_Name/Text_Des").GetComponent<Text>();
            imgPage = transform.Find("Animator/View_Right/RawImage1").GetComponent<RawImage>();
            btnLeft = transform.Find("Animator/View_Right/Button_Left").GetComponent<Button>();
            btnLeft.onClick.AddListener(OnBtnLeftClick);
            btnRight = transform.Find("Animator/View_Right/Button_Right").GetComponent<Button>();
            btnRight.onClick.AddListener(OnBtnRightClick);
            goPointParent = transform.Find("Animator/View_Right/View_Bottom/Image/Grid").gameObject;
            goMenuParent = transform.Find("Animator/View_Left/Scroll View/Grid_Btn").gameObject;
            goFirstMenu = transform.Find("Animator/View_Left/Scroll View/Grid_Btn/WIdget_Menu_Big").gameObject;
            goFirstMenu.SetActive(false);
            goSecondMenu = transform.Find("Animator/View_Left/Scroll View/Grid_Btn/WIdget_Menu_Small").gameObject;
            goSecondMenu.SetActive(false);
            inputText = transform.Find("Animator/View_Left/Button").GetComponent<InputField>();
            inputText.onValueChanged.AddListener(OnInputValueChanged);
            btnClearInput = transform.Find("Animator/View_Left/Button/Button_Delete").GetComponent<Button>();
            btnClearInput.onClick.AddListener(OnBtnClearInputClick);
            goRightView = transform.Find("Animator/View_Right").gameObject;
            goNone = transform.Find("Animator/View_None").gameObject;
            txtNoneText = transform.Find("Animator/View_None/View_Npc022/View_Npc02_frame/Text_frame").GetComponent<Text>();
            InitLeftMenuView();
        }
        private void InitLeftMenuView()
        {
            listFirstMenus.Clear();
            listFirstTitles = Sys_CommonCourse.Instance.GetFirstHeadingList(courseId);
            if(listFirstTitles!= null)
            {
                for (int i = 0; i < listFirstTitles.Count; i++)
                {
                    var firstTitleData = listFirstTitles[i];
                    GameObject goFirst = FrameworkTool.CreateGameObject(goFirstMenu, goMenuParent);
                    goFirst.SetActive(true);
                    UI_CommonCourse_FirstMenuCell firstCell = new UI_CommonCourse_FirstMenuCell();
                    firstCell.Init(goFirst.transform);
                    firstCell.RegisterAction(OnMenuCellClick);
                    firstCell.UpdateData(firstTitleData);
                    listFirstMenus.Add(firstCell);
                    var listSecondTitles = Sys_CommonCourse.Instance.GetSecondHeadingList(firstTitleData.id);
                    if (listSecondTitles.Count > 1)
                    {
                        for (int j = 0; j < listSecondTitles.Count; j++)
                        {
                            var SecondTitleData = listSecondTitles[j];
                            GameObject goSecond = FrameworkTool.CreateGameObject(goSecondMenu, goMenuParent);
                            //goSecond.SetActive(true);
                            UI_CommonCourse_SecondMenuCell secondCell = new UI_CommonCourse_SecondMenuCell();
                            secondCell.Init(goSecond.transform);
                            secondCell.RegisterAction(firstCell.OnChildCellClick);
                            secondCell.UpdateData(SecondTitleData);
                            firstCell.AddSecondMenu(secondCell);
                        }
                    }
                }
            }
        }
        private void UpdateView()
        {
            bool hasKeyword = strInputValue.Length > 0;
            if (!hasKeyword)
            {
                if (selectFirstTileId == 0)
                {
                    selectFirstTileId = listFirstTitles[0].id;
                }
                if (selectSecondTileId == 0)
                {
                    var listSecondTitleId = Sys_CommonCourse.Instance.GetSecondHeadingList(selectFirstTileId);
                    selectSecondTileId = listSecondTitleId[0].id;
                }
            }
            for (int i = 0; i < listFirstMenus.Count; i++)
            {
                listFirstMenus[i].UpdateView(selectFirstTileId, selectSecondTileId, strInputValue);
            }
            UpdateRightView(selectFirstTileId, selectSecondTileId);
        }

        private void UpdateRightView(uint _firstId, uint _secondId)
        {
            if (_firstId > 0)
            {
                goNone.SetActive(false);
                goRightView.SetActive(true);
                CSVTutorial.Data tutorialData = CSVTutorial.Instance.GetConfData(courseId);
                if (tutorialData != null)
                {
                    txtTitle.text = LanguageHelper.GetTextContent(tutorialData.title);
                }
                uint secondId = Sys_CommonCourse.Instance.GetSecondTitleID(_firstId, _secondId);
                CSVTutorialSecondHeading.Data secondData = CSVTutorialSecondHeading.Instance.GetConfData(secondId);
                if (secondData != null)
                {
                    txtSecondTitle.text = LanguageHelper.GetTextContent(secondData.secondHeadingName);
                    listPage = secondData.tutorial_array;
                    FrameworkTool.CreateChildList(goPointParent.transform, listPage.Count);
                    listPagePoint.Clear();
                    for (int i = 0; i < listPage.Count; i++)
                    {
                        UI_CommonCourse_PagePointCell pointCell = new UI_CommonCourse_PagePointCell();
                        Transform child = goPointParent.transform.GetChild(i);
                        pointCell.Init(child);
                        pointCell.SetIndex(i);
                        listPagePoint.Add(pointCell);
                    }
                    pageIndex = 0;
                    UpdateRightPage();
                }
            }
            else
            {
                goNone.SetActive(true);
                goRightView.SetActive(false);
                txtNoneText.text = LanguageHelper.GetTextContent(3219000002, strInputValue);
            }
        }
        private void UpdateRightPage()
        {
            uint pageId = listPage[pageIndex];
            CSVTutorialDetails.Data pageData = CSVTutorialDetails.Instance.GetConfData(pageId);
            if (pageData != null)
            {
                txtDesc.text = LanguageHelper.GetTextContent(pageData.tutorial_text);
                //Debug.Log("CommonCourse imgPath " + pageData.tutorial_pic);
                ImageHelper.SetTexture(imgPage, pageData.tutorial_pic);
            }
            //检测是否是第一个page
            bool isFirstPage = pageIndex == 0 && Sys_CommonCourse.Instance.CheckIsFirstTitle(courseId, selectFirstTileId, selectSecondTileId);
            btnLeft.interactable = !isFirstPage;
            ImageHelper.SetImageGray(btnLeft.GetComponent<Image>(), isFirstPage);
            //检测是否是最后一个page
            bool isLastPage = pageIndex == listPage.Count - 1 && Sys_CommonCourse.Instance.CheckIsLastTitle(courseId, selectFirstTileId, selectSecondTileId);
            btnRight.interactable = !isLastPage;
            ImageHelper.SetImageGray(btnRight.GetComponent<Image>(), isLastPage);
            for (int i = 0; i < listPagePoint.Count; i++)
            {
                listPagePoint[i].SetSelected(pageIndex);
            }
        }
        #endregion

        #region event
        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }

        private void OnMenuCellClick(uint firstMenuId, uint secondMenuId = 0)
        {
            selectFirstTileId = firstMenuId;
            selectSecondTileId = secondMenuId;
            UpdateView();
        }
        private void OnBtnLeftClick()
        {
            if(pageIndex > 0)
            {
                pageIndex--;
                UpdateRightPage();
            }
            else
            {
                //选择上一个标题
                var param = Sys_CommonCourse.Instance.GetFrontTitleInfo(courseId, selectFirstTileId, selectSecondTileId, strInputValue);
                selectFirstTileId = param.FirstTitleId;
                selectSecondTileId = param.SecondTitleId;
                UpdateView();
            }
        }
        private void OnBtnRightClick()
        {
            if (pageIndex < listPage.Count - 1)
            {
                pageIndex++;
                UpdateRightPage();
            }
            else
            {
                //选择下一个标题
                var param = Sys_CommonCourse.Instance.GetAfterTitleInfo(courseId, selectFirstTileId, selectSecondTileId, strInputValue);
                selectFirstTileId = param.FirstTitleId;
                selectSecondTileId = param.SecondTitleId;
                UpdateView();
            }
        }
        private void OnInputValueChanged(string str)
        {
            strInputValue = str;
            var validParam = Sys_CommonCourse.Instance.GetValidTitleInfo(courseId, selectFirstTileId, selectSecondTileId, strInputValue);
            //Debug.Log("FirstTitleId" + selectFirstTileId + "->" + validParam.FirstTitleId + " SecondTitleId" + selectSecondTileId + "->" + validParam.SecondTitleId);
            selectFirstTileId = validParam.FirstTitleId;
            selectSecondTileId = validParam.SecondTitleId;
            UpdateView();
        }
        private void OnBtnClearInputClick()
        {
            //清空input内容
            inputText.text = "";
            UpdateView();
        }
        #endregion

        #region class
        public class UI_CommonCourse_FirstMenuCell
        {
            private Transform transform;
            private Text txtDarkTitle;
            private Text txtLightTitle;
            private GameObject goSelect;
            private GameObject goArrowRight;
            private GameObject goArrowDown;
            private Button btnSelf;

            private CSVTutorialFirstHeading.Data csvData;
            private List<UI_CommonCourse_SecondMenuCell> secondList = new List<UI_CommonCourse_SecondMenuCell>();

            private Action<uint, uint> action;
            public void Init(Transform trans)
            {
                transform = trans;
                txtDarkTitle = transform.Find("Text_Menu_Dark").GetComponent<Text>();
                txtLightTitle = transform.Find("Text_Menu_Light").GetComponent<Text>();
                goSelect = transform.Find("Btn_Menu").gameObject;
                goArrowRight = transform.Find("Image2").gameObject;
                goArrowDown = transform.Find("Image").gameObject;
                btnSelf = transform.GetComponent<Button>();
                btnSelf.onClick.AddListener(OnBtnSelfClick); 
                secondList.Clear();
            }
            public void UpdateData(CSVTutorialFirstHeading.Data data)
            {
                csvData = data;
            }
            public void UpdateView(uint selectedFristId, uint selecedSecondId, string keyWord = "")
            {
                bool isSelectedSelf = selectedFristId == csvData.id;
                bool hasKeyword = keyWord.Length > 0;
                var secondTitleList = Sys_CommonCourse.Instance.GetSecondHeadingList(csvData.id);
                if (isSelectedSelf && selecedSecondId == 0)
                {
                    selecedSecondId = secondTitleList[0].id;
                }
                bool isAcrive = selectedFristId > 0;
                transform.gameObject.SetActive(isAcrive);
                if (isAcrive)
                {
                    string txtValue = LanguageHelper.GetTextContent(csvData.firstHeadingName);
                    bool containsKeyword = txtValue.Contains(keyWord);
                    if (hasKeyword && containsKeyword)
                    {
                        txtValue = Sys_CommonCourse.Instance.GetHightLightText(txtValue, keyWord);
                    }
                    txtDarkTitle.text = txtValue;
                    txtLightTitle.text = txtValue;
                    bool showArrow = secondList.Count > 0;
                    SetSelectState(isSelectedSelf, showArrow);
                }
                for (int i = 0; i < secondList.Count; i++)
                {
                    var secondCell = secondList[i];
                    secondCell.UpdateView(isSelectedSelf, selecedSecondId, keyWord);
                }
            }
            private void SetSelectState(bool isSelected, bool showArrow)
            {
                txtLightTitle.gameObject.SetActive(isSelected);
                goSelect.gameObject.SetActive(isSelected);
                goArrowRight.gameObject.SetActive(!isSelected && showArrow);
                goArrowDown.gameObject.SetActive(isSelected && showArrow);
            }
            public void AddSecondMenu(UI_CommonCourse_SecondMenuCell childMenu)
            {
                secondList.Add(childMenu);
            }
            public void RegisterAction(Action<uint, uint> act)
            {
                action = act;
            }
            private void OnBtnSelfClick()
            {
                UIManager.HitButton(EUIID.UI_CommonCourse, "FirstMenuClick_FirstId:" + csvData.id);
                action?.Invoke(csvData.id, (uint)0);
            }
            public void OnChildCellClick(uint secondId)
            {
                UIManager.HitButton(EUIID.UI_CommonCourse, "SecondMenuClick_FirstId:" + csvData.id + "|SecondId:" + secondId);
                action?.Invoke(csvData.id, secondId);
            }
        }
        public class UI_CommonCourse_SecondMenuCell
        {
            private Transform transform;
            private Text txtDarkTitle;
            private Text txtLightTitle;
            private GameObject goSelect;
            private Button btnSelf;

            private bool isActive;

            private CSVTutorialSecondHeading.Data csvData;
            private Action<uint> action;
            public void Init(Transform trans)
            {
                transform = trans;
                txtDarkTitle = transform.Find("Text_Menu_Dark").GetComponent<Text>();
                txtLightTitle = transform.Find("Text_Menu_Light").GetComponent<Text>();
                goSelect = transform.Find("Btn_Menu").gameObject;
                btnSelf = transform.GetComponent<Button>();
                btnSelf.onClick.AddListener(OnBtnSelfClick);
            }
            public void UpdateData(CSVTutorialSecondHeading.Data data)
            {
                csvData = data;
            }
            public void UpdateView(bool isParentSelected, uint secondId, string keyWord = "")
            {
                string txtValue = LanguageHelper.GetTextContent(csvData.secondHeadingName);
                bool hasKeyword = keyWord.Length > 0;
                bool containsKeyword = hasKeyword && txtValue.Contains(keyWord);
                isActive = isParentSelected || hasKeyword && containsKeyword;
                transform.gameObject.SetActive(isActive);
                if (isActive)
                {
                    if (containsKeyword)
                    {
                        txtValue = Sys_CommonCourse.Instance.GetHightLightText(txtValue, keyWord);
                    }
                    txtDarkTitle.text = txtValue;
                    txtLightTitle.text = txtValue;
                    bool isSelected = secondId == csvData.id;
                    SetSelectState(isSelected);
                }
            }
            public void SetSelectState(bool isSelected)
            {
                txtLightTitle.gameObject.SetActive(isSelected);
                goSelect.gameObject.SetActive(isSelected);
            }
            public void RegisterAction(Action<uint> act)
            {
                action = act;
            }
            private void OnBtnSelfClick()
            {
                action?.Invoke(csvData.id);
            }
        }

        public class UI_CommonCourse_PagePointCell
        {
            private GameObject goSelected;
            private int pageIndex;
            public void Init(Transform trans)
            {
                goSelected = trans.Find("Image_Select").gameObject;
            }

            public void SetIndex(int index)
            {
                pageIndex = index;
            }
            public void SetSelected(int selectIndex)
            {
                bool isSelected = selectIndex == pageIndex;
                goSelected.SetActive(isSelected);
            }
        }
        #endregion
    }
}
