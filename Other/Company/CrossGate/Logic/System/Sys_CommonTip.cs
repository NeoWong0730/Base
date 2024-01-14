using System;
using System.Collections.Generic;
using Logic.Core;
using Table;

namespace Logic
{
    /// 服务器时间
    public class Sys_CommonTip : SystemModuleBase<Sys_CommonTip>, ISystemModuleUpdate
    {
        public class CommonTipParam
        {
            public uint tipId;
            public uint knowledgeId;
            public string strContent;
        }

        private Queue<CommonTipParam> _ques = new Queue<CommonTipParam>(8);

        private float timeCounter;

        public override void OnLogin()
        {
            _ques.Clear();
        }

        public override void OnLogout()
        {
            _ques.Clear();
        }

        public void OnUpdate()
        {
            timeCounter += GetUnscaledDeltaTime();
            if (timeCounter > 1f)
            {
                timeCounter = 0f;
                if (!UIManager.IsOpen(EUIID.UI_Common_Tip))
                {
                    if (_ques.Count > 0)
                        UIManager.OpenUI(EUIID.UI_Common_Tip, false, _ques.Dequeue());
                }
            }
        }

        #region 填充提示数据
        public void TipKnowledge(Sys_Knowledge.ETypes type, uint knowledgeId)
        {
            CommonTipParam para = new CommonTipParam();

            switch(type)
            {
                case Sys_Knowledge.ETypes.Gleanings:
                    {
                        CSVGleanings.Data data = CSVGleanings.Instance.GetConfData(knowledgeId);
                        if (data != null)
                        {
                            if (data.type_id == 2)
                                para.tipId = 1005;
                            else if (data.type_id == 1)
                                para.tipId = 1004;

                            para.strContent = LanguageHelper.GetTextContent(2010331, LanguageHelper.GetTextContent(data.name_id));
                        }
                    }
                    break;
                case Sys_Knowledge.ETypes.Annals:
                    {
                        para.tipId = 1001;
                        CSVChronology.Data data = CSVChronology.Instance.GetConfData(knowledgeId);
                        if (data != null)
                        {
                            para.strContent = LanguageHelper.GetTextContent(2010331, LanguageHelper.GetTextContent(data.event_titel));
                        }
                    }
                    break;
                case Sys_Knowledge.ETypes.Fragment:
                    {
                        para.tipId = 1006;
                        CSVMemoryPieces.Data data = CSVMemoryPieces.Instance.GetConfData(knowledgeId);
                        if (data != null)
                        {
                            para.strContent = LanguageHelper.GetTextContent(2010331, LanguageHelper.GetTextContent(data.memory_name));
                        }
                    }
                    break;
                case Sys_Knowledge.ETypes.Brave:
                    {
                        para.tipId = 1002;
                        CSVBraveBiography.Data data = CSVBraveBiography.Instance.GetConfData(knowledgeId);
                        if (data != null)
                        {
                            para.strContent = LanguageHelper.GetTextContent(2010331, LanguageHelper.GetTextContent(data.biography_name));
                        }
                    }
                    break;
                case Sys_Knowledge.ETypes.PetBook:
                    {
                        para.tipId = 1009;
                    }
                    break;
                case Sys_Knowledge.ETypes.Achievement:
                    {
                        para.tipId = 1008;
                        para.knowledgeId = knowledgeId;
                        CSVAchievement.Data data = CSVAchievement.Instance.GetConfData(knowledgeId);
                        if (data != null)
                        {
                            para.strContent = LanguageHelper.GetTextContent(2010331, LanguageHelper.GetAchievementContent(data.Achievement_Title));
                        }
                    }
                    break;
            }

            _ques.Enqueue(para);
        }
        #endregion
    }
}
