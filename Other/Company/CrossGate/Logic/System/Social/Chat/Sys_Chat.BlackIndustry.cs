using GME;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Table;
using UnityEngine;

namespace Logic
{
    public partial class Sys_Chat : SystemModuleBase<Sys_Chat>, ISystemModuleUpdate
    {
        private float _nextBlackIndustryTime = -1f;
        private uint _chatBlackIndustryID = 0;

        public void _OnLogin_BlackIndustry()
        {
            _nextBlackIndustryTime = Time.unscaledTime + 1f;
        }

        public void _OnLogout_BlackIndustry()
        {
            _nextBlackIndustryTime = -1f;
            _chatBlackIndustryID = 0;
        }

        public override bool CanUpdate()
        {
            if (_nextBlackIndustryTime > 0f && Time.unscaledTime >= _nextBlackIndustryTime)
                return true;

            return false;
        }

        public void OnUpdate()
        {
            PushBlackIndustry(Sys_Role.Instance.Role.Level);
        }

        private void PushBlackIndustry(uint lv)
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(30202))
            {
                _nextBlackIndustryTime = Time.unscaledTime + 1200;
                return;
            }

            CSVChatBlackIndustry.Data data = null;

            if (lv <= _chatBlackIndustryID)
            {
                data = CSVChatBlackIndustry.Instance.GetConfData(_chatBlackIndustryID);
            }

            if (data == null)
            {
                //等级段过了重新计算
                data = GetChatBlackIndustryData(lv);
            }

            if (data == null)
            {
                _nextBlackIndustryTime = Time.unscaledTime + 1200;
                return;
            }

            _chatBlackIndustryID = data.id;

            int count = data.random_content != null ? data.random_content.Count : 0;
            if (count > 0)
            {
                int index = Mathf.Min(UnityEngine.Random.Range(0, count), count - 1);

                DebugUtil.LogFormat(ELogType.eChat, "PushBlackIndustry(ChatType.System, null, {0})", LanguageHelper.GetTextContent(data.random_content[index]));
                PushMessage(ChatType.System, null, LanguageHelper.GetTextContent(data.random_content[index]));
            }

            _nextBlackIndustryTime = Time.unscaledTime + data.play_interval;
        }

        private CSVChatBlackIndustry.Data GetChatBlackIndustryData(uint lv)
        {
            IReadOnlyList<CSVChatBlackIndustry.Data> datas = CSVChatBlackIndustry.Instance.GetAll();

            CSVChatBlackIndustry.Data data = null;

            for (int i = 0, len = datas.Count; i < len; ++i)
            {
                if (lv <= datas[i].id)
                {
                    data = datas[i];
                    break;
                }
            }

            if (data == null)
            {
                data = datas[datas.Count - 1];
            }

            return data;
        }
    }
}