using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using Table;
using Net;
using Packet;
using Lib.Core;
using UnityEngine;
using System;

namespace Logic
{
    public partial class Sys_Bag : SystemModuleBase<Sys_Bag>
    {
        public CSVCompose.Data GetComposeDataWhenCommon()
        {
            //List<uint> idList = new List<uint>(CSVComposeMenu.Instance.Count);
            //for (int i = 0; i < CSVComposeMenu.Instance.Count; i++)
            //{
            //    idList.Add(CSVComposeMenu.Instance[i].id);
            //}

            //List<uint> idList = new List<uint>(CSVComposeMenu.Instance.GetKeys());
            //
            //uint mainMenu = 1;
            //for (int i = 0; i < idList.Count; i++)
            //{
            //    mainMenu = Math.Min(mainMenu, idList[i]);
            //}

            //CSVComposeMenu.Data minMenu = CSVComposeMenu.Instance.GetConfData(mainMenu);

            CSVComposeMenu.Data minMenu = CSVComposeMenu.Instance.GetByIndex(0);
            if (null != minMenu)
            {
                var composeDatas = CSVCompose.Instance.GetAll();
                for (int i = 0, len = composeDatas.Count; i < len; i++)
                {
                    CSVCompose.Data composeData = composeDatas[i];
                    if (composeData.SynthesisTab == minMenu.SynthesisTab && composeData.Number == 1)
                    {
                        return composeData;
                    }
                }
            }
            return null;
        }

        public List<ItemIdCount> GetComposeItemId(uint composeId)
        {
            CSVCompose.Data data = CSVCompose.Instance.GetConfData(composeId);
            List<ItemIdCount> temp = new List<ItemIdCount>();
            if (null != data)
            {
                temp = CSVDrop.Instance.GetDropItem(data.drop_id);
            }
            return temp;
        }

        public List<uint> GetAllMenu()
        {
            List<uint> allIds = new List<uint>();

            var composeMenuDatas = CSVComposeMenu.Instance.GetAll();
            for (int i = 0, len = composeMenuDatas.Count; i < len; i++)
            {
                allIds.Add(composeMenuDatas[i].SynthesisTab);
            }
            return allIds;
        }

        public List<CSVCompose.Data>  SecondMenus(uint mainMenuId)
        {
            List<CSVCompose.Data>  temp = new List<CSVCompose.Data>();

            var composeDatas = CSVCompose.Instance.GetAll();
            for (int i = 0, len = composeDatas.Count; i < len; i++)
            {
                CSVCompose.Data composeData = composeDatas[i];
                if (composeData.SynthesisTab == mainMenuId)
                {
                    temp.Add(composeData);
                }
            }

            if (temp.Count > 1)
            {
                temp.Sort(Comp);
            }
            return temp;
        }

        private int Comp(CSVCompose.Data a, CSVCompose.Data b)
        {
            return a.Number.CompareTo(b.Number);
        }

}
}



