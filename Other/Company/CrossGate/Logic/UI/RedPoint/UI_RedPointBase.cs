using System.Collections.Generic;
using Framework;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Logic
{
    public class UI_ChatSimplify_RedPoint : UI_RedPointBase<UI_ChatSimplify>
    {
        public Transform societyRedPointParent;
        public Vector3 redPointVec;

        protected override void InitializeFields()
        {
            base.InitializeFields();
            redPointVec = new Vector3(18f, 18f, 0f);
        }

        protected override void SetCps()
        {
            societyRedPointParent = mTrans.gameObject.FindChildByName("_btn_Friend").transform;
        }

        protected override void InitRedPoints()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, societyRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_Society.Instance.HasUnReadRoleChat() || Sys_Society.Instance.HasUnReadGroupChat() || Sys_Society.Instance.HasUnGetGift())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnGetChat, (int)RedPointElement.EEvents.OnGetGroupChat, (int)RedPointElement.EEvents.OnGetGift, (int)RedPointElement.EEvents.OnUIMenuShow,
            (int)RedPointElement.EEvents.OnReadRoleChat, (int)RedPointElement.EEvents.OnReadGift}, redPointVec);
            redPoints.Add(redPoint);
        }
    }

    public class UI_Society_RedPoint : UI_RedPointBase<UI_Society>
    {
        public Vector3 redPointVec;

        public RedPointElement friendRedPoint;
        public Transform friendRedPointParent;

        public RedPointElement recentRedPoint;
        public Transform recentRedPointParent;

        public RedPointElement groupRedPoint;
        public Transform groupRedPointParent;

        protected override void InitializeFields()
        {
            base.InitializeFields();
            redPointVec = new Vector3(38f, 9f, 0f);
        }

        protected override void SetCps()
        {
            friendRedPointParent = mTrans.gameObject.FindChildByName("Toggle_Friend").transform;
            recentRedPointParent = mTrans.gameObject.FindChildByName("Toggle_Recent").transform;
            groupRedPointParent = mTrans.gameObject.FindChildByName("Toggle_Group").transform;
        }

        protected override void InitRedPoints()
        {
            InitFriendRedPoint();
            InitRecentRedPoint();
            InitGroupRedPoint();
        }

        private void InitFriendRedPoint()
        {
            friendRedPoint = new RedPointElement(ERedPointType.Pure, friendRedPointParent, args =>
            {
                var result = ERedPointShowMode.Hide;
                if (Sys_Society.Instance.HasUnReadFriendRoleChat() || Sys_Society.Instance.HasUnGetGiftFriend())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int>
            {
                (int)RedPointElement.EEvents.OnGetChat, (int)RedPointElement.EEvents.OnReadRoleChat, (int)RedPointElement.EEvents.OnGetGift, (int)RedPointElement.EEvents.OnReadGift,
            }, redPointVec);
            redPoints.Add(friendRedPoint);
        }

        void InitRecentRedPoint()
        {
            recentRedPoint = new RedPointElement(ERedPointType.Pure, recentRedPointParent, args =>
            {
                var result = ERedPointShowMode.Hide;
                if (Sys_Society.Instance.HasUnReadRecentRoleChat() || Sys_Society.Instance.HasUnGetGiftRecent())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int>
            {
                (int)RedPointElement.EEvents.OnGetChat, (int)RedPointElement.EEvents.OnReadRoleChat, (int)RedPointElement.EEvents.OnGetGift, (int)RedPointElement.EEvents.OnReadGift,
            }, redPointVec);
            redPoints.Add(recentRedPoint);
        }

        void InitGroupRedPoint()
        {
            groupRedPoint = new RedPointElement(ERedPointType.Pure, groupRedPointParent, args =>
            {
                var result = ERedPointShowMode.Hide;
                if (Sys_Society.Instance.HasUnReadGroupChat())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int>
            {
                (int)RedPointElement.EEvents.OnGetGroupChat, (int)RedPointElement.EEvents.OnReadGroupChat
            }, redPointVec);
            redPoints.Add(groupRedPoint);
        }
    }

    public class UI_Family_RedPoint : UI_RedPointBase<UI_Family>
    {
        //家族成员菜单
        public Transform memberRedPointParent;
        public Vector3 memberRedPointVec;
        //家族建筑菜单
        public Transform buildRedPointParent;
        public Vector3 buildRedPointVec;
        //申请列表按钮
        public Transform applyRedPointParent;
        public Vector3 applyRedPointVec;
        //银行按钮
        public Transform bankRedPointParent;
        public Vector3 bankRedPointVec;
        //家族大厅菜单
        public Transform hallRedPointParent;
        public Vector3 hallRedPointVec;
        //家族大厅签到按钮
        public Transform hallSignRedPointParent;
        public Vector3 hallSignRedPointVec;

        //家族活动页签
        public Transform activeRedPointParent;
        public Vector3 activeRedPointVec;

        //合并申请按钮
        public Transform mergeApplyPointParent;
        public Vector3 mergeApplyPointVec;
        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.memberRedPointVec = new Vector3(15.1f, 30.9f, 0f);
            this.buildRedPointVec = new Vector3(15.1f, 30.9f, 0f);
            this.applyRedPointVec = new Vector3(78.5f, 22f, 0f);
            this.bankRedPointVec = new Vector3(46.4f, 91.7f, 0f);
            this.hallRedPointVec = new Vector3(15.1f, 30.9f, 0f);
            this.activeRedPointVec = new Vector3(15.1f, 30.9f, 0f);
            this.hallSignRedPointVec = new Vector3(17.3f, 17.9f, 0f);
            this.mergeApplyPointVec = new Vector3(78.0f, 19.0f, 0f);
        }
        protected override void SetCps()
        {
            this.memberRedPointParent = this.mTrans.gameObject.FindChildByName("Toggle_Member").transform;
            this.buildRedPointParent = this.mTrans.gameObject.FindChildByName("Toggle_Build").transform;
            this.applyRedPointParent = this.mTrans.gameObject.FindChildByName("Button_Apply").transform;
            this.bankRedPointParent = this.mTrans.gameObject.FindChildByName("View_Storage/Button").transform;
            this.hallRedPointParent = this.mTrans.gameObject.FindChildByName("Toggle_Hall").transform;
            this.activeRedPointParent = this.mTrans.gameObject.FindChildByName("Toggle_Activity").transform;
            this.hallSignRedPointParent = this.mTrans.gameObject.FindChildByName("Sign/Button_Sign").transform;
            this.mergeApplyPointParent = this.mTrans.gameObject.FindChildByName("View_Hall/Button_Merge").transform;
        }
        protected override void InitRedPoints()
        {
            InitMemberRedPoint();
            InitApplyRedPoint();
            InitBuildRedPoint();
            InitBankRedPoint();
            InitHallRedPoint();
            InitActiveRedPoint();
            InitSignRedPoint();
            IntMergeApplyRedPoint();
        }
        private void InitMemberRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.memberRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_Family.Instance.IsRedPoint_Apply())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnFamilyApplyMember}, this.memberRedPointVec);

            this.redPoints.Add(redPoint);
        }
        private void InitApplyRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.applyRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_Family.Instance.IsRedPoint_Apply())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnFamilyApplyMember }, this.applyRedPointVec);
            this.redPoints.Add(redPoint);
        }
        private void InitBuildRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.buildRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_Family.Instance.IsRedPoint_BankView())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnFamilyDonate, (int)RedPointElement.EEvents.OnFamilyDonateReward }, this.buildRedPointVec);
            this.redPoints.Add(redPoint);
        }
        private void InitBankRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.bankRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_Family.Instance.IsRedPoint_BankView())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnFamilyDonate, (int)RedPointElement.EEvents.OnFamilyDonateReward }, this.bankRedPointVec);
            this.redPoints.Add(redPoint);
        }
        private void InitHallRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.hallRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_Family.Instance.IsRedPoint_Hall())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnFamilySign, (int)RedPointElement.EEvents.OnFamilyMergeApply }, this.hallRedPointVec);
            this.redPoints.Add(redPoint);
        }

        private void InitActiveRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.activeRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_Family.Instance.IsRedPoint_Active())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnFamilyActiveRedPoint }, this.activeRedPointVec);
            this.redPoints.Add(redPoint);
        }

        private void InitSignRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.hallSignRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_Family.Instance.IsRedPoint_Sign())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnFamilySign }, this.hallSignRedPointVec);
            this.redPoints.Add(redPoint);
        }
        private void IntMergeApplyRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.mergeApplyPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_Family.Instance.HasMergeApply())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnFamilyMergeApply }, this.mergeApplyPointVec);
            this.redPoints.Add(redPoint);
        }
    }

    public class UI_Menu_RedPoint : UI_RedPointBase<UI_Menu>
    {
        public Transform familyRedPointParent;
        public Vector3 familyRedPointVec;

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.familyRedPointVec = new Vector3(23f, 23.4f, 0f);
        }

        protected override void SetCps()
        {
            this.familyRedPointParent = this.mTrans.gameObject.FindChildByName("Button_Family").transform;
        }

        protected override void InitRedPoints()
        {
            InitFamilyRedPoint();
        }

        private void InitFamilyRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.familyRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_Family.Instance.IsRedPoint())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null,
            new List<int>{
                (int)RedPointElement.EEvents.OnFamilyApplyMember, (int)RedPointElement.EEvents.OnFamilyDonate,
                (int)RedPointElement.EEvents.OnFamilyDonateReward,(int)RedPointElement.EEvents.OnUIMenuShow,
                (int)RedPointElement.EEvents.OnFamilySign, (int)RedPointElement.EEvents.OnFamilyActiveRedPoint,
                (int)RedPointElement.EEvents.OnFamilyConsignRedCondChanged,
                (int)RedPointElement.EEvents.OnFamilyMergeApply
            }, this.familyRedPointVec);
            this.redPoints.Add(redPoint);
        }
    }

    public class UI_Mainbattle_RedPoint : UI_RedPointBase<UI_Mainbattle_Function>
    {
        public Transform travellerDailyRedPointParent;   //旅人志
        public Transform awakenRedPointParent;   //旅人觉醒
        public Transform welfareRedPointParent;   //福利
        public Transform firstPayRedPointParent;   //首充
        public Transform magicbookRedPointParent;   //魔力宝典
        public Transform skillRedPointParent;   //技能

        public Vector3 travellerDailyRedPointVec;
        public Vector3 awakenRedPointVec;
        public Vector3 welfareRedPointVec;
        public Vector3 firstPayRedPointVec;
        public Vector3 magicbookRedPointVec;
        public Vector3 skillRedPointVec;


        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.travellerDailyRedPointVec = new Vector3(23f, 23.4f, 0f);
            this.awakenRedPointVec = new Vector3(23f, 23.4f, 0f);
            this.welfareRedPointVec = new Vector3(23f, 23.4f, 0f);
            this.firstPayRedPointVec = new Vector3(23f, 23.4f, 0f);
            this.magicbookRedPointVec = new Vector3(23f, 23.4f, 0f);
            this.skillRedPointVec = new Vector3(23f, 23.4f, 0f);
        }

        protected override void SetCps()
        {
            CSVBattleMenuFunction.Data travellerDailyData = CSVBattleMenuFunction.Instance.GetConfData(4);
            bool isTravellerDailyShow = IsShowInBattle(travellerDailyData);
            if (isTravellerDailyShow)
            {
                this.travellerDailyRedPointParent = this.mTrans.gameObject.FindChildByName("470").transform;
            }
            CSVBattleMenuFunction.Data awakenData = CSVBattleMenuFunction.Instance.GetConfData(18);
            bool isAwakenShow = IsShowInBattle(awakenData);
            if (isAwakenShow)
            {         
                this.awakenRedPointParent = this.mTrans.gameObject.FindChildByName("580").transform;
            }
            CSVBattleMenuFunction.Data welfareData = CSVBattleMenuFunction.Instance.GetConfData(14);
            bool iswelfareShow = IsShowInBattle(welfareData);
            if (iswelfareShow)
            {
                this.welfareRedPointParent = this.mTrans.gameObject.FindChildByName("420").transform;
            }
            CSVBattleMenuFunction.Data firstPayData = CSVBattleMenuFunction.Instance.GetConfData(15);
            bool isfirstPayShow = IsShowInBattle(firstPayData);
            if (isfirstPayShow)
            {
                this.firstPayRedPointParent = this.mTrans.gameObject.FindChildByName("550").transform;
            }
            CSVBattleMenuFunction.Data magicbookData = CSVBattleMenuFunction.Instance.GetConfData(17);
            bool ismagicbookDataShow = IsShowInBattle(magicbookData);
            if (ismagicbookDataShow)
            {
                this.magicbookRedPointParent = this.mTrans.gameObject.FindChildByName("377").transform;
            }
            CSVBattleMenuFunction.Data skillData = CSVBattleMenuFunction.Instance.GetConfData(2);
            bool isskillDataShow = IsShowInBattle(skillData);
            if (isskillDataShow)
            {
                this.skillRedPointParent = this.mTrans.gameObject.FindChildByName("58").transform;
            }
        }

        private bool IsShowInBattle(CSVBattleMenuFunction.Data data)
        {
            bool isOpen = Sys_FunctionOpen.Instance.IsOpen(data.functionId);
            bool isShowInFamilyBattle = (Sys_FamilyResBattle.Instance.InFamilyBattle && data.ResourcebattleShow == 1) || (!Sys_FamilyResBattle.Instance.InFamilyBattle && data.isBattle == 1);

            return data != null && isOpen && isShowInFamilyBattle;
        }  

        protected override void InitRedPoints()
        {
            InitTravellerDailyRedPoint();
            InitTravellerAwakenRedPoint();
            InitWelfareRedPoint();
            InitFirstPayRedPoint();
            InitMagicBookRedPoint();
            InitSkillRedPoint();
        }

        private void InitTravellerDailyRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.travellerDailyRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_Knowledge.Instance.IsRedPoint())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null,
            new List<int>{
                (int)RedPointElement.EEvents.OnTravellerDailyRedPoint}, this.travellerDailyRedPointVec);
            this.redPoints.Add(redPoint);
        }

        private void InitTravellerAwakenRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.awakenRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_TravellerAwakening.Instance.CheckTarget())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null,
            new List<int>{
                (int)RedPointElement.EEvents.OnAwakenRedPoint}, this.awakenRedPointVec);
            this.redPoints.Add(redPoint);

        }

        private void InitWelfareRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.welfareRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_OperationalActivity.Instance.CheckOperationalActivityRedPoint())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null,
            new List<int>{
                (int)RedPointElement.EEvents.OnWelfareRedPoint}, this.welfareRedPointVec);
            this.redPoints.Add(redPoint);

        }

        private void InitFirstPayRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.firstPayRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_OperationalActivity.Instance.CheckFirstChargeRedPoint()
)
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null,
            new List<int>{
                (int)RedPointElement.EEvents.OnFirstPayRedPoint}, this.firstPayRedPointVec);
            this.redPoints.Add(redPoint);
        }

        private void InitMagicBookRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.magicbookRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_MagicBook.Instance.CheckMagicBookAndTeachRedPoint())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null,
            new List<int>{
                (int)RedPointElement.EEvents.OnMagicBookRedPoint}, this.magicbookRedPointVec);
            this.redPoints.Add(redPoint);
        }

        private void InitSkillRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.skillRedPointParent, args =>
            {
                ERedPointShowMode result;
                if(Sys_Skill.Instance.ExistedLearnShowTipSkill() || Sys_Talent.Instance.CanLianhua())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null,
            new List<int>{
                (int)RedPointElement.EEvents.OnSkillRedPoint}, this.skillRedPointVec);
            this.redPoints.Add(redPoint);
        }
    }

    public class UI_Pet_RedPoint : UI_RedPointBase<UI_Pet_Message>
    {
        //宠物菜单
        public Transform attrRedPointParent;
        public Vector3 attrRedPointVec;

        //加点
        public Transform addPetRedPointParent;
        public Vector3 addRedPointVec;

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.attrRedPointVec = new Vector3(27.3f, 25.9f, 0f);
            this.addRedPointVec = new Vector3(70.3f, 24.1f, 0f);
        }

        protected override void SetCps()
        {
            this.attrRedPointParent = this.mTrans.gameObject.FindChildByName("Toggle_Attribute").transform;
            addPetRedPointParent = this.mTrans.gameObject.FindChildByName("View_Right/Btn_Add").transform;
        }

        protected override void InitRedPoints()
        {
            InitAttrRedPoints();
            //InitAddPointRedPoints();
        }

        private void InitAttrRedPoints()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.attrRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_Pet.Instance.IsHasFightPetPointNotUse())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnPetAddPonit }, this.attrRedPointVec);
            this.redPoints.Add(redPoint);
        }

        private void InitAddPointRedPoints()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.addPetRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_Pet.Instance.IsHasFightPetPointNotUse())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnPetAddPonit }, this.addRedPointVec);
            this.redPoints.Add(redPoint);
        }
    }

    public class UI_PetAdd_RedPoint : UI_RedPointBase<UI_Pet_AddPoint>
    {
        //加点
        public Transform addPetRedPointParent;
        public Vector3 addRedPointVec;

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.addRedPointVec = new Vector3(140.1f, 31.2f, 0f);
        }

        protected override void SetCps()
        {
            addPetRedPointParent = this.mTrans.gameObject.FindChildByName("Text_Potency").transform;
        }

        protected override void InitRedPoints()
        {
            InitAddPointRedPoints();
        }

        private void InitAddPointRedPoints()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.addPetRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_Pet.Instance.IsHasFightPetPointNotUse())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnPetAddPonit }, this.addRedPointVec);
            this.redPoints.Add(redPoint);
        }
    }

    public class UI_MessageBag_RedPoint : UI_RedPointBase<UI_MessageBag>
    {
        public Transform teamRedPointParent;
        public Vector3 teamRedPointVec;

        public Transform friendRedPointParent;
        public Vector3 friendRedPointVec;

        public Transform familyRedPointParent;
        public Vector3 familyRedPointVec;

        protected override void InitializeFields()
        {

            base.InitializeFields();
            this.teamRedPointVec = new Vector3(49.0f, 15f, 0f);
            this.friendRedPointVec = new Vector3(49.0f, 15f, 0f);
            this.familyRedPointVec = new Vector3(49.0f, 15f, 0f);
        }
        protected override void SetCps()
        {
            this.teamRedPointParent = this.mTrans.gameObject.FindChildByName("Toggle0").transform;
            this.friendRedPointParent = this.mTrans.gameObject.FindChildByName("Toggle1").transform;
            this.familyRedPointParent = this.mTrans.gameObject.FindChildByName("Toggle2").transform;

        }

        protected override void InitRedPoints()
        {

            InitTeamRedPoint();
            InitFriendRedPoint();
            InitFamilyRedPoint();
        }

        private void InitTeamRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.teamRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_MessageBag.Instance.IsMessageBagRedPoint(0) || Sys_MessageBag.Instance.IsMessageBagRedPoint(3))
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnMessageBagTeam }, this.teamRedPointVec);
            this.redPoints.Add(redPoint);
        }
        private void InitFriendRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.friendRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_MessageBag.Instance.IsMessageBagRedPoint(1))
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnMessageBagFriend }, this.friendRedPointVec);
            this.redPoints.Add(redPoint);
        }
        private void InitFamilyRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.familyRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_MessageBag.Instance.IsMessageBagRedPoint(2))
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnMessageBagFamily }, this.familyRedPointVec);
            this.redPoints.Add(redPoint);
        }

    }
    public class UI_Awaken_RedPoint : UI_RedPointBase<UI_Awaken>
    {
        public Transform imprintRedPointParent;
        public Vector3 imprintRedPointVec;

        protected override void InitializeFields()
        {

            base.InitializeFields();
            this.imprintRedPointVec = new Vector3(23f, 23.4f, 0f);

        }
        protected override void SetCps()
        {
            this.imprintRedPointParent = this.mTrans.gameObject.FindChildByName("ImprintButton").transform;
        }
        protected override void InitRedPoints()
        {

            InitImprintRedPoint();
        }

        private void InitImprintRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.imprintRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_TravellerAwakening.Instance.CheckUIMenuImprintRedPoint())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnAwakenRedPoint }, this.imprintRedPointVec);
            this.redPoints.Add(redPoint);
        }
    }

    public class UI_DailyGift_RedPoint : UI_RedPointBase<UI_DailyGift>
    {

        public Transform firstRedPointParent;
        public Vector3 firstRedPointVec;

        public Transform secondRedPointParent;
        public Vector3 secondRedPointVec;

        public Transform thirdRedPointParent;
        public Vector3 thirdRedPointVec;

        public Transform sevenDayRedPointParent;
        public Vector3 sevenDayRedPointVec;

        protected override void InitializeFields()
        {

            base.InitializeFields();
            this.firstRedPointVec = new Vector3(-185.0f, -58.0f, 0f);
            this.secondRedPointVec = new Vector3(-185.0f, -58.0f, 0f);
            this.thirdRedPointVec = new Vector3(-185.0f, -58.0f, 0f);
            this.sevenDayRedPointVec = new Vector3(100.0f, 65.0f, 0f);
        }
        protected override void SetCps()
        {
            this.firstRedPointParent = this.mTrans.gameObject.FindChildByName("0").transform;
            this.secondRedPointParent = this.mTrans.gameObject.FindChildByName("1").transform;
            this.thirdRedPointParent = this.mTrans.gameObject.FindChildByName("2").transform;
            this.sevenDayRedPointParent = this.mTrans.gameObject.FindChildByName("Text_Tip").transform;

        }

        protected override void InitRedPoints()
        {

            InitFirstRedPoint();
            InitSecondRedPoint();
            InitThirdRedPoint();
            InitSevenDayRedPoint();
        }

        private void InitFirstRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.firstRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_OperationalActivity.Instance.CheckSingleGiftRedPoint(1))
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnDailyGiftRedPoint }, this.firstRedPointVec);
            this.redPoints.Add(redPoint);
        }
        private void InitSecondRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.secondRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_OperationalActivity.Instance.CheckSingleGiftRedPoint(2))
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnDailyGiftRedPoint }, this.secondRedPointVec);
            this.redPoints.Add(redPoint);
        }
        private void InitThirdRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.thirdRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_OperationalActivity.Instance.CheckSingleGiftRedPoint(3))
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnDailyGiftRedPoint }, this.thirdRedPointVec);
            this.redPoints.Add(redPoint);
        }

        private void InitSevenDayRedPoint()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.sevenDayRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_OperationalActivity.Instance.CheckSevenDayRedPoint())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnDailyGiftRedPoint }, this.sevenDayRedPointVec);
            this.redPoints.Add(redPoint);
        }


    }

    public class UI_JsBattle_RedPoint : UI_RedPointBase<UI_JSBattle>
    {
        //奖励
        public Transform rewardRedPointParent;
        public Vector3 rewardRedPointVec;
        //纪录
        public Transform recordRedPointParent;
        public Vector3 recordRedPointVec;
        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.rewardRedPointVec = new Vector3(18f, 15f, 0f);
            this.recordRedPointVec = new Vector3(18f, 15f, 0f);
        }

        protected override void SetCps()
        {
            rewardRedPointParent = this.mTrans.gameObject.FindChildByName("Btn_Reward").transform;
            recordRedPointParent = this.mTrans.gameObject.FindChildByName("Btn_Rerord").transform;
        }

        protected override void InitRedPoints()
        {
            InitJSBattleRewardRedPoints();
            InitJSBattleRecordRedPoints();

        }

        private void InitJSBattleRewardRedPoints()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.rewardRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_JSBattle.Instance.HasJSBattleReward())
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnJSBattleHasReward }, this.rewardRedPointVec);
            this.redPoints.Add(redPoint);
        }

        private void InitJSBattleRecordRedPoints()
        {
            RedPointElement redPoint;
            redPoint = new RedPointElement(ERedPointType.Pure, this.recordRedPointParent, args =>
            {
                ERedPointShowMode result;
                if (Sys_JSBattle.Instance.showNewDefence)
                {
                    result = ERedPointShowMode.Show;
                }
                else
                {
                    result = ERedPointShowMode.Hide;
                }
                return result;
            }, null, new List<int> { (int)RedPointElement.EEvents.OnJSBattleHasRecord }, this.recordRedPointVec);
            this.redPoints.Add(redPoint);
        }
    }

    public static class T_RedPoint
    {
        public static ERedPointShowMode Convert(bool value) { return value ? ERedPointShowMode.Show : ERedPointShowMode.Hide; }
    }

    [System.Serializable]
    public class RedPointElement
    {
        public Transform parent { get; private set; }
        public GameObject redPoint { get; private set; }

        public System.Func<object[], ERedPointShowMode> canDisplay { get; set; }
        public System.Action<GameObject, bool, object[]> showAction { get; set; }
        // 这里EEventType需要参数为params object[] args的回调事件
        private readonly List<int> events = new List<int>();

        private bool hasListened = false;

        private readonly ERedPointType eType;
        private Vector3 offsetBaseCornerIndex;
        private bool hasLoaded = false;

        private AsyncOperationHandle<GameObject> assetRequest;

        public RedPointElement() { }

        // nornerIndex：是矩形的四个角的下标表示， 顺时针从左下角[0]开始。
        // offset：是基于所选gameobject中心点的偏移。
        public RedPointElement(ERedPointType eType, Transform parent, System.Func<object[],
            ERedPointShowMode> canDisplay, System.Action<GameObject, bool, object[]> showAction,
            List<int> events, Vector3 offset = default(Vector3))
        {
            this.hasListened = false;
            this.parent = parent;
            this.canDisplay = canDisplay;
            this.showAction = showAction;
            this.events = events ?? this.events;

            this.eType = eType;
            this.offsetBaseCornerIndex = offset;

            this.Listen(true);
        }
        public void Load()
        {
            if (!this.hasLoaded)
            {
                // 根据红点类型动态加载红点prefab，赋值给redPoint
                switch (this.eType)
                {
                    case ERedPointType.WithNumber:
                        redPoint = GameObject.Instantiate<GameObject>(GlobalAssets.GetAsset<GameObject>(GlobalAssets.sPrefab_RedPoint_Big));
                        OnRedPointInstantiated();
                        break;
                    default:
                        redPoint = GameObject.Instantiate<GameObject>(GlobalAssets.GetAsset<GameObject>(GlobalAssets.sPrefab_RedPoint_Small));
                        OnRedPointInstantiated();
                        break;
                }
            }
        }

        private void OnRedPointInstantiated()
        {
            this.redPoint.transform.SetParent(this.parent, false);

            this.CalPosition(this.redPoint);

            this.hasLoaded = this.redPoint != null;
        }

        // 卸载红点资源
        public void Unload()
        {
            AddressablesUtil.ReleaseInstance(ref this.assetRequest, this.OnAddressableAssetLoaded);
        }

        private void OnAddressableAssetLoaded(AsyncOperationHandle<GameObject> handle)
        {
            this.redPoint = handle.Result;
            this.redPoint.transform.SetParent(this.parent, false);

            this.CalPosition(this.redPoint);

            this.hasLoaded = this.redPoint != null;
        }


        void CalPosition(GameObject redPoint)
        {
            if (redPoint != null)
            {
                /*
                CP_TransformAdjuster cp = redPoint.GetNeedComponent<CP_TransformAdjuster>();
                if (cp != null)
                {
                    cp.frameRate = 8;
                    cp.localPosition.use = true;
                    cp.localPosition.v3 = this.offsetBaseCornerIndex;
                }
                */
                redPoint.transform.localScale = Vector3.one;
                this.SetPosition();
            }
        }

        public void SetPosition()
        {
            if (this.redPoint != null)
            {
                RectTransform rectTransform = this.redPoint.transform as RectTransform;
                if (null != rectTransform)
                {
                    rectTransform.anchoredPosition3D = this.offsetBaseCornerIndex;
                }
                else
                {
                    this.redPoint.transform.localPosition = this.offsetBaseCornerIndex;
                }
            }
        }

        public static EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            OnReadRoleChat,
            OnGetChat,
            OnReadGift,
            OnGetGift,
            OnUpdateMailState,
            OnFamilyApplyMember,
            OnFamilyDonate,
            OnFamilyDonateReward,
            OnUIMenuShow,
            OnGetGroupChat,
            OnReadGroupChat,
            OnFamilySign,
            OnPetAddPonit,
            OnAwakenRedPoint,
            OnTravellerDailyRedPoint,
            OnWelfareRedPoint,
            OnFirstPayRedPoint,
            OnMagicBookRedPoint,
            OnRoleIconRedPoint,
            OnFamilyActiveRedPoint,
            OnMessageBagTeam,
            OnMessageBagFriend,
            OnMessageBagFamily,
            OnFamilyRedPacketRedPoint,
            OnDailyGiftRedPoint,
            OnFamilyConsignRedCondChanged,
            OnFamilyMergeApply,
            OnJSBattleHasReward,
            OnJSBattleHasRecord,
            OnSkillRedPoint,
        }

        public void Listen(bool toRegister)
        {
            if (!this.hasListened && toRegister)
            {
                this.events.ForEach(e =>
                {
                    eventEmitter.Handle<object[]>((EEvents)e, this.Process, true);
                });
                this.hasListened = true;
            }
            else if (!toRegister)
            {
                this.events.ForEach(e =>
                {
                    eventEmitter.Handle<object[]>((EEvents)e, this.Process, false);
                });
                this.hasListened = false;
            }
        }

        public void Process(params object[] args)
        {
            if (this.canDisplay != null)
            {
                ERedPointShowMode canMode = this.canDisplay(args);
                if (canMode == ERedPointShowMode.Show && this.redPoint == null) { this.Load(); if (this.redPoint == null) { return; } }
                if (canMode != ERedPointShowMode.Ignore)
                {
                    this.SetPosition();
                    bool can = canMode == ERedPointShowMode.Show ? true : false;
                    if (this.redPoint != null)
                    {
                        this.redPoint.SetActive(can);
                        this.showAction?.Invoke(this.redPoint, can, args);
                    }
                }
            }
        }
    }

    //TODO： 有空优化
    public class UI_RedPointBase<T> : MonoBehaviour
    {
        public T mono { get; set; } // 基础ui脚本
        public List<RedPointElement> redPoints;

        protected Transform mTrans;
        protected GameObject mGo;

        private bool mStarted = false;

        private void Awake()
        {
            this.mTrans = this.transform;
            this.mGo = this.gameObject;


            this.redPoints = new List<RedPointElement>();
            this.InitializeFields();
            this.SetCps();
            this.InitRedPoints();
        }

        public virtual void Init(T mono)
        {
            this.mono = mono;
        }

        protected virtual void InitializeFields() { }
        protected virtual void SetCps() { }
        protected virtual void OnEnable()
        {
            if (this.mStarted)
            {
                this.RefreshAllRedPoints();
            }
            this.ProcessEventsForEnable(true);
        }

        protected void Start()
        {
            this.RefreshAllRedPoints();
            this.mStarted = true;
        }
        protected virtual void InitRedPoints() { }
        public void RefreshAllRedPoints()
        {
            for (int i = 0, length = this.redPoints.Count; i < length; ++i) {
                this.redPoints[i].Process();
            }
            this.RefreshAllRedPointsEvent();
        }
        protected virtual void RefreshAllRedPointsEvent()
        {

        }
        protected virtual void OnDisable()
        {
            this.ProcessEventsForEnable(false);
        }
        protected virtual void ProcessEventsForEnable(bool toRegister) { }
        protected virtual void OnDestroy()
        {
            if(!GameMain.hasExited)
            {
                for (int i = 0, length = this.redPoints.Count; i < length; ++i) {
                    this.redPoints[i].Listen(false);
                }
            }
        }
    }
}
