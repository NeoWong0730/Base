using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using Net;
using Packet;

namespace Logic
{
    public partial class Sys_Knowledge : SystemModuleBase<Sys_Knowledge>
    {
        private class GleaningType
        {
            public uint typeName;
            public List<uint> subTypes;
            public List<uint> subNames;
        }
        private Dictionary<uint, GleaningType> dictTypes = new Dictionary<uint, GleaningType>();

        private Dictionary<uint, Dictionary<uint, List<uint>>> dictGleanings = new Dictionary<uint, Dictionary<uint, List<uint>>>();

        public uint SelectGleaningItemId = 0;

        private void InitGleanings()
        {
            dictTypes.Clear();
            dictGleanings.Clear();

            foreach (var data in CSVGleanings.Instance.GetAll())
            {
                GleaningType typeTemp;
                if (dictTypes.TryGetValue(data.type_id, out typeTemp))
                {
                    if (!dictTypes[data.type_id].subTypes.Contains(data.type2_id))
                    {
                        dictTypes[data.type_id].subTypes.Add(data.type2_id);
                        dictTypes[data.type_id].subNames.Add(data.SubTypeName);
                    }
                }
                else
                {
                    typeTemp = new GleaningType();
                    typeTemp.typeName = data.TypeName;
                    typeTemp.subTypes = new List<uint>() { data.type2_id};
                    typeTemp.subNames = new List<uint>() { data.SubTypeName};

                    dictTypes.Add(data.type_id, typeTemp);
                }
               

                Dictionary<uint, List<uint>> temp;
                if (dictGleanings.TryGetValue(data.type_id, out temp))
                {
                    List<uint> listTemp;
                    if (dictGleanings[data.type_id].TryGetValue(data.type2_id, out listTemp))
                    {
                        dictGleanings[data.type_id][data.type2_id].Add(data.id);
                    }
                    else
                    {
                        listTemp = new List<uint>() { data.id };
                        dictGleanings[data.type_id].Add(data.type2_id, listTemp);
                    }
                }
                else
                {
                    temp = new Dictionary<uint, List<uint>>();
                    temp.Add(data.type2_id, new List<uint>() { data.id });

                    dictGleanings.Add(data.type_id, temp);
                }
            }
        }

        /// <summary>
        /// 沧海拾遗类型分类
        /// </summary>
        /// <returns></returns>
        public List<Widget_List_Left03.ItemList> GetGleaningsTypeData()
        {
            List<Widget_List_Left03.ItemList> temp = new List<Widget_List_Left03.ItemList>();

            uint index = 0;
            foreach (var data in dictTypes)
            {
                for (int i = 0; i < data.Value.subTypes.Count; ++i)
                {
                    temp.Add(new Widget_List_Left03.ItemList()
                    {
                        id = ++index,
                        Group = data.Key,
                        GroupName = data.Value.typeName,
                        Sub = data.Value.subTypes[i],
                        SubName = data.Value.subNames[i],
                    });
                }
            }

            return temp;
        }

        /// <summary>
        /// 根据分类,获得沧海拾遗
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="subTypeId"></param>
        /// <returns></returns>
        public List<uint> GetGleanings(uint typeId, uint subTypeId)
        {
            List<uint> listTemp = new List<uint>();

            Dictionary<uint, List<uint>> temp;
            if (dictGleanings.TryGetValue(typeId, out temp))
            {
                if (temp.TryGetValue(subTypeId, out listTemp))
                {
                    return listTemp;
                }
            }

            return listTemp;
        }

        /// <summary>
        /// 获得地理激活列表
        /// </summary>
        /// <param name="subType"></param>
        /// <returns></returns>
        public List<uint> GetGeoActiveList(uint subTypeId)
        {
            List<uint> temp = new List<uint>();

            List<uint> total = GetGleanings(1u, subTypeId);
            for (int i = 0; i < total.Count; ++i)
            {
                if (IsKnowledgeActive(total[i]))
                    temp.Add(total[i]);
            }

            return temp;
        }

        /// <summary>
        /// 沧海拾遗大分类红点
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public bool IsGlealingRed(uint typeId)
        {
            bool isRed = false;

            for (int i = 0; i < listDatas.Count; ++i)
            {
                if (listDatas[i].Type == (uint)ETypes.Gleanings)
                {
                    for (int j = 0; j < listDatas[i].ShowNewList.Count; ++j)
                    {
                        CSVGleanings.Data data = CSVGleanings.Instance.GetConfData(listDatas[i].ShowNewList[j]);
                        if (data != null)
                        {
                            if (data.type_id == typeId)
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
        /// 沧海拾遗小分类红点
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public bool IsSubGlealingRed(uint typeId, uint subTypeId)
        {
            bool isRed = false;

            for (int i = 0; i < listDatas.Count; ++i)
            {
                if (listDatas[i].Type == (uint)ETypes.Gleanings)
                {
                    for (int j = 0; j < listDatas[i].ShowNewList.Count; ++j)
                    {
                        CSVGleanings.Data data = CSVGleanings.Instance.GetConfData(listDatas[i].ShowNewList[j]);
                        if (data != null)
                        {
                            if (data.type_id == typeId && data.type2_id == subTypeId)
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
    }
}