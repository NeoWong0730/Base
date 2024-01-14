using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using System;

namespace Logic
{

    public static class InstanceHelper
    {
        /// <summary>
        /// 在副本关卡表中，通过副本ID获得关卡
        /// </summary>
        /// <param name="instanceID">副本ID</param>
        /// <returns></returns>
        public static List<CSVInstanceDaily.Data>  getDailyByInstanceID(uint instanceID)
        {
            var datas = CSVInstanceDaily.Instance.GetAll();

            var list = new List<CSVInstanceDaily.Data>();

            foreach (var item in datas)
            {
                if (item.InstanceId == instanceID)
                {
                    list.Add(item);
                }
            }

            list.Sort((x, y) =>
            {
                if (x.LayerStage > y.LayerStage)
                    return 1;

                if (x.LayerStage < y.LayerStage)
                    return -1;

                if (x.Layerlevel > y.Layerlevel)
                    return 1;
                if (x.Layerlevel < y.Layerlevel)
                    return -1;

                return 0;
            });

            return list;
        }
    }
   

}
