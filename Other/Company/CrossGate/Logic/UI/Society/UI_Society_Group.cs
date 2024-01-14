using Lib.Core;
using Logic.Core;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Logic
{
    public partial class UI_Society : UIBase, UI_Society_Layout.IListener
    {
        uint groupCurrentSelectGroupID;

        CoroutineHandler groupHandler;

        List<UI_Society_Layout.ChatRoleItem> groupChatRoleItems = new List<UI_Society_Layout.ChatRoleItem>();

        void GroupProcessEventsForEnable(bool toRegister)
        {
            Sys_Society.Instance.eventEmitter.Handle(Sys_Society.EEvents.OnGetGroupInfo, OnGetGroupInfo, toRegister);
            Sys_Society.Instance.eventEmitter.Handle(Sys_Society.EEvents.OnCreateGroupSuccess, OnCreateGroupSuccess, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<uint>(Sys_Society.EEvents.OnDismissGroup, OnDismissGroup, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<uint>(Sys_Society.EEvents.OnGetOneGroupDetailInfo, OnGetOneGroupDetailInfo, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<Sys_Society.GroupInfo>(Sys_Society.EEvents.OnChangeGroupNotice, OnChangeGroupNotice, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<uint>(Sys_Society.EEvents.OnClearGroupChatLog, OnClearGroupChatLog, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<uint>(Sys_Society.EEvents.OnSelfQuitGroupSuccess, OnOnSelfQuitGroupSuccess, toRegister);
        }

        public void OnClickGroupToggle()
        {          

            if (!Sys_FunctionOpen.Instance.IsOpen(30102, true))
                    return;

            layout.recentToggle_Light.SetActive(false);
            layout.recentToggle_Dark.SetActive(true);

            layout.groupToggle_Light.SetActive(true);
            layout.groupToggle_Dark.SetActive(false);

            layout.contactsToggle_Light.SetActive(false);
            layout.contactsToggle_Dark.SetActive(true);

            layout.mailToggle_Light.SetActive(false);
            layout.mailToggle_Dark.SetActive(true);

            ReqGroupBriefInfo();

            layout.recentRoot.SetActive(false);
            layout.groupRoot.SetActive(true);
            layout.contactsRoot.SetActive(false);
            layout.mailRoot.SetActive(false);
            layout.buttonRoot.SetActive(false);           

            layout.recentRightRoot.SetActive(false);
            layout.contactsRightRoot.SetActive(false);
            layout.groupRightRoot.SetActive(false);
            layout.mailRightRoot.SetActive(false);
            layout.rightMOLIRoot.SetActive(false);
            layout.rightNoneRoot.SetActive(true);
            TextHelper.SetText(layout.rightNoneText, LanguageHelper.GetTextContent(2004005));
            layout.inputRoot.SetActive(false);
        }

        GameObject createGroupItemGo;
        void ReqGroupBriefInfo()
        {
            createGroupItemGo = GameObject.Instantiate(layout.group_createGroupPrefab);
            createGroupItemGo.SetActive(false);
            UI_Society_Layout.Group_CreateGroupItem createGroupItem = new UI_Society_Layout.Group_CreateGroupItem(createGroupItemGo);
            createGroupItemGo.transform.SetParent(layout.groupContentRoot.transform, false);

            Sys_Society.Instance.ReqGetGroupBriefInfo();
        }

        void OnGetGroupInfo()
        {
            groupCurrentSelectGroupID = 0;
            UpdateGroupList();
            UpdateGroupChatList(groupCurrentSelectGroupID);
            createGroupItemGo.SetActive(true);
        }

        void OnCreateGroupSuccess()
        {
            UpdateGroupList();
        }

        void OnDismissGroup(uint groupID)
        {
            UpdateGroupList();

            layout.recentRoot.SetActive(false);
            layout.groupRoot.SetActive(true);
            layout.contactsRoot.SetActive(false);
            layout.mailRoot.SetActive(false);
            layout.buttonRoot.SetActive(false);

            layout.recentRightRoot.SetActive(false);
            layout.contactsRightRoot.SetActive(false);
            layout.groupRightRoot.SetActive(false);
            layout.rightMOLIRoot.SetActive(false);
            layout.rightNoneRoot.SetActive(true);
            TextHelper.SetText(layout.rightNoneText, LanguageHelper.GetTextContent(2004005));
            layout.inputRoot.SetActive(false);
        }

        void OnOnSelfQuitGroupSuccess(uint groupID)
        {
            UpdateGroupList();

            layout.recentRoot.SetActive(false);
            layout.groupRoot.SetActive(true);
            layout.contactsRoot.SetActive(false);
            layout.mailRoot.SetActive(false);
            layout.buttonRoot.SetActive(false);

            layout.recentRightRoot.SetActive(false);
            layout.contactsRightRoot.SetActive(false);
            layout.groupRightRoot.SetActive(false);
            layout.rightMOLIRoot.SetActive(false);
            layout.rightNoneRoot.SetActive(true);
            TextHelper.SetText(layout.rightNoneText, LanguageHelper.GetTextContent(2004005));
            layout.inputRoot.SetActive(false);
        }

        void UpdateGroupNotice()
        {
            string noticeStr = Sys_Society.Instance.socialGroupsInfo.groupsDic[groupCurrentSelectGroupID].notice;
            if (string.IsNullOrWhiteSpace(noticeStr))
            {
                TextHelper.SetText(layout.notice, LanguageHelper.GetTextContent(2004057));
            }
            else
            {
                TextHelper.SetText(layout.notice, noticeStr);
            }
        }

        void UpdateGroupList()
        {
            layout.groupContentRoot.DestoryAllChildren();

            foreach (Sys_Society.GroupInfo group in Sys_Society.Instance.socialGroupsInfo.groupsDic.Values)
            {
                GameObject groupInfoItemGo = GameObject.Instantiate(layout.group_groupInfoPrefab);
                groupInfoItemGo.SetActive(true);
                UI_Society_Layout.Group_GroupInfoItem groupInfoItem = new UI_Society_Layout.Group_GroupInfoItem(groupInfoItemGo);
                groupInfoItem.chooseGroupInfoItem = (groupID) =>
                {                   
                    Sys_Society.Instance.ReqGetGroupDetailInfo(groupID);                    
                };
                groupInfoItem.Update(group);
                groupInfoItemGo.transform.SetParent(layout.groupContentRoot.transform, false);
            }

            GameObject createGroupItemGo = GameObject.Instantiate(layout.group_createGroupPrefab);
            createGroupItemGo.SetActive(true);
            UI_Society_Layout.Group_CreateGroupItem createGroupItem = new UI_Society_Layout.Group_CreateGroupItem(createGroupItemGo);
            createGroupItemGo.transform.SetParent(layout.groupContentRoot.transform, false);
        }

        void OnGetOneGroupDetailInfo(uint groupID)
        {
            groupCurrentSelectGroupID = groupID;

            layout.groupRightRoot.SetActive(true);
            layout.rightMOLIRoot.SetActive(false);
            layout.rightNoneRoot.SetActive(false);
            layout.inputRoot.SetActive(true);

            UpdateGroupNotice();
            UpdateGroupChatList(groupID);
        }

        void OnChangeGroupNotice(Sys_Society.GroupInfo groupInfo)
        {
            if (groupInfo.groupID == groupCurrentSelectGroupID)
            {
                UpdateGroupNotice();
            }
        }

        void OnClearGroupChatLog(uint groupID)
        {
            if (groupCurrentSelectGroupID == groupID)
            {
                UpdateGroupChatList(groupID);
            }
        }

        public void OnClickGroupSettingButton()
        {
            if (Sys_Society.Instance.socialGroupsInfo.groupsDic[groupCurrentSelectGroupID].leader == Sys_Role.Instance.RoleId)
            {
                UIManager.OpenUI(EUIID.UI_Society_GroupMasterSetting, false, Sys_Society.Instance.socialGroupsInfo.groupsDic[groupCurrentSelectGroupID]);
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_Society_GroupMemberSetting, false, Sys_Society.Instance.socialGroupsInfo.groupsDic[groupCurrentSelectGroupID]);
            }
        }

        void UpdateGroupChatList(uint groupID)
        {
            groupChatRoleItems.Clear();
            layout.groupRightContentRoot.DestoryAllChildren();

            if (Sys_Society.Instance.groupChatDataInfo.groupChatDatas.ContainsKey(groupID))
            {
                if (groupHandler != null)
                {
                    CoroutineManager.Instance.Stop(groupHandler);
                    groupHandler = null;
                }
                groupHandler = CoroutineManager.Instance.StartHandler(UpdateGroupChatListCoroutine(groupID));
            }
        }

        IEnumerator UpdateGroupChatListCoroutine(uint groupID)
        {
            foreach (Sys_Society.ChatData groupChatData in Sys_Society.Instance.groupChatDataInfo.groupChatDatas[groupID].chatsInfo)
            {
                if (groupChatData.needShowTimeText)
                {
                    GameObject chatTimeItemGo = GameObject.Instantiate(layout.chatTimePrefab);
                    chatTimeItemGo.SetActive(true);
                    UI_Society_Layout.ChatTimeItem chatTimeItem = new UI_Society_Layout.ChatTimeItem(chatTimeItemGo);
                    chatTimeItem.Update(groupChatData.sendTime);
                    chatTimeItemGo.transform.SetParent(layout.groupRightContentRoot.transform, false);
                }

                if (groupChatData.roleID != Sys_Role.Instance.RoleId)
                {
                    GameObject leftChatItemGo = GameObject.Instantiate(layout.chatLeftPrefab);
                    leftChatItemGo.SetActive(true);
                    UI_Society_Layout.ChatRoleItem chatRoleItem = new UI_Society_Layout.ChatRoleItem(leftChatItemGo);
                    chatRoleItem.Update(groupChatData);
                    leftChatItemGo.transform.SetParent(layout.groupRightContentRoot.transform, false);
                    groupChatRoleItems.Add(chatRoleItem);
                }
                else
                {
                    GameObject rightChatItemGo = GameObject.Instantiate(layout.chatRightPrefab);
                    rightChatItemGo.SetActive(true);
                    UI_Society_Layout.ChatRoleItem chatRoleItem = new UI_Society_Layout.ChatRoleItem(rightChatItemGo);
                    chatRoleItem.Update(groupChatData);
                    rightChatItemGo.transform.SetParent(layout.groupRightContentRoot.transform, false);
                    groupChatRoleItems.Add(chatRoleItem);
                }
            }

            yield return new WaitForEndOfFrame();
            layout.groupRightContentScrollbar.value = 0;

            //GroupUpdateChatRoleItemReadFlag(groupID);
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnReadGroupChat, null);
        }

        void GroupJumpToFirstUnRead(UI_Society_Layout.ChatRoleItem chatRoleItem)
        {
            RectTransform rectTransform = layout.groupRightContentRoot.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -chatRoleItem.root.GetComponent<RectTransform>().anchoredPosition.y - chatRoleItem.root.GetComponent<RectTransform>().sizeDelta.y / 2);
        }
    }
}