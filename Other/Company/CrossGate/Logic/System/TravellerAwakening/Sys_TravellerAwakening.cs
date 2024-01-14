using System.Collections.Generic;
using Logic.Core;
using Packet;
using Net;
using Lib.Core;
using Table;
using logic;

namespace Logic
{
    public partial class Sys_TravellerAwakening : SystemModuleBase<Sys_TravellerAwakening>
    {
        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public Dictionary<uint, bool> awaketargetInfo = new Dictionary<uint, bool>();

        public uint awakeLevel;

        public bool IsShowRedpoint=true;

        public enum EEvents
        {
            OnTravellerAwakening,//觉醒
            OnTripperAwakeUpdate,   //觉醒任务刷新
            OnAwakeRedPoint,//觉醒红点
            OnAwakeImprintUpdate,//觉醒印记升级
        }

        private enum EGoType
        {
            Comfirm = 11828,
            Cancel = 11829,
            Jump = 11830,
        }
        #region 系统函数

        public override void Init()
        {
            ProcessEvents(true);
        }
        protected void ProcessEvents(bool toRegister)
        {
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener((ushort)CmdTripperAwake.AwakeReq, (ushort)CmdTripperAwake.AwakeRes, OnTravellerAwakeningRes, CmdTripperAwakeAwakeRes.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTripperAwake.DataNtf, OnTripperAwakeDataNtf, CmdTripperAwakeDataNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTripperAwake.UpdateNtf, OnTripperAwakeUpdateNtf, CmdTripperAwakeUpdateNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdTripperAwake.UpdateRuneReq, (ushort)CmdTripperAwake.UpdateRuneRes, OnAwakenImprintUpdateRes, CmdTripperAwakeUpdateRuneRes.Parser);
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTripperAwake.AwakeRes, OnTravellerAwakeningRes);
                EventDispatcher.Instance.RemoveEventListener( (ushort)CmdTripperAwake.DataNtf, OnTripperAwakeDataNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTripperAwake.UpdateNtf, OnTripperAwakeUpdateNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTripperAwake.UpdateRuneRes, OnAwakenImprintUpdateRes);
            }
        }

        public override void OnLogin(){
            base.OnLogin();
            IsShowRedpoint = true;
            SelectIndex = 0;
        }

        public override void OnLogout()
        {
            base.OnLogout();
            DestoryAllContainer();
        }
        public override void Dispose()
        {
            ProcessEvents(false);
        }
        #endregion

        #region net
        //觉醒请求
        public void TravellerAwakeningReq()
        {
            CmdTripperAwakeAwakeReq req = new CmdTripperAwakeAwakeReq();
            NetClient.Instance.SendMessage((ushort)CmdTripperAwake.AwakeReq, req);
        }
        //觉醒后同步最新数据
        public void OnTravellerAwakeningRes(NetMsg msg)
        {
            CmdTripperAwakeAwakeRes res = NetMsgUtil.Deserialize<CmdTripperAwakeAwakeRes>(CmdTripperAwakeAwakeRes.Parser, msg);
            awakeLevel = res.Data.Level;
            awaketargetInfo.Clear();
            for (int i = 0; i < res.Data.Tasklist.Tasks.Count; ++i)
            {//更新任务列表
                awaketargetInfo[res.Data.Tasklist.Tasks[i].Type] = res.Data.Tasklist.Tasks[i].Finished;
            }
            if (CheckAwakenCondition()&&(awakeLevel- GetAwakenOpenLevel()>1))//更新觉醒印记
            {//awakeLevel- GetAwakenOpenLevel()>1  ——这里折算出觉醒后开启印记的等级。见印记标签表
                int _id = 1;
                for (int i = 2; i <= CSVImprintLabel.Instance.Count; i++)
                {
                    CSVImprintLabel.Data lData = CSVImprintLabel.Instance.GetConfData((uint)i);
                    if (awakeLevel == lData.Title_Des)
                    {
                        _id = i;
                        break;
                    }
                }
                AddImprintDictionary((uint)_id);

            }
            eventEmitter.Trigger(EEvents.OnTravellerAwakening);
            eventEmitter.Trigger(EEvents.OnAwakeRedPoint);
        }
        private void AddImprintDictionary(uint _index)
        {

            if (imprintDic.ContainsKey(_index)) return;
            IsShowRedpoint = true;
            imprintDic.Add(_index, new ImprintEntry());
            InitImprintEntry((int)_index,false);
        }
        //上线发觉醒信息
        public void OnTripperAwakeDataNtf(NetMsg msg)
        {
            CmdTripperAwakeDataNtf ntf = NetMsgUtil.Deserialize<CmdTripperAwakeDataNtf>(CmdTripperAwakeDataNtf.Parser, msg);
            awakeLevel = ntf.Data.Level;
            awaketargetInfo.Clear();
            severDic.Clear();
            severDicSec.Clear();
            for (int i = 0; i < ntf.Data.Tasklist.Tasks.Count; i++)
            {
                awaketargetInfo[ntf.Data.Tasklist.Tasks[i].Type] = ntf.Data.Tasklist.Tasks[i].Finished;
            }
            for (int i = 0; i < ntf.Runelist.Runes.Count; i++)
            {
                string str = ntf.Runelist.Runes[i].Levelid.ToString();
                int level = int.Parse(str.Substring(str.Length - 2, 2));
                int id = int.Parse(str.Substring(0, str.Length - 2));
                severDicSec[(uint)id] = (uint)level;
                severDic[ntf.Runelist.Runes[i].Levelid] = (uint)level;
            }

            InitImprintDictionary();
            eventEmitter.Trigger(EEvents.OnAwakeRedPoint);
        }
        //觉醒任务更新主动发
        public void OnTripperAwakeUpdateNtf(NetMsg msg)
        {
            CmdTripperAwakeUpdateNtf ntf = NetMsgUtil.Deserialize<CmdTripperAwakeUpdateNtf>(CmdTripperAwakeUpdateNtf.Parser,msg);
            if (awaketargetInfo.ContainsKey(ntf.Task.Type))
            {
                awaketargetInfo[ntf.Task.Type] = ntf.Task.Finished;
            }
            eventEmitter.Trigger(EEvents.OnTripperAwakeUpdate);
            eventEmitter.Trigger(EEvents.OnAwakeRedPoint);
        }
        //觉醒印记升级请求
        public void OnOnAwakenImprintUpdateReq(uint nodeId,uint costType)
        {
            CmdTripperAwakeUpdateRuneReq req = new CmdTripperAwakeUpdateRuneReq();
            req.Nodeid = nodeId;
            req.Costtype = costType;
            NetClient.Instance.SendMessage((ushort)CmdTripperAwake.UpdateRuneReq, req);

        }
        //觉醒印记升级返回
        public void OnAwakenImprintUpdateRes(NetMsg msg)
        {
            CmdTripperAwakeUpdateRuneRes res = NetMsgUtil.Deserialize<CmdTripperAwakeUpdateRuneRes>(CmdTripperAwakeUpdateRuneRes.Parser, msg);
            ImprintRunesUpdate(res.Levelid);//更新印记字典
            string str = res.Levelid.ToString();
            int level = int.Parse(str.Substring(str.Length - 2, 2));
            int id = int.Parse(str.Substring(0, str.Length - 2));

            CSVImprintNode.Data _nodeD = CSVImprintNode.Instance.GetConfData((uint)id);
            UpdateOneImprintEntry(_nodeD.Label_Id);//更新当前觉醒实体
            nowNode = GetImprintEntry(nowNode.csv.Label_Id).GetOneNode(nowNode.id);//更新目前显示节点信息
            #region 下一节点
            nextNode = null;
            if (nowNode.ThisNodeIsMaxGrade())
            {
                nextNode = GetImprintEntry(nowNode.csv.Label_Id).GetNextNode(nowNode.id);
                if (nextNode != null)
                {
                    if (nextNode.csv.Node_Type == 1)
                    {
                        nextNode = GetImprintEntry(nowNode.csv.Label_Id).endNode;
                    }
                }
            }
            #endregion
            if ((nowNode.csv.Node_Type == 3) && nowNode.ThisNodeIsMaxGrade() && (_nodeD.Label_Id + 1 <= (awakeLevel - GetAwakenOpenLevel())))
            {//如果升级的是尾节点并且升级到最大，且下个实体已经激活，更新下个实体的信息
                UpdateOneImprintEntry(_nodeD.Label_Id + 1);
            }
            InitImprintLabelRedPoint();
            InitImprintAttrDictionary();
            eventEmitter.Trigger(EEvents.OnAwakeImprintUpdate);
            eventEmitter.Trigger(EEvents.OnAwakeRedPoint);
        }


        private void OnRoleLevelUp()
        {

        }
        #endregion

        #region RedPoint
        public bool CheckRealTarget()//检查旅人觉醒红点
        {
            CSVTravellerAwakening.Data csvAwakeData = CSVTravellerAwakening.Instance.GetConfData(awakeLevel);
            if (csvAwakeData.ActProject == null)
            {
                return false;
            }
            foreach (var item in awaketargetInfo)
            {
                if (!item.Value)
                {
                    return false;
                }
            }
            return true;
        }
        public bool CheckTarget()//更新觉醒总红点
        {
            if (CheckRealTarget() || CheckUIMenuImprintRedPoint())
            {
                return true;
            }
            return false;
        }
        #endregion

        #region TravellerAwakening Function
        private void DestoryAllContainer()
        {
            awakeLevel = 0;
            awaketargetInfo.Clear();
            imprintDic.Clear();
            severDic.Clear();
            severDicSec.Clear();
            iAttrDic.Clear();
            imprintLabelRedPointList.Clear();
        }
      
        public void OpenWarning(uint unFinType)
        {//点击觉醒条件后的提示框
            CSVTravellerAwakening.Data csvAwakeData = CSVTravellerAwakening.Instance.GetConfData(awakeLevel);
            CSVAwakeningCondtion.Data awakeCondition = CSVAwakeningCondtion.Instance.GetConfData(unFinType);
            if (awakeCondition.tel_type == 4)
            {//不跳转时弹框
                OpenBox(awakeCondition, ContentString(csvAwakeData, awakeCondition, unFinType), EGoType.Comfirm);
            }
            else
            {//跳转时弹框
                if (!Sys_FunctionOpen.Instance.IsOpen(GetOpenId(awakeCondition)))
                {
                    FunctionNotOpen(awakeCondition);//功能未开启时弹框
                }
                else
                {
                    OpenBox(awakeCondition, ContentString(csvAwakeData, awakeCondition, unFinType), EGoType.Jump);//功能开启时弹框
                }
            }
        }
        private void OpenBox(CSVAwakeningCondtion.Data awakeCondition,string contentStr, EGoType eType)
        {
            bool isOpen=true;
            if (eType==EGoType.Comfirm)
            {
                isOpen = false;
            }
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = contentStr;
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                if (eType == EGoType.Comfirm)
                {
                    UIManager.CloseUI(EUIID.UI_PromptBox, false, true);
                }
                else
                {
                    switch (awakeCondition.tel_type)
                    {
                        case 1:
                            UIManager.OpenUI((int)awakeCondition.skip_Id[0][0]);
                            break;
                        case 2:
                            UIManager.OpenUI((int)awakeCondition.skip_Id[0][0], false, awakeCondition.skip_Id[0][1]);
                            break;
                        case 3:
                            int _type = GetFunctionPos(awakeCondition);
                            uint skipId = awakeCondition.skip_Id[_type][0];
                            UIDailyActivitesParmas uiDaily = new UIDailyActivitesParmas();
                            uiDaily.IsSkipDetail = true;
                            uiDaily.SkipToID = skipId;
                            UIManager.CloseUI(EUIID.UI_Awaken);
                            UIManager.OpenUI(EUIID.UI_DailyActivites, false, uiDaily);
                            break;
                        default: break;
                    }
                }
                

            }, (uint)eType);
            PromptBoxParameter.Instance.SetCancel(isOpen, () =>
            {
                UIManager.CloseUI(EUIID.UI_PromptBox, false, true);

            });
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);

        }
        public void FunctionNotOpen(CSVAwakeningCondtion.Data awakeCondition)
        {//功能未开启
            string str = string.Empty;
            CSVCheckseq.Data checkData = CSVCheckseq.Instance.GetConfData(GetOpenId(awakeCondition));
            if (checkData == null)
            {
                DebugUtil.LogError("CSVCheckseq.Data Is NULL.");
                return;
            }
            int IdOne = checkData.CheckCondi1[0][1];
            EConditionType eConditionType1= (EConditionType)checkData.CheckCondi1[0][0];

            if (checkData.CheckCondi1.Count==1)
            {//单个条件
                str = FucntionNotOpenContent(eConditionType1, awakeCondition.not_open, IdOne);
            }
            else
            {//两个条件，且第二个条件是完成任务
                EConditionType eConditionType2 = (EConditionType)checkData.CheckCondi1[1][0];
                if (eConditionType2== EConditionType.TaskSubmitted)
                {
                    uint taskId = (uint)checkData.CheckCondi1[1][1];
                    switch (eConditionType1)
                    {
                        case EConditionType.GreaterThanLv:
                            {
                                str = LanguageHelper.GetTextContent(awakeCondition.not_open, (IdOne + 1).ToString(), TaskContent(taskId));
                            }
                            break;
                        case EConditionType.GreaterOrEqualLv:
                            {
                                str = LanguageHelper.GetTextContent(awakeCondition.not_open, IdOne.ToString(), TaskContent(taskId));
                            }
                            break;
                        default:
                            DebugUtil.LogError("TravellerAwakening:Brand New ConditionType!");
                            break;
                    }
                }else if (eConditionType2 == EConditionType.LessThanLv)
                {//两个条件，实际开启条件还是前一个，后一个是限制条件，走单个条件
                    str = FucntionNotOpenContent(eConditionType1, awakeCondition.not_open, IdOne);
                }
                else
                {
                    DebugUtil.LogError("TravellerAwakening:Brand New ConditionType!");
                }
            }

            OpenBox(awakeCondition, str, EGoType.Comfirm);
        }
        private string ContentString(CSVTravellerAwakening.Data csvAwakeData, CSVAwakeningCondtion.Data awakeCondition, uint targetType)
        {// 功能开启或无跳转时文本（觉醒条件表确认弹出框）
            string contentStr = string.Empty;
            int conditionPos = TargetPosReflectConditionPos(targetType);
            uint targetParam = csvAwakeData.ActCondition[conditionPos][0];
            switch (targetType)
            {
                case 3://任务
                    contentStr = LanguageHelper.GetTextContent(awakeCondition.pop_up_box, TaskContent(targetParam));
                    break;
                case 4://百人道场——层数关卡，并有特殊计算
                    uint layerStage = CSVInstanceDaily.Instance.GetConfData(targetParam).LayerStage;
                    uint layerlevel = CSVInstanceDaily.Instance.GetConfData(targetParam).Layerlevel;
                    uint levelCount = (layerStage - 1) * 10 + layerlevel;
                    contentStr = LanguageHelper.GetTextContent(awakeCondition.pop_up_box, layerStage.ToString(), levelCount.ToString());
                    break;
                case 6://人物传记——章节数关卡
                    contentStr = LanguageHelper.GetTextContent(awakeCondition.pop_up_box, CharpterCaculate(CSVInstanceDaily.Instance.GetConfData(targetParam).InstanceId), CSVInstanceDaily.Instance.GetConfData(targetParam).Layerlevel.ToString());
                    break;
                case 1:
                case 2:
                case 5:
                case 7:
                case 11:
                case 12:
                case 13://条件单独一个参数
                    contentStr = LanguageHelper.GetTextContent(awakeCondition.pop_up_box, targetParam.ToString());
                    break;
                case 10://条件两个参数，读后面那个
                case 20:
                case 21:
                case 22:
                case 23:
                    contentStr = LanguageHelper.GetTextContent(awakeCondition.pop_up_box, csvAwakeData.ActCondition[conditionPos][1].ToString());
                    break;
                case 9://技能{0}阶{1}
                    contentStr = LanguageHelper.GetTextContent(awakeCondition.pop_up_box, targetParam.ToString(), csvAwakeData.ActCondition[conditionPos][1].ToString());
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19://语言表配好一句话
                    contentStr = LanguageHelper.GetTextContent(awakeCondition.pop_up_box);
                    break;
                case 8://黑色祈祷——关卡
                    uint _targetParam= csvAwakeData.ActCondition[conditionPos][csvAwakeData.ActCondition[conditionPos].Count - 1];
                    contentStr = LanguageHelper.GetTextContent(awakeCondition.pop_up_box, _targetParam.ToString());
                    break;
                default: break;


            }
            return contentStr;

        }
        private uint GetOpenId(CSVAwakeningCondtion.Data awakeCondition)
        {//多个功能开启条件时，找到对应id
            uint openid;
            if (awakeCondition.function_Id.Count > 1)
            {
                openid = awakeCondition.function_Id[GetFunctionPos(awakeCondition)];
            }
            else
            {
                openid = awakeCondition.function_Id[0];
            }
            return openid;
        }
        public int TargetPosReflectConditionPos(uint targetType)//任务位置找条件位置（此处传入的targetType是觉醒条件表的id,故需要转换）
        {
            for (int i = 0; i < CSVTravellerAwakening.Instance.GetConfData(awakeLevel).ActProject.Count; i++)
            {
                if (CSVTravellerAwakening.Instance.GetConfData(awakeLevel).ActProject[i] == targetType)
                {
                    return i;
                }
            }
            return 0;
        }
        private int GetFunctionPos(CSVAwakeningCondtion.Data awakeCondition)
        {
            int _type = 0;
            if (awakeCondition.skip_Id.Count > 1)
            {
                for (int i = 0; i < awakeCondition.function_Id.Count; i++)
                {
                    CSVCheckseq.Data cData = CSVCheckseq.Instance.GetConfData(awakeCondition.function_Id[i]);
                    if (cData.CheckCondi1.Count == 1)
                    {
                        EConditionType eConditionType1 = (EConditionType)cData.CheckCondi1[0][0];
                        switch (eConditionType1)
                        {
                            case EConditionType.GreaterOrEqualLv:
                                if (Sys_Role.Instance.Role.Level >= cData.CheckCondi1[0][1])
                                {
                                    _type = i;
                                }
                                break;
                            default:
                                DebugUtil.LogError("TravellerAwakening:Brand New ConditionType!");
                                break;
                        }

                    }
                    else if(cData.CheckCondi1.Count ==2)
                    {
                        EConditionType eConditionType1 = (EConditionType)cData.CheckCondi1[0][0];
                        EConditionType eConditionType2 = (EConditionType)cData.CheckCondi1[1][0];
                        switch (eConditionType1)
                        {
                            case EConditionType.GreaterOrEqualLv:
                                switch (eConditionType2)
                                {
                                    case EConditionType.LessThanLv:
                                        if (Sys_Role.Instance.Role.Level >= cData.CheckCondi1[0][1] && Sys_Role.Instance.Role.Level < cData.CheckCondi1[1][1])
                                        {
                                            _type = i;
                                        }
                                        break;
                                    default:
                                        DebugUtil.LogError("TravellerAwakening:Brand New ConditionType!");
                                        break;
                                }
                                    break;
                            default:
                                DebugUtil.LogError("TravellerAwakening:Brand New ConditionType!");
                                break;
                        }
                        
                    }

                }

            }
            return _type;
        }
        public string TaskContent(uint taskId)//获取任务文本
        {
            return LanguageHelper.GetTaskTextContent(CSVTask.Instance.GetConfData(taskId).taskName);
        }
        private string FucntionNotOpenContent(EConditionType _type,uint LangId,int IdOne)
        {
            string str = string.Empty;
            switch (_type)
            {
                case EConditionType.GreaterThanLv:
                    {
                        str = LanguageHelper.GetTextContent(LangId, (IdOne + 1).ToString());
                    }
                    break;
                case EConditionType.TaskSubmitted:
                    {
                        str = LanguageHelper.GetTextContent(LangId, TaskContent((uint)IdOne));
                    }
                    break;
                case EConditionType.GreaterOrEqualLv:
                    {
                        str = LanguageHelper.GetTextContent(LangId, (IdOne).ToString());
                    }
                    break;
                default:
                    DebugUtil.LogError("Brand New ConditionType!");
                    break;
            }
            return str;

        }

        public string CharpterCaculate(uint _id)
        {
            string str = _id.ToString();
            int ch = int.Parse(str.Substring(1, 3));
            return ch.ToString();
        }
        #endregion
    }

}