using System;
using Framework;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace Logic
{
    /// <summary> 预览地图 </summary>
    public class UI_PreviewMap : UIBase
    {
        #region 委托事件
        #endregion
        #region 界面组件
        /// <summary> 导演脚本 </summary>
        private UIMapTimeline uIMapTimeline;
        #endregion
        #region 数据
        /// <summary> 岛屿ID </summary>
        private uint IslandId;
        /// <summary> 任务ID </summary>
        private uint taskId;
        #endregion
        #region 系统函数
        protected override void OnLoaded()
        {            
            OnParseComponent();
        }

        protected override void OnOpen(object arg)
        {
            Tuple<uint, object> tuple = arg as Tuple<uint, object>;
            if (tuple != null)
            {
                taskId = tuple.Item1;
                IslandId = tuple.Item2 == null ? 0 : Convert.ToUInt32(tuple.Item2);
            }
        }
        protected override void OnShow()
        {            
            PlayTimeline();
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
            uIMapTimeline = transform.Find("Timeline").GetComponent<UIMapTimeline>();
            transform.Find("Animator/SkipButton").GetComponent<Button>().onClick.AddListener(OnClick_Skip);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            if (toRegister)
                uIMapTimeline.onPlayableDirectorStopped += OnPlayableDirectorStopped;
            else
                uIMapTimeline.onPlayableDirectorStopped -= OnPlayableDirectorStopped;
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 播放动画
        /// </summary>
        public void PlayTimeline()
        {
            CSVIsland.Data cSVIslandData = CSVIsland.Instance.GetConfData(IslandId);
            if (null != cSVIslandData)
            {
                List<string> message = new List<string>();
                if (null != cSVIslandData.subIds)
                {
                    for (int i = 0, count = cSVIslandData.subIds.Count; i < count; i++)
                    {
                        var id = cSVIslandData.subIds[i];
                        message.Add(LanguageHelper.GetTextContent(id));
                    }
                }
                uIMapTimeline.SetData(cSVIslandData.id, message);
                uIMapTimeline.PlayTimeline();
            }
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            if (taskId != 0)
            {
                Sys_Task.Instance.ReqStepGoalFinishEx(taskId);
            }
            CloseSelf();
        }
        /// <summary>
        /// 跳过动画
        /// </summary>
        public void OnClick_Skip()
        {
            uIMapTimeline.EndAnimation();
        }
        /// <summary>
        /// 动画播放结束
        /// </summary>
        public void OnPlayableDirectorStopped()
        {
            OnClick_Close();
        }
        #endregion
        #region 提供功能
        #endregion
    }
}
