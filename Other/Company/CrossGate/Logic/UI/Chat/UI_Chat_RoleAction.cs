using UnityEngine;
using UnityEngine.UI;
using Lib.Core;
using Packet;
using Net;
using Table;
using Logic.Core;
using System.Collections.Generic;

namespace Logic
{
    public class Sys_RoleAction : SystemModuleBase<Sys_RoleAction>      
    {
        public Dictionary<uint, CSVActionState.Data> recentRoleActionsDic = new Dictionary<uint, CSVActionState.Data>();
        public List<CSVActionState.Data>  recentRoleActionsList = new List<CSVActionState.Data>();
        public List<CSVActionState.Data>  cacheRoleActionsList = new List<CSVActionState.Data>();
        public void PlayRoleAction(CSVActionState.Data _data)
        {
            GameCenter.mainHero.movementComponent.Stop();
            Sys_Pet.Instance.OnPetSetCurrentMountReq(0);

            cacheRoleActionsList.Clear();
            for (int index = 0, len = recentRoleActionsList.Count; index < len; index++)
            {
                cacheRoleActionsList.Add(recentRoleActionsList[index]);
            }

            recentRoleActionsList.Clear();
            recentRoleActionsList.Add(_data);
            if (!recentRoleActionsDic.ContainsKey(_data.id))
            {
                recentRoleActionsDic.Add(_data.id, _data);

                for (int index = 0, len = cacheRoleActionsList.Count; index < len; index++)
                {
                    recentRoleActionsList.Add(cacheRoleActionsList[index]);
                }
            }
            else
            {
                for (int index = 0, len = cacheRoleActionsList.Count; index < len; index++)
                {
                    if (cacheRoleActionsList[index].id != _data.id)
                    {
                        recentRoleActionsList.Add(cacheRoleActionsList[index]);
                    }
                }
            }
            

            Timer.Register(0.1f, () =>
            {

                CmdMapRoleActionReq cmdMapRoleActionReq = new CmdMapRoleActionReq();
                cmdMapRoleActionReq.Actionid = _data.id;
                if (GameCenter.mainHero.transform.localEulerAngles.y < 0)
                {
                    cmdMapRoleActionReq.Direction = (uint)((GameCenter.mainHero.transform.localEulerAngles.y + 360f) * 1000);
                }
                else
                {
                    cmdMapRoleActionReq.Direction = (uint)(GameCenter.mainHero.transform.localEulerAngles.y * 1000);
                }
                NetClient.Instance.SendMessage((ushort)CmdMap.RoleActionReq, cmdMapRoleActionReq);
            });
        }
        
        public override void OnLogin()
        {
            base.OnLogin();

            recentRoleActionsDic.Clear();
            recentRoleActionsList.Clear();
            cacheRoleActionsList.Clear();
        }
    }

    public class UI_Chat_RoleAction
    {
        //static float CD = 3.0f;
        //static Timer timer;
        //static bool flag = true;

        public UI_ChatSimplify_Layout uI_ChatSimplify_Layout;

        public class UI_Chat_RoleActionCell
        {
            GameObject root;
             
            Image icon;
            Text text;
            Button button;

            CSVActionState.Data _data;

            public void BindGameObject(GameObject go)
            {
                root = go;
                icon = root.FindChildByName("Image").GetComponent<Image>();
                text = root.FindChildByName("Text").GetComponent<Text>();
                button = icon.GetNeedComponent<Button>();
                button.onClick.AddListener(OnClickButton);
            }

            public void UpdateCellView(CSVActionState.Data data)
            {
                _data = data;
                if (data.Iconid != 0)
                {
                    ImageHelper.SetIcon(icon, data.Iconid);
                }
                text.gameObject.SetActive(false);
            }

            void OnClickButton()
            {
                //if (flag)
                //{
                    //flag = false;

                    Sys_RoleAction.Instance.PlayRoleAction(_data);

                ActionCtrl.Instance.Reset();
                    //timer?.Cancel();
                    //timer = Timer.Register(CD, () =>
                    //{
                    //    flag = true;
                    //});
                    // }
            }
        }

        Button roleActionBtn;
        Button returnBtn;
        GameObject roleActionRoot;
        GameObject roleActionItem;
        InfinityGrid infinity_single;
        InfinityGrid infinity_recent;

        Button recentBtn;
        Button singleBtn;

        public void Parse(GameObject gameObject)
        {
            roleActionBtn = gameObject.FindChildByName("_btn_Motion").GetComponent<Button>();
            roleActionBtn.onClick.AddListener(OnClickButton);

            returnBtn = gameObject.FindChildByName("_btn_Arrow").GetNeedComponent<Button>();
            returnBtn.onClick.AddListener(OnClickReturnButton);

            roleActionRoot = gameObject.FindChildByName("_rtBG2");
            roleActionItem = roleActionRoot.FindChildByName("Item");

            infinity_single = roleActionRoot.FindChildByName("_sv_content_single").GetComponent<InfinityGrid>();
            infinity_single.onCreateCell += OnCreateCellSingle;
            infinity_single.onCellChange += OnCellChangeSingle;

            infinity_recent = roleActionRoot.FindChildByName("_sv_content_recent").GetComponent<InfinityGrid>();
            infinity_recent.onCreateCell += OnCreateCellRecent;
            infinity_recent.onCellChange += OnCellChangeRecent;

            recentBtn = gameObject.FindChildByName("_btn_Motion0").GetComponent<Button>();
            recentBtn.onClick.AddListener(OnClickRecentButton);
            singleBtn = gameObject.FindChildByName("_btn_Motion1").GetComponent<Button>();
            singleBtn.onClick.AddListener(OnClickSingleButton);
        }

        void OnClickRecentButton()
        {
            infinity_recent.gameObject.SetActive(true);
            infinity_single.gameObject.SetActive(false);

            infinity_recent.CellCount = Sys_RoleAction.Instance.recentRoleActionsDic.Count;
            infinity_recent.ForceRefreshActiveCell();
        }

        void OnClickSingleButton()
        {
            infinity_recent.gameObject.SetActive(false);
            infinity_single.gameObject.SetActive(true);

            infinity_single.CellCount = CSVActionState.Instance.ShowRoleActions.Count;
            infinity_single.ForceRefreshActiveCell();
        }

        void OnCreateCellSingle(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            UI_Chat_RoleActionCell itemCell = new UI_Chat_RoleActionCell();
            itemCell.BindGameObject(go);
            cell.BindUserData(itemCell);
        }

        void OnCellChangeSingle(InfinityGridCell cell, int index)
        {
            UI_Chat_RoleActionCell mCell = cell.mUserData as UI_Chat_RoleActionCell;
            if (index < CSVActionState.Instance.ShowRoleActions.Count)
            {
                var item = CSVActionState.Instance.ShowRoleActions[index];
                mCell.UpdateCellView(item);
            }
        }

        void OnCreateCellRecent(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            UI_Chat_RoleActionCell itemCell = new UI_Chat_RoleActionCell();
            itemCell.BindGameObject(go);
            cell.BindUserData(itemCell);
        }

        void OnCellChangeRecent(InfinityGridCell cell, int index)
        {
            UI_Chat_RoleActionCell mCell = cell.mUserData as UI_Chat_RoleActionCell;
            List<CSVActionState.Data>  datas = Sys_RoleAction.Instance.recentRoleActionsList;
            if (index < datas.Count)
            {
                var item = datas[index];
                mCell.UpdateCellView(item);
            }
        }

        void OnClickButton()
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
                return;

            roleActionRoot.SetActive(true);
            uI_ChatSimplify_Layout.rtBG_RectTransform.gameObject.SetActive(false);
            if (roleActionRoot.activeSelf)
            {
                singleBtn.onClick.Invoke();               
            }
        }

        void OnClickReturnButton()
        {
            roleActionRoot.SetActive(false);
            uI_ChatSimplify_Layout.rtBG_RectTransform.gameObject.SetActive(true);
        }

#if UNITY_STANDALONE_WIN && (OPEN_PC_KEYCODE_FUN || !UNITY_EDITOR)
        public void OpenRoleAction()
        {
            OnClickButton();
        }
       
#endif
        public void OnAfterEnterFightEffect()
        {
            OnClickReturnButton();
        }
    }
}
