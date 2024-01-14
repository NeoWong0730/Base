using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_JewelCompound : UIBase, UI_JewelCompound_Layout.IListener, UI_JewelCompound_Left.IListener, UI_JewelCompound_Right.IListener
    {
        private UI_JewelCompound_Layout layout;

        private Sys_Equip.JewelGroupData jewelGroupData;

        private EJewelType jewelType = EJewelType.All;

        private ItemData _selectItem;

        protected override void OnLoaded()
        {            
            layout = new UI_JewelCompound_Layout();
            layout.Parse(gameObject);
            layout.RegisterEvents(this);
            layout.compoundLeft.ReisterListener(this);
            layout.compoundRight.ReisterListener(this);
        }

        protected override void OnOpen(object arg)
        {
            _selectItem = null;
            if (arg != null)
                _selectItem = (ItemData)arg;
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Equip.Instance.eventEmitter.Handle<bool>(Sys_Equip.EEvents.OnJewelCompose, OnJewelComposeNtf, toRegister);
        }

        protected override void OnShow()
        {
            Sys_Equip.Instance.SelectJewelGroupIndex = 0;
            layout.compoundLeft.Show();
            layout.compoundLeft.UpdateType(_selectItem);
        }

        protected override void OnHide()
        {
            layout.compoundLeft.Hide();         
        }

        #region implement_interface
        public void OnClickClose()
        {
            UIManager.CloseUI(EUIID.UI_JewelCompound);
        }

        public void SelectedJewel(Sys_Equip.JewelGroupData groupData)
        {
            jewelGroupData = groupData;
            if (jewelGroupData != null)
            {
                layout.goNone.SetActive(false);
                //layout.compoundRight.Hide();
                layout.compoundRight.Show();
                layout.compoundRight.UpdateComoundInfo(jewelGroupData);
            }
            else
            {
                layout.goNone.SetActive(true);
                layout.compoundRight.Hide();
            }
        }

        public void SwitchJewelType(EJewelType type)
        {
            jewelType = type;

            List<Sys_Equip.JewelGroupData> list = Sys_Equip.Instance.GetJewelTotalList(type);

            //如果有来源宝石，则跳转到来源宝石
            int index = 0;
            if (_selectItem != null)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].itemId == _selectItem.Id)
                    {
                        index = i;
                        break;
                    }
                }
            }

            layout.compoundLeft.UpdateJewelList(list, index);
        }

        public void OnCompoundOnce()
        {
            if (jewelGroupData != null)
            {
                CSVJewel.Data jewelData = CSVJewel.Instance.GetConfData(jewelGroupData.itemId);
                if (jewelData != null)
                {
                    if (jewelGroupData.count >= jewelData.num) //宝石数量够
                    {
                        Sys_Equip.Instance.JewelCompoundReq(jewelGroupData.itemId);
                    }
                    else
                    {
                        //TODO: 后续会提示购买
                        Sys_Hint.Instance.PushContent_Normal("宝石数量不足");
                    }
                }
                else
                {
                    Debug.LogErrorFormat("not found jewel id = {0}", jewelGroupData.itemId);
                }
            }
        }

        public void OnCompoundHot()
        {
            if (jewelGroupData != null)
            {
                CSVJewel.Data jewelData = CSVJewel.Instance.GetConfData(jewelGroupData.itemId);
                if (jewelData != null)
                {
                    if (jewelGroupData.count >= jewelData.num) //宝石数量够
                    {
                        uint times = jewelGroupData.count / jewelData.num;

                        Sys_Equip.Instance.JewelCompoundReq(jewelGroupData.itemId, times);
                    }
                    else
                    {
                        //TODO: 后续会提示购买
                        Sys_Hint.Instance.PushContent_Normal("宝石数量不足");
                    }
                }
                else
                {
                    Debug.LogErrorFormat("not found jewel id = {0}", jewelGroupData.itemId);
                }
            }
        }
        #endregion

        #region Ntf
        private void OnJewelComposeNtf(bool isTen)
        {
            
            layout.SetAnimator(isTen);

            List<Sys_Equip.JewelGroupData> list = Sys_Equip.Instance.GetJewelTotalList(jewelType);
            int selectIndex = 0;

            if (jewelGroupData != null)
            {
                Sys_Equip.JewelGroupData tempGroupData = Sys_Equip.Instance.GetJewelGroupData(jewelGroupData.itemId);
                if (tempGroupData != null)
                {
                    layout.compoundRight.UpdateComoundInfo(tempGroupData);

                    for (int i = 0; i < list.Count; ++i)
                    {
                        if (list[i].itemId == tempGroupData.itemId)
                        {
                            selectIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    //找下一级
                    CSVJewel.Data tempData = CSVJewel.Instance.GetConfData(jewelGroupData.itemId);
                    for (int i = 0; i < list.Count; ++i)
                    {
                        if (list[i].itemId == tempData.next_id)
                        {
                            selectIndex = i;
                            break;
                        }
                    }
                }
            }

            
            layout.compoundLeft.UpdateJewelList(list, selectIndex);
        }
        #endregion
    }
}

