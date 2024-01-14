using Packet;
using UnityEngine;
using Logic.Core;
using System.Collections.Generic;
using UnityEngine.UI;
using Table;
using Lib.Core;
using System;
using Framework;

namespace Logic
{
    public class UI_Share_Friend_Layout
    {
        public Transform transform;
        public GameObject itemGo;
        public Button shareBtn;
        public Button closeBtn;
        public Button cancleBtn;
        public InfinityGrid infinityGrid;

        public void Init(Transform transform)
        {
            this.transform = transform;
            itemGo = transform.Find("Animator/Scroll View/Viewport/Content/Item").gameObject;
            closeBtn = transform.Find("Animator/View_TipsBgNew07/Btn_Close").GetComponent<Button>();
            cancleBtn = transform.Find("Animator/Btn_Cancel").GetComponent<Button>();
            shareBtn = transform.Find("Animator/Btn_Confirm").GetComponent<Button>();
            infinityGrid = transform.Find("Animator/Scroll View").GetComponent<InfinityGrid>();
        }
        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OnClose_ButtonClicked);
            cancleBtn.onClick.AddListener(listener.OnClose_ButtonClicked);
            shareBtn.onClick.AddListener(listener.OnShare_ButtonClicked);
        }

        public interface IListener
        {
            void OnClose_ButtonClicked();
            void OnShare_ButtonClicked();
        }
    }

    public class UI_Friend_Item
    {
        public Transform transform;
        private Text name;
        private Text level;
        private Text profession;
        private Image professionImage;
        private Image head;
        public Toggle toggle;
        private GameObject checkGo;
        private Action<UI_Friend_Item> onClick;
        public Sys_Society.RoleInfo roleInfo;


        public void BingGameObject(GameObject gameObject)
        {
            transform = gameObject.transform;
            name = transform.Find("Text_Name").GetComponent<Text>();
            level = transform.Find("Level").GetComponent<Text>();
            profession = transform.Find("Text_Profession").GetComponent<Text>();
            professionImage = transform.Find("Image_Prop").GetComponent<Image>();
            head = transform.Find("Head/Icon").GetComponent<Image>();
            checkGo = transform.Find("Check/Image").gameObject;
            toggle = transform.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(OnClicked);
            toggle.isOn = false;
        }

        public void AddClickListener(Action<UI_Friend_Item> onclicked = null)
        {
            onClick = onclicked;
        }

        private void OnClicked(bool isOn)
        {
            onClick?.Invoke(this);
            checkGo.SetActive(isOn);
        }

        public void SetData(Sys_Society.RoleInfo _roleInfo)
        {
            roleInfo = _roleInfo;
            if (roleInfo == null)
            {
                return;
            }
            name.text = roleInfo.roleName;
            level.text = roleInfo.level.ToString();
           if( CSVCareer.Instance.TryGetValue(roleInfo.occ, out CSVCareer.Data data))
            {
                profession.text = LanguageHelper.GetTextContent(data.name);
                ImageHelper.SetIcon(professionImage, data.logo_icon);
            }
           uint headId=  CharacterHelper.getHeadID(roleInfo.heroID,roleInfo.iconId);
            ImageHelper.SetIcon(head,headId);
            checkGo.SetActive(false);
        }
    }

    public class UI_Share_Friend : UIBase, UI_Share_Friend_Layout.IListener
    {
        private UI_Share_Friend_Layout layout = new UI_Share_Friend_Layout();
        private List<ulong> selectedList = new List<ulong>();
        private int infinityCount;
        private ClientVideo clientVideo;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg)
        {
            if (arg == null)
            {
                return;
            }
            clientVideo = arg as ClientVideo;
        }

        protected override void OnShow()
        {
            layout.infinityGrid.onCreateCell += OnCreateCell;
            layout.infinityGrid.onCellChange += OnCellChange;
            infinityCount = Sys_Society.Instance.socialFriendsInfo.GetAllSortedFriendsInfos().Count;              
            layout.infinityGrid.CellCount = infinityCount;
            layout.infinityGrid.ForceRefreshActiveCell();
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnUpdateCrossSrvState, OnUpdateCrossSrvState, toRegister);
        }

        private void OnUpdateCrossSrvState()
        {
            UIManager.CloseUI(EUIID.UI_Video_Friend);
        }

        protected override void OnHide()
        {
            selectedList.Clear();
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_Friend_Item entry = new UI_Friend_Item();
            GameObject go = cell.mRootTransform.gameObject;
            entry.AddClickListener(OnItemSelect);
            entry.BingGameObject(go);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            Sys_Society.RoleInfo roleInfo = new Sys_Society.RoleInfo();
            roleInfo = Sys_Society.Instance.socialFriendsInfo.GetAllSortedFriendsInfos()[index];
            UI_Friend_Item entry = cell.mUserData as UI_Friend_Item;
            entry.SetData(roleInfo);
        }

        private void OnItemSelect(UI_Friend_Item item)
        {
            if (item.toggle.isOn)
            {
                if (!selectedList.Contains(item.roleInfo.roleID))
                {
                    selectedList.Add(item.roleInfo.roleID);
                }
            }
            else
            {
                if (selectedList.Contains(item.roleInfo.roleID))
                {
                    selectedList.Remove(item.roleInfo.roleID);
                }
            }
        }

        public void OnClose_ButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Video_Friend);
        }

        public void OnShare_ButtonClicked()
        {
            ClientShareVideo clientShareVideo = new ClientShareVideo();
            clientShareVideo.Author = clientVideo.authorBrif.Author;
            clientShareVideo.Channel = ShareChannelType.ShareChannelFriend;
            clientShareVideo.VideoId = clientVideo.video;
            clientShareVideo.VideoType = clientVideo.baseBrif.VideoType;
            clientShareVideo.Title = clientVideo.authorBrif.Title;
            Sys_Video.Instance.ShareChannelReq(Sys_Role.Instance.RoleId, clientShareVideo, selectedList);
            UIManager.CloseUI(EUIID.UI_Video_Friend);
        }
    }
}
