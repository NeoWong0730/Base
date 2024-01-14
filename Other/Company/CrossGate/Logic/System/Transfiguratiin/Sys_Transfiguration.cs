using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System.Collections.Generic;
using Table;

namespace Logic
{
    public class ClientShapeShiftPlan
    {
        public int index;
        public string name;
        public List<uint> shapeShiftSubNodeIds = new List<uint>();
        public Dictionary<uint, ShapeShiftSubNode> shapeShiftRaceSubNodes = new Dictionary<uint, ShapeShiftSubNode>();
        public Dictionary<uint, ShapeShiftSkillGrid> allRaceSkillGrids = new Dictionary<uint, ShapeShiftSkillGrid>();

        public void SetSubData(ShapeShiftPlan shapeShiftPlan)
        {
            shapeShiftSubNodeIds.Clear();
            shapeShiftRaceSubNodes.Clear();
            allRaceSkillGrids.Clear();
            ShapeShiftSubNode allRaceNode = null;
            for (int i = 0; i < shapeShiftPlan.Subnodes.Count; ++i)
            {
                if (shapeShiftPlan.Subnodes[i].Subnodeid == 0)
                {
                    allRaceNode = shapeShiftPlan.Subnodes[i];
                }
                else
                {
                    shapeShiftSubNodeIds.Add(shapeShiftPlan.Subnodes[i].Subnodeid);
                    CSVRaceDepartmentResearch.Data csvData = CSVRaceDepartmentResearch.Instance.GetConfData(shapeShiftPlan.Subnodes[i].Subnodeid);
                    if (csvData != null)
                    {
                        shapeShiftRaceSubNodes.Add(csvData.type, shapeShiftPlan.Subnodes[i]);
                    }
                }
            }
            if (allRaceNode != null)
            {
                for (int i = 0; i < allRaceNode.Grids.Count; ++i)
                {
                    allRaceSkillGrids.Add(allRaceNode.Grids[i].Gridid, allRaceNode.Grids[i]);
                }
            }
        }

        public void RefreshSubNode(uint formernodeid, uint currentnodeid, uint type)
        {
            if (shapeShiftSubNodeIds.Contains(formernodeid))
            {
                shapeShiftSubNodeIds.Remove(formernodeid);
            }
            shapeShiftSubNodeIds.Add(currentnodeid);
            if (shapeShiftRaceSubNodes.ContainsKey(type))
            {
                shapeShiftRaceSubNodes[type].Subnodeid = currentnodeid;
            }
            else
            {
                ShapeShiftSubNode node = new ShapeShiftSubNode();
                node.Subnodeid = currentnodeid;
                shapeShiftRaceSubNodes.Add(type, node);
            }
        }

        public void RefershSkillGrid(uint currentnodeid, uint type, ShapeShiftSkillGrid shapeShiftSkillGrid)
        {
            if (shapeShiftRaceSubNodes.ContainsKey(type))
            {
                if (shapeShiftSkillGrid != null)
                {
                    shapeShiftRaceSubNodes[type].Grids.Add(shapeShiftSkillGrid);
                    UIManager.OpenUI(EUIID.UI_Transfiguration_Result, false, shapeShiftSkillGrid);
                }
            }
            else
            {
                ShapeShiftSubNode node = new ShapeShiftSubNode();
                node.Subnodeid = currentnodeid;
                if (shapeShiftSkillGrid != null)
                {
                    node.Grids.Add(shapeShiftSkillGrid);
                }
                shapeShiftRaceSubNodes.Add(type, node);
            }
        }

         public void ResetSubNode(uint formernodeid)
        {
            if (shapeShiftSubNodeIds.Contains(formernodeid))
            {
                shapeShiftSubNodeIds.Remove(formernodeid);
            }
            CSVRaceDepartmentResearch.Data data = CSVRaceDepartmentResearch.Instance.GetConfData(formernodeid);
            if (shapeShiftRaceSubNodes.ContainsKey(data.type))
            {
                shapeShiftRaceSubNodes.Remove(data.type);
            }
        }

        public void UnlockAllRaceSkill(ShapeShiftSkillGrid shapeShiftSkillGrid)
        {
            if (allRaceSkillGrids.ContainsKey(shapeShiftSkillGrid.Gridid))
            {
                allRaceSkillGrids[shapeShiftSkillGrid.Gridid] = shapeShiftSkillGrid;
            }
            else
            {
                allRaceSkillGrids.Add(shapeShiftSkillGrid.Gridid, shapeShiftSkillGrid);
            }
            UIManager.OpenUI(EUIID.UI_Transfiguration_Unlock_Result, false, shapeShiftSkillGrid);
            Sys_Transfiguration.Instance.eventEmitter.Trigger<uint>(Sys_Transfiguration.EEvents.OnRefreshSkill, shapeShiftSkillGrid.Gridid);
        }

        public Dictionary<uint, ShapeShiftSkillGrid> GetSkillGridByRaceIdByServer(uint raceId)
        {
            Dictionary<uint, ShapeShiftSkillGrid> dic = new Dictionary<uint, ShapeShiftSkillGrid>();
            if (shapeShiftRaceSubNodes.ContainsKey(raceId))
            {
                ShapeShiftSubNode node = shapeShiftRaceSubNodes[raceId];
                for (int i = 0; i < node.Grids.Count; ++i)
                {
                    dic.Add(node.Grids[i].Gridid, node.Grids[i]);
                }
            }
            return dic;
        }

        public ShapeShiftSkillGrid GetReplaceSkillGridByRaceId(uint raceId)
        {
            ShapeShiftSkillGrid skillGride = null;
            if (shapeShiftRaceSubNodes.ContainsKey(raceId))
            {
                ShapeShiftSubNode node = shapeShiftRaceSubNodes[raceId];
                for (int i = 0; i < node.Grids.Count; ++i)
                {
                    if (node.Grids[i].Replaceid != 0)
                    {
                        skillGride = node.Grids[i];
                        break;
                    }
                }
            }
            return skillGride;
        }

        public bool CanUseChangeCard(uint cardId)
        {
            if (CSVRaceChangeCard.Instance.TryGetValue(cardId, out CSVRaceChangeCard.Data curCSVRaceChangeCardData))
            {
                CSVRaceDepartmentResearch.Data csvRaceDepartmentResearchData = CSVRaceDepartmentResearch.Instance.GetConfData(curCSVRaceChangeCardData.need_race_lv);
                CSVGenus.Data csvGenusData = CSVGenus.Instance.GetConfData(curCSVRaceChangeCardData.type);
                uint raceReId = 0;
                if (shapeShiftRaceSubNodes.ContainsKey(csvGenusData.id))
                {
                    raceReId = shapeShiftRaceSubNodes[csvGenusData.id].Subnodeid;
                }
                else
                {
                    raceReId = csvGenusData.race_change_id;
                }
                CSVRaceDepartmentResearch.Data csvRaceDepartmentResearchDataUnLock = CSVRaceDepartmentResearch.Instance.GetConfData(raceReId);
                if (csvRaceDepartmentResearchData.rank <= csvRaceDepartmentResearchDataUnLock.rank && csvRaceDepartmentResearchData.level <= csvRaceDepartmentResearchDataUnLock.level)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool CanUnLockAllStudy(uint needRank)
        {
            int count = Sys_Transfiguration.Instance.GetRacesIds().Count;
            if (shapeShiftSubNodeIds.Count < count)
            {
                return false;
            }
            for (int i = 0; i < shapeShiftSubNodeIds.Count; ++i)
            {
                if (CSVRaceDepartmentResearch.Instance.GetConfData(shapeShiftSubNodeIds[i]).rank < needRank)
                {
                    return false;
                }
            }
            return true;
        }

        public ShapeShiftSkillGrid GetReplaceSkillGridByAllRace()
        {
            ShapeShiftSkillGrid skillGride = null;
            foreach (var item in allRaceSkillGrids)
            {
                if (item.Value.Replaceid != 0)
                {
                    skillGride = item.Value;
                    break;
                }
            }
            return skillGride;
        }
    }

    public class Sys_Transfiguration : SystemModuleBase<Sys_Transfiguration>
    {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public uint mainNodeId;
        public Dictionary<uint, uint> shapeShiftAddDic = new Dictionary<uint, uint>();
        public bool checkHightQualityReStudy = true;
        public bool checkReplaceSkillReStudy = true;
        public uint curSortType = 101;
        public bool isToggleShow = false;
        public uint allRaceId= 0;
        public uint curIndex;
        public List<ClientShapeShiftPlan> clientShapeShiftPlans = new List<ClientShapeShiftPlan>();
        public List<string> listPlansName = new List<string>();

        public enum EEvents
        {
            OnSelectRaceMenu, // 选择种族类型
            OnSelectCardSort, // 选择变身卡排序方式
            OnForgetRaceStudy, // 遗忘种族变身研究
            OnUpdateSubNode,   //升级分系种族研究
            OnUpdateMainNode,   //升阶变身研究
            OnRefreshSkill,     //重修技能
            OnConfirmRefresh,    //确认重修技能
            OnUseChangeCard,     //使用变身卡
            OnUnlockMasterNode,    //解锁全系技能格      
            OnSelectPlan,   //选择重修方案
            OnRenamePlan,    //重命名方案
            OnUpdatePlan,    //方案信息更新
        }

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdShapeShift.DataNty, OnDataNty, CmdShapeShiftDataNty.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdShapeShift.UpdateMainNodeReq, (ushort)CmdShapeShift.UpdateMainNodeNty, OnUpdateMainNodeNty, CmdShapeShiftUpdateMainNodeNty.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdShapeShift.UpdateSubNodeReq, (ushort)CmdShapeShift.UpdateSubNodeNty, OnUpdateSubNodeNty, CmdShapeShiftUpdateSubNodeNty.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdShapeShift.ResetSubNodeNty, (ushort)CmdShapeShift.ResetSubNodeNty, OnResetSubNodeNty, CmdShapeShiftResetSubNodeNty.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdShapeShift.RefreshSkillReq, (ushort)CmdShapeShift.RefreshSkillNty, OnRefreshSkillNty, CmdShapeShiftRefreshSkillNty.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdShapeShift.ConfirmRefreshReq, (ushort)CmdShapeShift.ConfirmRefreshNty, OnConfirmRefreshNty, CmdShapeShiftConfirmRefreshNty.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdShapeShift.UnlockMasterNodeReq, (ushort)CmdShapeShift.UnlockMasterNodeNty, OnUnlockMasterNodeNty, CmdShapeShiftUnlockMasterNodeNty.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdShapeShift.AddPlanReq, (ushort)CmdShapeShift.AddPlanNty, OnAddPlanNty, CmdShapeShiftAddPlanNty.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdShapeShift.PlanRenameReq, (ushort)CmdShapeShift.PlanRenameNty, OnPlanRenameNty, CmdShapeShiftPlanRenameNty.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdShapeShift.ChangePlanReq, (ushort)CmdShapeShift.ChangePlanNty, OnChangePlanNty, CmdShapeShiftChangePlanNty.Parser);
        }

        private void OnDataNty(NetMsg msg)
        {
            CmdShapeShiftDataNty res = NetMsgUtil.Deserialize<CmdShapeShiftDataNty>(CmdShapeShiftDataNty.Parser, msg);
            curIndex = res.Data.CurPlanIndex;
            mainNodeId = res.Data.Mainnode.Mainnodeid;
            if (mainNodeId == 0)
            {
                mainNodeId = CSVRaceChangeResearch.Instance.GetByIndex(0).id;
            }
            shapeShiftAddDic.Clear();
            clientShapeShiftPlans.Clear();
            for (int i = 0; i < res.Data.Plans.Count; ++i)
            {
                ClientShapeShiftPlan clientShapeShiftPlan = new ClientShapeShiftPlan();
                clientShapeShiftPlan.index = i;
                clientShapeShiftPlan.name = res.Data.Plans[i].PlanName.ToStringUtf8();
                clientShapeShiftPlan.SetSubData(res.Data.Plans[i]);
                clientShapeShiftPlans.Add(clientShapeShiftPlan);
            }
            for (int i = 0; i < res.Data.Typelist.Types_.Count; ++i)
            {
                shapeShiftAddDic.Add(res.Data.Typelist.Types_[i].Type, res.Data.Typelist.Types_[i].Attrper);
            }
        }

        public void UpdateMainNodeReq()
        {
            CmdShapeShiftUpdateMainNodeReq req = new CmdShapeShiftUpdateMainNodeReq();
            NetClient.Instance.SendMessage((ushort)CmdShapeShift.UpdateMainNodeReq, req);
        }

        private void OnUpdateMainNodeNty(NetMsg msg)
        {
            CmdShapeShiftUpdateMainNodeNty res = NetMsgUtil.Deserialize<CmdShapeShiftUpdateMainNodeNty>(CmdShapeShiftUpdateMainNodeNty.Parser, msg);
            mainNodeId = res.Currentnodeid;
            for (int i = 0; i < res.Typelist.Types_.Count; ++i)
            {
                if (shapeShiftAddDic.ContainsKey(res.Typelist.Types_[i].Type))
                {
                    shapeShiftAddDic[res.Typelist.Types_[i].Type] = res.Typelist.Types_[i].Attrper;
                }
                else
                {
                    shapeShiftAddDic.Add(res.Typelist.Types_[i].Type, res.Typelist.Types_[i].Attrper);
                }
            }
            eventEmitter.Trigger(EEvents.OnUpdateMainNode);
        }

        public void UpdateSubNodeReq(uint subnodetype)
        {
            CmdShapeShiftUpdateSubNodeReq req = new CmdShapeShiftUpdateSubNodeReq();
            req.Subnodetype = subnodetype;
            NetClient.Instance.SendMessage((ushort)CmdShapeShift.UpdateSubNodeReq, req);
        }

        private void OnUpdateSubNodeNty(NetMsg msg)
        {
            CmdShapeShiftUpdateSubNodeNty res = NetMsgUtil.Deserialize<CmdShapeShiftUpdateSubNodeNty>(CmdShapeShiftUpdateSubNodeNty.Parser, msg);
            CSVRaceDepartmentResearch.Data csvData = CSVRaceDepartmentResearch.Instance.GetConfData(res.Formernodeid);
            uint type = 0;
            if (csvData != null)
            {
                type = csvData.type;
                for(int i = 0; i < clientShapeShiftPlans.Count; ++i)
                {
                    clientShapeShiftPlans[i].RefreshSubNode(res.Formernodeid,res.Currentnodeid, type);
                }
                for(int i = 0; i < res.Newgrids.Count; ++i)
                {
                    if (clientShapeShiftPlans.Count > i&& clientShapeShiftPlans[i].index==i)
                    {
                        clientShapeShiftPlans[i].RefershSkillGrid(res.Currentnodeid,type, res.Newgrids[i]);
                    }
                }

                for (int i = 0; i < res.Typelist.Types_.Count; ++i)
                {
                    if (shapeShiftAddDic.ContainsKey(res.Typelist.Types_[i].Type))
                    {
                        shapeShiftAddDic[res.Typelist.Types_[i].Type] = res.Typelist.Types_[i].Attrper;
                    }
                    else
                    {
                        shapeShiftAddDic.Add(res.Typelist.Types_[i].Type, res.Typelist.Types_[i].Attrper);
                    }
                }
                eventEmitter.Trigger<uint>(EEvents.OnUpdateSubNode, type);
            }
        }

        public void ResetSubNodeReq(uint subnodetype)
        {
            CmdShapeShiftResetSubNodeReq req = new CmdShapeShiftResetSubNodeReq();
            req.Subnodetype = subnodetype;
            NetClient.Instance.SendMessage((ushort)CmdShapeShift.ResetSubNodeReq, req);
        }

        private void OnResetSubNodeNty(NetMsg msg)
        {
            CmdShapeShiftResetSubNodeNty res = NetMsgUtil.Deserialize<CmdShapeShiftResetSubNodeNty>(CmdShapeShiftResetSubNodeNty.Parser, msg);
            for (int i = 0; i < clientShapeShiftPlans.Count; ++i)
            {
                if (clientShapeShiftPlans.Count > i && clientShapeShiftPlans[i].index == i)
                {
                    clientShapeShiftPlans[i].ResetSubNode(res.Formernodeid);
                }
            }
            for (int i = 0; i < res.Typelist.Types_.Count; ++i)
            {
                if (shapeShiftAddDic.ContainsKey(res.Typelist.Types_[i].Type))
                {
                    shapeShiftAddDic[res.Typelist.Types_[i].Type] = res.Typelist.Types_[i].Attrper;
                }
                else
                {
                    shapeShiftAddDic.Add(res.Typelist.Types_[i].Type, res.Typelist.Types_[i].Attrper);
                }
            }
            eventEmitter.Trigger(EEvents.OnForgetRaceStudy);
        }

        public void RefreshSkillReq(uint gridid,uint index)
        {
            CmdShapeShiftRefreshSkillReq req = new CmdShapeShiftRefreshSkillReq();
            req.Gridid = gridid;
            req.PlanIndex = index;
            NetClient.Instance.SendMessage((ushort)CmdShapeShift.RefreshSkillReq, req);
        }

        private void OnRefreshSkillNty(NetMsg msg)
        {
            CmdShapeShiftRefreshSkillNty res = NetMsgUtil.Deserialize<CmdShapeShiftRefreshSkillNty>(CmdShapeShiftRefreshSkillNty.Parser, msg);
            CSVRaceChangeSkill.Data csvRaceChangeSkillData = CSVRaceChangeSkill.Instance.GetConfData(res.Grid.Gridid);
            if (csvRaceChangeSkillData != null)
            {
                int index = (int)res.PlanIndex;
                if (clientShapeShiftPlans[index].shapeShiftRaceSubNodes.ContainsKey(csvRaceChangeSkillData.type))
                {
                    ShapeShiftSubNode node = clientShapeShiftPlans[index].shapeShiftRaceSubNodes[csvRaceChangeSkillData.type];
                    for (int i = 0; i < node.Grids.Count; ++i)
                    {
                        if (node.Grids[i].Gridid == res.Grid.Gridid)
                        {
                            node.Grids[i].Skillid = res.Grid.Skillid;
                            node.Grids[i].Replaceid = res.Grid.Replaceid;
                            break;
                        }
                    }
                }
                else if (clientShapeShiftPlans[index].allRaceSkillGrids.ContainsKey(res.Grid.Gridid))
                {
                    ShapeShiftSkillGrid grid = clientShapeShiftPlans[index].allRaceSkillGrids[res.Grid.Gridid];
                    grid.Skillid = res.Grid.Skillid;
                    grid.Replaceid = res.Grid.Replaceid;
                }
            } 
            eventEmitter.Trigger(EEvents.OnRefreshSkill, res.Grid.Gridid);
        }

        public void OnConfirmRefreshReq(uint gridid, bool save,uint index)
        {
            CmdShapeShiftConfirmRefreshReq req = new CmdShapeShiftConfirmRefreshReq();
            req.Gridid = gridid;
            req.Save = save;
            req.PlanIndex = index;
            NetClient.Instance.SendMessage((ushort)CmdShapeShift.ConfirmRefreshReq, req);
        }

        private void OnConfirmRefreshNty(NetMsg msg)
        {
            CmdShapeShiftConfirmRefreshNty res = NetMsgUtil.Deserialize<CmdShapeShiftConfirmRefreshNty>(CmdShapeShiftConfirmRefreshNty.Parser, msg);
            CSVRaceChangeSkill.Data csvRaceChangeSkillData = CSVRaceChangeSkill.Instance.GetConfData(res.Grid.Gridid);
            int index = (int)res.PlanIndex;
            if (clientShapeShiftPlans[index].allRaceSkillGrids.ContainsKey(res.Grid.Gridid))
            {
                if (clientShapeShiftPlans[index].allRaceSkillGrids[res.Grid.Gridid].Skillid != res.Grid.Skillid)
                {
                    uint skillID = clientShapeShiftPlans[index].allRaceSkillGrids[res.Grid.Gridid].Skillid;
                    CSVPassiveSkillInfo.Data info = CSVPassiveSkillInfo.Instance.GetConfData(skillID);
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680001017, LanguageHelper.GetTextContent(info.name)));
                    clientShapeShiftPlans[index].allRaceSkillGrids[res.Grid.Gridid].Skillid = res.Grid.Skillid;
                }
                if (clientShapeShiftPlans[index].allRaceSkillGrids[res.Grid.Gridid].Replaceid != 0 && res.Grid.Replaceid == 0)
                {
                    uint skillID = clientShapeShiftPlans[index].allRaceSkillGrids[res.Grid.Gridid].Replaceid;
                    CSVPassiveSkillInfo.Data info = CSVPassiveSkillInfo.Instance.GetConfData(skillID);
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680001019, LanguageHelper.GetTextContent(info.name)));
                    clientShapeShiftPlans[index].allRaceSkillGrids[res.Grid.Gridid].Replaceid = res.Grid.Replaceid;
                }
            }
            else
            {
                if (csvRaceChangeSkillData != null)
                {
                    ShapeShiftSubNode node = clientShapeShiftPlans[index].shapeShiftRaceSubNodes[csvRaceChangeSkillData.type];
                    for (int i = 0; i < node.Grids.Count; ++i)
                    {
                        if (node.Grids[i].Gridid == res.Grid.Gridid)
                        {
                            if (node.Grids[i].Skillid != res.Grid.Skillid)
                            {
                                CSVPassiveSkillInfo.Data info = CSVPassiveSkillInfo.Instance.GetConfData(node.Grids[i].Skillid);
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680001017, LanguageHelper.GetTextContent(info.name)));
                                node.Grids[i].Skillid = res.Grid.Skillid;
                            }
                            if (node.Grids[i].Replaceid != 0 && res.Grid.Replaceid == 0)
                            {
                                CSVPassiveSkillInfo.Data info = CSVPassiveSkillInfo.Instance.GetConfData(node.Grids[i].Replaceid);
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680001019, LanguageHelper.GetTextContent(info.name)));
                                node.Grids[i].Replaceid = res.Grid.Replaceid;
                            }
                            break;
                        }
                    }
                }
            }
            eventEmitter.Trigger(EEvents.OnConfirmRefresh, res.Grid.Gridid);
        }

        public void UnlockMasterNodeReq(uint gridid)
        {
            CmdShapeShiftUnlockMasterNodeReq req = new CmdShapeShiftUnlockMasterNodeReq();
            req.Gridid = gridid;
            NetClient.Instance.SendMessage((ushort)CmdShapeShift.UnlockMasterNodeReq, req);
        }

        private void OnUnlockMasterNodeNty(NetMsg msg)
        {
            CmdShapeShiftUnlockMasterNodeNty res = NetMsgUtil.Deserialize<CmdShapeShiftUnlockMasterNodeNty>(CmdShapeShiftUnlockMasterNodeNty.Parser, msg);
            for(int i = 0; i < res.Grid.Count; ++i)
            {
                clientShapeShiftPlans[i].UnlockAllRaceSkill(res.Grid[i]);
            }
        }

        public void AddPlanReq(bool copy)
        {
            CmdShapeShiftAddPlanReq req = new CmdShapeShiftAddPlanReq();
            req.Copy = copy;
            NetClient.Instance.SendMessage((ushort)CmdShapeShift.AddPlanReq, req);
        }

        private void OnAddPlanNty(NetMsg msg)
        {
            CmdShapeShiftAddPlanNty res = NetMsgUtil.Deserialize<CmdShapeShiftAddPlanNty>(CmdShapeShiftAddPlanNty.Parser, msg);
            ClientShapeShiftPlan clientShapeShiftPlan = new ClientShapeShiftPlan();
            clientShapeShiftPlan.SetSubData(res.Plan);
            clientShapeShiftPlans.Add(clientShapeShiftPlan);
            eventEmitter.Trigger(EEvents.OnUpdatePlan);
            uint index = (uint)clientShapeShiftPlans.Count - 1;
            Sys_Plan.Instance.eventEmitter.Trigger<uint, uint>(Sys_Plan.EEvents.AddNewPlan, (uint)Sys_Plan.EPlanType.TransfigurationStudy, index);
        }

        public void PlanRenameReq(uint index,string name)
        {
            CmdShapeShiftPlanRenameReq req = new CmdShapeShiftPlanRenameReq();
            req.PlanIndex = index;
            req.NewName = FrameworkTool.ConvertToGoogleByteString(name);
            NetClient.Instance.SendMessage((ushort)CmdShapeShift.PlanRenameReq, req);
        }

        private void OnPlanRenameNty(NetMsg msg)
        {
            CmdShapeShiftPlanRenameNty res = NetMsgUtil.Deserialize<CmdShapeShiftPlanRenameNty>(CmdShapeShiftPlanRenameNty.Parser, msg);
           if(clientShapeShiftPlans.Count> res.PlanIndex)
            {
                clientShapeShiftPlans[(int)res.PlanIndex].name = res.NewName.ToStringUtf8();
                eventEmitter.Trigger<uint>(EEvents.OnRenamePlan,res.PlanIndex);
                Sys_Plan.Instance.eventEmitter.Trigger<uint, uint, string>(Sys_Plan.EEvents.ChangePlanName, (uint)Sys_Plan.EPlanType.TransfigurationStudy, res.PlanIndex, res.NewName.ToStringUtf8());
            }
        }

        public void ChangePlanReq(uint index)
        {
            CmdShapeShiftChangePlanReq req = new CmdShapeShiftChangePlanReq();
            req.PlanIndex = index;
            NetClient.Instance.SendMessage((ushort)CmdShapeShift.ChangePlanReq, req);
        }

        private void OnChangePlanNty(NetMsg msg)
        {
            CmdShapeShiftChangePlanNty res = NetMsgUtil.Deserialize<CmdShapeShiftChangePlanNty>(CmdShapeShiftChangePlanNty.Parser, msg);
            curIndex = res.PlanIndex;
            eventEmitter.Trigger(EEvents.OnUpdatePlan);
            Sys_Plan.Instance.eventEmitter.Trigger<uint, uint>(Sys_Plan.EEvents.OnChangePlanSuccess, (uint)Sys_Plan.EPlanType.TransfigurationStudy, curIndex);
        }

        public override void OnLogin()
        {
            ReadRecordFile();
        }

        public override void OnLogout()
        {
            shapeShiftAddDic.Clear();
            mainNodeId = 0;
            checkHightQualityReStudy = true;
            checkReplaceSkillReStudy = true;
            curSortType = 101;
            clientShapeShiftPlans.Clear();
    }

        public List<uint> GetTransformCardIdsByRaceId(uint raceId)
        {
            List<uint> list = new List<uint>();
            foreach (var item in CSVRaceChangeCard.Instance.GetAll())
            {
                if (item.type == raceId)
                {
                    list.Add(item.id);
                }
            }
            return list;
        }

        public List<uint> GetRacesIds()
        {
            List<uint> list = new List<uint>();
            foreach (var item in CSVGenus.Instance.GetAll())
            {
                if (item.race_change_id != 0)
                {
                    list.Add(item.id);
                }
            }
            list.Sort(RaceSortGenus);
            return list;
        }

        private int RaceSortGenus(uint a, uint b)
        {
            CSVGenus.Data dataA = CSVGenus.Instance.GetConfData(a);
            CSVGenus.Data dataB = CSVGenus.Instance.GetConfData(b);
            return dataA.sort.CompareTo(dataB.sort);
        }

        public uint GetTypeLanIdByCareer(uint careerID)
        {
            uint lanID = 0;
            switch (careerID)
            {
                case 1:
                    lanID = 1000441;
                    break;
                case 2:
                    lanID = 1000443;
                    break;
                case 3:
                    lanID = 1000454;
                    break;
                case 4:
                    lanID = 1000451;
                    break;
                case 5:
                    lanID = 1000466;
                    break;
                default:
                    break;
            }
            return lanID;
        }

        public uint GetSortTypeLanIdBySortId(uint sortId)
        {
            uint lanID = 0;
            switch (sortId)
            {
                case 1:
                    lanID = 2013010;
                    break;
                case 2:
                    lanID = 2013011;
                    break;
                case 3:
                    lanID = 2013012;
                    break;
                case 4:
                    lanID = 2013013;
                    break;
                case 5:
                    lanID = 2013014;
                    break;
                case 102:
                    lanID = 2013009;
                    break;
                case 101:
                    lanID = 2013008;
                    break;
                default:
                    break;
            }
            return lanID;
        }

        public List<ItemData> GetCardItemDataInBag()
        {
            List<ItemData> listTemp = new List<ItemData>();
            Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdShapeshift, out List<ItemData> BoxIdTransformCardtList);
            for (int i = 0; i < BoxIdTransformCardtList.Count; ++i)
            {
                listTemp.Add(BoxIdTransformCardtList[i]);
            }
            listTemp.Sort(RaceSort);
            return listTemp;
        }

        public List<ulong> GetCardsIdInBag(List<ItemData> list)
        {
            List<ulong> listTemp = new List<ulong>();
            if (list != null)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    listTemp.Add(list[i].Uuid);
                }
            }
            return listTemp;
        }

        private int RaceSort(ItemData a, ItemData b)
        {
            CSVItem.Data dataA = CSVItem.Instance.GetConfData(a.Id);
            CSVItem.Data dataB = CSVItem.Instance.GetConfData(b.Id);
            CSVRaceChangeCard.Data cardDataA = CSVRaceChangeCard.Instance.GetConfData(a.Id);
            CSVRaceChangeCard.Data cardDataB = CSVRaceChangeCard.Instance.GetConfData(b.Id);

            if (dataB.quality.CompareTo(dataA.quality) != 0)
                return dataB.quality.CompareTo(dataA.quality);
     
            else
                return cardDataA.career.CompareTo(cardDataB.career);
        }

        public void SortCardListByTab(uint id, List<ItemData> list)
        {
            if (list.Count == 0)
            {
                return;
            }
            List<ItemData> listTemp = new List<ItemData>();
            list.Sort(RaceSort);
            switch (id)
            {
                case 1:
                    SetListSortByType(list, listTemp, 1);
                    break;
                case 2:
                    SetListSortByType(list, listTemp, 2);
                    break;
                case 3:
                    SetListSortByType(list, listTemp, 3);
                    break;
                case 4:
                    SetListSortByType(list, listTemp, 4);
                    break;
                case 5:
                    SetListSortByType(list, listTemp, 5);
                    break;
                case 101:
                    break;
                case 102:
                    SetListSortByType(list, listTemp, 102);
                    break;
                default:
                    break;
            }
        }

        private void SetListSortByType(List<ItemData> list, List<ItemData> listTemp, uint type)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (type == 102)
                {
                    CSVRaceChangeCard.Data data = CSVRaceChangeCard.Instance.GetConfData(list[i].Id);
                    {
                        if (data == null)
                        {
                            return;
                        }
                    }
                    CSVRaceDepartmentResearch.Data csvRaceDepartmentResearchData = CSVRaceDepartmentResearch.Instance.GetConfData(data.need_race_lv);
                    CSVGenus.Data csvGenusData = CSVGenus.Instance.GetConfData(data.type);
                    uint raceReId = 0;
                    if (clientShapeShiftPlans[(int)curIndex].shapeShiftRaceSubNodes.ContainsKey(csvGenusData.id))
                    {
                        raceReId = clientShapeShiftPlans[(int)curIndex].shapeShiftRaceSubNodes[csvGenusData.id].Subnodeid;
                    }
                    else
                    {
                        raceReId = csvGenusData.race_change_id;
                    }
                    CSVRaceDepartmentResearch.Data csvRaceDepartmentResearchDataUnLock = CSVRaceDepartmentResearch.Instance.GetConfData(raceReId);
                    if (csvRaceDepartmentResearchData.rank <= csvRaceDepartmentResearchDataUnLock.rank && csvRaceDepartmentResearchData.level <= csvRaceDepartmentResearchDataUnLock.level)
                    {
                        listTemp.Add(list[i]);
                    }
                }
                else
                {
                    if (CSVRaceChangeCard.Instance.TryGetValue(list[i].Id, out CSVRaceChangeCard.Data data) && data.career == type)
                    {
                        listTemp.Add(list[i]);
                    }
                }
            }        
            for (int i = 0; i < listTemp.Count; ++i)
            {
                if (list.Contains(listTemp[i]))
                {
                    list.Remove(listTemp[i]);
                }
            }
           // list.Sort(RaceSort);
            listTemp.AddRange(list);
            list.Clear();
            list.AddRange(listTemp);
        }

        public List<CSVRaceChangeSkill.Data> GetSkillGridByRaceId(uint raceId)
        {
            List<CSVRaceChangeSkill.Data> listTemp = new List<CSVRaceChangeSkill.Data>();
            foreach (var data in CSVRaceChangeSkill.Instance.GetAll())
            {
                if (data.type == raceId)
                {
                    listTemp.Add(data);
                }
            }
            return listTemp;
        }

        public List<uint> GetSkillIdsByGridId(uint gridId, uint raceId, out List<uint> mutexlist)
        {
            List<uint> listSkillBasic = new List<uint>();
            List<uint> listSkillMutex = new List<uint>();
            List<uint> listBasic = new List<uint>();
            mutexlist = new List<uint>();
            CSVRaceChangeSkill.Data data = CSVRaceChangeSkill.Instance.GetConfData(gridId);
            if (data != null)
            {
                foreach (var item in CSVRaceChangeGroup.Instance.GetAll())
                {
                    if (data.skill_group.Contains(item.group_id) && item.race_restrict.Contains(raceId) && item.career_restrict.Contains(Sys_Role.Instance.Role.Career))
                    {
                        if (item.mutex == 0)
                        {
                            if (!listSkillBasic.Contains(item.skill_id))
                            {
                                listSkillBasic.Add(item.skill_id);
                                listBasic.Add(item.id);
                            }
                        }
                        else
                        {
                            if (!listSkillMutex.Contains(item.skill_id))
                            {
                                listSkillMutex.Add(item.skill_id);
                                mutexlist.Add(item.id);
                            }
                        }
                    }
                }
            }
            return listBasic;
        }

        public ulong GetTransfornCardItemCount(uint cardId)
        {
            ulong count = 0;
            List<ItemData> BoxIdTransformCardtList = new List<ItemData>();
            Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdShapeshift, out BoxIdTransformCardtList);
            if (BoxIdTransformCardtList != null)
            {
                for (int i = 0; i < BoxIdTransformCardtList.Count; ++i)
                {
                    if (BoxIdTransformCardtList[i].Id == cardId)
                    {
                        count+= BoxIdTransformCardtList[i].Count;
                    }
                }
            }
            return count;
        }

        public ItemData GetTransfornCardItemData(ulong uuId)
        {
            List<ItemData> BoxIdTransformCardtList = new List<ItemData>();
            Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdShapeshift, out BoxIdTransformCardtList);
            if (BoxIdTransformCardtList != null)
            {
                for (int i = 0; i < BoxIdTransformCardtList.Count; ++i)
                {
                    if (BoxIdTransformCardtList[i].Uuid == uuId)
                    {
                        return BoxIdTransformCardtList[i];
                    }
                }
            }
            return null;
        }



        class TransformAddToggle
        {
            public bool isShow = false;
        }

        public void SaveRecordFile()
        {
            string name = "TransformAddToggleShow";
            TransformAddToggle toggle = new TransformAddToggle();
            toggle.isShow = isToggleShow;
            FileStore.WriteJson(name, toggle);
        }

        public void ReadRecordFile()
        {
            string name = "TransformAddToggleShow";
            var jsonValue = FileStore.ReadJson(name);
            if (jsonValue == null)
            {
                isToggleShow = false;
                return;
            }
            TransformAddToggle toggle = new TransformAddToggle();
            JsonHeler.DeserializeObject(jsonValue, toggle);
            isToggleShow = toggle.isShow;
        }

        //全系加成
        public List<CSVAllRaceBonus.Data> GetAllRaceSkillGridData()
        {
            List<CSVAllRaceBonus.Data> listTemp = new List<CSVAllRaceBonus.Data>();
            foreach (var data in CSVAllRaceBonus.Instance.GetAll())
            {
                listTemp.Add(data);
            }
            return listTemp;
        }

        public ClientShapeShiftPlan GetCurUseShapeShiftData()
        {
            if (clientShapeShiftPlans.Count > curIndex)
            {
                return clientShapeShiftPlans[(int)curIndex];
            }
            else
            {
                return null;
            }
        }
    }
}
