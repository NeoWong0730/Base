using Logic.Core;
using Lib.Core;
using Table;
using System.Collections.Generic;
using System;
using Logic;

namespace logic
{
    public class ImprintEntry
    {
        public List<ImprintNode> imprintList = new List<ImprintNode>();
        public ImprintNode endNode;
        public List<ImprintNode> startList = new List<ImprintNode>();
        public bool CheckStartListState()
        {
            bool isShow = true;
            for (int i = 0; i < startList.Count; i++)
            {
                if (startList[i].level>0)
                {
                    return false;
                }
                if (startList[i].isActive)
                {
                    isShow = (startList[i].level == 0);
                }
                else
                {
                    isShow = false;
                }
            }
            return isShow;
        }
        public void InitEntry()
        {
            startList.Clear();
            for (int i = 0; i < imprintList.Count; i++)
            {
                if (imprintList[i].csv.Node_Type == 1)
                {
                    startList.Add(imprintList[i]);
                }
                else if (imprintList[i].csv.Node_Type ==3)
                {
                    endNode = imprintList[i];
                }
            }
        }
        public void RefreshOneNode(uint _id,uint _level,bool _isActive)
        {
            GetOneNode(_id).Refresh(_level, _isActive);
        }
        public ImprintNode GetOneNode(uint _id)
        {
            for (int i=0;i< imprintList.Count;i++)
            {
                if (imprintList[i].id==_id)
                {
                    return imprintList[i];
                }
            }
            return null;
        }
        public ImprintNode GetNextNode(uint _id)
        {
            for (int i = 0; i < imprintList.Count; i++)
            {
                if (imprintList[i].id == _id && i != imprintList.Count - 1)
                {
                    return imprintList[i + 1];
                }
            }
            return null;
        }

    }
    public class ImprintNode
    {
        public uint id;
        public CSVImprintNode.Data csv;
        public uint level=0;
        public bool isActive=false;

        public ImprintNode(CSVImprintNode.Data csv)
        {
            this.id = csv.id;
            this.csv = csv;
            
        }
        public ImprintNode(uint id) : this(CSVImprintNode.Instance.GetConfData(id)) { }
        public ImprintNode(uint id, CSVImprintNode.Data csv) : this(csv) { }

        public void Refresh(uint _level,bool _isAcitve)
        {
            isActive = _isAcitve;
            level = _level;
            if (csv.Front_Node == null)
            {
                isActive = true;
            }
        }
        //得到对应位置
        public uint GetPositionIndex()
        {
            uint posIndex = 0;

            if (IsOdd(csv.Coordinate[0]))
            {
                posIndex = 11 * (csv.Coordinate[0] / 2) + (csv.Coordinate[1] / 2 - 1);
            }
            else
            {
                posIndex = 11 * (csv.Coordinate[0] / 2) + (csv.Coordinate[1] + 1) / 2 - 7;
            }


            return posIndex;
        }
        public bool ThisNodeIsMaxGrade()
        {
            if (level==csv.Level_Cap)
            {
                return true;
            }
            return false;
        }
      
        public bool IsNodeActive()
        {
            if (csv.Front_Node == null)
            {
                return true;
            }
            bool isActive = false;
            for (int i=0;i<csv.Front_Node.Count;i++)
            {
                uint _pre = GetPreNodeId(i);//获得前置节点id
                if (Sys_TravellerAwakening.Instance.severDicSec.ContainsKey(_pre))
                {
                    if (Sys_TravellerAwakening.Instance.severDicSec[_pre] >= GetPreActiveGrade(i))
                    {
                        isActive=true;
                    }
                }

            }
            return isActive;
        }
        //得到前置节点id
        public uint GetPreNodeId(int _index)
        {
            if (csv.Front_Node == null)
            {
                return 0;
            }
            string str = csv.Front_Node[_index].ToString();
            int id = int.Parse(str.Substring(0, str.Length-2));

            return (uint)id;

        }
        // 得到该节点激活的前置节点等级
        public int GetPreActiveGrade(int _index)
        {
            if (csv.Front_Node == null)
            {
                DebugUtil.LogError("csv.Front_Node is null");
            }
            string str = csv.Front_Node[_index].ToString();
            int level = int.Parse(str.Substring(str.Length - 2, 2));
            return level;
        }
        

        private bool IsOdd(uint num)
        {
            return Convert.ToBoolean(num % 2);
        }



    }

}

