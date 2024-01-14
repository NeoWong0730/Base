using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Framework
{
    public class UIMapTimeline : MonoBehaviour
    {
        /// <summary> 结束事件 </summary>
        public Action onPlayableDirectorStopped;
        /// <summary> 通用Timeline </summary>
        public PlayableDirector playableDirector_common;
        /// <summary> 岛屿分支Timeline </summary>
        public List<PlayableDirector> playableDirectors_map;
        /// <summary> 文本效果 </summary>
        public TypewriterEffect typewriterEffect;
        /// <summary> 文本 </summary>
        //public Text text;
        /// <summary> 岛屿Id </summary>
        public uint IslandId;
        /// <summary> 文本下标 </summary>
        public int Index_Text;
        /// <summary> 消息 </summary>
        public List<string> Message = new List<string>();
        private void Awake()
        {
            playableDirector_common.playOnAwake = false;

            foreach (var item in playableDirectors_map)
            {
                item.playOnAwake = false;
            }
        }

        private void OnEnable()
        {
            playableDirector_common.stopped += OnPlayableDirectorStopped;
        }

        private void OnDisable()
        {
            playableDirector_common.stopped -= OnPlayableDirectorStopped;
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        public void SetData(uint id, List<string> message)
        {
            IslandId = id;
            Message = message;
            Index_Text = 0;
            //text.text = string.Empty;
            typewriterEffect.WordByWord(string.Empty);
        }
        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="partIndex"></param>
        public void PlayText()
        {
            if (Index_Text < 0 || Index_Text >= Message.Count) return;
            //text.text = Message[Index_Text];
            typewriterEffect.WordByWord(Message[Index_Text]);
            Index_Text++;
        }
        /// <summary>
        /// 播放Timeline
        /// </summary>
        public void PlayTimeline()
        {
            playableDirector_common.Resume();
            playableDirector_common.Play();
            foreach (var item in playableDirectors_map)
            {
                if (item.name == IslandId.ToString())
                {
                    item.Resume();
                    item.Play();
                    break;
                }
            }
        }
        /// <summary>
        /// 播放Timeline结束事件
        /// </summary>
        /// <param name="director"></param>
        public void OnPlayableDirectorStopped(PlayableDirector director)
        {
            if (playableDirector_common == director)
            {
                if (null != onPlayableDirectorStopped)
                    onPlayableDirectorStopped();
            }
        }
        /// <summary>
        /// 结束动画
        /// </summary>
        public void EndAnimation()
        {
            double endtime = 32f; 
            playableDirector_common.Pause();
            playableDirector_common.time = endtime;
            playableDirector_common.Play();

            foreach (var item in playableDirectors_map)
            {
                if (item.name == IslandId.ToString())
                {
                    item.Pause();
                    item.time = endtime;
                    item.Play();
                    break;
                }
            }
        }

    }
}