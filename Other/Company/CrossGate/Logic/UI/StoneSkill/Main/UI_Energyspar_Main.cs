using System.Collections.Generic;
using UnityEngine.UI;
using Logic.Core;
using System;
using UnityEngine;
using Table;

namespace Logic
{

    public class UI_Energyspar_Main : UIComponent, UI_Energyspar_Main_Left.IListener
    {
        public UI_Energyspar_Main_Left leftView;
        public UI_Energyspar_Main_Right rightView;
        private uint selectId;        
        protected override void Loaded()
        {
            leftView = AddComponent<UI_Energyspar_Main_Left>(transform.Find("View_Left"));
            leftView.Register(this);
            rightView = AddComponent<UI_Energyspar_Main_Right>(transform.Find("View_Right"));
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForEnable(toRegister);
            Sys_StoneSkill.Instance.eventEmitter.Handle<uint>(Sys_StoneSkill.EEvents.OnSkillActive, RefreshVIew, toRegister);
            Sys_StoneSkill.Instance.eventEmitter.Handle<uint>(Sys_StoneSkill.EEvents.OnNonaSkillUpGa, UpgLevel, toRegister);
            Sys_StoneSkill.Instance.eventEmitter.Handle<uint>(Sys_StoneSkill.EEvents.AdvancedEnd, AdvancedEnd, toRegister);
            Sys_StoneSkill.Instance.eventEmitter.Handle<uint>(Sys_StoneSkill.EEvents.AdvancedResultClose, AdvancedResultClose, toRegister);
            
            Sys_StoneSkill.Instance.eventEmitter.Handle(Sys_StoneSkill.EEvents.AdvancedEndUpWait, AdvancedEndUpWait, toRegister); 
            Sys_StoneSkill.Instance.eventEmitter.Handle<uint>(Sys_StoneSkill.EEvents.ResolveStone, ResolveStone, toRegister);
            Sys_StoneSkill.Instance.eventEmitter.Handle<uint>(Sys_StoneSkill.EEvents.ResolveStoneCerrer, ResolveStoneCerrer, toRegister);
            
            Sys_StoneSkill.Instance.eventEmitter.Handle<uint, uint>(Sys_StoneSkill.EEvents.ReverseEnd, ReverseEnd, toRegister);
            Sys_StoneSkill.Instance.eventEmitter.Handle(Sys_StoneSkill.EEvents.FlyStarEnd, FlyStarEnd, toRegister);
            Sys_StoneSkill.Instance.eventEmitter.Handle<uint>(Sys_StoneSkill.EEvents.ChaoSkillChange, ChaoSkillChange, toRegister);
        }


        public override void Show()
        {
            base.Show();
            leftView.Show();
        }

        public override void Hide()
        {
            base.Hide();
            rightView.Hide();
        }

        private void RefreshVIew(uint stoneId)
        {
            leftView.SetSelect(stoneId);
            leftView.ActiveStone(stoneId);
            rightView.ResetEx(stoneId);
        }

        private void UpgLevel(uint stoneId)
        {
            leftView.SetSelect(stoneId);
            leftView.ResetEx(stoneId);
            rightView.ResetEx(stoneId);
        }

        private void AdvancedEnd(uint stoneId)
        {
        }

        private void AdvancedResultClose(uint stoneId)
        {
            leftView.SetSelect(stoneId);
            leftView.ResetEx(stoneId);
            leftView.AdvanceStone(stoneId, leftView.ceilGo.transform);
            rightView.StoneAdvanceEnd(stoneId);
        }        

        private void AdvancedEndUpWait()
        {
            leftView.WaiteAnimator();
        }

        private void FlyStarEnd()
        {
            leftView.ShowWaitAnimator();
        }

        private void ResolveStone(uint stoneId)
        {
            leftView.SetSelect(stoneId);
            leftView.ResetEx(stoneId);
            rightView.ResetEx(stoneId);
        }
        private void ResolveStoneCerrer(uint stoneId)
        {
            leftView.ResetEx(stoneId);
        }        

        private void ReverseEnd(uint stoneId, uint stage)
        {
            leftView.SetSelect(stoneId);
            rightView.ReverseEnd(stoneId, stage);
        }        

        private void ChaoSkillChange(uint stoneId)
        {
            leftView.SetSelect(stoneId);
            rightView.ChaoSkillChange(stoneId);
        }

        public void OnSelect(uint id)
        {
            leftView.SetSelect(id);
            //点击晶石技能反馈给右边界面
            rightView.SetData(id);
        }
    }
}
