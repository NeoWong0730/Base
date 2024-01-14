using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Packet;
using Net;
using Table;
using Lib.Core;


namespace Logic
{
    public class Sys_PreHeatActivity : SystemModuleBase<Sys_PreHeatActivity>
    {
        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public enum EEvents : int
        {
            /// <summary>
            /// 刷新活动状态
            /// </summary>
           UpdateActivityState,
        }
        #region 系统函数
        public override void Init()
        {

        }
        #endregion

        #region func
        /// <summary>
        /// 预热活动总开关,不管预热功能开启或关闭
        /// </summary>
        public bool CheckPreHeatActivityIsOpen()
        {
            return SDKManager.IsOpenGetExtJsonParam(SDKManager.EThirdSdkType.PreWarmAll.ToString(), out string paramsValue);
        }
        /// <summary>
        /// 刷新预热活动开启状态
        /// </summary>
        public void UpdatePreHeatActivityOpenState()
        {
            eventEmitter.Trigger(EEvents.UpdateActivityState);
        }
       
        /// <summary>
        /// 跳转到活动界面
        /// </summary>
        public bool JumpToPreHeatActivityPage()
        {
            //快跳接口
           return SDKManager.SDKPreWarmActivity();
        }
        #endregion
    }
}
