using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;


namespace Logic
{
    
    /// <summary> 继续推荐 </summary>
    public class UI_Construct_Continue : UIBase
    {
        #region 界面组件 
        private CP_ToggleRegistry toggleRegistry;
        #endregion
        #region 系统函数        
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnShow()
        {
            RefreshView();
        }
        protected override void OnHide()
        {

        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {

        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件
        /// </summary>
        private void OnParseComponent()
        {
            transform.Find("Animator/Button_Sure").GetComponent<Button>().onClick.AddListener(OnContinueBtnClicked);
            transform.Find("Animator/Button_No").GetComponent<Button>().onClick.AddListener(OnCloseBtnClicked);
            toggleRegistry = transform.Find("Animator/Object").GetComponent<CP_ToggleRegistry>();
        }

        #endregion
        private uint selectId;
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            int selectindex = 0;
            bool hasTj = false;
            var values = System.Enum.GetValues(typeof(EConstructs));
            for (int i = 0, count = values.Length; i < count; i++)
            {
                EConstructs type = (EConstructs)values.GetValue(i);
                string path = string.Empty;
                switch (type)
                {
                    case EConstructs.Agriculture:
                        {
                            path = "Animator/Object/Agri";
                        }
                        break;
                    case EConstructs.Business:
                        {
                            path = "Animator/Object/Business";
                        }
                        break;
                    case EConstructs.Security:
                        {
                            path = "Animator/Object/Safe";
                        }
                        break;
                    case EConstructs.Religion:
                        {
                            path = "Animator/Object/Rei";
                        }
                        break;
                    case EConstructs.Technology:
                        {
                            path = "Animator/Object/Science";
                        }
                        break;
                }

                Transform tr = transform.Find(path);
                var toggle = tr.GetComponent<CP_Toggle>();
                toggle.onValueChanged.AddListener((bool value) =>
                {
                    OnValueChange(tr, value);
                });
                bool isShowRed = Sys_Family.Instance.IsShowConstructRedPoint(type);
                tr.Find("Lable").gameObject.SetActive(isShowRed);
                if (Sys_Family.Instance.IsShowConstructRedPoint(type))
                {
                    hasTj = true;
                    toggle.SetSelected(true, true);//默认prefab上菜单显示不正确，需要强制刷新下。 
                    selectindex = i;
                }
            }
            if(!hasTj)
            {
                uint minExp = 0;
                for (int i = 0, count = values.Length; i < count; i++)
                {
                    EConstructs type = (EConstructs)values.GetValue(i);
                    var exp = Sys_Family.Instance.familyData.GetConstructExp(type);
                    if (minExp > exp)
                    {
                        minExp = exp;
                        selectindex = i;
                    }
                }
                toggleRegistry.SwitchTo(selectindex, true, true);
            }
            TextHelper.SetText(transform.Find("Animator/Text_Tip").GetComponent<Text>(), 3290000007u, Sys_Family.Instance.familyData.GetGuidStamina().ToString());
        }

        #endregion
        #region 响应事件
        private void OnValueChange(Transform tr, bool value)
        {
            if(value)
            {
                switch (tr.name)
                {
                    case "Agri":
                        selectId = 17;
                        break;
                    case "Business":
                        selectId = 18;
                        break;
                    case "Safe":
                        selectId = 19;
                        break;
                    case "Rei":
                        selectId = 20;
                        break;
                    case "Science":
                        selectId = 21;
                        break;
                }
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        private void OnCloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Construct_Continue);
        }

        /// <summary>
        /// 继续
        /// </summary>
        private void OnContinueBtnClicked()
        {
            CSVIndustryActivity.Data constructInfo = CSVIndustryActivity.Instance.GetConfData(selectId);
            if (null != constructInfo)
            {
                ActionCtrl.Instance.MoveToTargetNPCAndInteractive(constructInfo.findNPC);
                UIManager.CloseUI(EUIID.UI_Family);
                UIManager.CloseUI(EUIID.UI_Family_Construct);
            }
            else
            {
                DebugUtil.Log(ELogType.eNone, $"Table CSVIndustryActivity not find id {selectId}");
            }
            UIManager.CloseUI(EUIID.UI_Construct_Continue);
        }
        #endregion
        #region 提供功能
        #endregion

    }
}