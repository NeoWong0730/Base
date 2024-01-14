using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Packet;
using Table;

namespace Logic {
    public enum HintType {
        Normal,     // 通用提示
        GetReward,   // 获得道具提示
        Property,   // 属性
        PropertyDirect,   // 属性
        Map,        // 地图
        Static, // 静止状态
        Marquee,//跑马灯
        CommonInfo,//公共信息
        InBattle,    //战斗内
    }

    public enum EPropertyType {
        Property,
        ActiveSkill,
    }

    public class HintElement {
        public System.Func<bool> canDisplay;
        public HintType hintType;
        public EPropertyType propertyType;

        public HintElement() { }
        public HintElement Reset(System.Func<bool> canDisplay = null, HintType hintType = HintType.Normal) {
            this.canDisplay = canDisplay;
            this.hintType = hintType;
            return this;
        }
        public void SetAction(System.Func<bool> canDisplay) {
            this.canDisplay = canDisplay;
        }
    }

    public class HintElement_Normal : HintElement {
        public string content = "";

        public HintElement_Normal SetData(string content) {
            this.content = content;
            return this;
        }
        public override string ToString() {
            return this.content;
        }
    }
    public class HintElement_GetReward : HintElement {
        public uint itemid;
        public string content;

        public HintElement_GetReward SetData(uint itemid, string content) {
            this.content = content;
            this.itemid = itemid;
            return this;
        }

        public override string ToString() {
            return this.content;
        }
    }

    public class HintElement_Property : HintElement {
        public uint propertyId;
        public long diff;

        public Table.CSVAttr.Data csvAttr;
        public Table.CSVActiveSkillInfo.Data csvActiveSkill;

        public HintElement_Property SetData(uint propertyId, long diff, EPropertyType propertyType = EPropertyType.Property) {
            this.propertyId = propertyId;
            this.diff = diff;

            this.csvAttr = null;
            this.csvActiveSkill = null;
            if (propertyType == EPropertyType.Property) {
                this.csvAttr = CSVAttr.Instance.GetConfData(propertyId);
            }
            else if (propertyType == EPropertyType.ActiveSkill) {
                this.csvActiveSkill = CSVActiveSkillInfo.Instance.GetConfData(propertyId);
            }
            return this;
        }
    }
    public class HintElement_PropertyDirect : HintElement {
        public string content;

        public HintElement_PropertyDirect SetData(string content) {
            this.content = content;
            return this;
        }
    }

    public class HintElement_InBattle : HintElement
    {
        public string content = "";

        public HintElement_InBattle SetData(string content)
        {
            this.content = content;
            return this;
        }
        public override string ToString()
        {
            return this.content;
        }
    }
    public class HintElement_Static : HintElement_Normal {
    }

    public class HitElement_Marquee : HintElement {
        public uint id;
        public string content;
        public CSVAnnouncement.Data announcementData;
        public bool isSelf;
        public uint marqueeType;
        public bool isSort;
        public HitElement_Marquee(string connect, uint id = 0,uint type=0) {
            this.id = id;
            if (id == 0)
                this.announcementData = null;
            else
            {
                this.announcementData = CSVAnnouncement.Instance.GetConfData(id);
                if (this.announcementData == null)
                    DebugUtil.LogError("CSVAnnouncement not found id：" + id);
            }
            this.content = connect;
            this.marqueeType = type;
        }
    }
    public class HitElement_CommonInfo : HintElement {
        public uint id;
        public string content;
        public HitElement_CommonInfo(string content, uint id = 0) {
            this.id = id;
            this.content = content;
        }
    }

    public class HintQueue {
        public Queue<HintElement> queue = new Queue<HintElement>();
        public int Count { get { return this.queue.Count; } }

        public void Push(HintElement hint) {
            if (hint != null) {
                this.queue.Enqueue(hint);
            }
        }
        public HintElement Pop() {
            HintElement ret = null;
            if (this.Count > 0) {
                ret = this.queue.Peek();
                if (ret != null) {
                    if (ret.canDisplay == null || (ret.canDisplay != null && ret.canDisplay())) {
                        this.queue.Dequeue();
                    }
                }
            }
            return ret;
        }
        public HintElement Top() {
            HintElement ret = null;
            if (this.Count > 0) {
                ret = this.queue.Peek();
            }
            return ret;
        }
        public void Clear() {
            this.queue.Clear();
        }
    }

    public class Sys_Hint : SystemModuleBase<Sys_Hint> {
        private bool isOnLogin = false;
        public HintQueue hintNormal = new HintQueue();
        public HintQueue hintOther = new HintQueue();
        public HintQueue hintStatic = new HintQueue();
        public HintQueue hintCommonInfo = new HintQueue();

        // 外部从此处获取
        private readonly Lib.Core.ObjectPool<HintElement_Normal> pool_Normal = new Lib.Core.ObjectPool<HintElement_Normal>(20, () => {
            return new HintElement_Normal();
        });
        private readonly Lib.Core.ObjectPool<HintElement_GetReward> pool_GetReward = new Lib.Core.ObjectPool<HintElement_GetReward>(20, () => {
            return new HintElement_GetReward();
        });
        private readonly Lib.Core.ObjectPool<HintElement_Property> pool_Property = new Lib.Core.ObjectPool<HintElement_Property>(20, () => {
            return new HintElement_Property();
        });
        private readonly Lib.Core.ObjectPool<HintElement_PropertyDirect> pool_PropertyDirect = new Lib.Core.ObjectPool<HintElement_PropertyDirect>(20, () => {
            return new HintElement_PropertyDirect();
        });
        private readonly Lib.Core.ObjectPool<HintElement_InBattle> pool_InBattle = new Lib.Core.ObjectPool<HintElement_InBattle>(20, () => {
            return new HintElement_InBattle();
        });
        private readonly Lib.Core.ObjectPool<HintElement_Static> pool_Static = new Lib.Core.ObjectPool<HintElement_Static>(1, () => {
            return new HintElement_Static();
        });

        #region 系统级别函数
        public override void Init() {
            // cutscene不会弹出相关提示UI
            Sys_CutScene.Instance.eventEmitter.Handle<uint, uint>(Sys_CutScene.EEvents.OnRealStart, this.OnStart, true);
            Sys_CutScene.Instance.eventEmitter.Handle<uint, uint>(Sys_CutScene.EEvents.OnRealEnd, this.OnEnd, true);
            needHighSpeedCount = int.Parse(CSVParam.Instance.GetConfData(1071).str_value);
            normalSpeed = float.Parse(CSVParam.Instance.GetConfData(1056).str_value);
            highSpeed = float.Parse(CSVParam.Instance.GetConfData(1064).str_value);

            var announcementDatas = CSVAnnouncement.Instance.GetAll();
            for (int i = 0, len = announcementDatas.Count; i < len; ++i)
            {
                var item = announcementDatas[i];
                if (!priorityOrderList.Contains(item.Priority))
                    priorityOrderList.Add(item.Priority);
            }
            priorityOrderList.Add(0);
            priorityOrderList.Sort((x, y) => x.CompareTo(y));
        }
        public enum EEvents {
            RefreshMarqueeData,  //更新跑马灯数据
            RefreshCommonInfoData,  //更新底部公共信息数据
            HideGameObject,       //隐藏播报对象
            RefreshPromptBoxData,   //更新通用提示框PromptBox的界面数据
        }
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        private void OnStart(uint _, uint __) {
            this.ClearAll();
        }
        private void OnEnd(uint _, uint __) {
            this.ClearAll();
        }
        public override void OnLogin() {
            this.ClearAll();
            isOnLogin = true;
        }
        public override void OnLogout() {
            this.ClearAll();
            base.OnLogout();
            isOnLogin = false;
            Sys_Hint.Instance.eventEmitter.Trigger(EEvents.HideGameObject);
        }
        private void ClearAll() {
            this.hintNormal.Clear();
            this.hintOther.Clear();
            this.hintCommonInfo.Clear();
            selfMarqueeList.Clear();
            gmtMarqueeist.Clear();
            dicItemCount = 0;
            dicPrioriyToMarqueeQueue.Clear();
        }
        #endregion

        public int CountForOther() { return this.hintOther.Count; }
        public int CountForNormal() { return this.hintNormal.Count; }
        public int CountForStatic() { return this.hintStatic.Count; }
        private HintElement NewHint(HintType hintType) {
            /*UnityEngine.Debug.LogError(pool.Count);*/
            if (hintType == HintType.Normal || hintType == HintType.Map) {
                return this.pool_Normal.Get();
            }
            else if (hintType == HintType.GetReward) {
                return this.pool_GetReward.Get();
            }
            else if (hintType == HintType.Property) {
                return this.pool_Property.Get();
            }
            else if (hintType == HintType.PropertyDirect) {
                return this.pool_PropertyDirect.Get();
            }
            else if (hintType == HintType.InBattle)
            {
                return this.pool_InBattle.Get();
            }
            else if (hintType == HintType.Static) {
                return this.pool_Static.Get();
            }
            return null;
        }
        public void PushToPool(HintElement hint) {
            /*UnityEngine.Debug.LogError("After:" + pool.Count); */
            if (hint != null) {
                if (hint.hintType == HintType.Normal) {
                    this.pool_Normal.Push(hint as HintElement_Normal);
                }
                else if (hint.hintType == HintType.GetReward) {
                    this.pool_GetReward.Push(hint as HintElement_GetReward);
                }
                else if (hint.hintType == HintType.Property) {
                    this.pool_Property.Push(hint as HintElement_Property);
                }
                else if (hint.hintType == HintType.InBattle)
                {
                    this.pool_InBattle.Push(hint as HintElement_InBattle);
                }
                else if (hint.hintType == HintType.Static) {
                    this.pool_Static.Push(hint as HintElement_Static);
                }
            }
        }

        public HintElement_Normal PushContent_Normal(string text) {
            if (text == null) return null;
            HintElement_Normal normal = this.NewHint(HintType.Normal).Reset(null, HintType.Normal) as HintElement_Normal;
            normal.SetData(text);
            this.PushToNormal(normal);
            return normal;
        }
        public HintElement_GetReward PushContent_GetReward(string text, uint itemId) {
            HintElement_GetReward normal = this.NewHint(HintType.GetReward).Reset(null, HintType.GetReward) as HintElement_GetReward;
            normal.SetData(itemId, text);
            this.PushToNormal(normal);
            return normal;
        }
        public HintElement_Property PushContent_Property(uint propertyId, long changedValue, EPropertyType propertyType = EPropertyType.Property) {
            HintElement_Property normal = this.NewHint(HintType.Property).Reset(null, HintType.Property) as HintElement_Property;
            normal.SetData(propertyId, changedValue, propertyType);
            this.PushToOther(normal);
            return normal;
        }
        public HintElement_PropertyDirect PushContent_Property(string content) {
            HintElement_PropertyDirect normal = this.NewHint(HintType.PropertyDirect).Reset(null, HintType.PropertyDirect) as HintElement_PropertyDirect;
            normal.SetData(content);
            this.PushToOther(normal);
            return normal;
        }
        public HintElement_InBattle PushContent_InBattle(string text)
        {
            if (text == null) return null;
            HintElement_InBattle inBattle = this.NewHint(HintType.InBattle).Reset(null, HintType.InBattle) as HintElement_InBattle;
            inBattle.SetData(text);
            this.PushToNormal(inBattle);
            return inBattle;
        }
        public HintElement_Static PushContent_Static(string text) {
            // 只允许存在一个
            if (this.CountForStatic() <= 0) {
                HintElement_Static elem = this.NewHint(HintType.Static).Reset(null, HintType.Static) as HintElement_Static;
                elem.SetData(text);
                this.PushToStatic(elem);
                return elem;
            }
            return null;
        }

        private void PushToNormal(HintElement hint) { this.hintNormal.Push(hint); }
        private void PushToOther(HintElement hint) { this.hintOther.Push(hint); }
        private void PushToStatic(HintElement hint) {
            this.hintStatic.Push(hint);
        }

        public bool MarqueeObjIsShow { get; set; }
        List<HitElement_Marquee> selfMarqueeList = new List<HitElement_Marquee>();
        List<HitElement_Marquee> gmtMarqueeist = new List<HitElement_Marquee>();
        Dictionary<uint, Queue<HitElement_Marquee>> dicPrioriyToMarqueeQueue = new Dictionary<uint, Queue<HitElement_Marquee>>();
        int dicItemCount = 0;
        List<uint> priorityOrderList = new List<uint>();
        float normalSpeed;
        float highSpeed;
        int needHighSpeedCount;
        bool isSelfMarqueeListDirty = false;
        bool isGmtLMarqueeListDirty = false;

        public int GetHintCount()
        {
            return dicItemCount + selfMarqueeList.Count + gmtMarqueeist.Count;
        }
        public float ChechMarqueeSpeed()
        {
            return GetHintCount() > needHighSpeedCount ? highSpeed : normalSpeed;
        }
        /// <summary>
        /// 添加跑马灯信息
        /// </summary>
        /// <param name="id"></param>
        public void PushContent_Marquee(string connect,uint messageId=0, bool isSelf=false,CmdSocialSysTipNtf response=null) 
        {
            if (!isOnLogin)
                return;
            if (string.IsNullOrEmpty(connect))
                return;
            if (!isSelf && response != null && response.Fields.Count > 0)
            {
                for (int i = 0; i < response.Fields.Count; i++)
                {
                    SysTipFieldType tipFieldType = (SysTipFieldType)response.Fields[i].Type;
                    if (tipFieldType == SysTipFieldType.SysTipFieldRoleName)
                    {
                        //此条消息是玩家自身的 
                        if (response.Fields[i].Role.RoleId == Sys_Role.Instance.RoleId)
                        {
                            isSelf = true;
                            break;
                        }
                    }
                }
            }
            HitElement_Marquee marquee = new HitElement_Marquee(connect, messageId, response != null ? response.Type : 0);
            //Type=1 GMT跑马灯，Type=0为普通跑马灯
            bool isAdd = false;
            if (marquee.marqueeType == 0)
            {
                if (isSelf)
                {
                    isAdd = true;
                    selfMarqueeList.Add(marquee);
                    isSelfMarqueeListDirty = true;
                }
            }
            else if (marquee.marqueeType == 1)
            {
                isAdd = true;
                gmtMarqueeist.Add(marquee);
                isGmtLMarqueeListDirty = true;
            }
            if (!isAdd)
            {
                if (marquee.announcementData != null)
                {
                    //根据角色等级是否达到、任务是否完成 添加，
                    if (marquee.announcementData.LvShow != 0)
                    {
                        if (Sys_Role.Instance.Role.Level < marquee.announcementData.LvShow)
                            return;
                    }
                    if (marquee.announcementData.MissionShow != 0)
                    {
                        if (!Sys_Task.Instance.IsFinish(marquee.announcementData.MissionShow))
                            return;
                    }
                    if (dicPrioriyToMarqueeQueue.ContainsKey(marquee.announcementData.Priority))
                    {
                        var queue = dicPrioriyToMarqueeQueue[marquee.announcementData.Priority];
                        queue.Enqueue(marquee);
                    }
                    else
                    {
                        dicPrioriyToMarqueeQueue.Add(marquee.announcementData.Priority, new Queue<HitElement_Marquee>());
                        var queue = dicPrioriyToMarqueeQueue[marquee.announcementData.Priority];
                        queue.Enqueue(marquee);
                    }
                }
                else
                {
                    if (dicPrioriyToMarqueeQueue.ContainsKey(0))
                    {
                        var queue = dicPrioriyToMarqueeQueue[0];
                        queue.Enqueue(marquee);
                    }
                    else
                    {
                        dicPrioriyToMarqueeQueue.Add(0, new Queue<HitElement_Marquee>());
                        var queue = dicPrioriyToMarqueeQueue[0];
                        queue.Enqueue(marquee);
                    }
                }
                dicItemCount += 1;
            }
            
            if (!this.MarqueeObjIsShow)
            {
                this.GetNextMarqueeData();
                this.MarqueeObjIsShow = true;
            }
        }

        static int SortBy_Priority(HitElement_Marquee a, HitElement_Marquee b)
        {
            if (a.announcementData != null && b.announcementData != null)
            {
                if (a.announcementData.Priority > b.announcementData.Priority)
                    return -1;
                else if (a.announcementData.Priority == b.announcementData.Priority)
                    return 0;
                else
                    return 1;
            }
            else
                return 0;
        }
        public void GetNextMarqueeData()
        {
            HitElement_Marquee marquee = null;
            if (gmtMarqueeist.Count > 0)
            {
                if (isGmtLMarqueeListDirty)
                {
                    gmtMarqueeist.Sort(SortBy_Priority);
                    isGmtLMarqueeListDirty = false;
                }
                marquee = gmtMarqueeist[0];
                gmtMarqueeist.Remove(marquee);
                
            }
            if (marquee == null && selfMarqueeList.Count > 0)
            {
                if (isSelfMarqueeListDirty)
                {
                    selfMarqueeList.Sort(SortBy_Priority);
                    isSelfMarqueeListDirty = false;
                }
                marquee = selfMarqueeList[0];
                selfMarqueeList.Remove(marquee);
            }
          
            if(marquee == null && dicItemCount > 0)
            {
                for(int i = 0; i < priorityOrderList.Count; ++i)
                {
                    uint priority = priorityOrderList[i];
                    if (dicPrioriyToMarqueeQueue.ContainsKey(priority))
                    {
                        var queue = dicPrioriyToMarqueeQueue[priority];
                        if (queue.Count > 0)
                        {
                            marquee = queue.Dequeue();
                            dicItemCount -= 1;
                            break;
                        }
                    }
                }
            }
            if (marquee != null)
            {
                Sys_Hint.Instance.eventEmitter.Trigger(EEvents.RefreshMarqueeData, marquee);
            }

        }
        public bool BottomCommonObjIsShow { get; set; }
        /// <summary>
        /// 添加底部公共信息
        /// </summary>
        /// <param name="content"></param>
        /// <param name="id"></param>
        public void PushContent_CommonInfo(string content) {
            if (!isOnLogin) return;
            if (string.IsNullOrEmpty(content)) return;
            HitElement_CommonInfo commonInfo = new HitElement_CommonInfo(content);
            this.hintCommonInfo.Push(commonInfo);
            if (!this.BottomCommonObjIsShow) {
                this.GetNextCommonInfoData();
                this.BottomCommonObjIsShow = true;
            }
        }
        public void GetNextCommonInfoData() {
            if (this.hintCommonInfo.Count > 0) {
                HitElement_CommonInfo commonInfo = (HitElement_CommonInfo)this.hintCommonInfo.Pop();
                if (commonInfo != null) {
                    Sys_Hint.Instance.eventEmitter.Trigger(EEvents.RefreshCommonInfoData, commonInfo);
                }
            }
        }
        //战斗内提示：当前处于战斗中，无法进行该操作
        public bool PushForbidOprationInFight() {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight) {
                this.PushContent_Normal(LanguageHelper.GetTextContent(2020699));
                return true;
            }

            return false;
        }

        // //战斗内提示：当前战斗不生效，将在下一场战斗生效
        public void PushEffectInNextFight() {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight && !Net_Combat.Instance.m_IsWatchBattle)
            {
                this.PushContent_Normal(LanguageHelper.GetTextContent(2020698));
            }
        }
    }
}