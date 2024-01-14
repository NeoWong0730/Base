using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using System.Text;
using System;
using System.Linq;

namespace Logic
{
    public class UI_Awaken_Layout
    {
        public Button btnClose;
        public Button awakenBtn;
        public GameObject redPoint;
        public Image awakenBtnImage;
        public Text currTitle;
        public Text nextTitle;
        public Text currStep;
        public Text nextStep;
        public GameObject allAttrPanel;
        public GameObject awakeTargetGo;
        public GameObject currPanel;
        public GameObject nextPanel;
        public GameObject currAttrGo;
        public GameObject nextAttrGo;
        public GameObject currAwardPanel;
        public GameObject nextAwardPanel;
        public GameObject currAwardGO;
        public GameObject nextAwardGO;
        public GameObject targetPanel;
        public GameObject ConditionPanel;
        public GameObject MaxLevel;
        public Animator animImage;

        public Button awakenMarkBtn;

        public void Init(Transform transform)
        {
            btnClose = transform.Find("Animator/View_Title08/Btn_Close").GetComponent<Button>();
            awakenBtn = transform.Find("Animator/View_Content/Condition/Btn_05").GetComponent<Button>();
            redPoint= transform.Find("Animator/View_Content/Condition/Btn_05/Image").gameObject;
            awakenBtnImage = transform.Find("Animator/View_Content/Condition/Btn_05").GetComponent<Image>();
            currTitle = transform.Find("Animator/View_Content/Des/Current/Text1").GetComponent<Text>();
            nextTitle = transform.Find("Animator/View_Content/Des/Next/Text1").GetComponent<Text>();
            currStep = transform.Find("Animator/View_Content/Des/Current/Text1/Text2").GetComponent<Text>();
            nextStep = transform.Find("Animator/View_Content/Des/Next/Text1/Text2").GetComponent<Text>();
            currAttrGo = transform.Find("Animator/View_Content/Des/Current/View_Des/Text").gameObject;
            nextAttrGo = transform.Find("Animator/View_Content/Des/Next/View_Des/Text").gameObject;
            currPanel = transform.Find("Animator/View_Content/Des/Current/View_Des").gameObject;
            currAwardPanel = transform.Find("Animator/View_Content/Des/Current/View_Des/Award").gameObject;
            nextAwardPanel = transform.Find("Animator/View_Content/Des/Next/View_Des/Award").gameObject;
            currAwardGO = transform.Find("Animator/View_Content/Des/Current/View_Des/Award/Item").gameObject;
            nextAwardGO = transform.Find("Animator/View_Content/Des/Next/View_Des/Award/Item").gameObject;
            allAttrPanel = transform.Find("Animator/View_Content/Des").gameObject;
            targetPanel = transform.Find("Animator/View_Content/Condition/Scroll_View/Viewport/Content").gameObject;
            awakeTargetGo = transform.Find("Animator/View_Content/Condition/Item").gameObject;
            animImage = transform.Find("Animator/View_Content").GetComponent<Animator>();
            nextPanel = transform.Find("Animator/View_Content/Des/Next").gameObject;
            ConditionPanel = transform.Find("Animator/View_Content/Condition").gameObject;
            MaxLevel = transform.Find("Animator/View_Content/Text").gameObject;
            awakenMarkBtn=transform.Find("Animator/ImprintButton").GetComponent<Button>();


            currAwardGO.SetActive(false);
            nextAwardGO.SetActive(false);
            currAttrGo.SetActive(false);
            nextAttrGo.SetActive(false);
            currAwardPanel.SetActive(false);
            nextAwardPanel.SetActive(false);
        }
        public void RegisterEvents(IListener listener)
        {
            btnClose.onClick.AddListener(listener.OnCloseBtnClicked);
            awakenBtn.onClick.AddListener(listener.OnAwakenBtnClicked);
            awakenMarkBtn.onClick.AddListener(listener.OnAwakenMarkBtnClicked);
        }

        public interface IListener
        {
            void OnCloseBtnClicked();
            void OnAwakenBtnClicked();
            void OnAwakenMarkBtnClicked();
        }
    }

    public class UI_AwakeTargetItem:UIComponent
    {
        private uint id;
        public int type;
        public bool isFinish = false;
        public uint targetT;
        private CSVTravellerAwakening.Data csvAwakeData;
        private CSVAwakeningCondtion.Data csvAwakeCondition;
        #region 界面组件
        public Text targetName;
        public Text targetNum;
        private GameObject targetFinish;
        private GameObject targetUnFinish;
        public Button targetButton;

        public UI_AwakeTargetItem(uint _id,int _type) : base()
        {
            id = _id;
            type = _type;
        }

        #endregion

        #region 系统函数

        protected override void Loaded()
        {
            isFinish = false;
            targetName = transform.Find("Text").GetComponent<Text>();
            targetNum = transform.Find("Text_Level").GetComponent<Text>();                      
            targetFinish = transform.Find("Image_finish").gameObject;
            targetUnFinish = transform.Find("Text_unfinish").gameObject;
            targetButton = transform.Find("Image").GetComponent<Button>();
            targetButton.onClick.AddListener(OnTargetBtnClick);
        }

        #endregion

        #region Function
        public void SetValue()
        {//条件框内容
            csvAwakeData = CSVTravellerAwakening.Instance.GetConfData(id);
            csvAwakeCondition = CSVAwakeningCondtion.Instance.GetConfData(csvAwakeData.ActProject[type]);
            targetName.text = LanguageHelper.GetTextContent(csvAwakeCondition.descripvive_lag_Id);
            if (Sys_TravellerAwakening.Instance.awaketargetInfo.ContainsKey(csvAwakeData.ActProject[type]))
            {
                isFinish = Sys_TravellerAwakening.Instance.awaketargetInfo[csvAwakeData.ActProject[type]];
            }
            else
            {
                DebugUtil.LogError("The given key was not in the Dictionary!");
            }      
            uint targetParam = csvAwakeData.ActCondition[type][0];
            targetT = csvAwakeData.ActProject[type];
            switch (csvAwakeData.ActProject[type])
            {
                case 1:
                case 2:
                case 5:
                case 7:
                    targetNum.text = LanguageHelper.GetTextContent(csvAwakeCondition.Condition_lag_Id, targetParam.ToString());
                    break;
                case 3://完成任务
                    targetNum.text = CSVTaskLanguage.Instance.GetConfData(CSVTask.Instance.GetConfData(targetParam).taskName).words;
                    break;
                case 4:
                    uint layerStage = CSVInstanceDaily.Instance.GetConfData(targetParam).LayerStage;//副本关卡表
                    uint layerlevel = CSVInstanceDaily.Instance.GetConfData(targetParam).Layerlevel;
                    uint levelCount = (layerStage - 1) * 10 + layerlevel;
                    targetNum.text = LanguageHelper.GetTextContent(csvAwakeCondition.Condition_lag_Id, layerStage.ToString(), levelCount.ToString());
                    break;
                case 6://人物传记x章x关
                    targetNum.text = LanguageHelper.GetTextContent(csvAwakeCondition.Condition_lag_Id, Sys_TravellerAwakening.Instance.CharpterCaculate(CSVInstanceDaily.Instance.GetConfData(targetParam).InstanceId), CSVInstanceDaily.Instance.GetConfData(targetParam).Layerlevel.ToString());
                    break;
                case 9:
                    targetNum.text = LanguageHelper.GetTextContent(csvAwakeCondition.Condition_lag_Id, targetParam.ToString(), csvAwakeData.ActCondition[type][1].ToString());
                    break;
                case 8://黑色祈祷
                case 10:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                    targetNum.text = LanguageHelper.GetTextContent(csvAwakeCondition.Condition_lag_Id, csvAwakeData.ActCondition[type][csvAwakeData.ActCondition[type].Count - 1].ToString());
                    break;
                case 11:
                case 12:
                case 13:
                    targetNum.text = LanguageHelper.GetTextContent(csvAwakeCondition.Condition_lag_Id, targetParam.ToString());
                    break;
                default:
                    targetNum.gameObject.SetActive(false);
                    DebugUtil.LogError("Target does not exist!"); break;
            }

            targetFinish.SetActive(isFinish);
            targetUnFinish.SetActive(!isFinish);
        }
        #endregion

        #region ButtonClick

        public void OnTargetBtnClick()
        {
            if (isFinish)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11883));
            }
            else
            {
                Sys_TravellerAwakening.Instance.OpenWarning(targetT);
            }
        }
        #endregion
    }

    public class UI_Awaken : UIBase, UI_Awaken_Layout.IListener
    {
        public enum EAwakeType
        {
            curr = 1,
            next = 2,
        }
        private uint currAwakenId;
        private bool isAllTargetFin;
        private CSVTravellerAwakening.Data csvData;
        private List<UI_AwakeTargetItem> awakeTarget = new List<UI_AwakeTargetItem>();
        private Timer m_timer;
        private Timer a_timer;
        private Timer i_timer;
        private uint unFinType;
        private UI_Awaken_Layout layout = new UI_Awaken_Layout();
        private UI_Awaken_RedPoint imprint_redPoint;
        private int isOpenImprint=0;

        #region 系统函数
        protected override void OnOpen(object arg)
        {
            if (arg==null)
            {
                return;
            }
            else
            {
                isOpenImprint = (int)arg;
            }

        }
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            imprint_redPoint= gameObject.AddComponent<UI_Awaken_RedPoint>();
            imprint_redPoint?.Init(this);
        }

        protected override void OnShow()
        {            
            bool isopen = Sys_FunctionOpen.Instance.IsOpen(51402);
            layout.allAttrPanel.SetActive(true);
            layout.ConditionPanel.SetActive(true);
            layout.awakenMarkBtn.gameObject.SetActive(false);
            CheckImprintShow();
            DestoryPanel();
            TestValue();
            if (isOpenImprint==1)
            {
                UIManager.OpenUI(EUIID.UI_AwakenImprint);
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_TravellerAwakening.Instance.eventEmitter.Handle(Sys_TravellerAwakening.EEvents.OnTravellerAwakening, OnTravellerAwakening, toRegister);
            Sys_TravellerAwakening.Instance.eventEmitter.Handle(Sys_TravellerAwakening.EEvents.OnTripperAwakeUpdate, OnTripperAwakeUpdate, toRegister);
            Sys_TravellerAwakening.Instance.eventEmitter.Handle(Sys_TravellerAwakening.EEvents.OnAwakeImprintUpdate, OnAwakeImprintUpdate, toRegister);
            Sys_TravellerAwakening.Instance.eventEmitter.Handle(Sys_TravellerAwakening.EEvents.OnAwakeRedPoint, OnAwakeImprintUpdate, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, CheckImprintShow, toRegister);
        }

        protected override void OnHide()
        {            
            m_timer?.Cancel();
            a_timer?.Cancel();
            i_timer?.Cancel();


        }
        #endregion

        #region Function
        private void TestValue()
        {
            currAwakenId = Sys_TravellerAwakening.Instance.awakeLevel;
            csvData = CSVTravellerAwakening.Instance.GetConfData(currAwakenId);
            TitleShow(currAwakenId, layout.currAwardPanel, layout.currTitle, layout.currStep, EAwakeType.curr);//设置当前阶段属性加成
            TitleShow(currAwakenId + 1, layout.nextAwardPanel, layout.nextTitle, layout.nextStep, EAwakeType.next);//设置下一阶段属性加成 
            CheckTarget();

            
        }

        public void TitleShow(uint indexId, GameObject panelGo, Text nameText, Text stepText, EAwakeType eType)
        {
            CSVTravellerAwakening.Data csvAwakeData;
            if (CSVTravellerAwakening.Instance.TryGetValue(indexId, out csvAwakeData) && csvAwakeData != null)
            {
                layout.MaxLevel.SetActive(false);
                layout.nextPanel.SetActive(true);
                layout.ConditionPanel.SetActive(true);
                nameText.text = LanguageHelper.GetTextContent(csvAwakeData.NameId);
                if (eType == EAwakeType.curr)
                {
                    layout.currPanel.SetActive(true);
                    SetAttribute(csvAwakeData, eType);
                    if (csvAwakeData.ActCondition != null)
                    {
                        SetTarget(indexId);
                    }
                    else
                    {
                        layout.nextPanel.SetActive(false);
                        layout.ConditionPanel.SetActive(false);
                        layout.MaxLevel.SetActive(true);
                    }
                }
                else
                {
                    SetAttribute(csvAwakeData, eType);
                }
            }
            else
            {
                layout.nextPanel.SetActive(false);
                layout.ConditionPanel.SetActive(false);
                layout.MaxLevel.SetActive(true);
            }
            
        }
        private void SetAttribute(CSVTravellerAwakening.Data awakedata, EAwakeType eType)
        {
            StringBuilder str=new StringBuilder();

            if (eType == EAwakeType.curr)
            {
                for (int i = 0; i < awakedata.show_attr_name.Count; i++)
                {
                    str.Clear();
                    str.Append(LanguageHelper.GetTextContent(awakedata.show_attr_name[i])).Append(LanguageHelper.GetTextContent(2107003, (awakedata.show_attr_value[i] / 100).ToString()));
                    GameObject go = GameObject.Instantiate<GameObject>(layout.currAttrGo, layout.currAttrGo.transform.parent);
                    go.SetActive(true);
                    go.GetComponent<Text>().text = str.ToString();
                }
            }else
            {
                for (int i = 0; i < awakedata.show_attr_name.Count; i++)
                {
                    str.Clear();
                    str.Append(LanguageHelper.GetTextContent(awakedata.show_attr_name[i])).Append(LanguageHelper.GetTextContent(2107003, (awakedata.show_attr_value[i] / 100).ToString()));
                    GameObject go = GameObject.Instantiate<GameObject>(layout.nextAttrGo, layout.nextAttrGo.transform.parent);
                    go.SetActive(true);
                    go.GetComponent<Text>().text = str.ToString();
                }
            }
            
            if (awakedata.Award != null)
            {
                SetAward(awakedata, eType);
            }
            
        }
        public void SetAward(CSVTravellerAwakening.Data awakedata, EAwakeType eType)
        {
            GameObject panelGo;
            if (eType == EAwakeType.curr)
            {
                panelGo = GameObject.Instantiate<GameObject>(layout.currAwardPanel, layout.currAwardPanel.transform.parent);
                for (int i = 0; i < awakedata.Award.Count; i++)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(layout.currAwardGO, panelGo.transform);
                    go.SetActive(true);
                    PropItem propItem = new PropItem();
                    propItem.BindGameObject(go);
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_Awaken, new PropIconLoader.ShowItemData(awakedata.Award[i][0], awakedata.Award[i][1], true, false, false, false, false,
                            _bShowCount: true, _bUseClick: true, _bShowBagCount: false)));
                }


            }
            else
            {
                panelGo = GameObject.Instantiate<GameObject>(layout.nextAwardPanel, layout.nextAwardPanel.transform.parent);
                for (int i = 0; i < awakedata.Award.Count; i++)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(layout.nextAwardGO, panelGo.transform);
                    go.SetActive(true);
                    PropItem propItem = new PropItem();
                    propItem.BindGameObject(go);
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_Awaken, new PropIconLoader.ShowItemData(awakedata.Award[i][0], awakedata.Award[i][1], true, false, false, false, false,
                            _bShowCount: true, _bUseClick: true, _bShowBagCount: false)));
                }
            }
            panelGo.SetActive(true);
        }
        public void SetTarget(uint awakenId)
        {
            awakeTarget.Clear();
            layout.redPoint.SetActive(false);
            if (csvData == null)
            {
                return;
            }
            for (int i = 0; i < csvData.ActProject.Count; i++)
            {
                GameObject go = GameObject.Instantiate<GameObject>(layout.awakeTargetGo, layout.targetPanel.transform);
                go.SetActive(true);
                UI_AwakeTargetItem targetItem = new UI_AwakeTargetItem(awakenId, i);
                targetItem.Init(go.transform);
                targetItem.SetValue();
                awakeTarget.Add(targetItem);
            }
        }
        public void CheckTarget()
        {
            isAllTargetFin = true;
            unFinType = 50;
            for (int i=0;i<awakeTarget.Count;i++)
            {
                if (!awakeTarget[i].isFinish)
                {
                    isAllTargetFin = false;
                    unFinType = awakeTarget[i].targetT;
                    break;
                }
            }
            if (isAllTargetFin)
            {
                isAllTargetFin = true;
                layout.redPoint.SetActive(true);
            }
            //ImageHelper.SetImageGray(layout.awakenBtnImage, !isAllTargetFin);
        }
        public void DestoryPanel()
        {
            FrameworkTool.DestroyChildren(layout.currAttrGo.transform.parent.gameObject, layout.currAttrGo.transform.name, layout.currAwardPanel.transform.name);
            FrameworkTool.DestroyChildren(layout.nextAttrGo.transform.parent.gameObject, layout.nextAttrGo.transform.name, layout.nextAwardPanel.transform.name);
            DefaultTarget();
        }
        public void DefaultTarget()
        {
            FrameworkTool.DestroyChildren(layout.targetPanel.gameObject, layout.awakeTargetGo.transform.name);
            for (int i = 0; i < awakeTarget.Count; ++i)
            {
                awakeTarget[i].OnDestroy();
            }
            awakeTarget.Clear();
        }
        public void UpdateAwaken()
        {
            
            CheckTarget();
            if (!isAllTargetFin)
            {
                Sys_TravellerAwakening.Instance.OpenWarning(unFinType);
            }
            else
            {
                Sys_TravellerAwakening.Instance.TravellerAwakeningReq();      
            }
        }

        private void CheckImprintShow()
        {
            layout.awakenMarkBtn.gameObject.SetActive(Sys_FunctionOpen.Instance.IsOpen(51402));
            imprint_redPoint.RefreshAllRedPoints();
        }

        #endregion

        #region 事件回调

        private void OnTravellerAwakening()
        {
            float animTime = 1.25f;
            m_timer?.Cancel();
            m_timer = Timer.Register(animTime, OnComplete);
            layout.allAttrPanel.SetActive(false);
            layout.ConditionPanel.SetActive(false);
            layout.animImage.Play("Awake", -1, 0);
        }

        private void OnComplete()
        {
            UIManager.OpenUI(EUIID.UI_Awaken_Tips);
            float showtime = 1.30f;
            a_timer = Timer.Register(showtime, OnShowComplete); 
        }

        private void OnShowComplete()
        {
            layout.allAttrPanel.SetActive(true);
            layout.ConditionPanel.SetActive(true);
            imprint_redPoint.RefreshAllRedPoints();
            DestoryPanel();
            TestValue();

        }

        private void OnTripperAwakeUpdate()
        {
            DefaultTarget();
            SetTarget(currAwakenId);
        }
        private void OnAwakeImprintUpdate()
        {
            imprint_redPoint.RefreshAllRedPoints();
        }

        #endregion

        #region 响应事件

        public void OnCloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Awaken);

        }

        public void OnAwakenBtnClicked()
        {
            UpdateAwaken();
        }

        public void OnAwakenMarkBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_AwakenImprint);
        }

        #endregion   
    }
}
