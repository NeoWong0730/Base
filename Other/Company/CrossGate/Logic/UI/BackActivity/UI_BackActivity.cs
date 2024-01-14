using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;
using Logic.Core;
using Packet;
using UnityEngine.Playables;
using System.Collections.Generic;

namespace Logic
{
    public class UI_BackActivity : UIBase
    {
        private Button btnClose;
        private Transform pageParent;
        private Transform tabParent;
        private Text txtTime;
        private Text txtDesc;

        /// <summary>
        /// 页签字典 key对应页签枚举
        /// </summary>
        private Dictionary<uint, UI_BackActivityBase> dicPageView = new Dictionary<uint, UI_BackActivityBase>();
        /// <summary>
        /// 页签uint列表 对应页签枚举
        /// </summary>
        private List<uint> listType = new List<uint>();
        private List<UI_BackActivityTab> listTab = new List<UI_BackActivityTab>();
        private uint curSelectId = 0;
        private uint OpenKey = 1;
        private UI_BackActivityBase curUIView;

        private Timer timer;
        private float countDownTime = 0;
        #region 系统函数
        protected override void OnOpen(object arg)
        {

        }
        protected override void OnLoaded()
        {
            Parse();
        }
        protected override void OnShow()
        {
            curUIView?.Show();
            UpdateView(curSelectId);
        }
        protected override void OnHide()
        {
            timer?.Cancel();
            foreach (var item in dicPageView.Values)
            {
                if (item != null && item.transform != null)
                {
                    item.Hide();
                }
            }
        }
        protected override void OnDestroy()
        {
            timer?.Cancel();
            foreach (var item in dicPageView.Values)
            {
                if (item != null && item.transform != null)
                {
                    item.OnDestroy();
                }
            }
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
            Sys_BackActivity.Instance.eventEmitter.Handle(Sys_BackActivity.EEvents.OnBackActivityRedPointUpdate, OnBackActivityRedPointUpdate, toRegister);
            Sys_BackActivity.Instance.eventEmitter.Handle(Sys_BackActivity.EEvents.OnBackActivityInfoUpdate, OnBackActivityInfoUpdate, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateOperatinalActivityShowOrHide, OnUpdateOperatinalActivityShowOrHide, toRegister);
        }
        #endregion

        #region func
        private void Parse()
        {
            btnClose = transform.Find("Animator/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            pageParent = transform.Find("Animator/Content_Node");
            tabParent = transform.Find("Animator/Menu/Scroll View/Viewport/Content");
            txtTime = transform.Find("Animator/Tips/Text_Time/Text_Value").GetComponent<Text>();
            txtDesc = transform.Find("Animator/Tips/Text_Tips").GetComponent<Text>();

            List<uint> keyList = new List<uint>();
            keyList.AddRange(CSVReturnMenu.Instance.GetKeys());
            FrameworkTool.CreateChildList(tabParent, keyList.Count);
            for (int i = 0; i < keyList.Count; i++)
            {
                var goTab = tabParent.GetChild(i);
                UI_BackActivityTab tab = new UI_BackActivityTab();
                tab.Init(goTab);
                tab.RegisterEvent(OnTabClick);
                listTab.Add(tab);
                tab.UpdateCellView(keyList[i]);
                CSVReturnMenu.Data data = CSVReturnMenu.Instance.GetConfData(keyList[i]);
                listType.Add(data.id);
                //先创建UI脚本,但是不加载对应prefab
                var uiComponent = CreateUIComponent(data.id);
                dicPageView.Add(data.id, uiComponent);
            }

            OnTabClick(OpenKey);
        }
        private void UpdateView(uint tabKey)
        {
            if (curSelectId != tabKey)
            {
                curSelectId = tabKey;
                curUIView?.Hide();
                if (dicPageView.TryGetValue(curSelectId, out UI_BackActivityBase UIView))
                {
                    curUIView = UIView;
                    if (!UIView.CheckFunctionIsOpen())
                    {
                        //界面未开启，则选择第一个开启的tab
                        bool isFind = false;
                        for (int i = 0; i < listType.Count; i++)
                        {
                            if (dicPageView.TryGetValue(listType[i], out UI_BackActivityBase nextUIview) && nextUIview.CheckFunctionIsOpen())
                            {
                                curSelectId = listType[i];
                                curUIView = nextUIview;
                                isFind = true;
                                break;
                            }
                        }
                        if (!isFind)
                        {
                            DebugUtil.Log(ELogType.eNone, "回归活动界面没有任何页签开启");
                            this.CloseSelf();
                            return;
                        }
                    }
                    if (curUIView.transform == null)
                    {
                        //UIprefab在这边实例化
                        var data = CSVReturnMenu.Instance.GetConfData(curSelectId);
                        Transform uiTran = CreateOperationCellGameobject(data.prefabNode).transform;
                        curUIView.Init(uiTran);
                    }
                    curUIView.Show();
                }

            }
            for (int i = 0; i < listTab.Count; i++)
            {
                var tab = listTab[i];
                bool isOpen = dicPageView.TryGetValue(tab.curId, out UI_BackActivityBase nextUIview) && nextUIview.CheckFunctionIsOpen();
                tab.transform.gameObject.SetActive(isOpen);
                if (isOpen)
                {
                    tab.SetSelectState(curSelectId);
                }
            }
            UpdateTabRedPoint();

            txtDesc.text = LanguageHelper.GetTextContent(2014411, CSVReturnParam.Instance.GetConfData(3).str_value);
            uint nowtime = Sys_Time.Instance.GetServerTime();
            var targetTime = Sys_BackActivity.Instance.GetBackActivityEndTime();
            countDownTime = targetTime - nowtime;
            timer?.Cancel();
            timer = Timer.Register(countDownTime, OnTimerComplete, OnTimerUpdate, false, false);
        }
        private void UpdateTabRedPoint()
        {
            for (int i = 0; i < listTab.Count; i++)
            {
                var curTab = listTab[i];
                if (dicPageView.TryGetValue(curTab.curId, out UI_BackActivityBase value))
                {
                    curTab.UpdateRedPoint(value.CheckTabRedPoint());
                }
            }
        }
        /// <summary>
        /// 创建活动脚本实例 menuId对应回归活动菜单表id
        /// </summary>
        public UI_BackActivityBase CreateUIComponent(uint menuId)
        {
            switch (menuId)
            {
                case 1://回归赠礼
                    return new UI_BackGift();
                case 2://豪礼商铺
                    return new UI_BackMall();
                case 3://回归签到
                    return new UI_BackSign();
                case 4://回归试炼
                    return new UI_ReturnTask();
                case 5://奖励找回
                    return new UI_BackAwardGet();
                default:
                    return new UI_BackActivityBase();
            }
        }
        /// <summary>
        /// 创建活动节点(预设)实例
        /// 新增节点的时候，需要手动将预设体添加到,福利UIprefab根节点下的AssetDependencies脚本内
        /// </summary>
        public GameObject CreateOperationCellGameobject(string prefabNode)
        {
            var goList = transform.GetComponent<AssetDependencies>().mCustomDependencies;
            for (int i = 0; i < pageParent.childCount; i++)
            {
                Transform cell = pageParent.GetChild(i);
                if (cell.name == prefabNode)
                {
                    return cell.gameObject;
                }
            }
            //在现有列表里没找到，则实例化
            for (int i = 0; i < goList.Count; i++)
            {
                var prefabCell = goList[i] as GameObject;
                if (prefabCell.name == prefabNode)
                {
                    var goChild = FrameworkTool.CreateGameObject(prefabCell, pageParent.gameObject);
                    //新增节点设置列表位置，以免层级太高遮挡其他非节点UI
                    //if (pageParent.childCount > 3)//防止报错
                    //{
                    //    goChild.transform.SetSiblingIndex(3);
                    //}
                    return goChild;
                }
            }
            return new GameObject();
        }
        #endregion

        #region event
        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }
        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            UpdateView(curSelectId);
        }
        private void OnTabClick(uint tabKey)
        {
            UpdateView(tabKey);
        }
        private void OnBackActivityRedPointUpdate()
        {
            UpdateView(curSelectId);
        }
        private void OnBackActivityInfoUpdate()
        {
            UpdateView(curSelectId);
        }
        private void OnUpdateOperatinalActivityShowOrHide()
        {
            //刷新活动状态
            bool isBackOpen = Sys_BackActivity.Instance.CheckBackActivityIsOpen();//回归活动总开关
            if (isBackOpen)
            {
                UpdateView(curSelectId);
            }
            else
            {
                this.CloseSelf();
            }
        }
        private void OnTimerComplete()
        {
            timer?.Cancel();
            //关闭页签
            this.CloseSelf();
        }
        private void OnTimerUpdate(float time)
        {
            if (countDownTime >= time && txtTime != null)
            {
                txtTime.text = LanguageHelper.TimeToString((uint)(countDownTime - time), LanguageHelper.TimeFormat.Type_1);
            }
        }
        #endregion

        #region class
        public class UI_BackActivityTab
        {
            public uint curId;//id对应页签枚举
            public Transform transform;
            private Button btnSelf;
            private Action<uint> act;
            private GameObject goDark;
            private Text txtDarkTitle;
            private GameObject goLight;
            private Text txtLightTitle;
            private GameObject goRedPoint;
            public void Init(Transform _transform)
            {
                transform = _transform;
                btnSelf = transform.GetComponent<Button>();
                btnSelf.onClick.AddListener(OnBtnSelfClick);
                goDark = transform.Find("Btn_Menu_Dark").gameObject;
                txtDarkTitle = transform.Find("Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
                goLight = transform.Find("Image_Menu_Light").gameObject;
                txtLightTitle = transform.Find("Image_Menu_Light/Text_Menu_Light").GetComponent<Text>();
                goRedPoint = transform.Find("Image_Dot").gameObject;
            }

            public void UpdateCellView(uint id)
            {
                curId = id;
                var data = CSVReturnMenu.Instance.GetConfData(id);
                if(data!= null)
                {
                    txtDarkTitle.text = txtLightTitle.text = LanguageHelper.GetTextContent(data.lanId);
                }
            }

            public void SetSelectState(uint SelectId)
            {
                bool isSelect = SelectId == curId;
                goDark.SetActive(!isSelect);
                goLight.SetActive(isSelect);
            }
            public void UpdateRedPoint(bool isShow)
            {
                goRedPoint.SetActive(isShow);
            }
            public void RegisterEvent(Action<uint> _act)
            {
                act = _act;
            }
            private void OnBtnSelfClick()
            {
                act?.Invoke(curId);
            }
        }
        #endregion
    }
}
