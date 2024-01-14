 using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_MainBattle_Seal_Layout
    {
        public Transform transform;
        public GameObject itemGo;
        public Button closeBtn;
        public Text title;

        public void Init(Transform transform)
        {
            itemGo = transform.Find("Animator/Scroll_View_Skill/Grid/PropItem").gameObject;
            closeBtn = transform.Find("Button_Close").GetComponent<Button>();
            title = transform.Find("Animator/Text_Title").GetComponent<Text>();
        }
        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OnClose_ButtonClicked);
        }
        public interface IListener
        {
            void OnClose_ButtonClicked();
        }
    }

    public class UI_MainBattle_Seal : UIBase,UI_MainBattle_Seal_Layout.IListener
    {
        private UI_MainBattle_Seal_Layout layout = new UI_MainBattle_Seal_Layout();
        private PropItem propItem;
        private bool isBoom;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void OnShow()
        {
            CSVNienParameters.Data csvNienParametersData = CSVNienParameters.Instance.GetConfData(9);
            uint.TryParse(csvNienParametersData.str_value, out uint battleTypeId);
            if(Sys_Fight.Instance.BattleTypeId== battleTypeId)  //年兽战斗
            {
                isBoom = true;
                TextHelper.SetText(layout.title, 2016117);
                AddBoomItem();
            }
            else
            {
                isBoom = false;
                TextHelper.SetText(layout.title, 2012091);
                AddSealItemList(); 
            }
        }

        protected override void OnHide()
        {
            DefaultItem();
        }

        private  void AddSealItemList()
        {
            List<ItemData> listTemp = Sys_Bag.Instance.GetItemDatasByItemType(3018);
            for (int i=0;i< listTemp.Count;++i)
            {
                ItemData data = listTemp[i];
                GameObject go = GameObject.Instantiate<GameObject>(layout.itemGo, layout.itemGo.transform.parent);
                go.transform.name = listTemp[i].Id.ToString();
                propItem = new PropItem();
                propItem.OnEnableLongPress(true);
                propItem.BindGameObject(go);
                PropIconLoader.ShowItemData itemData=new PropIconLoader.ShowItemData(data.Id, 1, true, false, false, false, false, _bShowCount: true, _bShowBagCount: true, _bUseClick: true, (propItem) =>
                {
                    OnSelectItem(CSVItem.Instance.GetConfData(data.Id).active_skillid, data.Id);
                });
                itemData.bCopyBagGridCount = true;
                itemData.bagGridCount = (int)data.Count;
                propItem.SetData(new MessageBoxEvt( EUIID.UI_MainBattle_SealItem, itemData));
            }
            layout.itemGo.SetActive(false);
        }

        private void AddBoomItem()
        {
            CSVNienParameters.Data csvNienParametersData = CSVNienParameters.Instance.GetConfData(14);
            uint.TryParse(csvNienParametersData.str_value, out uint itemId);
            long count= Sys_Bag.Instance.GetItemCount(itemId);
            if (count == 0)
            {
                layout.itemGo.SetActive(false);
            }
            else
            {
                layout.itemGo.SetActive(true);
                propItem = new PropItem();
                propItem.OnEnableLongPress(true);
                propItem.BindGameObject(layout.itemGo);
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(itemId, count, true, false, false, false, false, _bShowCount: true, _bShowBagCount: true, _bUseClick: true, (propItem) =>
                {
                    OnSelectItem(CSVItem.Instance.GetConfData(itemId).active_skillid, itemId);
                });
                itemData.bCopyBagGridCount = true;
                itemData.bagGridCount = (int)count;
                propItem.SetData(new MessageBoxEvt(EUIID.UI_MainBattle_SealItem, itemData));
            }
        }

        private void OnSelectItem(uint skillid,uint itemid)
        {
            if (isBoom)
            {
                GameCenter.fightControl.AttackById(skillid, itemid);
                UIManager.CloseUI(EUIID.UI_MainBattle_SealItem);
            }
            else
            {
                uint petId = GameCenter.fightControl.HaveSealedPet();
                if (petId == 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3900016));
                }
                else
                {
                    if (Sys_Pet.Instance.GetPetIsActive(petId))
                    {
                        GameCenter.fightControl.AttackById(skillid, itemid);
                        UIManager.CloseUI(EUIID.UI_MainBattle_SealItem);
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009713));
                    }
                }
            }
        }

        private void DefaultItem()
        {
            layout.itemGo.SetActive(true);
            FrameworkTool.DestroyChildren(layout.itemGo.transform.parent.gameObject,layout.itemGo.transform.name);
        }

        public void OnClose_ButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_MainBattle_SealItem);
        }
    }
}
