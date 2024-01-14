using System.Collections.Generic;
using System.Json;
using Logic.Core;


namespace Logic
{
    public partial class Sys_Knowledge : SystemModuleBase<Sys_Knowledge>
    {
        /// <summary>
        /// 是否显示红点
        /// </summary>
        /// <returns></returns>
        public bool IsRedPoint()
        {
            bool red = false;
            for(int i = 0; i < listDatas.Count; ++i)
            {
                if (listDatas[i].ShowNewList.Count > 0)
                {
                    red = true;
                    break;
                }
            }

            if (Sys_Achievement.Instance.CheckRewardRedPoint())
                red = true;
            //if (!red) //判断图册
            //{
            //    Sys_Cooking.Instance.UpdateAllCookingSubmitState();
            //    Sys_Cooking.Instance.UpdateRewards();
            //    red = Sys_Cooking.Instance.HasSubmitItem(0) || Sys_Cooking.Instance.HasReward();
            //}
            return red;
        }

        /// <summary>
        /// 根据类型判断是否需要显示红点
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsRedPointByType(ETypes type)
        {
            bool isRed = false;

            //if (type == ETypes.Cooking)
            //{
            //    Sys_Cooking.Instance.UpdateAllCookingSubmitState();
            //    Sys_Cooking.Instance.UpdateRewards();
            //    return Sys_Cooking.Instance.HasSubmitItem(0) || Sys_Cooking.Instance.HasReward();
            //}
            if (type == ETypes.Achievement)
            {
                return Sys_Achievement.Instance.CheckRewardRedPoint();
            }

            for (int i = 0; i < listDatas.Count; ++i)
            {
                if (listDatas[i].Type == (uint)type && listDatas[i].ShowNewList.Count > 0)
                {
                    isRed = true;
                    break;
                }
            }

            return isRed;
        }

        /// <summary>
        /// 判断记忆碎片是否有红点
        /// </summary>
        /// <param name="fragId"></param>
        /// <returns></returns>
        public bool IsFragRedPoint(uint fragId)
        {
            bool isRed = false;

            List<uint> temp;
            if (dictFragments.TryGetValue(fragId, out temp))
            {
                for (int i = 0; i < listDatas.Count; ++i)
                {
                    if (listDatas[i].Type == (uint)ETypes.Fragment
                        && listDatas[i].ShowNewList.Count > 0)
                    {
                        for (int j = 0; j < temp.Count; ++j)
                        {
                            if (listDatas[i].ShowNewList.IndexOf(temp[j]) >= 0)
                            {
                                isRed = true;
                                break;
                            }
                        }
                    }
                }
            }

            return isRed;
        }

        /// <summary>
        /// 小知识是否红点显示
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <returns></returns>
        public bool IsRedPointByKnowledge(uint knowledgeId)
        {
            bool isRed = false;

            for (int i = 0; i < listDatas.Count; ++i)
            {
                if (listDatas[i].ShowNewList.IndexOf(knowledgeId) >= 0)
                {
                    isRed = true;
                    break;
                }
            }

            return isRed;
        }

        /// <summary>
        /// 删除UIKnowledge,表示已经点击过
        /// </summary>
        /// <param name="knowledgeId"></param>
        public void OnDelNewKnowledge(ETypes type, uint knowledgeId)
        {
            bool del = false; ;
            for (int i = 0; i < listDatas.Count; ++i)
            {
                if (listDatas[i].Type == (uint)type)
                {
                    if (listDatas[i].ShowNewList.IndexOf(knowledgeId) >= 0)
                    {
                        del = true;
                        listDatas[i].ShowNewList.Remove(knowledgeId);
                        break;
                    }
                }
            }

            if (del)
            {
                OnClearNewKnoledge(type, knowledgeId);

                eventEmitter.Trigger(EEvents.OnDelNewKnowledgeNtf);
            }
        }
    }
}