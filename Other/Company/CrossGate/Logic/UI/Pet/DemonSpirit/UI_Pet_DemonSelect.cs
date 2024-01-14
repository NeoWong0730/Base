using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using Packet;

namespace Logic
{
    public class UI_Pet_DemonSelect_Layout
    {
        private Button closeBtn;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        private Button confirmBtn;
        public void Init(Transform transform)
        {
            infinityGrid = transform.Find("Animator/Scroll View").GetComponent<InfinityGrid>();
            closeBtn = transform.Find("Blank").GetComponent<Button>();
            confirmBtn =transform.Find("Animator/Btn_Confirm").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            confirmBtn.onClick.AddListener(listener.ConfirmBtnClicked);
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
            void ConfirmBtnClicked();
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public class UI_Pet_DemonSelect : UIBase, UI_Pet_DemonSelect_Layout.IListener
    {
        private UI_Pet_DemonSelect_Layout layout = new UI_Pet_DemonSelect_Layout();
        private uint selectType = 1;
        private int selectIndex = -1;
        private uint currentPetUid;
        private List<PetSoulBeadInfo> sphereTemp = new List<PetSoulBeadInfo>();
        private List<UI_Pet_SelectDemonSpiritSphere> petSelectDemonSpiritSpheres = new List<UI_Pet_SelectDemonSpiritSphere>(8);
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnActiveDemonSpiritSphere, OnActiveDemonSpiritSphere, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnRefreshDemonSpiritSkill, OnRefreshDemonSpiritSkill, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnSaveOrDelectDemonSpiritSphereSkill, OnRefreshDemonSpiritSkill, toRegister);
        }

        protected override void OnOpen(object arg = null)
        {
            if(arg is Tuple<uint,uint>)
            {
                Tuple<uint,uint> tuple = arg as Tuple<uint,uint>;
                selectType = tuple.Item1;
                currentPetUid = tuple.Item2;
            }
            else
            {
                selectType = 1;
                currentPetUid = 0;
            }
        }

        protected override void OnShow()
        {
            sphereTemp = Sys_Pet.Instance.GetMySpheres((uint)selectType, false);
            layout.SetInfinityGridCell(sphereTemp.Count);
        }

        private void OnActiveDemonSpiritSphere()
        {
            sphereTemp = Sys_Pet.Instance.GetMySpheres((uint)selectType, false);
            layout.SetInfinityGridCell(sphereTemp.Count);
        }

        private void OnRefreshDemonSpiritSkill()
        {
            sphereTemp = Sys_Pet.Instance.GetMySpheres((uint)selectType, false);
            layout.SetInfinityGridCell(sphereTemp.Count);
        }

        protected override void OnHide()
        {
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            UI_Pet_SelectDemonSpiritSphere entry = new UI_Pet_SelectDemonSpiritSphere();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            entry.Register(OnSphereBeClicked);
            petSelectDemonSpiritSpheres.Add(entry);
            cell.BindUserData(entry);
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= sphereTemp.Count)
                return;
            UI_Pet_SelectDemonSpiritSphere entry = cell.mUserData as UI_Pet_SelectDemonSpiritSphere;
            entry.SetView(sphereTemp[index]);
            entry.SetSelectState(index, selectIndex);
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_DemonSelect);
        }

        private void OnSphereBeClicked(int index)
        {
            selectIndex = index;
            for (int i = 0; i < petSelectDemonSpiritSpheres.Count; i++)
            {
                petSelectDemonSpiritSpheres[i].SetSelect(selectIndex);
            }
        }

        public void ConfirmBtnClicked()
        {
            if (selectIndex < 0 || selectIndex >= sphereTemp.Count)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002048));
                return;
            }
            Sys_Pet.Instance.PetSoulAssembleBeadReq(currentPetUid, sphereTemp[selectIndex].Index, selectType, 1);
            UIManager.CloseUI(EUIID.UI_Pet_DemonSelect);
        }
    }

    public class UI_Pet_SelectDemonSpiritSphere: UI_Pet_DemonSpiritSphere
    {
        private GameObject selectGo;
        private GameObject backGo;
        private int currentIndex;
        private Action<int> action;
        private bool isActive;
        public override void Init(Transform transform)
        {
            base.Init(transform);
            selectGo = transform.Find("Image_Select").gameObject;
            backGo =transform.Find("Text_Name/Image").gameObject;
        }

        protected override void BingBackBtn(Transform transform)
        {
            backBtn = transform.GetComponent<Button>();
        }

        public void Register(Action<int> action)
        {
            this.action = action;
        }

        public override void SetView(PetSoulBeadInfo sphereTemp)
        {
            base.SetView(sphereTemp);
            if (null != sphereTemp)
            {
                isActive = sphereTemp.Level > 0;
                backGo.SetActive(isActive);
            }
        }


        public void SetSelectState(int index, int selectIndex)
        {
            currentIndex = index;
            SetSelect(selectIndex);
        }

        public void SetSelect(int selectIndex)
        {
            selectGo.SetActive(currentIndex == selectIndex);
        }

        protected override void OnBackBtnClicked()
        {
            if(isActive)
            {
                action?.Invoke(currentIndex);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002054));
            }
        }

    }
}
    