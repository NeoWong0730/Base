using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using System.Text;
using Table;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// 魔力宝典页签
    /// </summary>
    public enum EMagicBookViewType
    {
        None = 0,
        MagicBook = 1,      //魔力宝典
        Teaching = 2,    //教学关
        Strategy = 3,     //攻略
        Max = 4,
    }

    /// <summary>
    /// (Hand,  进行) (UnFinished,完成 未领取) (Finished, 已完成)
    /// </summary>
    public enum EMaigcBookTaskState
    {
        Hand,                           // 进行
        UnFinished,                     // 完成 未领取
        Finished,                       // 已完成
    }

    /// <summary>
    /// 1	添加好友（测试用）
    /// 2	消耗一次活力
    /// 3	分配一次历练点
    /// 4	分配一次角色加点  
    /// 5	成功进行一次职业进阶
    /// 6	成功佩戴一次称号
    /// 7	进行一次技能升级
    /// 8	分配一次天赋点
    /// 9	激活一个晶石技能
    /// 10	升级一个晶石技能
    /// 11	镶嵌一次宝石
    /// 12	捕捉一次宠物
    /// 13	进行一次宠物加点
    /// 14	洗练一次宠物
    /// 15	开启魔力宝典
    /// 16	对任意宠物开启自动加点
    /// 17	学习任意宠物技能
    /// 18	进行一次自定义染色
    /// 19	上阵4个伙伴
    /// 20	学会任意一个生活技能
    /// 21	任意一个生活技能提升至2级
    /// 22	激活“无名的旅人”称号
    /// 23	加入任意家族
    /// 24	进行一次家族发言
    /// 25	进行一次家族捐献行为
    /// 26	通关灵堂任意关
    /// 27	参与一次经典头目战
    /// 28	声望达到“不见经传*5级”
    /// 29	完成一次推荐挂机点战斗
    /// 30	加入任意队伍
    /// 31	交易行购买任一件物品
    /// 32	参与一次地域防范战斗
    /// 33	了解元素克制
    /// 34	参与一次荣誉竞技场
    /// 35	参与一次人物传记
    /// 36	参与一次精英挑战
    /// 37	提升一次宠物强化等级
    /// 38	了解队长积分
    /// 39	了解援助系统
    /// 40	参与一次Boss挑战
    /// 41	参与一次巫师遇袭
    /// 42	参与一次黑色祈祷
    /// 43	进行一次改造书兑换
    /// 44	对宠物进行一次改造
    /// 45	添加一次改造技能
    /// 46	功能推送条件满足（开启任务）
    /// 47  宠物强化加点
    /// 48	完成一次伐木/挖矿/采药
    /// 49	完成一次单人烹饪	
    /// 50	完成一次多人烹饪	
    /// 51	参与一次生存竞技	
    /// 52	进行一次定制染色	
    /// 53  消耗一个特定道具type 1601 - 目前为了狩猎和钓鱼事件
    /// 54 对宠物进行一次技能领悟（无论成功，失败）
    /// </summary>
    public enum EMagicAchievement
    {
        AddFriend = 1, 
        DrainFatigue = 2, 
        Distributional = 3, 
        AddRoleAttribute = 4, 
        OccipationAdvance = 5, 
        WearTitle = 6, 
        SkillUp = 7, 
        TheTalent = 8, 
        ActiveEnergysparSkill = 9, 
        EnergysparSkillUp = 10, 
        WearGem = 11, 
        SealPet = 12, 
        AddPetAttribute = 13, 
        XiLianPet = 14,
        /// <summary>
        /// 开启魔力宝典
        /// </summary>
        Event15 = 15,

        /// <summary>
        /// 对任意宠物开启自动加点
        /// </summary>
        Event16 = 16,

        /// <summary>
        /// 学习任意宠物技能
        /// </summary>
        Event17 = 17,

        /// <summary>
        /// 进行一次自定义染色
        /// </summary>
        Event18 = 18,

        /// <summary>
        /// 上阵4个伙伴  事件参数 目前上阵数量
        /// </summary>
        Event19 = 19,

        /// <summary>
        /// 学会任意一个生活技能
        /// </summary>
        Event20 = 20,

        /// <summary>
        /// 任意一个生活技能提升至2级  事件参数 等级
        /// </summary>
        Event21 = 21,

        /// <summary>
        /// 激活“无名的旅人”称号 事件参数 称号id
        /// </summary>
        Event22 = 22,

        /// <summary>
        /// 加入任意家族
        /// </summary>
        Event23 = 23,

        /// <summary>
        /// 进行一次家族发言
        /// </summary>
        Event24 = 24,

        /// <summary>
        /// 进行一次家族捐献行为
        /// </summary>
        Event25 = 25,

        /// <summary>
        /// 通关灵堂任意关
        /// </summary>
        Event26 = 26,

        /// <summary>
        /// 参与一次经典头目战
        /// </summary>
        Event27 = 27,

        /// <summary>
        /// 声望达到“不见经传*5级” 事件参数 服务器数据等级
        /// </summary>
        Event28 = 28,

        /// <summary>
        /// 完成一次推荐挂机点战斗
        /// </summary>
        Event29 = 29,

        /// <summary>
        /// 加入任意队伍
        /// </summary>
        Event30 = 30,

        /// <summary>
        /// 交易行购买任一件物品
        /// </summary>
        Event31 = 31,

        /// <summary>
        /// 参与一次地域防范战斗
        /// </summary>
        Event32 = 32,

        /// <summary>
        /// 了解元素克制
        /// </summary>
        Event33 = 33,

        /// <summary>
        /// 参与一次荣誉竞技场
        /// </summary>
        Event34 = 34,

        /// <summary>
        /// 参与一次人物传记
        /// </summary>
        Event35 = 35,

        /// <summary>
        /// 参与一次精英挑战
        /// </summary>
        Event36 = 36,

        /// <summary>
        /// 提升一次宠物强化等级
        /// </summary>
        Event37 = 37,

        /// <summary>
        /// 了解队长积分
        /// </summary>
        Event38 = 38,

        /// <summary>
        /// 了解援助系统
        /// </summary>
        Event39 = 39,

        /// <summary>
        /// 参与一次Boss挑战
        /// </summary>
        Event40 = 40,

        /// <summary>
        /// 参与一次巫师遇袭

        /// </summary>
        Event41 = 41,

        /// <summary>
        /// 参与一次黑色祈祷
        /// </summary>
        Event42 = 42,

        /// <summary>
        /// 进行一次改造书兑换
        /// </summary>
        Event43 = 43,

        /// <summary>
        /// 44	对宠物进行一次改造
        /// </summary>
        Event44 = 44,

        /// <summary>
        /// 添加一次改造技能
        /// </summary>
        Event45 = 45,

        /// <summary>
        /// 功能推送条件满足（开启任务）事件参数 功能开放id
        /// </summary>
        Event46 = 46,

        /// <summary>
        /// 宠物强化加点
        /// </summary>
        Event47 = 47,

        /// <summary>
        /// 完成一次伐木/挖矿/采药
        /// </summary>
        Event48 = 48,

        /// <summary>
        /// 完成一次单人烹饪
        /// </summary>
        Event49 = 49,

        /// <summary>
        /// 完成一次多人烹饪
        /// </summary>
        Event50 = 50,

        /// <summary>
        /// 参与一次生存竞技
        /// </summary>
        Event51 = 51,

        /// <summary>
        /// 自定义染色
        /// </summary>
        Event52 = 52,

        /// <summary>
        /// 53  消耗一个特定道具type 1601 - 目前为了狩猎和钓鱼事件
        /// </summary>
        Event53 = 53,

        /// <summary>
        ///  54 对宠物进行一次技能领悟（无论成功，失败）
        /// </summary>
        Event54 = 54,
    }
    public class Sys_MagicBook : SystemModuleBase<Sys_MagicBook>
    {
        public List<Chapter> chapterList = new List<Chapter>();
        public List<ChapterSys> sysList = new List<ChapterSys>();
        public List<SysTask> sysTaskList = new List<SysTask>();
        public List<Teaching> teachList = new List<Teaching>();

        private IReadOnlyList<CSVChapterFunctionList.Data>  sysConfigList
        {
            get
            {
                if (_sysConfigList == null)
                {
                    InitData();
                }
                return _sysConfigList;
            }
        }

        public IReadOnlyList<CSVChapterFunctionList.Data>  _sysConfigList = null;
       
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            OnSysBeClicked, // 当系统被点击一次 刷新
            SysDateCheckEnd, // 数据监测完 刷新
            ChapterReward, // 章节奖励领取
            TaskReward, // 子任务奖励领取
            /*MagicTaskCheckEvent, // 魔力宝典检查事件
            MagicTaskCheckEvent2, // 魔力宝典检查事件2 带特定参数*/
            OnNeedCheckItemRed, // 主界面检测是否展示红点
            EventEnd, // 提交事件返回
            OnSelectViewType, //选择页签
            TeachingProcessUpdate,    // 教学观进度刷新
        }

        #region 服务器协议消息
        public override void Init()
        {
            AddListeners();
        }

        public override void OnLogin()
        {
            chapterList.Clear();
            sysList.Clear();
            sysTaskList.Clear();
            teachList.Clear();
            base.OnLogin();            
        }

        private void InitData()
        {
            //_sysConfigList = new List<CSVChapterFunctionList.Data>(CSVChapterFunctionList.Instance.Count);
            //for (int i = 0; i < CSVChapterFunctionList.Instance.Count; i++)
            //{
            //    _sysConfigList.Add(CSVChapterFunctionList.Instance[i]);
            //}
            _sysConfigList = CSVChapterFunctionList.Instance.GetAll();
        }

        private void AddListeners()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMagicDict.ChapterAwardNtf, OnMagicDictChapterAwardNtf, CmdMagicDictChapterAwardNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMagicDict.SysTaskAwardAwardNtf, OnMagicDictSysTaskAwardAwardNtf, CmdMagicDictSysTaskAwardAwardNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMagicDict.SysTaskProcessNtf, OnMagicDictSysTaskProcessNtf, CmdMagicDictSysTaskProcessNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMagicDict.SysProcessNtf, OnMagicDictSysProcessNtf, CmdMagicDictSysProcessNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMagicDict.ChapterProcessNtf, OnMagicDictChapterProcessNtf, CmdMagicDictChapterProcessNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMagicDict.ChapterSysTaskInfoNtf, OnMagicDictChapterSysTaskInfoNtf, CmdMagicDictChapterSysTaskInfoNtf.Parser);

            EventDispatcher.Instance.AddEventListener((ushort)CmdMagicDict.GetChapterAwardReq, (ushort)CmdMagicDict.GetChapterAwardRes, OnMagicDictGetChapterAwardRes, CmdMagicDictGetChapterAwardRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdMagicDict.GetSysTaskAwardReq, (ushort)CmdMagicDict.GetSysTaskAwardRes, OnMagicDictGetSysTaskAwardRes, CmdMagicDictGetSysTaskAwardRes.Parser);

            EventDispatcher.Instance.AddEventListener((ushort)CmdMagicDict.ClickChapterSysReq, (ushort)CmdMagicDict.ClickChapterSysRes, OnMagicDictClickChapterSysRes, CmdMagicDictClickChapterSysRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMagicDict.TeachingProcessNtf, OnTeachingProcessNtf, CmdMagicDictTeachingProcessNtf.Parser);
        }

        public void MagicDictGetChapterAwardReq(uint chapterId)
        {
            CmdMagicDictGetChapterAwardReq req = new CmdMagicDictGetChapterAwardReq();
            req.ChapterId = chapterId;
            NetClient.Instance.SendMessage((ushort)CmdMagicDict.GetChapterAwardReq, req);
        }

        private void OnMagicDictGetChapterAwardRes(NetMsg msg)
        {
            CmdMagicDictGetChapterAwardRes res = NetMsgUtil.Deserialize<CmdMagicDictGetChapterAwardRes>(CmdMagicDictGetChapterAwardRes.Parser, msg);
            for (int i = 0; i < chapterList.Count; i++)
            {
                Chapter data = chapterList[i];
                if (res.ChapterId == data.ChapterId)
                {
                    data.Awarded = true;
                }
            }
            eventEmitter.Trigger(EEvents.ChapterReward);
            //ChapterReward
        }

        public void MagicDictGetSysTaskAwardReq(uint taskId)
        {
            CmdMagicDictGetSysTaskAwardReq req = new CmdMagicDictGetSysTaskAwardReq();
            req.TaskId = taskId;
            NetClient.Instance.SendMessage((ushort)CmdMagicDict.GetSysTaskAwardReq, req);
        }

        private void OnMagicDictGetSysTaskAwardRes(NetMsg msg)
        {
            CmdMagicDictGetSysTaskAwardRes res = NetMsgUtil.Deserialize<CmdMagicDictGetSysTaskAwardRes>(CmdMagicDictGetSysTaskAwardRes.Parser, msg);
            bool isHas = false;
            SysTask data = null;
            for (int i = 0; i < sysTaskList.Count; i++)
            {
                data = sysTaskList[i];
                if (res.TaskId == data.TaskId)
                {
                    isHas = true;
                    data.Awarded = true;
                }
            }
            if (!isHas)
            {
                data = new SysTask();
                data.TaskId = res.TaskId;
                data.Process = 1;
                data.Awarded = true;
                sysTaskList.Add(data);
            }

            //章节子系统奖励领取返回
            eventEmitter.Trigger(EEvents.TaskReward);
        }


        /// <summary>
        ///  点击章节子系统请求
        /// </summary>
        /// <param chapterId="章节子系统id"></param>
        public void MagicDictClickChapterSysReq(uint chapterSysId)
        {
            CmdMagicDictClickChapterSysReq req = new CmdMagicDictClickChapterSysReq();
            req.ChapterSysId = chapterSysId;
            NetClient.Instance.SendMessage((ushort)CmdMagicDict.ClickChapterSysReq, req);
        }

        private void OnMagicDictClickChapterSysRes(NetMsg msg)
        {
            CmdMagicDictClickChapterSysRes res = NetMsgUtil.Deserialize<CmdMagicDictClickChapterSysRes>(CmdMagicDictClickChapterSysRes.Parser, msg);
            bool isHas = false;
            for (int i = 0; i < sysList.Count; i++)
            {
                if (res.SysId == sysList[i].SysId)
                {
                    isHas = true;
                    sysList[i].Clicked = true;
                }
            }
            if (!isHas)
            {
                ChapterSys data = new ChapterSys();
                data.SysId = res.SysId;
                data.Process = 0;
                data.Clicked = true;
                sysList.Add(data);
            }
        }


        /// <summary>
        ///  通知章节奖励可领取
        /// </summary>
        private void OnMagicDictChapterAwardNtf(NetMsg msg)
        {
            CmdMagicDictChapterAwardNtf ntf = NetMsgUtil.Deserialize<CmdMagicDictChapterAwardNtf>(CmdMagicDictChapterAwardNtf.Parser, msg);
            //ntf.ChapterId 章节id
        }

        /// <summary>
        ///  通知章节内系统任务奖励领取(也就是任务完成)
        /// </summary>
        private void OnMagicDictSysTaskAwardAwardNtf(NetMsg msg)
        {
            CmdMagicDictSysTaskAwardAwardNtf ntf = NetMsgUtil.Deserialize<CmdMagicDictSysTaskAwardAwardNtf>(CmdMagicDictSysTaskAwardAwardNtf.Parser, msg);
            //ntf.TaskId 任务id
        }

        /// <summary>
        ///  通知章节内系统任务进度(也就是任务完成)
        /// </summary>
        private void OnMagicDictSysTaskProcessNtf(NetMsg msg)
        {
            CmdMagicDictSysTaskProcessNtf ntf = NetMsgUtil.Deserialize<CmdMagicDictSysTaskProcessNtf>(CmdMagicDictSysTaskProcessNtf.Parser, msg);
            bool hasData = false;
            for (int i = 0; i < sysTaskList.Count; i++)
            {
                SysTask task = sysTaskList[i];
                if (task.TaskId == ntf.TaskId)
                {
                    hasData = true;
                    task.Process = ntf.TaskProcess;
                }
            }
            if (!hasData)
            {
                SysTask task = new SysTask();
                task.TaskId = ntf.TaskId;
                task.Process = ntf.TaskProcess;
                task.Awarded = false;
                sysTaskList.Add(task);
            }
            //ntf.TaskId 任务id
            //ntf.TaskProcess 任务进度
            CSVChapterSysTask.Data checkData = CSVChapterSysTask.Instance.GetConfData(ntf.TaskId);
            if (null != checkData)
            {
                if (null != checkData.ReachTypeAchievement && checkData.ReachTypeAchievement.Count > 0 && ntf.TaskProcess >= checkData.ReachTypeAchievement[0])
                {
                    eventEmitter.Trigger(EEvents.OnNeedCheckItemRed); // 主界面检测是否展示红点
                }
            }

        }

        /// <summary>
        ///  通知子系统进度
        /// </summary>
        private void OnMagicDictSysProcessNtf(NetMsg msg)
        {
            CmdMagicDictSysProcessNtf ntf = NetMsgUtil.Deserialize<CmdMagicDictSysProcessNtf>(CmdMagicDictSysProcessNtf.Parser, msg);
            bool hasData = false;
            for (int i = 0; i < sysList.Count; i++)
            {
                ChapterSys sys = sysList[i];
                if (sys.SysId == ntf.SysId)
                {
                    hasData = true;
                    sys.Process = ntf.SysProcess;
                }
            }
            if (!hasData)
            {
                ChapterSys task = new ChapterSys();
                task.SysId = ntf.SysId;
                task.Process = ntf.SysProcess;
                task.Clicked = false;
                sysList.Add(task);
            }

            //ntf.ChapterId  章节id
            //ntf.SysId  子系统id
            //ntf.SysProcess   子系统进度
        }

        /// <summary>
        ///  通知章节进度
        /// </summary>
        private void OnMagicDictChapterProcessNtf(NetMsg msg)
        {
            CmdMagicDictChapterProcessNtf ntf = NetMsgUtil.Deserialize<CmdMagicDictChapterProcessNtf>(CmdMagicDictChapterProcessNtf.Parser, msg);
            bool hasData = false;
            for (int i = 0; i < chapterList.Count; i++)
            {
                Chapter chp = chapterList[i];
                if (chp.ChapterId == ntf.ChapterId)
                {
                    hasData = true;
                    chp.Process = ntf.ChapterProcess;
                }
            }
            if (!hasData)
            {
                Chapter task = new Chapter();
                task.ChapterId = ntf.ChapterId;
                task.Process = ntf.ChapterProcess;
                task.Awarded = false;
                chapterList.Add(task);
            }
            eventEmitter.Trigger(EEvents.OnNeedCheckItemRed); // 主界面检测是否展示红点
            //ntf.ChapterId  章节id
            //ntf.ChapterProcess    章节进度
        }

        /// <summary>
        ///  通知章节和子系统及任务信息
        /// </summary>
        private void OnMagicDictChapterSysTaskInfoNtf(NetMsg msg)
        {
            CmdMagicDictChapterSysTaskInfoNtf ntf = NetMsgUtil.Deserialize<CmdMagicDictChapterSysTaskInfoNtf>(CmdMagicDictChapterSysTaskInfoNtf.Parser, msg);
            for (int i = 0; i < ntf.ChapterList.Count; i++)
            {
                chapterList.Add(ntf.ChapterList[i]);
            }

            for (int i = 0; i < ntf.SysList.Count; i++)
            {
                sysList.Add(ntf.SysList[i]);
            }

            for (int i = 0; i < ntf.SysTaskList.Count; i++)
            {
                sysTaskList.Add(ntf.SysTaskList[i]);
            }
            for (int i = 0; i < ntf.TeachList.Count; i++)
            {
                teachList.Add(ntf.TeachList[i]);
            }
        }

        /// <summary>
        ///  点击教学关
        /// </summary>
        /// <param teachId="教学id"></param>
        public void ClickTeachingReq(uint teachId)
        {
            CmdMagicDictClickTeachingReq req = new CmdMagicDictClickTeachingReq();
            req.TeachId = teachId;
            NetClient.Instance.SendMessage((ushort)CmdMagicDict.ClickTeachingReq, req);
        }

        /// <summary>
        ///通知教学关进度
        /// </summary>
        private void OnTeachingProcessNtf(NetMsg msg)
        {
            CmdMagicDictTeachingProcessNtf ntf = NetMsgUtil.Deserialize<CmdMagicDictTeachingProcessNtf>(CmdMagicDictTeachingProcessNtf.Parser, msg);
            bool hasData = false;
            for (int i = 0; i < teachList.Count; ++i)
            {
                if (teachList[i].Id == ntf.TeachId)
                {
                    teachList[i].Process = ntf.Process;
                    hasData = true;
                    break;
                }
            }
            if (!hasData)
            {
                Teaching teaching = new Teaching();
                teaching.Id = ntf.TeachId;
                teaching.Process = ntf.Process;
                teachList.Add(teaching);
            }
            eventEmitter.Trigger(EEvents.TeachingProcessUpdate);
        }

        private void OnReceivedType15(int type, uint taskId, TaskEntry taskEntry)
        {
            if (type == 15)
            {
                Sys_Task.Instance.TryDoTask(taskEntry, false, false, true);
            }
        }
        #endregion
         
        #region 数据
        /// <summary>
        ///  通过章节id获取章节功能组
        /// </summary>
        /// <param chapterId="章节id"></param>
        /// <param func="剔除预览不可见"></param>
        public List<CSVChapterFunctionList.Data>  GetSysListByChapterId(uint chapterId, bool func = false)
        {
            List<CSVChapterFunctionList.Data>  sysListtemp = new List<CSVChapterFunctionList.Data>();
            for (int i = 0; i < sysConfigList.Count; i++)
            {
                CSVChapterFunctionList.Data temp = sysConfigList[i];
                if (chapterId == temp.Chapter && (!func || (func && temp.IsFunction)))
                {
                    sysListtemp.Add(temp);
                }
            }
            return sysListtemp;
        }

        /// <summary>
        ///  通过章节id获取可领取id
        /// </summary>
        /// <param chapterId="章节id"></param>
        /// <param func="剔除预览不可见"></param>
        public List<CSVChapterFunctionList.Data>  GetCanRewardSysListByChapterId(uint chapterId, bool func = false)
        {
            List<CSVChapterFunctionList.Data>  sysListtemp = new List<CSVChapterFunctionList.Data>();
            for (int i = 0; i < sysConfigList.Count; i++)
            {
                CSVChapterFunctionList.Data temp = sysConfigList[i];
                if (chapterId == temp.Chapter && (!func || (func && temp.IsFunction)) && Sys_MagicBook.Instance.CheckSysReward(temp.id))
                {
                    sysListtemp.Add(temp);
                }
            }
            return sysListtemp;
        }

        /// <summary>
        ///  通过章节id不可领取
        /// </summary>
        /// <param chapterId="章节id"></param>
        /// <param func="剔除预览不可见"></param>
        public List<CSVChapterFunctionList.Data>  GetUnRewardSysListByChapterId(uint chapterId, bool func = false)
        {
            List<CSVChapterFunctionList.Data>  sysListtemp = new List<CSVChapterFunctionList.Data>();
            for (int i = 0; i < sysConfigList.Count; i++)
            {
                CSVChapterFunctionList.Data temp = sysConfigList[i];
                if (chapterId == temp.Chapter && (!func || (func && temp.IsFunction)) && !Sys_MagicBook.Instance.CheckSysReward(temp.id))
                {
                    sysListtemp.Add(temp);
                }
            }
            return sysListtemp;
        }

        /// <summary>
        ///  通过系统id获取子任务
        /// </summary>
        /// <param sysId="系统id"></param>
        public List<CSVChapterSysTask.Data>  GetTaskSubBySysId(uint sysId)
        {
            List<CSVChapterSysTask.Data>  sysListtemp = new List<CSVChapterSysTask.Data>();
            CSVChapterFunctionList.Data sysData = CSVChapterFunctionList.Instance.GetConfData(sysId);
            if (null != sysData)
            {
                if (null != sysData.OtherRegionalTask)
                {
                    for (int i = 0; i < sysData.OtherRegionalTask.Count; i++)
                    {
                        CSVChapterSysTask.Data data = CSVChapterSysTask.Instance.GetConfData(sysData.OtherRegionalTask[i]);
                        if (null != data)
                        {
                            sysListtemp.Add(data);
                        }
                        else
                        {
                            DebugUtil.LogErrorFormat("CSVChapterSysTask.Data not Find id {0}", sysData.OtherRegionalTask[i]);
                        }

                    }
                }
            }
            return sysListtemp;
        }


        /// <summary>
        ///  通过章节id获取服务器章节信息
        /// </summary>
        /// <param chapterId="章节id"></param>
        public Chapter GetSeverChapterByChapterId(uint chapterId)
        {
            for (int i = 0; i < chapterList.Count; i++)
            {
                Chapter temp = chapterList[i];
                if (temp.ChapterId == chapterId)
                {
                    return temp;
                }
            }
            return null;
        }


        /// <summary>
        ///  通过系统id获取服务器章节系统信息
        /// </summary>
        /// <param sysId="系统id"></param>
        public ChapterSys GetSeverChapterSysByChapterId(uint sysId)
        {
            for (int i = 0; i < sysList.Count; i++)
            {
                ChapterSys temp = sysList[i];
                if (temp.SysId == sysId)
                {
                    return temp;
                }
            }
            return null;
        }

        /// <summary>
        ///  通过章节id 获取章节解锁的系统
        /// </summary>
        /// <param chapterId="章节id"></param>
        public List<CSVChapterFunctionList.Data>  GetUnlockSysIdBySubSysId(uint chapterId, ref List<CSVChapterFunctionList.Data>  allList)
        {
            List<CSVChapterFunctionList.Data>  sysListtemp = new List<CSVChapterFunctionList.Data>();
            for (int i = 0; i < sysConfigList.Count; i++)
            {
                CSVChapterFunctionList.Data temp = sysConfigList[i];
                if (chapterId == temp.Chapter && IsSysUnlcok(temp))
                {
                    sysListtemp.Add(temp);
                }
                allList.Add(temp);
            }
            return sysListtemp;
        }


        /// <summary>
        ///  通过子任务id获取服务器任务信息
        /// </summary>
        /// <param taskId="子任务id"></param>
        public SysTask GetTaskDataByTaskIdId(uint taskId)
        {
            for (int i = 0; i < sysTaskList.Count; i++)
            {
                SysTask temp = sysTaskList[i];
                if (temp.TaskId == taskId)
                {
                    return temp;
                }
            }
            return null;
        }
        #endregion

        #region util
        public bool IsSysUnlcok(uint sysId)
        {
            CSVChapterFunctionList.Data data = CSVChapterFunctionList.Instance.GetConfData(sysId);
            return IsSysUnlcok(data);
        }

        public bool IsSysUnlcok(CSVChapterFunctionList.Data sysData)
        {
            if (null != sysData)
            {
                return Sys_FunctionOpen.Instance.IsOpen(sysData.FunctionId, false, true);
            }
            return false;
        }

        private bool TaskState(List<List<uint>> lists)
        {
            if (null != lists)
            {
                int count1 = lists.Count;
                if (count1 == 1)
                {
                    return Sys_Task.Instance.GetTaskState(lists[0][0]) == ETaskState.Submited;
                }
                else
                {
                    for (int i = 0; i < count1; i++)
                    {
                        // 参数不一样 大于1个  职业与任务匹配
                        if (lists[i].Count >= 2)
                        {
                            if (Sys_Role.Instance.Role.Career == lists[i][0])
                            {
                                return Sys_Task.Instance.GetTaskState(lists[i][1]) == ETaskState.Submited;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public string[] GetUnlockTexts(uint sysId, ref bool taskSubmited)
        {
            CSVChapterFunctionList.Data data = CSVChapterFunctionList.Instance.GetConfData(sysId);
            return GetUnlockTexts(data, ref taskSubmited);
        }

        public string[] GetUnlockTexts(CSVChapterFunctionList.Data sysData, ref bool taskSubmited)
        {
            string[] strs = new string[2];
            if (null != sysData)
            {
                if (null != sysData.TirggerTask && sysData.TirggerLv != 0)
                {
                    CSVWordStyle.Data wordStyle = CSVWordStyle.Instance.GetConfData(14u);
                    strs[0] = LanguageHelper.GetTextContent(10759, sysData.TirggerLv.ToString());
                    if (Sys_Role.Instance.Role.Level < sysData.TirggerLv)
                    {
                    }
                    strs[1] = TaskText(sysData.TirggerTask, ref taskSubmited);
                }
                else if (null != sysData.TirggerTask)
                {
                    if (!TaskState(sysData.TirggerTask))
                    {
                        strs[0] = TaskText(sysData.TirggerTask, ref taskSubmited);
                    }
                }
                else if (sysData.TirggerLv != 0)
                {
                    strs[0] = LanguageHelper.GetTextContent(10758, sysData.TirggerLv.ToString());
                    if (Sys_Role.Instance.Role.Level < sysData.TirggerLv)
                    {
                    }
                }
                else
                {
                    DebugUtil.LogError("CSVChapterFunctionList.Data ,TirggerTask TirggerLv not Find");
                }
            }
            return strs;
        }

        public string GetUnlockText(uint sysId)
        {
            CSVChapterFunctionList.Data data = CSVChapterFunctionList.Instance.GetConfData(sysId);
            return GetUnlockText(data);
        }

        public string GetUnlockText(CSVChapterFunctionList.Data sysData)
        {
            if (null != sysData)
            {
                if (null != sysData.TirggerTask && sysData.TirggerLv != 0)
                {
                    bool taskSubmited = false;
                    StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
                    bool isLevel = false;
                    if (Sys_Role.Instance.Role.Level < sysData.TirggerLv)
                    {
                        isLevel = true;
                        stringBuilder.Append(LanguageHelper.GetTextContent(10759, sysData.TirggerLv.ToString()));
                    }

                    if (isLevel)
                    {
                        stringBuilder.Append(LanguageHelper.GetTextContent(10760));
                        stringBuilder.Append(TaskText(sysData.TirggerTask, ref taskSubmited));
                    }
                    else
                    {
                        stringBuilder.Append(TaskText(sysData.TirggerTask, ref taskSubmited));
                    }
                    return StringBuilderPool.ReleaseTemporaryAndToString(stringBuilder);

                }
                else if (null != sysData.TirggerTask)
                {
                    if (!TaskState(sysData.TirggerTask))
                    {
                        bool taskSubmited = false;
                        return TaskText(sysData.TirggerTask, ref taskSubmited);
                    }
                }
                else if (sysData.TirggerLv != 0)
                {
                    if (Sys_Role.Instance.Role.Level < sysData.TirggerLv)
                    {
                        return LanguageHelper.GetTextContent(10758, sysData.TirggerLv.ToString());
                    }
                }
                else
                {
                    DebugUtil.LogError("CSVChapterFunctionList.Data ,TirggerTask TirggerLv not Find");
                }
            }
            return "";
        }

        private string TaskText(List<List<uint>> lists, ref bool taskSubmited)
        {
            if (null != lists)
            {
                int count1 = lists.Count;
                if (count1 == 1)
                {
                    CSVTask.Data cSVTaskData = CSVTask.Instance.GetConfData(lists[0][0]);
                    if (null != cSVTaskData)
                    {
                        taskSubmited = Sys_Task.Instance.GetTaskState(lists[0][0]) == ETaskState.Submited;
                        return LanguageHelper.GetTextContent(10757, LanguageHelper.GetTaskTextContent(cSVTaskData.taskName));
                    }

                }
                else
                {
                    for (int i = 0; i < count1; i++)
                    {
                        // 参数不一样 大于1个  职业与任务匹配
                        if (lists[i].Count >= 2)
                        {
                            if (Sys_Role.Instance.Role.Career == lists[i][0])
                            {
                                CSVTask.Data cSVTaskData = CSVTask.Instance.GetConfData(lists[i][1]);
                                if (null != cSVTaskData)
                                {
                                    taskSubmited = Sys_Task.Instance.GetTaskState(lists[i][1]) == ETaskState.Submited;
                                    return LanguageHelper.GetTextContent(10757, LanguageHelper.GetTaskTextContent(cSVTaskData.taskName));
                                }
                            }
                        }
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// 获取任务状态枚举
        /// </summary>
        /// <param taskData="子任务"></param>
        /// <returns></returns>
        public EMaigcBookTaskState GetTaskStateByTaskId(CSVChapterSysTask.Data taskData)
        {
            if (null != taskData)
            {
                SysTask sever_SysTask = GetTaskDataByTaskIdId(taskData.id);
                if (null != sever_SysTask)
                {
                    if (sever_SysTask.Awarded)
                    {
                        return EMaigcBookTaskState.Finished;
                    }
                    else
                    {
                        if (taskData.Type == 5) // 暂时只有5种类型 1- 4 类型只要添加进服务器数据内即 事件完成
                        {
                            if (null != taskData.SideQuest)
                            {
                                int count = taskData.SideQuest.Count;
                                if (count > 0)
                                {
                                    return Sys_Task.Instance.IsSubmited(taskData.SideQuest[count - 1]) ? EMaigcBookTaskState.UnFinished : EMaigcBookTaskState.Hand;
                                }
                            }
                        }
                        else
                        {
                            if (null != taskData.ReachTypeAchievement) // 不为空即是有值
                            {
                                if (sever_SysTask.Process >= taskData.ReachTypeAchievement[0])
                                {
                                    return EMaigcBookTaskState.UnFinished;
                                }
                                else
                                {
                                    return EMaigcBookTaskState.Hand;
                                }
                            }
                            else
                            {
                                return EMaigcBookTaskState.UnFinished;
                            }
                        }
                    }
                }
                else
                {
                    if (taskData.Type == 5) // 暂时只有5种类型 1- 4 类型只要添加进服务器数据内即 事件完成
                    {
                        if (null != taskData.SideQuest)
                        {
                            int count = taskData.SideQuest.Count;
                            if (count > 0)
                            {
                                return Sys_Task.Instance.IsSubmited(taskData.SideQuest[count - 1]) ? EMaigcBookTaskState.UnFinished : EMaigcBookTaskState.Hand;
                            }
                        }
                    }
                    else
                    {
                        return EMaigcBookTaskState.Hand;
                    }
                }
            }
            return EMaigcBookTaskState.Hand;
        }

        public EMaigcBookTaskState GetTaskStateByTaskId(uint taskid)
        {
            CSVChapterSysTask.Data data = CSVChapterSysTask.Instance.GetConfData(taskid);
            return GetTaskStateByTaskId(data);
        }

        /// <summary>
        /// 子任务行为控制
        /// </summary>
        /// <param name="taskData"></param>
        public void MagicBookTaskGo(CSVChapterSysTask.Data taskData)
        {
            if (!Sys_Hint.Instance.PushForbidOprationInFight())
            {
                bool isCloseUI = true;
                if (taskData.Type == 1) // 暂时只有5种类型 1- 4 类型只要添加进服务器数据内即 事件完成
                {
                    //Debug.LogErrorFormat("MagicBook 触发功能引导 {0}", taskData.GhidanceGroup);
                    Sys_Guide.Instance.TriggerGuideGroup(taskData.GhidanceGroup);
                }
                else if (taskData.Type == 2)
                {
                    //Debug.LogErrorFormat("MagicBook 触发功能引导 {0}", taskData.GhidanceGroup);
                    Sys_Guide.Instance.TriggerGuideGroup(taskData.GhidanceGroup);
                }
                else if (taskData.Type == 3)
                {
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(taskData.Reach);
                }
                else if (taskData.Type == 4)
                {
                    TryCloseMagicBookUI();
                    isCloseUI = false;
                    uint Openid = taskData.Reach;
                    CSVOpenUi.Data cSVOpenUiData = CSVOpenUi.Instance.GetConfData(Openid);
                    if (null != cSVOpenUiData)
                    {
                        uint para = cSVOpenUiData.ui_para;
                        if (para != 0)
                        {
                            UIManager.OpenUI((EUIID)cSVOpenUiData.Uiid, false, para);
                        }
                        else
                        {
                            UIManager.OpenUI((EUIID)cSVOpenUiData.Uiid);
                        }
                    }
                }
                else if (taskData.Type == 5)
                {
                    if (null != taskData.SideQuest)
                    {
                        if (IsHaveTaskType5(out TaskEntry taskEntry))
                        {
                            if (taskData.SideQuest.Contains(taskEntry.id))
                            {
                                Sys_Task.Instance.TryDoTask(taskEntry, true, false, true);
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10755, LanguageHelper.GetTaskTextContent(taskEntry.csvTask.taskName)));
                            }
                            else
                            {
                                isCloseUI = false;
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10756));
                            }
                        }
                        else
                        {
                            Sys_Task.Instance.ReqReceive(taskData.SideQuest[0], true);
                        }
                    }
                }
                else if (taskData.Type == 6)
                {
                    isCloseUI = false;
                    if(taskData.Reach > 10)
                    {
                        uint partnerId = taskData.Reach / 10;
                        uint subId = taskData.Reach % 10;
                        MessageEx practiceEx = new MessageEx
                        {
                            messageState = (EPetMessageViewState)partnerId,
                            subPage = (int)subId
                        };
                        Sys_Pet.Instance.OnGetPetInfoReq(practiceEx, EPetUiType.UI_Message);
                    }
                    else
                    {
                        MessageEx practiceEx = new MessageEx
                        {
                            messageState = (EPetMessageViewState)taskData.Reach,

                        };
                        Sys_Pet.Instance.OnGetPetInfoReq(practiceEx, EPetUiType.UI_Message);
                    }
                }
                else if (taskData.Type == 7)
                {
                    isCloseUI = false;
                    if (!Sys_FunctionOpen.Instance.IsOpen(30401, true)) return;

                    Sys_Family.Instance.OpenUI_Family();
                }
                else if (taskData.Type == 8)
                {
                    isCloseUI = false;
                    if (!Sys_FunctionOpen.Instance.IsOpen(10301, true))
                        return;

                    Sys_Equip.UIEquipPrama data = new Sys_Equip.UIEquipPrama();
                    data.curEquip = null;
                    data.opType = (Sys_Equip.EquipmentOperations)taskData.Reach;
                    UIManager.OpenUI(EUIID.UI_Equipment, false, data);
                }


                if (isCloseUI)
                {
                    TryCloseMagicBookUI();
                }
            }
        }

        public bool IsHaveTaskType5(out TaskEntry taskEntry)
        {
            taskEntry = null;
            Sys_Task.Instance.receivedTasks.TryGetValue(15, out SortedDictionary<uint, TaskEntry> magicBookTask);
            if (null != magicBookTask)
            {
                List<TaskEntry> tempList = new List<TaskEntry>(magicBookTask.Values);
                if (tempList.Count > 0) // 类型唯一任务
                {
                    taskEntry = tempList[0];
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public bool IsSysClicked(CSVChapterFunctionList.Data ShowData)
        {
            for (int i = 0; i < sysList.Count; i++)
            {
                if (sysList[i].SysId == ShowData.SystemType)
                {
                    return sysList[i].Clicked;
                }
            }
            return false;
        }

        private void TryCloseMagicBookUI()
        {
            if (UIManager.IsOpen(EUIID.UI_MagicBook))
            {
                UIManager.CloseUI(EUIID.UI_MagicBook);
            }
            if (UIManager.IsOpen(EUIID.UI_MagicBook_Detail))
            {
                UIManager.CloseUI(EUIID.UI_MagicBook_Detail);
            }
            if (UIManager.IsOpen(EUIID.UI_FunctionPreview))
            {
                UIManager.CloseUI(EUIID.UI_FunctionPreview);
            }
        }

        /// <summary>
        /// 章节奖励是否可以领取
        /// </summary>
        /// <param name="chapterId"></param>
        /// <returns></returns>
        public bool CheckChapterReward(uint chapterId)
        {
            for (int i = 0; i < chapterList.Count; i++)
            {
                Chapter chapter = chapterList[i];
                if (chapterId == chapter.ChapterId)
                {
                    CSVChapterInfo.Data chapterInfoData = CSVChapterInfo.Instance.GetConfData(chapterId);
                    if (null != chapterInfoData && Sys_Role.Instance.Role.Level >= chapterInfoData.ChapterOpenlv)
                    {
                        if (chapterInfoData.category == 1)
                        {
                            if (chapter.Process >= chapterInfoData.ChapterTaskNum)
                            {
                                return !chapter.Awarded;
                            }
                        }
                        else if (Sys_FunctionOpen.Instance.IsOpen(50620) && chapterInfoData.category == 2 && chapterInfoData.id != 207)
                        {
                            CSVCareerTeaching.Instance.TryGetValue(Sys_Role.Instance.Role.Career, out CSVCareerTeaching.Data cSVCareerTeachingData);
                            List<uint> listInfo = CheckTeachChapterInfo(chapterInfoData.id, cSVCareerTeachingData);
                            if (chapter.Process >= listInfo.Count)
                            {
                                return !chapter.Awarded;
                            }
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
            }
            return false;
        }

        /// <summary>
        /// 魔力宝典内是否有可领取项目   
        /// </summary>
        /// <returns></returns>
        public bool CheckMagicReward()
        {
            List<CSVChapterInfo.Data>  checkList = GetNeedChapterInfoData();
            for (int i = 0; i < checkList.Count; i++)
            {
                CSVChapterInfo.Data checkData = checkList[i];
                if (checkData.category==1 && CheckChapterHasReward(checkData.id) && Sys_Role.Instance.Role.Level >= checkData.ChapterOpenlv)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 魔力宝典和教学关红点显示
        /// </summary>
        /// <returns></returns>
        public bool CheckMagicBookAndTeachRedPoint()
        {
            if (CheckMagicReward())
            {
                return true;
            }
            if (CheckTeachShowRedPoint())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 单章节是否可以有领取
        /// </summary>
        /// <returns></returns>
        public bool CheckChapterHasReward(uint configId)
        {
            if (CheckChapterReward(configId))
            {
                return true;
            }
            else
            {
                List<CSVChapterFunctionList.Data>  sysListtemp = Sys_MagicBook.Instance.GetSysListByChapterId(configId);
                for (int j = 0; j < sysListtemp.Count; j++)
                {
                    if (CheckSysReward(sysListtemp[j].id))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private List<CSVChapterInfo.Data>  GetNeedChapterInfoData()
        {
            List<CSVChapterInfo.Data>  GetList = new List<CSVChapterInfo.Data>();

            var chapterInfoDatas = CSVChapterInfo.Instance.GetAll();
            for (int i = 0, len = chapterInfoDatas.Count; i < len; i++)
            {
                CSVChapterInfo.Data temp = chapterInfoDatas[i];
                if (Sys_Role.Instance.Role.Level >= temp.ChapterOpenlv)
                {
                    GetList.Add(temp);
                }
            }

            return GetList;
        }

        /// <summary>
        /// 子系统是否有可领取的任务
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public bool CheckSysReward(uint sysId)
        {
            CSVChapterFunctionList.Data funcData = CSVChapterFunctionList.Instance.GetConfData(sysId);
            if(null != funcData)
            {
                if(null != funcData.OtherRegionalTask)
                {
                    for (int i = 0; i < funcData.OtherRegionalTask.Count; i++)
                    {
                        if(GetTaskStateByTaskId(funcData.OtherRegionalTask[i]) == EMaigcBookTaskState.UnFinished)
                        {
                            return true;
                        }
                    }
                    
                }
            }           
            return false;
        }


        /// <summary>
        /// 教学观是否有未点击
        /// </summary>
        /// <returns></returns>
        public bool CheckTeachChapterHasUnFinish(uint configId)
        {
            CSVCareerTeaching.Instance.TryGetValue(Sys_Role.Instance.Role.Career, out CSVCareerTeaching.Data cSVCareerTeachingData);
            List<uint> chapterInfoList = Sys_MagicBook.Instance.CheckTeachChapterInfo(configId, cSVCareerTeachingData);
            CSVChapterInfo.Instance.TryGetValue(configId, out CSVChapterInfo.Data data);
            if (data != null && data.ChapterOpenlv > Sys_Role.Instance.Role.Level)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < chapterList.Count; ++i)
                {
                    if (chapterList[i].ChapterId == configId && chapterList[i].Process >= chapterInfoList.Count)
                    {
                        bool hasReward = CheckChapterHasReward(configId);
                        return hasReward;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 教学观红点显示
        /// </summary>
        /// <returns></returns>
        public bool CheckTeachShowRedPoint()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(50620))
            {
                return false;
            }
            bool hasUnFinish = false;
            for (uint i = 201; i <= 206; ++i)
            {
                hasUnFinish = CheckTeachChapterHasUnFinish(i);
                if (hasUnFinish)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 教学观章节数据
        /// </summary>
        /// <returns></returns>
        public List<uint> CheckTeachChapterInfo(uint configId, CSVCareerTeaching.Data cSVCareerTeachingData)
        {
            List<uint> IdList = new List<uint>();
            if (cSVCareerTeachingData == null)
            {
                return IdList; ;
            }
            switch (configId)
            {
                case 201:
                    IdList.AddRange(cSVCareerTeachingData.primary);
                    break;
                case 202:
                    IdList.AddRange(cSVCareerTeachingData.middle);
                    break;
                case 203:
                    IdList.AddRange(cSVCareerTeachingData.high);
                    break;
                case 204:
                    IdList.AddRange(cSVCareerTeachingData.state_desc);
                    break;
                case 205:
                    IdList.AddRange(cSVCareerTeachingData.damage_desc);
                    break;
                case 206:
                    IdList.AddRange(cSVCareerTeachingData.recovery_desc);
                    break;
                case 207:
                    IdList.AddRange(cSVCareerTeachingData.faq);
                    break;
                default:
                    break;
            }
            return IdList;
        }
        #endregion
    }
}
