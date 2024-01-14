using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using Packet;
using Google.Protobuf.Collections;

namespace Logic
{
    public class Cooking
    {
        public uint id { get; private set; }

        public CSVCook.Data cSVCookData;

        public uint cookType { get; private set; }//烹饪类型 1:一个阶段 2：两个阶段 3：三个阶段

        public uint foodType { get; private set; }  //类别 1：主食 2：炒菜 3：汤 4：点心

        public uint sort { get; private set; }

        public bool watch { get; private set; } //关注


        private int m_Active;//是否可以按食谱激活（1 :激活)

        public int active
        {
            get { return m_Active; }
            set
            {
                if (m_Active != value)
                {
                    m_Active = value;
                    Sys_Cooking.Instance.eventEmitter.Trigger<uint>(Sys_Cooking.EEvents.OnActiveCook, id);
                }
            }
        }

        private List<uint> m_SubmitItems = new List<uint>();//已上交道具

        private uint m_ConfigFood;

        public List<uint> submitConfigItems = new List<uint>();//配置的可上交道具

        private Dictionary<uint, uint> m_SubmitConfigItemMap = new Dictionary<uint, uint>(); //id,积分

        public List<uint> stage1Items = new List<uint>();

        public List<uint> stage2Items = new List<uint>();

        public int submitIndex = -1;

        public bool allSubmit
        {
            get { return m_SubmitItems.Count == 4; }
        }

        public bool allSubmitExceptFood
        {
            get
            {
                if (m_SubmitItems.Count == 4)//都提交了
                {
                    return true;
                }
                if (m_SubmitItems.Count == 3 && !m_SubmitItems.Contains(m_ConfigFood))//除了食谱 ,其他都提交了
                {
                    return true;
                }
                return false;
            }
        }


        public Cooking(CSVCook.Data data)//(uint _id)
        {
            //id = _id;
            //cSVCookData = CSVCook.Instance.GetConfData(_id);

            cSVCookData = data;
            id = cSVCookData.id;

            m_Active = 0;
            cookType = cSVCookData.cook_type;
            sort = cSVCookData.sort;
            foodType = cSVCookData.food_type;
            m_ConfigFood = cSVCookData.cookbook[0];

            submitConfigItems.Add(cSVCookData.cookbook[0]);
            m_SubmitConfigItemMap.Add(cSVCookData.cookbook[0], cSVCookData.cookbook[1]);
            for (int i = 0; i < cSVCookData.submit_item.Count; i++)
            {
                submitConfigItems.Add(cSVCookData.submit_item[i][0]);
                m_SubmitConfigItemMap.Add(cSVCookData.submit_item[i][0], cSVCookData.submit_item[i][1]);
            }
            CalFoods();
        }

        private void CalFoods()
        {
            if (cookType == 1)
            {
                for (int i = 0; i < cSVCookData.food1.Count; i++)
                {
                    for (int j = 0; j < cSVCookData.food1[i][1]; j++)
                    {
                        stage1Items.Add(cSVCookData.food1[i][0]);
                    }
                }
            }
            else if (cookType == 2)
            {
                for (int i = 0; i < cSVCookData.food1.Count; i++)
                {
                    for (int j = 0; j < cSVCookData.food1[i][1]; j++)
                    {
                        stage1Items.Add(cSVCookData.food1[i][0]);
                    }
                }
                for (int i = 0; i < cSVCookData.food2.Count; i++)
                {
                    for (int j = 0; j < cSVCookData.food2[i][1]; j++)
                    {
                        stage2Items.Add(cSVCookData.food2[i][0]);
                    }
                }
            }
        }

        public bool CanMake(uint tool, uint stage, List<uint> items)
        {
            if (stage == 1)
            {
                if (tool != cSVCookData.tool1)
                {
                    return false;
                }
                List<uint> tmp1 = new List<uint>(stage1Items);
                List<uint> tmp2 = new List<uint>(items);
                bool res = tmp1.EqualList<uint>(tmp2, (a, b) => { return a == b; });
                return res;
            }
            else if (stage == 2)
            {
                if (tool != cSVCookData.tool2)
                {
                    return false;
                }
                List<uint> tmp1 = new List<uint>(stage2Items);
                List<uint> tmp2 = new List<uint>(items);
                bool res = tmp1.EqualList<uint>(tmp2, (a, b) => { return a == b; });
                return res;
            }
            else
            {
                return false;
            }
        }

        public void SetActive(int active)
        {
            this.active = active;
        }

        public void SetWatch(bool watch)
        {
            this.watch = watch;
        }

        public void SetSubmitItems(RepeatedField<uint> submitIds)
        {
            m_SubmitItems.Clear();
            for (int i = 0; i < submitIds.Count; i++)
            {
                m_SubmitItems.Add(submitIds[i]);
                if (submitIds[i] == submitConfigItems[0])
                {
                    SetActive(1);
                }
            }
            UpdateSubmitableItem();
        }

        public void ClearData()
        {
            m_SubmitItems.Clear();
            SetActive(0);
        }

        public void AddSubmitItem(uint submitId)
        {
            if (m_SubmitItems.Contains(submitId))
            {
                DebugUtil.Log(ELogType.eCooking, $"已提交列表里面已经存在{submitId}");
                return;
            }
            if (submitId == submitConfigItems[0])
            {
                SetActive(1);
                //Sys_Cooking.Instance.SortCooking();
            }
            m_SubmitItems.Add(submitId);
            UpdateSubmitableItem();
            Sys_Cooking.Instance.eventEmitter.Trigger(Sys_Cooking.EEvents.OnRefreshLeftSubmitRedPoint);
        }

        public void UpdateSubmitableItem()
        {
            submitIndex = -1;
            if (m_SubmitItems.Count == 4)//都提交了
            {
                submitIndex = -1;
            }
            if (m_SubmitItems.Count == 3 && !m_SubmitItems.Contains(m_ConfigFood))//除了食谱 ,其他都提交了
            {
                submitIndex = -1;
            }
            else
            {
                for (int i = 0; i < submitConfigItems.Count; i++)
                {
                    uint itemId = submitConfigItems[i];
                    if (m_ConfigFood == itemId)
                    {
                        continue;
                    }
                    if (!m_SubmitItems.Contains(itemId))
                    {
                        long itemCount = Sys_Bag.Instance.GetItemCount(itemId);
                        if (itemCount > 0)
                        {
                            submitIndex = i;
                            break;
                        }
                    }
                }
            }
            Sys_Cooking.Instance.eventEmitter.Trigger<uint>(Sys_Cooking.EEvents.OnUpdateBookRightSubmitState, id);
        }

        public bool CanSubmit()
        {
            for (int i = 0; i < submitConfigItems.Count; i++)
            {
                uint itemId = submitConfigItems[i];
                if (!m_SubmitItems.Contains(itemId))
                {
                    long itemCount = Sys_Bag.Instance.GetItemCount(itemId);
                    if (itemCount > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //0:可上交 1:已上交 -1:道具不足 不能上交
        public int GetSubmitState(int index)
        {
            int state = 0;
            uint itemId = submitConfigItems[index];
            if (m_SubmitItems.Contains(itemId))
            {
                state = 1;
            }
            else
            {
                long itemCount = Sys_Bag.Instance.GetItemCount(itemId);
                state = itemCount > 0 ? 0 : -1;
            }
            return state;
        }

        public uint GetScore(int index)
        {
            uint itemId = submitConfigItems[index];
            uint score = 0;
            m_SubmitConfigItemMap.TryGetValue(itemId, out score);
            return score;
        }

        /// <summary>
        /// 提交道具
        /// </summary>
        public void Submit()
        {
            uint itemId = submitConfigItems[submitIndex];
            Sys_Cooking.Instance.CookCollectReq(id, itemId);
        }

        public void Submit(int index)
        {
            uint itemId = submitConfigItems[index];
            if (m_SubmitItems.Contains(itemId))
            {
                string content = LanguageHelper.GetTextContent(5921);
                Sys_Hint.Instance.PushContent_Normal(content);
            }
            else
            {
                long itemCount = Sys_Bag.Instance.GetItemCount(itemId);
                if (itemCount == 0)
                {
                    string content = LanguageHelper.GetTextContent(5901);
                    Sys_Hint.Instance.PushContent_Normal(content);
                }
                else
                {
                    Sys_Cooking.Instance.CookCollectReq(id, itemId);
                }
            }
        }
    }
}


