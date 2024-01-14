using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
namespace Logic
{
    public class UI_Pet_FashionApparel_Layout
    {
        private Button closeBtn;
        public Button downBtn;
        public Button equipBtn;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        public void Init(Transform transform)
        {
            infinityGrid = transform.Find("Animator/Scroll View").GetComponent<InfinityGrid>();
            closeBtn = transform.Find("Blank").GetComponent<Button>();
            equipBtn = transform.Find("Animator/Btn_Apparel").GetComponent<Button>();
            downBtn = transform.Find("Animator/Btn_Cancel").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            downBtn.onClick.AddListener(listener.OnDownBtnClicked);
            equipBtn.onClick.AddListener(listener.OnEquipBtnClicked);
            infinityGrid.onCreateCell += listener.OnCreateCell;
            infinityGrid.onCellChange += listener.OnCellChange;
        }

        public void SetInfinityGridCell(int count)
        {
            infinityGrid.CellCount = count;
            infinityGrid.ForceRefreshActiveCell();
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void OnDownBtnClicked();
            void OnEquipBtnClicked();
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public class UI_Pet_FashionApparelCeil
    {
        public Transform transform;
        public Image petIconImage;
        public Text petNameText;
        public GameObject unEquipGo;
        public GameObject equipGo;
        public GameObject otherEquipGo;
        public GameObject selectGO;
        public Button ceilBtn;
        public Button petBtn;
        public Action<UI_Pet_FashionApparelCeil> action;
        public int index;
        public uint petUid;
        public void Init(Transform transform)
        {
            this.transform = transform;
            petIconImage = transform.Find("PetItem01/Pet01/Image_Icon").GetComponent<Image>();
            petNameText = transform.Find("Text_Name").GetComponent<Text>();
            equipGo = transform.Find("Image_chuandai").gameObject;
            unEquipGo = transform.Find("Image_weichuandai").gameObject;
            otherEquipGo = transform.Find("Image_qita").gameObject;
            selectGO = transform.Find("Image_Select").gameObject;
            ceilBtn = transform.GetComponent<Button>();
            ceilBtn.onClick.AddListener(OnCeilBtnClicked);
            petBtn = transform.Find("PetItem01/ItemBg").GetComponent<Button>();
            petBtn.onClick.AddListener(OnPetBtnClicked);
        }

        public void AddAction(Action<UI_Pet_FashionApparelCeil> action)
        {
            this.action = action;
        }

        private void OnCeilBtnClicked()
        {
            action?.Invoke(this);
        }

        private void OnPetBtnClicked()
        {
            var clientPet = Sys_Pet.Instance.GetPetByUId(petUid);
            if(null != clientPet)
            {
                Sys_Pet.Instance.ShowPetTip(clientPet, 0);
            }
        }

        /// <summary>
        /// 设置详细信息
        /// </summary>
        /// <param name="petUid"></param>
        /// <param name="index"></param>
        /// <param name="select"></param>
        /// <param name="currentAppearanceId"> 使用外观表 * 10 + 染色index </param>
        public void SetCeilInfo(uint petUid, int index, bool select, uint currentAppearanceTempId)
        {
            this.petUid = petUid;
            this.index = index;
            ResetCeilInfo(currentAppearanceTempId);
            SetSelectState(select);
        }

        public void ResetCeilInfo(uint currentAppearanceTempId)
        {
            var pet = Sys_Pet.Instance.GetPetByUId(petUid);
            if (null != pet)
            {
                ImageHelper.SetIcon(petIconImage, pet.petData.icon_id);
                TextHelper.SetText(petNameText, pet.GetPetNmae());
                var equipId = pet.GetAppearanId();
                bool hasEquip = equipId != 0;
                unEquipGo.SetActive(!hasEquip);
                equipGo.SetActive(hasEquip && currentAppearanceTempId == equipId);
                otherEquipGo.SetActive(hasEquip && currentAppearanceTempId != equipId);
            }
        }

        public void SetSelectState(bool select)
        {
            selectGO.SetActive(select);
        }
    }


    public class UI_Pet_FashionApparel : UIBase, UI_Pet_FashionApparel_Layout.IListener
    {
        public class UI_Pet_FashionApparelEntry
        {
            /// <summary> 使用外观表 * 10 + 染色index </summary>
            public uint AppearanceTempId { get; set; }

            public uint PetId
            {
                get
                {
                    return AppearanceTempId / 100;
                }
                private set { }
            }

            public uint PetAppearanceId
            {
                get
                {
                    return AppearanceTempId / 10;
                }
                private set { }
            }

            public int PetAppearanceColorIndex
            {
                get
                {
                    return (int)AppearanceTempId % ((int)PetAppearanceId * 10);
                }
                private set { }
            }


            public List<uint> petList;

            public int petsCount;

            public int selectIndex = -1;

            public List<UI_Pet_FashionApparelCeil> ceils = new List<UI_Pet_FashionApparelCeil>();

            public bool IndexIsValid()
            {
                return selectIndex >= 0 && selectIndex < petList.Count;
            }

            public uint CurrentPetUid
            {
                get
                {
                    if (IndexIsValid())
                        return petList[selectIndex];
                    return 0;
                }
            }
        }
        private UI_Pet_FashionApparel_Layout layout = new UI_Pet_FashionApparel_Layout();
        private UI_Pet_FashionApparelEntry fashionApparalEntry = new UI_Pet_FashionApparelEntry();
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnPetEquipOrDownPetAppearance, OnPetEquipOrDownPetAppearance, toRegister);
        }

        private void OnPetEquipOrDownPetAppearance()
        {
            for (int i = 0; i < fashionApparalEntry.ceils.Count; i++)
            {
                var _ceil = fashionApparalEntry.ceils[i];
                _ceil.ResetCeilInfo(fashionApparalEntry.AppearanceTempId);
            }
        }

        protected override void OnOpen(object arg = null)
        {
            if(null != arg)
            {
                fashionApparalEntry.AppearanceTempId = Convert.ToUInt32(arg);
            }
        }

        protected override void OnShow()
        {
            if (fashionApparalEntry.AppearanceTempId != 0)
            {
                fashionApparalEntry.petList = Sys_Pet.Instance.GetSamePetsByPetId(fashionApparalEntry.PetId);
                fashionApparalEntry.petsCount = fashionApparalEntry.petList.Count;
                layout.SetInfinityGridCell(fashionApparalEntry.petsCount);
                if(fashionApparalEntry.selectIndex == -1)
                {
                    layout.downBtn.gameObject.SetActive(false);
                    layout.equipBtn.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            UI_Pet_FashionApparelCeil entry = new UI_Pet_FashionApparelCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            entry.AddAction(OnSelect);
            cell.BindUserData(entry);
            fashionApparalEntry.ceils.Add(entry);
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= fashionApparalEntry.petsCount)
                return;
            UI_Pet_FashionApparelCeil entry = cell.mUserData as UI_Pet_FashionApparelCeil;
            entry.SetCeilInfo(fashionApparalEntry.petList[index], index, index == fashionApparalEntry.selectIndex, fashionApparalEntry.AppearanceTempId);
        }

        public void OnSelect(UI_Pet_FashionApparelCeil ceil)
        {
            fashionApparalEntry.selectIndex = ceil.index;
            var pet = Sys_Pet.Instance.GetPetByUId(ceil.petUid);
            if (null != pet)
            {
                var equipId = pet.GetAppearanId();
                bool hasEquip = equipId != 0;
                layout.downBtn.gameObject.SetActive(hasEquip && fashionApparalEntry.AppearanceTempId == equipId);
                layout.equipBtn.gameObject.SetActive(!hasEquip || (hasEquip && fashionApparalEntry.AppearanceTempId != equipId));
            }
            for (int i = 0; i < fashionApparalEntry.ceils.Count; i++)
            {
                var _ceil = fashionApparalEntry.ceils[i];
                _ceil.SetSelectState(fashionApparalEntry.selectIndex == _ceil.index);
            }
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_FashionApparel);
        }

        public void OnDownBtnClicked()
        {
            if (fashionApparalEntry.IndexIsValid())
            {
                Sys_Pet.Instance.PetDressOnOffFashionReq(2, fashionApparalEntry.PetAppearanceId, fashionApparalEntry.CurrentPetUid, (uint)fashionApparalEntry.PetAppearanceColorIndex);
            }
        }

        public void OnEquipBtnClicked()
        {
            if (fashionApparalEntry.IndexIsValid())
            {
                Sys_Pet.Instance.PetDressOnOffFashionReq(1, fashionApparalEntry.PetAppearanceId, fashionApparalEntry.CurrentPetUid, (uint)fashionApparalEntry.PetAppearanceColorIndex);
            }
        }
    }
}