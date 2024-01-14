using System.Collections.Generic;
using Logic.Core;
using Packet;
using Net;
using Lib.Core;
using Table;
using logic;
using UnityEngine;
using System;
using System.Linq;
using Framework;

namespace Logic
{
    public class AttrInfo
    {
        public uint attrId;
        public uint attrNum;
        public AttrInfo(uint _id, uint _value)
        {
            attrId = _id;
            attrNum = _value;
        }
    }
    public class ImprintAttrEntry
    {
        public Dictionary<uint, uint> attrDic = new Dictionary<uint, uint>();
        public List<AttrInfo> aList = new List<AttrInfo>();
        public void AddAttrItem(uint _id, uint _value)
        {
            if (attrDic.ContainsKey(_id))
            {
                attrDic[_id] += _value;
            }
            else
            {
                attrDic[_id] = _value;
            }
            
        }
        private void SortDictionary()
        {
            attrDic = attrDic.OrderBy(p => p.Key).ToDictionary(p=>p.Key,o=>o.Value);
        }

        public void InitAttrList()
        {
            SortDictionary();
            foreach (var item in attrDic)
            {
                aList.Add(new AttrInfo(item.Key,item.Value));
            }
        }
        public void AutoClear()
        {
            aList.Clear();
            attrDic.Clear();
        }
    }

    public partial class Sys_TravellerAwakening : SystemModuleBase<Sys_TravellerAwakening>
    {
        public Dictionary<uint, ImprintEntry> imprintDic = new Dictionary<uint, ImprintEntry>();
        public Dictionary<uint,uint> severDic = new Dictionary<uint,uint>();//印记等级表id，等级
        public Dictionary<uint, uint> severDicSec = new Dictionary<uint, uint>();//印记节点表id，等级
        public Dictionary<uint, ImprintAttrEntry> iAttrDic=new Dictionary<uint, ImprintAttrEntry>();//统计加成总览
        public List<bool> imprintLabelRedPointList = new List<bool>();//觉醒印记红点表
        public uint SelectIndex = 0;
        public int panelOrder;
        public ImprintNode nowNode {
            get;
            set;
        }
        public ImprintNode nextNode
        {
            get;
            set;
        }
        public bool CheckUIMenuImprintRedPoint()//检查界面是否显示觉醒印记红点
        {
            if (!CheckAwakenCondition())
            {
                return false;
            }
            for (int i=0;i< imprintLabelRedPointList.Count;i++)
            {
                if (imprintLabelRedPointList[i])
                {
                    return true;
                }
            }
            return false;
        }
        public void InitImprintLabelRedPoint()//构建觉醒印记标签红点
        {
            imprintLabelRedPointList.Clear();
            for (int i=1;i<=CSVImprintLabel.Instance.Count;i++)
            {
                if (imprintDic.ContainsKey((uint)i))
                {
                    imprintLabelRedPointList.Add(imprintDic[(uint)i].CheckStartListState());
                }
                else
                {
                    imprintLabelRedPointList.Add(false);
                }
            }

        }

        public void ImprintLabelRedPointDayCheck()//构建觉醒印记标签红点（每日只出现一次红点提醒）
        {
            imprintLabelRedPointList.Clear();
            for (int i = 1; i <= CSVImprintLabel.Instance.Count; i++)
            {
                if (imprintDic.ContainsKey((uint)i))
                {
                    bool isShow = imprintDic[(uint)i].CheckStartListState() && CheckCanRedPointShow()&&IsShowRedpoint;
                    imprintLabelRedPointList.Add(isShow);
                }
                else
                {
                    imprintLabelRedPointList.Add(false);
                }
            }
            eventEmitter.Trigger(EEvents.OnAwakeRedPoint);

        }

        private bool CheckCanRedPointShow()
        {
            bool isRed = Sys_Time.IsServerSameDay5(Sys_Role.Instance.lastLoginTime, Sys_Time.Instance.GetServerTime());
            return !isRed;
        }
        public void InitImprintDictionary()//构建觉醒印记字典
        {
            imprintDic.Clear();
            imprintDic.Add(1, new ImprintEntry());
            for (int i = 2; i <= awakeLevel - GetAwakenOpenLevel(); i++)
            {
                imprintDic.Add((uint)i, new ImprintEntry());
            }
            InitImprintEntry(1,true);
            InitImprintAttrDictionary();
        }
        public void InitImprintEntry(int _index,bool isCheckRed)//构建觉醒印记字典里的实体
        {
            for (int i = _index; i <= imprintDic.Count; i++)
            {
                imprintDic.TryGetValue((uint)i, out ImprintEntry iEntry);
                iEntry.imprintList = InitImprintEntryList((uint)i);
                iEntry.InitEntry();
            }
            if (isCheckRed)
            {
                ImprintLabelRedPointDayCheck();
            }
            else
            {
                InitImprintLabelRedPoint();
            }
            
        }

        public List<ImprintNode> InitImprintEntryList(uint id)//构建觉醒印记字典里实体的list
        {
            List<ImprintNode> iList = new List<ImprintNode>();

            var dataList = CSVImprintNode.Instance.GetAll();
            for (int i = 0, len = dataList.Count; i < len; i++)
            {
                CSVImprintNode.Data iData = dataList[i];
                if (iData.Label_Id == id)
                {
                    ImprintNode _node = new ImprintNode(iData.id);
                    bool _isActive = _node.IsNodeActive();
                    uint level = 0;
                    if (severDicSec.ContainsKey(iData.id))
                    {
                        level = severDicSec[iData.id];
                    }
                    _node.Refresh(level, _isActive);
                    iList.Add(_node);
                }
                if (iData.Label_Id == id + 1)
                {
                    break;
                }

            }
            return iList;
        }
        public void InitImprintAttrDictionary()//构建觉醒印记加成字典
        {
            iAttrDic.Clear();
            for (int i = 1; i <= 3; i++)
            {
                iAttrDic.Add((uint)i, new ImprintAttrEntry());
            }
            InitImprintAttrEntry();
            InitAttrDictionary();
            EntryInitAttrList();
        }
        public void InitImprintAttrEntry()//构建觉醒印记加成字典实体
        {
            for (int i = 1; i <= iAttrDic.Count; i++)
            {
                iAttrDic.TryGetValue((uint)i, out ImprintAttrEntry aEntry);
                aEntry.attrDic = InitAttrEntryDictionary((uint)i);
            }

        }
        public void EntryInitAttrList()//构建觉醒印记加成字典实体里的list
        {
            for (int i = 1; i <= 3; i++)
            {
                GetImprintAttrEntry((uint)i).InitAttrList();
            }
        }
        public void InitAttrDictionary()//构建觉醒印记加成字典实体里的Dic
        {
            foreach(var item in severDic)
            {
                
                CSVImprintUpgrade.Data iNode = CSVImprintUpgrade.Instance.GetConfData(item.Key);
                for (int i=0;i<iNode.Target_Type.Count;i++)
                {
                    for (int j=0;j< iNode.Attribute_Bonus.Count;j++)
                    {
                        GetImprintAttrEntry(iNode.Target_Type[i]).AddAttrItem(iNode.Attribute_Bonus[j][0],iNode.Attribute_Bonus[j][1]);
                    }
                    
                }

            }
        }
        public Dictionary<uint, uint> InitAttrEntryDictionary(uint id)
        {//创建觉醒印记加成字典实体里的Dic
            Dictionary<uint, uint> aDic = new Dictionary<uint, uint>();
            return aDic;
        }
        public ImprintAttrEntry GetImprintAttrEntry(uint id)
        {
            iAttrDic.TryGetValue(id, out ImprintAttrEntry aEntry);
            return aEntry;
        }
        public ImprintEntry GetImprintEntry(uint id)
        {
            imprintDic.TryGetValue(id, out ImprintEntry iEntry);
            return iEntry;  
        }
        public List<ImprintNode> GetImprintEntryList(uint id)
        {
            return GetImprintEntry(id).imprintList;
        }
        private void UpdateOneImprintEntry(uint _labelId)//更新某个觉醒印记里的实体
        {
            GetImprintEntry(_labelId).imprintList = InitImprintEntryList(_labelId);
            GetImprintEntry(_labelId).InitEntry();
        }
        private void ImprintRunesUpdate(uint _levelId)//更新某个节点信息
        {
            string str = _levelId.ToString();
            int level = int.Parse(str.Substring(str.Length - 2, 2));
            int id = int.Parse(str.Substring(0, str.Length - 2));
            foreach (var item in severDic)
            {
                CSVImprintUpgrade.Data uData = CSVImprintUpgrade.Instance.GetConfData(item.Key);
                if (uData.Node_ID==(uint)id)
                {
                    if (severDic[item.Key] < level)
                    {
                        severDic.Remove(item.Key);
                        break;
                    }
                }
            }
            severDicSec[(uint)id] = (uint)level;
            severDic[_levelId] = (uint)level;
        }
        public Color FrameShow(uint type)//获得文字表颜色
        {
            CSVWordStyle.Data sData = CSVWordStyle.Instance.GetConfData(type);
             return sData.FontColor;
        }
        public int GetAwakenOpenLevel()
        {
            CSVImprintLabel.Data lData = CSVImprintLabel.Instance.GetConfData((uint)CSVImprintLabel.Instance.Count);
            return (int)(lData.Title_Des- lData.id);

        }
        public bool CheckAwakenCondition()
        {
            var cSVCheckseq = CSVCheckseq.Instance.GetConfData(51402);
            if (cSVCheckseq != null&&cSVCheckseq.IsValid())
            {
                return true;
            }
            return false;
        }

        public string AwakenImprintErrorText()
        {//ATTENTION:特殊处理，如果条件表改，这里的提示也要改
            CSVCheckseq.Data cData = CSVCheckseq.Instance.GetConfData(51402);
            string ErrorTips = string.Empty;
            bool islevel = Sys_Role.Instance.Role.Level >= cData.CheckCondi1[0][1];
            //bool isTask = Sys_Task.Instance.IsSubmited((uint)cData.CheckCondi1[1][1]);
            if (!islevel)
            {
                string strParameter1 = (cData.CheckCondi1[0][1]).ToString();
                ErrorTips = LanguageHelper.GetTextContent(1004030, strParameter1);
            }
            else
            {
                CSVTask.Data cSVTaskData = CSVTask.Instance.GetConfData((uint)cData.CheckCondi1[1][1]);
                string strParameter1 = LanguageHelper.GetTaskTextContent(cSVTaskData.taskName);
                ErrorTips = LanguageHelper.GetTextContent(1004031, strParameter1);


            }

            return ErrorTips;

        }
        public bool CheckAwakenImprintFirstOpenAnim()
        {

            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "AwakenImprint";
            if (!PlayerPrefs.HasKey(key))
            {
                return true;
            }
            return false;
        }
        public void SetAwakenImprintFirstOpenAnim()
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "AwakenImprint";
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 1);
            }
        }
    }

}
