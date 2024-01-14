using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine;
using UnityEngine.UI;
using Lib.Core;
using Packet;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Logic
{
    /// <summary>
    /// 建设行业枚举
    /// </summary>
    public enum EConstructs
    {
        /// <summary> 农业 </summary>
        Agriculture = 17,
        /// <summary> 商业 </summary>
        Business = 18,
        /// <summary> 治安 </summary>
        Security = 19,
        /// <summary> 宗教 </summary>
        Religion = 20,
        /// <summary> 科技 </summary>
        Technology = 21
    }

    /// <summary> 建设行业经验进度 </summary>
    public class UI_ConstructLevelSlider : UIComponent
    {
        //private Image constructImage;
        /// <summary> 行业名称 </summary>
        private Text constructNameText;
        /// <summary> 行业经验 </summary>
        private Text constructExpPercentText;
        /// <summary> 行业经验条 </summary>
        private Slider constructExpSlider;
        /// <summary> 行业名称语言id基础 </summary>
        private readonly uint constructNameId = 3200000000;
        protected override void Loaded()
        {
            //constructImage = transform.Find("Image_Icon").GetComponent<Image>();
            constructNameText = transform.Find("Text_Name")?.GetComponent<Text>();
            if(null == constructNameText)//任务界面节点不一样
            {
                constructNameText = transform.Find("Image/Text_Name").GetComponent<Text>();
            }
            constructExpPercentText = transform.Find("Text_Percent").GetComponent<Text>();
            constructExpSlider = transform.Find("Slider_Lv").GetComponent<Slider>();
        }

        public void SetSliderInfo(EConstructs eConstructs, float maxValue, float currentValue, bool changeName = true)
        {
            if(changeName)
            {
                TextHelper.SetText(constructNameText, constructNameId + (uint)eConstructs);
            }
            constructExpPercentText.text = string.Format("{0}/{1}", currentValue.ToString(), maxValue.ToString());
            constructExpSlider.value = currentValue / maxValue;
        }
    }

    /// <summary> 家族建设等级 </summary>
    public class UI_Family_Construct_Level : UIComponent
    {
        #region 界面组件 
        /// <summary> 繁荣度等级 </summary>
        private Text constructLevel;
        /// <summary> 繁荣度等级图片 </summary>
        private Image constructImage;
        /// <summary> 异步加载图片 </summary>
        private AsyncOperationHandle<Sprite> mHandle;
        #endregion

        #region 数据定义 
        private List<UI_ConstructLevelSlider> constructLevelInfos = new List<UI_ConstructLevelSlider>();
        #endregion
        #region 系统函数        
        protected override void Loaded()
        {
            OnParseComponent();
        }
        public override void Show()
        {
            base.Show();
            SetData();
            RefreshView();
        }
        public override void Hide()
        {
            base.Hide();
            if (mHandle.IsValid())
            {
                AddressablesUtil.Release<Sprite>(ref mHandle, MHandle_Completed);
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件
        /// </summary>
        private void OnParseComponent()
        {
            var values = System.Enum.GetValues(typeof(EConstructs));
            for (int i = 0, count = values.Length; i < count; i++)
            {
                EConstructs type = (EConstructs)values.GetValue(i);
                UI_ConstructLevelSlider constructSlider = null;
                string path = string.Empty;
                switch (type)
                {
                    case EConstructs.Agriculture:
                        {
                            path = "View_Left/List/Agri";
                        }
                        break;
                    case EConstructs.Business:
                        {
                            path = "View_Left/List/Business";
                        }
                        break;
                    case EConstructs.Security:
                        {
                            path = "View_Left/List/Safe";
                        }
                        break;
                    case EConstructs.Religion:
                        {
                            path = "View_Left/List/Rei";
                        }
                        break;
                    case EConstructs.Technology:
                        {
                            path = "View_Left/List/Science";
                        }
                        break;
                }
                if(!string.IsNullOrEmpty(path))
                    constructSlider = AddComponent<UI_ConstructLevelSlider>(transform.Find(path));
                constructLevelInfos.Add(constructSlider);
            }

            constructLevel = transform.Find("View_Right/Text2").GetComponent<Text>();
            constructImage = transform.Find("View_Right/Image1 (1)").GetComponent<Image>();

        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {

        }
        /// <summary>
        /// 设置数据
        /// </summary>
        private void SetData()
        {

        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            uint level = Sys_Family.Instance.familyData.GetConstructLevel();
            CSVFamilyProsperity.Data familyConstructLevelData = CSVFamilyProsperity.Instance.GetConfData(level);
            if (null != constructImage && null != familyConstructLevelData)
            {
                constructLevel.text = LanguageHelper.GetTextContent(3290000003, level.ToString());
                var values = System.Enum.GetValues(typeof(EConstructs));
                for (int i = 0, count = values.Length; i < count; i++)
                {
                    EConstructs type = (EConstructs)values.GetValue(i);
                    constructLevelInfos[i].SetSliderInfo(type, Sys_Family.Instance.GetClientDataExp(type, familyConstructLevelData), Sys_Family.Instance.familyData.GetConstructExp(type), false);
                }
                AddressablesUtil.LoadAssetAsync<Sprite>(ref mHandle, familyConstructLevelData.familyPicture , MHandle_Completed);
            }
        }

        #endregion
        #region 响应事件
        /// <summary>
        /// 异步回调
        /// </summary>
        /// <param name="handle"></param>
        private void MHandle_Completed(AsyncOperationHandle<Sprite> handle)
        {
            constructImage.sprite = handle.Result;
        }
        #endregion
        #region 提供功能
        #endregion
    }
}