using Framework;
using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public class Sys_WelfareMenu : SystemModuleBase<Sys_WelfareMenu>
    {
        public Dictionary<uint, List<CSVWelfareMenu.Data>> MenuType = new Dictionary<uint, List<CSVWelfareMenu.Data>>();

        #region 系统函数
        public override void Init()
        {
            InitData();
            ProcessEvents(true);
        }
        public override void Dispose()
        {
            MenuType.Clear();
            ProcessEvents(false);
        }

        public override void OnLogin()
        {
        }
        public override void OnLogout()
        {
        }
        #endregion

        private void ProcessEvents(bool v)
        {

        }

        private void InitData()
        {
            MenuType.Clear();
            var menuList = CSVWelfareMenu.Instance.GetAll();
            for (int i = 0; i < menuList.Count; i++)
            {
                CSVWelfareMenu.Data data = menuList[i];
                if (!MenuType.ContainsKey(data.menuId))
                {
                    List<CSVWelfareMenu.Data> list = new List<CSVWelfareMenu.Data>();
                    list.Add(data);
                    MenuType.Add(data.menuId, list);
                }
                else
                    MenuType[data.menuId].Add(data);

            }
        }

        /// <summary>
        /// 创建活动脚本实例
        /// </summary>
        public UI_OperationalActivityBase CreateOperationUIComponent(EOperationalActivity eType)
        {
            switch (eType)
            {
                case EOperationalActivity.SevenDaysSign:
                    return new UI_SevenDaysSign();
                case EOperationalActivity.LevelGift:
                    return new UI_LevelGift();
                case EOperationalActivity.ActivitySubscribe:
                    return new UI_ActivitySubscribe();
                case EOperationalActivity.GrowthFund:
                    return new UI_GrowthFund();
                case EOperationalActivity.TotalCharge:
                    return new UI_TotalCharge();
                case EOperationalActivity.SpecialCard:
                    return new UI_SpecialCard();
                case EOperationalActivity.DailySign:
                    return new UI_DailySign();
                case EOperationalActivity.LotteryActivity:
                    return new UI_LotteryActivity();
                case EOperationalActivity.DailyGift:
                    return new UI_DailyGift();
                case EOperationalActivity.PhoneBindGift:
                    return new UI_PhoneBindGift();
                case EOperationalActivity.Qa:
                    return new UI_OperationQa();
                case EOperationalActivity.RankActivity:
                    return new UI_RankActivity();
                case EOperationalActivity.AddQQGroup:
                    return new UI_AddQQGroup();
                case EOperationalActivity.ExpRetrieve:
                    return new UI_ExpRetrieve();
                case EOperationalActivity.Rebate:
                    return new UI_TopUpRebate();
                case EOperationalActivity.Alipay:
                    return new UI_AlipayActivity();
                case EOperationalActivity.OneDollar:
                    return new UI_FightTreasure_OneDollar();
                case EOperationalActivity.HundredDollar:
                    return new UI_FightTreasure_HundredDollar();
                case EOperationalActivity.PedigreedDraw:
                    return new UI_PedigreedDraw();
                case EOperationalActivity.ActivityReward:
                    return new UI_ActivityReward();
                case EOperationalActivity.ChargeAB:
                    return new UI_ChargeAB();
                case EOperationalActivity.KingPet:
                    return new UI_KingPetActivity();
                case EOperationalActivity.SinglePay:
                    return new UI_SinglePay();
                case EOperationalActivity.BackAssist:
                    return new UI_BackAssist();
                case EOperationalActivity.ActivityTotalCharge:
                    return new UI_ActivityTotalCharge();
                case EOperationalActivity.ActivityTotalConsume:
                    return new UI_ActivityTotalConsume();
                case EOperationalActivity.SinglePayHeFu:
                    return new UI_SinglePayHeFu();
            }
            return null;
        }

        /// <summary>
        /// 创建活动节点(预设)实例
        /// 新增节点的时候，需要手动将预设体添加到,福利UIprefab根节点下的AssetDependencies脚本内
        /// </summary>
        public GameObject CreateOperationCellGameobject(string prefabNode)
        {
            Transform transform = UIManager.GetUI((int)EUIID.UI_OperationalActivity).transform;
            var goList = transform.GetComponent<AssetDependencies>().mCustomDependencies;
            Transform tParent = transform.Find("Animator");
            for (int i = 0; i < tParent.childCount; i++)
            {
                Transform cell = tParent.GetChild(i);
                if (cell.name == prefabNode)
                {
                    return cell.gameObject;
                }
            }
            //在现有列表里没找到，则实例化
            for (int i = 0; i < goList.Count; i++)
            {
                var prefabCell = goList[i] as GameObject;
                if(prefabCell.name == prefabNode)
                {
                    var goChild = FrameworkTool.CreateGameObject(prefabCell, tParent.gameObject);
                    //新增节点设置列表位置，以免层级太高遮挡其他非节点UI
                    if (tParent.childCount > 3)//防止报错
                    {
                        goChild.transform.SetSiblingIndex(3);
                    }
                    return goChild;
                }
            }
            return null;
        }
    }
}