using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using Table;
using System.Json;

namespace Logic
{
    public partial class Sys_Trade : SystemModuleBase<Sys_Trade>
    {
        public class CateItem
        {
            public uint cateId;
            public List<uint> subCateIds;
        }

        private List<CateItem> cateList = new List<CateItem>();
        private List<CateItem> catePubList = new List<CateItem>();
        private List<CateItem> cateCrossList = new List<CateItem>();
        private List<CateItem> cateCrossPubList = new List<CateItem>();

        public void ParseCateData()
        {
            cateList.Clear();
            catePubList.Clear();

            foreach(var data in CSVCommodityList.Instance.GetAll())
            {
                if (data.trade_ID == 2u) //cross server
                {
                    if (data.type == 1u) //父分类
                    {
                        CateItem item = new CateItem();
                        item.cateId = data.id;

                        cateCrossList.Add(item);

                        //公示
                        if (data.show)
                        {
                            CateItem temp = new CateItem();
                            temp.cateId = data.id;

                            cateCrossPubList.Add(temp);
                        }
                    }
                    else
                    {
                        CateItem item = cateCrossList[cateCrossList.Count - 1];
                        if (item.subCateIds == null)
                            item.subCateIds = new List<uint>();

                        item.subCateIds.Add(data.id);

                        //公示
                        if (data.show)
                        {
                            CateItem temp = cateCrossPubList[cateCrossPubList.Count - 1];
                            if (temp.subCateIds == null)
                                temp.subCateIds = new List<uint>();
                            temp.subCateIds.Add(data.id);
                        }
                    }
                }
                else if (data.trade_ID == 1u)
                {
                    if (data.type == 1u) //父分类
                    {
                        CateItem item = new CateItem();
                        item.cateId = data.id;

                        cateList.Add(item);

                        //公示
                        if (data.show)
                        {
                            CateItem temp = new CateItem();
                            temp.cateId = data.id;

                            catePubList.Add(temp);
                        }
                    }
                    else
                    {
                        CateItem item = cateList[cateList.Count - 1];
                        if (item.subCateIds == null)
                            item.subCateIds = new List<uint>();

                        item.subCateIds.Add(data.id);

                        //公示
                        if (data.show)
                        {
                            CateItem temp = catePubList[catePubList.Count - 1];
                            if (temp.subCateIds == null)
                                temp.subCateIds = new List<uint>();
                            temp.subCateIds.Add(data.id);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 列表分类
        /// </summary>
        /// <returns></returns>
        public List<Widget_List_Trade.ItemList> GetTypesData(PageType pageType)
        {
            List<Widget_List_Trade.ItemList> temp = new List<Widget_List_Trade.ItemList>();

            List<CateItem> list;
            if (pageType == PageType.Buy)
            {
                if (CurBuyServerType == ServerType.Local)
                    list = cateList;
                else
                    list = cateCrossList;
            }
            else
            {
                if (CurPublicityServerType == ServerType.Local)
                    list = catePubList;
                else
                    list = cateCrossPubList;
            }
            //var list = pageType == PageType.Buy ? cateList : catePubList;

            uint index = 0;
            foreach (var data in list)
            {
                if (data.subCateIds != null)
                {
                    for (int i = 0; i < data.subCateIds.Count; ++i)
                    {
                        temp.Add(new Widget_List_Trade.ItemList()
                        {
                            id = ++index,
                            Group = data.cateId,
                            GroupName = CSVCommodityList.Instance.GetConfData(data.cateId).name,
                            Sub = data.subCateIds[i],
                            SubName = CSVCommodityList.Instance.GetConfData(data.subCateIds[i]).name,
                        });
                    }
                }
                else
                {
                    temp.Add(new Widget_List_Trade.ItemList()
                    {
                        id = ++index,
                        Group = data.cateId,
                        GroupName = CSVCommodityList.Instance.GetConfData(data.cateId).name,
                        //Sub = data.subCateIds[i],
                        //SubName = CSVCommodityList.Instance.GetConfData(data.subCateIds[i]).name,
                    });
                }
            }

            return temp;
        }

        public bool HasChild(uint cateId)
        {
            var list = CurPageType == PageType.Buy ? cateList : catePubList;

            bool isHasChild = false;
            foreach(var data in list)
            {
                if (cateId == data.cateId)
                {
                    if (data.subCateIds != null)
                    {
                        isHasChild = true;
                        break;
                    }
                }
            }

            return isHasChild;
        }

        public uint GetCateId(uint childCateId)
        {
            var list = CurPageType == PageType.Buy ? cateList : catePubList;

            uint cateId = 0;
            foreach (var data in list)
            {
                if (data.subCateIds != null && data.subCateIds.IndexOf(childCateId) >= 0)
                {
                    cateId = data.cateId;
                    break;
                }
            }

            return cateId;
        }
    }
}
