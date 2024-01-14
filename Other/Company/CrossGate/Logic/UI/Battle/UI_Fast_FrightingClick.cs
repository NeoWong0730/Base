using Framework;
using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Command_Item
    {
        private Button btn;

        private Text name;
        private Sys_Team.CommandItem itemData;

        public void BingTrans(Transform transform, Sys_Team.CommandItem data)
        {
            btn = transform.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnBtnClick);
            name = transform.Find("Text").GetComponent<Text>();
            itemData = data;

            name.text = data.Name;
        }

        private void OnBtnClick()
        {
            if (itemData.Type == 999)
            {
                UIManager.OpenUI(EUIID.UI_FrightingClick, false);
            }
            else
            {
                GameCenter.fightControl.isCommanding = true;
                GameCenter.fightControl.CommandingState = EFightCommandingState.Ready;
                Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnCommandChoose, true);

                Net_Combat.Instance.eventEmitter.Trigger<int, int>(Net_Combat.EEvents.OnFastCommandChoose, itemData.CommandIndex, itemData.Type);
            }
        }
    }

    public class UI_Fast_FrightingClick : UIComponent
    {
        private Button openBtn;
        private Button closeBtn;
        private GameObject view;
        private GameObject arrowGo;
        private Transform itemParant;

        private bool isWaitClickHero = false;
        private int CommandIndex ;
        private bool isClearCommand = false;
        private int ShowActorType = 0;
        private int mState = 1; // 0 我方，1 敌方
        private List<uint> mShowTemp = new List<uint>(0) { 2 };
        private List<Sys_Team.CommandItem> commandsList = new List<Sys_Team.CommandItem>();
        private ClickItemEvent ClickEvent = new ClickItemEvent();
        private List<UI_Command_Item> itemList = new List<UI_Command_Item>();


        protected override void Loaded()
        {
            base.Loaded();
            view = transform.Find("List").gameObject;
            arrowGo = transform.Find("Button_Goods/Image").gameObject;
            itemParant = transform.Find("List/Layout").transform;
            openBtn = transform.Find("Button_Goods").GetComponent<Button>();
            openBtn.onClick.AddListener(OnOpenBtnClick);
            closeBtn = transform.Find("List/GameObject").GetComponent<Button>();
            closeBtn.onClick.AddListener(OnCloseBtnClick);
        }

        public override void Show()
        {
            base.Show();
            SetData();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForEnable(toRegister);
            Net_Combat.Instance.eventEmitter.Handle<int,int>(Net_Combat.EEvents.OnFastCommandChoose, OnFastCommandChoose, toRegister);
            Sys_Interactive.Instance.eventEmitter.Handle<InteractiveEvtData>(EInteractiveType.Click, OnClickFrightHero, toRegister);
        }

        public override void Hide()
        {
            base.Hide();
            isWaitClickHero = false;
            CommandIndex = 0;
            isClearCommand = false;
            ShowActorType = 0;
            mState = 1; // 0 我方，1 敌方
            commandsList.Clear();
            FrameworkTool.DestroyChildren(itemParant.gameObject, itemParant.GetChild(0).name);
        }

        public void SetData()
        {
            view.SetActive(false);
            arrowGo.transform.localEulerAngles = new Vector3(0, 0, 0);
            commandsList.Clear();
            for (int i = 0; i < Sys_Team.Instance.FastCommand.Count; ++i)
            {
                if (Sys_Team.Instance.FastCommand[i].Index >= 0)
                {
                    commandsList.Add(Sys_Team.Instance.FastCommand[i]);
                }
            }
            Sys_Team.CommandItem moreItem = new Sys_Team.CommandItem();
            moreItem.Name = LanguageHelper.GetTextContent(2002105);
            moreItem.Type = 999;
            commandsList.Add(moreItem);
            int count = commandsList.Count;
            FrameworkTool.CreateChildList(itemParant, count);
            itemList.Clear();
            for (int i = 0; i < count; ++i)
            {
                UI_Command_Item item = new UI_Command_Item();
                item.BingTrans(itemParant.GetChild(i), commandsList[i]);
                if (commandsList[i].Type == 999)
                {
                    itemParant.GetChild(i).name = commandsList[i].Type.ToString();
                }
                itemList.Add(item);
            }
        }

        public void CancelCommand()
        {
            if (isWaitClickHero)
                Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnCombatOperateOver);

            isWaitClickHero = false;
            mState = 1;
            CommandIndex = 0;

            if (GameCenter.fightControl != null)
                GameCenter.fightControl.CheckShowSelect(false, ShowActorType, mShowTemp, true, true);

            Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnCommandChoose, false);

            if (GameCenter.fightControl != null)
                GameCenter.fightControl.CommandingState = EFightCommandingState.None;

            if (isWaitClickHero)
                Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnCombatOperateOver);
            view.SetActive(false);
        }

        private void OnFastCommandChoose(int index,int type)
        {
            mState = type;
            CommandIndex = index;
            isWaitClickHero = true;
            ShowActorType = mState == 1 ? 6 : 5;
            GameCenter.fightControl.CheckShowSelect(true, ShowActorType, mShowTemp, true,true);
            Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnCommandOperateStart);
        }

        private void OnClickFrightHero(InteractiveEvtData data)
        {
            if (isWaitClickHero == false || data.sceneActor == null || GameCenter.fightControl == null)
                return;
            bool isSide = false;
            uint battleid = 0;
            if (data.sceneActor is MonsterPart)
            {
                isSide = GameCenter.fightControl.isOwnSideActor(data.sceneActor as MonsterPart);
                battleid = GameCenter.fightControl.getUnitID(data.sceneActor as MonsterPart);
            }
            else
            {
                isSide = GameCenter.fightControl.isOwnSideActor(data.sceneActor as FightActor);
                battleid = GameCenter.fightControl.getUnitID(data.sceneActor as FightActor);
            }
            if (GameCenter.fightControl.IsSelectActorRight(battleid) == false)
            {

                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1012008));
                return;
            }
            uint uID = GameCenter.mainFightHero.battleUnit.UnitId;
            if ((isSide && mState != 0))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002103));//"标记的是我方英雄，请选择敌方"  );
                return;
            }
            if ((!isSide && mState == 0))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002104));//("标记的是敌方英雄，请选择我方");
                return;
            }
            uint isfrend = isSide ? (uint)1 : 0;
            uint isCap = (uint)(Sys_Team.Instance.isCaptain() ? 0 : 1);
            if (isClearCommand)
            {
                Net_Combat.Instance.SendSetTagClear(battleid);
                isClearCommand = false;
            }
            else
            {
                Net_Combat.Instance.SendSetTag(battleid, uID, (uint)CommandIndex, isCap, isfrend);
                if (GameCenter.fightControl != null && mState == 1 && CommandIndex == 0)
                {
                    foreach (var kvp in GameCenter.fightControl.m_DicFightCommand)
                    {
                        if (kvp.Value.CommandIndex == 0 && kvp.Value.Side == 0)
                        {
                            Net_Combat.Instance.SendSetTagClear(kvp.Value.MarkedID);
                        }
                    }
                }
            }
            isWaitClickHero = false;
            GameCenter.fightControl.CheckShowSelect(false, ShowActorType, mShowTemp, true, true);
            Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnCombatOperateOver);
            CancelCommand();
        }

        private void OnOpenBtnClick()
        {
            view.SetActive(!view.activeSelf);
            if (view.activeSelf)
            {
                arrowGo.transform.localEulerAngles = new Vector3(0, 0, 180);
            }
            else
            {
                arrowGo.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }

        private void OnCloseBtnClick()
        {
            view.SetActive(false);

        }
    }
}
