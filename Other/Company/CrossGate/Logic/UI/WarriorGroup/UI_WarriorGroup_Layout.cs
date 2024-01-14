using System.Collections.Generic;
using UnityEngine;
using Lib.Core;
using Logic.Core;
using UnityEngine.UI;
using Framework;

namespace Logic
{
    /// <summary>
    /// 勇者团UI布局///
    /// </summary>
    public partial class UI_WarriorGroup_Layout
    {
        public enum EMemeModelShowPos
        {
            Pos0 = 0,
            Pos1,
            Pos2,
            Pos3,
            Pos4
        }  

        public Transform transform;

        public Button closeButton;

        public CP_Toggle infoToggle;
        public CP_Toggle memberToggle;
        public CP_Toggle meetingToggle;
        public GameObject meetingRedPoint;

        public GameObject infoRoot;
        public GameObject memberRoot;
        public GameObject meetingRoot;

        #region info

        public Text groupNameTxt;
        public Text groupIDTxt;
        public Text groupRoleCountTxt;
        public Text groupLeaderNameTxt;
        public Text groupDeclarationTxt;

        public Button reportButton;
        public Button editDeclarationButton;
        public Button enterChatButton;

        public InfinityGrid actionInfinityGrid;

        #endregion

        #region member

        public Transform inviteFriendRoot;

        public Button leaveButton;
        public Button leaveCancelButton;
        public Button transferButton;
        public Button inviteButton;
        public Button teamButton;
        public Text teamButtonText;
        public Text leaveText;

        public GameObject pageRoot;
        public Button leftButton;
        public Button rightButton;
        public Text pageText;

        public List<ShoweModelMember> members = new List<ShoweModelMember>(5);

        private AssetDependencies infoAssetDependencies;
        public ShowSceneControl showSceneControl;
        public List<WarriorGroupShowModelPositionItem> showPosList = new List<WarriorGroupShowModelPositionItem>(5);
        public List<WarriorGroupModelShow> modelShowList = new List<WarriorGroupModelShow>(5);

        #endregion

        #region meeting

        public Text createMeetTimeText;

        public Toggle currentMeetingToggle;
        public Toggle historyMeetingToggle;
        public GameObject currentMeetingRedPoint;
        public GameObject historyMeetingRedPoint;

        public GameObject currentMeetingRoot;
        public GameObject historyMeetingRoot;

        public InfinityGrid currentMeetingGrid;
        public InfinityGrid historyMeetingGrid;

        #endregion

        public void Init(Transform transform)
        {
            this.transform = transform;

            closeButton = transform.gameObject.FindChildByName("Btn_Close").GetComponent<Button>();

            infoRoot = transform.gameObject.FindChildByName("View_Message");
            memberRoot = transform.gameObject.FindChildByName("View_Member");
            meetingRoot = transform.gameObject.FindChildByName("View_Meeting");

            infoToggle = transform.gameObject.FindChildByName("Toggle_Message").GetComponent<CP_Toggle>();
            memberToggle = transform.gameObject.FindChildByName("Toggle_Member").GetComponent<CP_Toggle>();
            meetingToggle = transform.gameObject.FindChildByName("Toggle_Meeting").GetComponent<CP_Toggle>();
            meetingRedPoint = meetingToggle.gameObject.FindChildByName("Image_Dot");

            groupNameTxt = infoRoot.FindChildByName("Text_Name").GetComponent<Text>();
            groupIDTxt = infoRoot.FindChildByName("Text_Num").GetComponent<Text>();
            groupRoleCountTxt = infoRoot.FindChildByName("Text_Count").GetComponent<Text>();
            groupLeaderNameTxt = infoRoot.FindChildByName("Text_LeaderName").GetComponent<Text>();
            groupDeclarationTxt = infoRoot.FindChildByName("DeclarationText").GetComponent<Text>();
            reportButton = infoRoot.FindChildByName("reportButton").GetComponent<Button>();
            editDeclarationButton = infoRoot.FindChildByName("DeclarationButton").GetComponent<Button>();
            enterChatButton = infoRoot.FindChildByName("Btn_01_Small").GetComponent<Button>();
            actionInfinityGrid = infoRoot.FindChildByName("ScrollView_News").GetComponent<InfinityGrid>();

            leaveButton = memberRoot.FindChildByName("Btn_Leave").GetComponent<Button>();
            leaveCancelButton = memberRoot.FindChildByName("Btn_LeaveCancel").GetComponent<Button>();
            transferButton = memberRoot.FindChildByName("Btn_Transter").GetComponent<Button>();
            inviteButton = memberRoot.FindChildByName("Btn_Invite").GetComponent<Button>();
            teamButton = memberRoot.FindChildByName("Btn_Team").GetComponent<Button>();
            teamButtonText = teamButton.gameObject.FindChildByName("Text_01").GetComponent<Text>();
            leaveText = leaveCancelButton.gameObject.FindChildByName("Text_Time").GetComponent<Text>();

            pageRoot = memberRoot.FindChildByName("Page");
            leftButton = pageRoot.FindChildByName("Image_L").GetComponent<Button>();
            rightButton = pageRoot.FindChildByName("Image_R").GetComponent<Button>();
            pageText = pageRoot.FindChildByName("Text").GetComponent<Text>();

            for (int index = 0, len = 5; index < len; index++)
            {
                members.Add(new ShoweModelMember().Load(memberRoot.FindChildByName($"Item_{index}").transform));
            }

            infoAssetDependencies = memberRoot.GetComponent<AssetDependencies>();

            createMeetTimeText = meetingRoot.FindChildByName("TimeText").GetComponent<Text>();
            currentMeetingToggle = meetingRoot.FindChildByName("Toggle").GetComponent<Toggle>();
            historyMeetingToggle = meetingRoot.FindChildByName("Toggle (1)").GetComponent<Toggle>();
            currentMeetingRedPoint = currentMeetingToggle.gameObject.FindChildByName("Image_Dot");
            historyMeetingRedPoint = historyMeetingToggle.gameObject.FindChildByName("Image_Dot");
            currentMeetingRoot = meetingRoot.FindChildByName("CurrentRoot");
            historyMeetingRoot = meetingRoot.FindChildByName("HistoryRoot");
            currentMeetingGrid = meetingRoot.FindChildByName("Current").GetComponent<InfinityGrid>();
            historyMeetingGrid = meetingRoot.FindChildByName("History").GetComponent<InfinityGrid>();

        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);

            infoToggle.onValueChanged.AddListener(listener.OnInfoToggleValueChanged);
            memberToggle.onValueChanged.AddListener(listener.OnMemberToggleValueChanged);
            meetingToggle.onValueChanged.AddListener(listener.OnMeetingToggleValueChanged);

            reportButton.onClick.AddListener(listener.OnClickReportButton);
            editDeclarationButton.onClick.AddListener(listener.OnClickEditDeclarationButton);
            enterChatButton.onClick.AddListener(listener.OnClickEnterChatButton);

            leaveButton.onClick.AddListener(listener.OnClickLeaveButton);
            leaveCancelButton.onClick.AddListener(listener.OnClickLeaveCancelButton);
            transferButton.onClick.AddListener(listener.OnClickTransferButton);
            inviteButton.onClick.AddListener(listener.OnClickInviteButton);
            teamButton.onClick.AddListener(listener.OnClickTeamButton);
            leftButton.onClick.AddListener(listener.OnClickLiftButton);
            rightButton.onClick.AddListener(listener.OnClickRightButton);

            currentMeetingToggle.onValueChanged.AddListener(listener.OnCurrentMeetingToggleValueChanged);
            historyMeetingToggle.onValueChanged.AddListener(listener.OnHistoryMeetingToggleValueChanged);
        }

        public interface IListener
        {
            void OnClickCloseButton();

            void OnInfoToggleValueChanged(bool isOn);

            void OnMemberToggleValueChanged(bool isOn);

            void OnMeetingToggleValueChanged(bool isOn);

            void OnClickReportButton();

            void OnClickEditDeclarationButton();

            void OnClickEnterChatButton();

            void OnClickLeaveButton();

            void OnClickLeaveCancelButton();

            void OnClickTransferButton();

            void OnClickInviteButton();

            void OnClickTeamButton();

            void OnClickLiftButton();

            void OnClickRightButton();

            void OnCurrentMeetingToggleValueChanged(bool isOn);

            void OnHistoryMeetingToggleValueChanged(bool isOn);
        }

        public void LoadShowScene()
        {
            GameObject scene = GameObject.Instantiate<GameObject>(infoAssetDependencies.mCustomDependencies[0] as GameObject);
            showSceneControl = new ShowSceneControl();
            scene.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(scene);

            for (int i = 1; i < 6; i++)
            {
                Transform objTrans = showSceneControl.mRoot.transform.Find("Pos_" + i.ToString());
                VirtualGameObject vobj = new VirtualGameObject();
                vobj.SetGameObject(objTrans.gameObject, true);
                showPosList.Add(new WarriorGroupShowModelPositionItem() { VGO = vobj });
            }
        }

        public void UpdataShowScenePosPosition()
        {
            for (int index = 0, len = showPosList.Count; index < len; index++)
            {
                Transform objTrans = showPosList[index].VGO.transform;
                Vector3 posshow = showSceneControl.mCamera.WorldToViewportPoint(objTrans.position);
                var itemposition = GetMemberItemPosition(index);
                Vector3 uiitem = UIManager.mUICamera.WorldToViewportPoint(itemposition);
                posshow.x = uiitem.x;
                Vector3 position = showSceneControl.mCamera.ViewportToWorldPoint(posshow);
                objTrans.position = position;
            }
        }

        Vector3 GetMemberItemPosition(int index)
        {
            var item = members[index];

            if (item == null)
                return Vector3.zero;

            return item.GetPosition();
        }

        public void UnLoadShowScene()
        {
            if (showSceneControl == null)
                return;

            showSceneControl.Dispose();
            showSceneControl = null;

            showPosList.Clear();

            for (int index = 0, len = modelShowList.Count; index < len; index++)
            {
                modelShowList[index].Dispose();
                modelShowList[index] = null;
            }
            modelShowList.Clear();
        }

        public void SetMemberModel(EMemeModelShowPos ePos, Sys_WarriorGroup.WarriorInfo warriorInfo)
        {
            WarriorGroupModelShow modelShow = modelShowList.Find(o => o.UID == warriorInfo.RoleID);
            if (modelShow != null)
            {
                modelShow.ChangeWeapon(warriorInfo.WeaponID);
            }
            else
            {
                modelShow = CreateModeShow(ePos, warriorInfo);
                modelShow.UID = warriorInfo.RoleID;
                modelShowList.Add(modelShow);
            }
            var value = GetModelPosition(ePos);
            if (value.ModelShow != null)
            {
                value.ModelShow.SetActive(false);
            }
            value.SetModelShow(modelShow);
        }

        WarriorGroupShowModelPositionItem GetModelPosition(EMemeModelShowPos ePos)
        {
            return showPosList[(int)ePos];
        }

        WarriorGroupModelShow CreateModeShow(EMemeModelShowPos ePos, Sys_WarriorGroup.WarriorInfo warriorInfo)
        {
            WarriorGroupModelShow modelShow = new WarriorGroupModelShow();
            modelShow.WeaponID = warriorInfo.WeaponID;

            var parent = GetModelPosition(ePos);
            modelShow.Parent = parent.VGO;
            modelShow.LoadModel(warriorInfo.HeroID, warriorInfo.Occ, warriorInfo.WeaponID, warriorInfo.DressID, warriorInfo.DressData);
            return modelShow;
        }
    }
}
