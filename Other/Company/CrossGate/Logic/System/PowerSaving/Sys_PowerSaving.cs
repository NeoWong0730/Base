using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;

namespace Logic
{
    public class Sys_PowerSaving : SystemModuleBase<Sys_PowerSaving>, ISystemModuleApplicationPause, ISystemModuleUpdate
    {
        public enum EEvents : int
        {
            /// <summary>
            /// 进入省电模式 
            /// </summary>
            OnEnterPowerSaving,
            /// <summary>
            /// 退出省电模式 
            /// </summary>
            OnQuitPowerSaving,
        }

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        /// <summary>
        /// 当前是否为省电模式
        /// </summary>
        private bool isSaving;
        /// <summary>
        /// 下次进入省电模式时间
        /// </summary>
        private float nextEnterPowerSavingTimePoint = 0;
        /// <summary>
        /// 进入省电模式的等待时间
        /// </summary>
        private float countDownTime = 0;
        /// <summary>
        /// 省电状态下屏幕亮度(0 - 255)
        /// </summary>
        private int powerSavingBrightness = 20;
        /// <summary>
        /// 省电前的屏幕亮度 (安卓不再对该值赋值，亮度设置为-1会变成和系统相同的亮度值)
        /// </summary>
        private int normalBrightness = -1;
        /// <summary>
        /// 玩家设置
        /// </summary>
        private bool closePowerSaving = false;

        /// <summary>
        /// 是否运行使用省电模式
        /// </summary>
        private bool allowPowerSaving;
        public bool bAllowPowerSaving { get { return allowPowerSaving; } }
        /// <summary>
        /// 是否在后台运行
        /// </summary>
        private bool IsPause = false;

        #region 系统函数
        public override void Init()
        {
            allowPowerSaving = false;
            countDownTime = float.Parse(CSVParam.Instance.GetConfData(1084).str_value);
            closePowerSaving = OptionManager.Instance.GetBoolean(OptionManager.EOptionID.ClosePowerSaving);

            Sys_Input.Instance.eventEmitter.Handle(Sys_Input.EEvents.OnScreenTouchDown, OnTouchScreen, true);
            OptionManager.Instance.eventEmitter.Handle<int>(OptionManager.EEvents.OptionFinalChange, OnOptionChange, true);
        }

        public override void Dispose()
        {
            Sys_Input.Instance?.eventEmitter?.Handle(Sys_Input.EEvents.OnScreenTouchDown, OnTouchScreen, false);
            OptionManager.Instance?.eventEmitter?.Handle<int>(OptionManager.EEvents.OptionFinalChange, OnOptionChange, false);
        }

        public override void OnLogin()
        {
            allowPowerSaving = CheckPowerSavingIsOpenInPlatform() && countDownTime > 0;
            nextEnterPowerSavingTimePoint = Time.unscaledTime + countDownTime;
        }

        public override void OnLogout()
        {
            allowPowerSaving = false;

            if (isSaving)
            {
                QuitPowerSaving();
            }
        }

        public void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                if (isSaving)
                {
                    QuitPowerSaving();
                }

                //重置时间
                nextEnterPowerSavingTimePoint = Time.unscaledTime + countDownTime;
            }
            IsPause = pause;
        }

        public void OnUpdate()
        {            
            if (!allowPowerSaving)
                return;
         
            if (isSaving)
                return;

            if (closePowerSaving)
                return;

            if (IsPause)
                return;

            if (Time.unscaledTime < nextEnterPowerSavingTimePoint)
                return;
            
            UIManager.OpenUI(EUIID.UI_PowerSaving);
            EnterPowerSaving();
        }
        #endregion


        #region func
        /// <summary>
        /// 检测省电模式是否在当前平台开启
        /// </summary>
        /// <returns></returns>
        private bool CheckPowerSavingIsOpenInPlatform()
        {
#if UNITY_ANDROID || UNITY_IOS || UNITY_TVOS
            return !SDKManager.SDKISEmulator();
#elif UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }

        /// <summary>
        /// 进入省电模式
        /// </summary>
        public void EnterPowerSaving()
        {
            isSaving = true;
            //亮度
            EnterPowersavingBrightness();
            //开启省电模式设置
            OptionManager.Instance.SwitchEnergySaving(true);
            //Debug.Log("进入省电模式");
            eventEmitter.Trigger(EEvents.OnEnterPowerSaving);
        }

        /// <summary>
        /// 退出省电模式
        /// </summary>
        public void QuitPowerSaving()
        {
            isSaving = false;
            //亮度
            QuitPowersavingBrightness();
            //关闭省电模式设置
            OptionManager.Instance.SwitchEnergySaving(false);
            //Debug.Log("退出省电模式");
            eventEmitter.Trigger(EEvents.OnQuitPowerSaving);
        }

        public void SetPowerSavingCDForGM(float cd)
        {
            countDownTime = cd;
            nextEnterPowerSavingTimePoint = Time.unscaledTime + countDownTime;
        }

        #endregion

        #region event
        private void OnTouchScreen()
        {
            if (isSaving)
            {
                QuitPowerSaving();
            }

            //重置时间
            nextEnterPowerSavingTimePoint = Time.unscaledTime + countDownTime;
        }

        private void OnOptionChange(int optionID)
        {
            if (optionID == (int)OptionManager.EOptionID.ClosePowerSaving)
            {
                closePowerSaving = OptionManager.Instance.GetBoolean(OptionManager.EOptionID.ClosePowerSaving);
                //重置时间
                nextEnterPowerSavingTimePoint = Time.unscaledTime + countDownTime;
            }
        }
        #endregion

        #region 亮度调节相关逻辑
        static int GetBrightness()
        {
#if UNITY_IOS || UNITY_TVOS
            return (int)(SDKManager.GetBrightnessNative() * 255);
#elif UNITY_ANDROID
            return (int)SDKManager.GetBrightnessNative();
#else
            return 0;
#endif
        }

        static void SetBrightness(int brightness)
        {
#if UNITY_IOS || UNITY_TVOS
            SDKManager.SetBrightnessNative(1.0f * brightness / 255);
#elif UNITY_ANDROID
            SDKManager.SetBrightnessNative(brightness);
#endif
        }

        /// <summary>
        /// 进入省电模式亮度状态
        /// </summary>
        private void EnterPowersavingBrightness()
        {
            if (!SDKManager.sdk.IsHaveSdk)
                return;

            //仅苹果下取当前亮度
#if UNITY_IOS || UNITY_TVOS
            normalBrightness = GetBrightness();
#endif
            SetBrightness(powerSavingBrightness);
        }

        /// <summary>
        /// 离开省电模式亮度状态(回复常规亮度)
        /// </summary>
        private void QuitPowersavingBrightness()
        {
            if (!SDKManager.sdk.IsHaveSdk)
                return;

            SetBrightness(normalBrightness);
        }
        #endregion
    }
}
