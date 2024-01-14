using Lib.Core;
using Net;
using System.Collections.Generic;
using Table;
using System.Json;
using UnityEngine;
using System.IO;
using Logic.Core;

namespace Logic
{
    public class Sys_Qa : SystemModuleBase<Sys_Qa>
    {
        public List<uint> Questionnaires = new List<uint>();

        private List<uint> WjxId = new List<uint>();        //已经做完的问卷调查


        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public bool hasShowRedPoint = false;

        public enum EEvents
        {
            OnRefreshQARedPoint,
        }

        public override void OnLogin()
        {
            hasShowRedPoint = false;
        }

        public void CacheWjxIds(List<uint> _WjxIds)
        {
            WjxId = _WjxIds;
            eventEmitter.Trigger(EEvents.OnRefreshQARedPoint);
        }


        /// <summary>
        /// 检测条件(事件式)
        /// </summary>
        public void CheckConditionVaild()
        {
            Questionnaires.Clear();
            var enumerator = CSVQuestionnaire.Instance.GetAll().GetEnumerator();
            while (enumerator.MoveNext())
            {
                CSVQuestionnaire.Data cSVQuestionnaireData = enumerator.Current;
                CSVCheckseq.Data cSVCheckseqData = CSVCheckseq.Instance.GetConfData(cSVQuestionnaireData.FunctionId);
                if (cSVCheckseqData.IsValid())
                {
                    Questionnaires.Add(cSVQuestionnaireData.id);
                }
            }

            for (int i = 0; i < WjxId.Count; i++)
            {
                if (!Questionnaires.Remove(WjxId[i]))
                {
                    DebugUtil.LogErrorFormat($"存在不满足条件 但是已经做完的问卷{WjxId[i]}");
                }
            }
            //没有问卷
            if (Questionnaires.Count == 0)
            {
                string content = CSVLanguage.Instance.GetConfData(1000996).words;
                Sys_Hint.Instance.PushContent_Normal(content);
                DebugUtil.Log(ELogType.eQa, "Questionnaires.Count == 0");
            }
            //做问卷
            else if (Questionnaires.Count == 1)
            {
                CSVQuestionnaire.Data cSVQuestionnaireData = CSVQuestionnaire.Instance.GetConfData(Questionnaires[0]);
                string url1 = cSVQuestionnaireData.QuestionURL;
                string url2 = string.Format("?sojumpparm={0};{1};{2};{3}&userid={4}", Sys_Role.Instance.RoleId.ToString(), cSVQuestionnaireData.id.ToString(),
                    Sys_Login.Instance.selectedServerId.ToString(), SDKManager.GetUID(), SDKManager.GetUID());

                DebugUtil.Log(ELogType.eQa, url1 + url2);
                SDKManager.SDKOpenH5Questionnaire(url1 + url2);
            }
            //弹出列表
            else
            {
                UIManager.OpenUI(EUIID.UI_Qa);
            }
        }


        public bool HasQa()
        {
            Questionnaires.Clear();
            var enumerator = CSVQuestionnaire.Instance.GetAll().GetEnumerator();
            while (enumerator.MoveNext())
            {
                CSVQuestionnaire.Data cSVQuestionnaireData = enumerator.Current;
                CSVCheckseq.Data cSVCheckseqData = CSVCheckseq.Instance.GetConfData(cSVQuestionnaireData.FunctionId);
                if (cSVCheckseqData.IsValid())
                {
                    Questionnaires.Add(cSVQuestionnaireData.id);
                }
            }
            for (int i = 0; i < WjxId.Count; i++)
            {
                if (!Questionnaires.Remove(WjxId[i]))
                {
                    DebugUtil.LogErrorFormat($"存在不满足条件 但是已经做完的问卷{WjxId[i]}");
                }
            }
            if (Questionnaires.Count > 0)
            {
                for (int i = 0; i < Questionnaires.Count; i++)
                {
                    DebugUtil.LogFormat(ELogType.eQa, string.Format("id:{0}", Questionnaires[i]));
                }
            }
            return Questionnaires.Count > 0;
        }
    }

}


