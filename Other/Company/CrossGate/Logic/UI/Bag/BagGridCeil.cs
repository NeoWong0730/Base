using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using System;
using UnityEngine.EventSystems;
using Packet;

namespace Logic
{
    /// <summary>
    /// 适用于可点击格子类，预制限道具 ，装备类
    /// </summary>
    public class CeilGrid
    {
        public enum EGridState
        {
            Normal = 0,     //格子有数据  
            Empty = 1,      //格子无数据，但已经解锁
            Unlock = 2,     //格子未解锁
        }

        public enum ESource
        {
            e_Bag,
            e_TemplateBag,
            e_SafeBox,
            e_BattleUse,
            e_Adventure,
            e_InputChat,
            e_LevelGift,
            e_UserPartition
        }

        #region Layout
        private Transform transform;
        private Image mBg;
        private Image mIcon;
        private Text mCount;
        private GameObject mSelectObj;
        private GameObject mLuckObj;
        private GameObject mNewObj;
        private GameObject mBoundObj;
        private GameObject mclock;
        private Button mIconClickButton;
        private GameObject mImageForbid;
        private GameObject mArrow;

        public Image imgSkillBook;
        #endregion

        #region Data
        public int gridIndex { get; private set; }
        public ItemData mItemData { get; private set; }
        public EGridState eGridState { get; private set; }

        public ESource eSource { get; private set; }

        public bool Selected { get; private set; }

        public int boxId;

        public uint Id
        {
            get
            {
                if (mItemData != null)
                {
                    return mItemData.Id;
                }
                return 0;
            }
        }
        #endregion

        #region EventProcess
        private Action<CeilGrid> onClick;
        private Action<CeilGrid> onLongPressed;
        #endregion

        /// <summary>
        /// 设置当前要控制的GameObject
        /// </summary>
        /// <param name="go"></param>
        public void BindGameObject(GameObject go)
        {
            transform = go.transform;

            mBg = transform.Find("Btn_Item/Image_BG").GetComponent<Image>();
            mIcon = transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();
            mCount = transform.Find("Text_Number").GetComponent<Text>();
            mSelectObj = transform.Find("Image_Select").gameObject;
            mLuckObj = transform.Find("Image_Lock").gameObject;
            mNewObj = transform.Find("Text_New").gameObject;
            mBoundObj = transform.Find("Text_Bound").gameObject;
            mclock = transform.Find("Image_Clock").gameObject;
            mImageForbid = transform.Find("Image_Forbid").gameObject;
            mIconClickButton = transform.Find("Btn_Item").GetComponent<Button>();
            mArrow = transform.Find("Image_UP").gameObject;
            mIconClickButton.onClick.AddListener(OnClicked);


            imgSkillBook = transform.Find("Btn_Item/Image_Skill").GetComponent<Image>();
            //textSkillBook = transform.Find("Btn_Item/Image_Skill/Text_Num").GetComponent<Text>();
            //Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(mBg.gameObject);
            //eventListener.AddEventListener(EventTriggerType.PointerClick, OnClicked);
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="itemData">物品数据</param>
        /// <param name="gridIndex">格子位置</param>
        /// <param name="state">格子状态</param>
        public void SetData(ItemData itemData, int gridIndex, EGridState state, ESource eSource = ESource.e_Bag)
        {
            eGridState = state;
            this.gridIndex = gridIndex;
            mItemData = itemData;
            this.eSource = eSource;
            imgSkillBook.gameObject.SetActive(false);
            RefreshIcon(eSource);
        }

        private void RefreshIcon(ESource eSource = ESource.e_Bag)
        {
            if (eGridState == EGridState.Normal)
            {
                mIcon.gameObject.SetActive(true);
                mLuckObj.SetActive(false);
                ImageHelper.SetIcon(mIcon, mItemData.cSVItemData.icon_id);
                ImageHelper.GetQualityColor_Frame(mBg, (int)mItemData.Quality);
                mNewObj.SetActive(mItemData.bNew);
                mBoundObj.SetActive(mItemData.bBind);
                mCount.gameObject.SetActive(true);
                if (mItemData.Count > 1)
                {
                    mCount.text = mItemData.Count.ToString();
                }
                else
                {
                    mCount.text = string.Empty;
                }
                mclock.SetActive(!mItemData.bMarketEnd);
                //transform.gameObject.name = mItemData.Id.ToString();

                if (eSource == ESource.e_SafeBox)
                {
                    mImageForbid.SetActive(mItemData.cSVItemData.bank_use == 0);
                    mArrow.SetActive(false);
                }
                else if (eSource == ESource.e_Bag)
                {
                    bool canUse = false;
                    if (mItemData.cSVItemData.scene_use == 0)
                    {
                        canUse = true;
                    }
                    else if (Sys_Role.Instance.Role.Level >= mItemData.cSVItemData.use_lv)
                    {
                        if (mItemData.cSVItemData.FunctionOpenId == 0)
                        {
                            canUse = true;
                        }
                        else if (Sys_FunctionOpen.Instance.IsOpen(mItemData.cSVItemData.FunctionOpenId))
                        {
                            canUse = true;
                        }
                    }
                    CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(mItemData.Id);
                    if (equipInfo != null)
                    {
                        if (equipInfo.career_condition != null && !equipInfo.career_condition.Contains(Sys_Role.Instance.Role.Career))
                        {
                            canUse = false;
                        }
                    }

                    int itemMapUse = Sys_Bag.Instance.GetItemMapUseState(mItemData);
                    canUse &= itemMapUse != 0;

                    mImageForbid.SetActive(!canUse);
                    mArrow.SetActive(Sys_Equip.Instance.IsShowArrow(mItemData));
                }
                else if (eSource == ESource.e_BattleUse)
                {
                    mNewObj.SetActive(false);
                    mImageForbid.SetActive(false);
                    mArrow.SetActive(false);
                }
                else
                {
                    mImageForbid.SetActive(false);
                    mArrow.SetActive(false);
                }

                //技能书等级特殊显示
                Sys_Skill.Instance.ShowPetSkillBook(imgSkillBook, mItemData.cSVItemData);
            }
            else if (eGridState == EGridState.Empty)
            {
                ImageHelper.GetQualityColor_Frame(mBg, 6);
                mIcon.gameObject.SetActive(false);
                mLuckObj.SetActive(false);
                mNewObj.SetActive(false);
                mBoundObj.SetActive(false);
                mCount.gameObject.SetActive(false);
                mCount.text = string.Empty;
                mclock.SetActive(false);
                mImageForbid.SetActive(false);
                mArrow.SetActive(false);
            }
            else if (eGridState == EGridState.Unlock)
            {
                ImageHelper.GetQualityColor_Frame(mBg, 6);
                mIcon.gameObject.SetActive(false);
                mLuckObj.SetActive(true);
                mNewObj.SetActive(false);
                mBoundObj.SetActive(false);
                mCount.gameObject.SetActive(false);
                mCount.text = string.Empty;
                mclock.SetActive(false);
                mImageForbid.SetActive(false);
                mArrow.SetActive(false);
            }
        }

        /// <summary>
        /// 设置消息监听
        /// </summary>
        /// <param name="type">消息处理方式</param>
        /// <param name="onclicked">自定义监听</param>
        public void AddClickListener(Action<CeilGrid> onclicked = null, Action<CeilGrid> onlongPressed = null)
        {
            onClick = onclicked;
            if (onlongPressed != null)
            {
                onLongPressed = onlongPressed;
                UI_LongPressButton uI_LongPressButton = mIconClickButton.gameObject.GetNeedComponent<UI_LongPressButton>();
                uI_LongPressButton.onStartPress.AddListener(OnLongPressed);
            }
        }

        public void SetBoxId(int boxid)
        {
            boxId = boxid;
        }

        public void Select()
        {
            mSelectObj.SetActive(true);
            Selected = true;
        }
        public void Release()
        {
            mSelectObj.SetActive(false);
            Selected = false;
        }

        public void SetActive(bool active)
        {
            transform.gameObject.SetActive(active);
        }

        private void OnClicked()
        {
            if (Sys_Bag.Instance.useItemReq)
            {
                return;
            }
            onClick?.Invoke(this);
        }
        private void OnLongPressed()
        {
            onLongPressed.Invoke(this);
        }
    }
}
