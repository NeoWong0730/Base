using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using Lib.Core;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using System;

namespace Logic
{
    /// <summary> 家族建设玩法介绍提示 </summary>
    public class UI_Family_Construct_Tips : UIBase
    {
        #region 界面组件
        /// <summary> 玩法图片 </summary>
        private RawImage constructImage;
        /// <summary> 异步加载图片 </summary>
        //private AsyncOperationHandle<Sprite> mHandle;
        #endregion
        #region 数据定义
        private string imagePath = string.Empty;

        #endregion
        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnOpen(object arg)
        {
            if (arg is Tuple<uint, object>)
            {
                Tuple<uint, object> tuple = arg as Tuple<uint, object>;
                CSVIndustryActivity.Data constructInfo = CSVIndustryActivity.Instance.GetConfData(Convert.ToUInt32(tuple.Item2));
                if (null != constructInfo)
                    imagePath = constructInfo.teachingPicture;
            }
        }

        protected override void OnShow()
        {
            // if (!string.IsNullOrEmpty(imagePath))
            //    AddressablesUtil.LoadAssetAsync<Sprite>(ref mHandle, imagePath, MHandle_Completed);
            ImageHelper.SetTexture(constructImage, imagePath);
        }

        protected override void OnHide()
        {
            
        }

        protected override void OnClose()
        {
            imagePath = string.Empty;
            /*if (mHandle.IsValid())
            {
                AddressablesUtil.Release<Sprite>(ref mHandle, MHandle_Completed);
            }*/
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
            constructImage = transform.Find("Animator/View_Content/Image_Bg").GetComponent<RawImage>();
            transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
        }

        #endregion
        #region 界面显示
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_Construct_Tips, "OnClick_Close");
            CloseSelf();
        }

        /// <summary>
        /// 异步回调
        /// </summary>
        /// <param name="handle"></param>
        /*private void MHandle_Completed(AsyncOperationHandle<Sprite> handle)
        {
            //constructImage.sprite = handle.Result;
        }*/

        #endregion
        #region 提供功能

        #endregion
    }
}