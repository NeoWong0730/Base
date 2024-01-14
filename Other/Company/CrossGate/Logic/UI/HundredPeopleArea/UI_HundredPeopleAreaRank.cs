using System.Collections.Generic;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public partial class UI_HundredPeopleAreaRank : UIBase, UI_HundredPeopleAreaRank_Layout.IListener {
        #region 数据定义
        /// <summary> 百人道场界面布局 </summary>
        private UI_HundredPeopleAreaRank_Layout m_Layout = new UI_HundredPeopleAreaRank_Layout();
        /// <summary> 百人道场界面数据 </summary>
        private UI_HundredPeopleAreaRank_Data m_Data = new UI_HundredPeopleAreaRank_Data();
        #endregion
        #region 系统函数
        protected override void OnLoaded() {
            this.OnParseComponent();
        }
        protected override void OnShow() {
            this.RefreshView();
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件 
        /// </summary>
        private void OnParseComponent() {
            this.m_Layout.Load(this.gameObject.transform);
            this.m_Layout.SetListener(this);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        protected override void ProcessEventsForEnable(bool toRegister) {
            Sys_HundredPeopleArea.Instance.eventEmitter.Handle(Sys_HundredPeopleArea.EEvents.HundredPeopleRankInfoRes, this.OnHundredPeopleRankInfoRes, toRegister);
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView() {
            this.SetMenuView();
        }
        /// <summary>
        /// 设置菜单界面
        /// </summary>
        private void SetMenuView() {
            ECareerType[] values = (ECareerType[])System.Enum.GetValues(typeof(ECareerType));
            List<ECareerType> list = new List<ECareerType>(values);
            list.Remove(ECareerType.None);

            for (int i = 0, count = this.m_Layout.list_MenuItem.Count; i < count; i++) {
                Transform tr = this.m_Layout.list_MenuItem[i].transform;
                CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData((uint)list[i]);
                if (null == cSVCareerData) {
                    tr.gameObject.SetActive(false);
                }
                else {
                    tr.gameObject.SetActive(true);
                    this.m_Layout.SetMenuItem(i, cSVCareerData.name, cSVCareerData.icon, cSVCareerData.select_icon);
                }
            }
        }
        /// <summary>
        /// 设置排行界面
        /// </summary>
        private void SetRankView(Google.Protobuf.Collections.RepeatedField<Packet.TowerInsPassRankInfo> rankInfos) {
            this.m_Layout.CreateRankItemList(rankInfos == null ? 0 : rankInfos.Count);

            for (int i = 0, count = this.m_Layout.list_RankItem.Count; i < count; i++) {
                Transform tr = this.m_Layout.list_RankItem[i];
                if (i < rankInfos.Count) {
                    tr.gameObject.SetActive(true);
                    Packet.TowerInsPassRankInfo towerInsPassRankInfo = rankInfos[i];
                    this.m_Layout.SetRankItem(i, towerInsPassRankInfo.RoleName.ToStringUtf8(), towerInsPassRankInfo.PassedStage);
                }
                else {
                    tr.gameObject.SetActive(false);
                }
            }
        }
        /// <summary>
        /// 设置我的排行界面
        /// </summary>
        private void SetMyRankView(Packet.TowerInsPassRankInfo towerInsPassRankInfo, int rankIndex = -1) {
            if (null == towerInsPassRankInfo) {
                this.m_Layout.SetMyRank(rankIndex, Sys_Role.Instance.sRoleName, 0);
            }
            else {
                this.m_Layout.SetMyRank(rankIndex, towerInsPassRankInfo.RoleName.ToStringUtf8(), towerInsPassRankInfo.PassedStage);
            }
        }
        #endregion
        #region 响应事件
        public void OnClick_Close() {
            this.CloseSelf();
        }
        public void OnHundredPeopleRankInfoRes() {
            CmdTowerInstanceRankInfoRes res = Sys_HundredPeopleArea.Instance.cmdTowerInstanceRankInfoRes;
            int rankIndex = -1;
            for (int i = 0, count = res.RankInfos.Count; i < count; i++) {
                if (res.RankInfos[i].RoleId == res.SelfRankInfo.RoleId) {
                    rankIndex = i;
                    break;
                }
            }
            this.SetRankView(res.RankInfos);
            this.SetMyRankView(res.SelfRankInfo, rankIndex);
        }

        public void OnClick_Menu(Toggle toggle) {
            uint careerId;
            if (!uint.TryParse(toggle.name, out careerId))
                return;

            m_Layout.rt_MyRank.gameObject.SetActive(careerId == (uint)GameCenter.mainHero.careerComponent.CurCarrerType);
            Sys_HundredPeopleArea.Instance.SendTowerInstanceRankInfoReq(Sys_HundredPeopleArea.Instance.activityid, careerId);
        }
        #endregion
    }
}
