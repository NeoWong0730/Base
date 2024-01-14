using Framework;
using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Logic
{
    public class UI_MagicBookDetailImage
    {
        private Text descText;
        private RawImage showRawimage;

        public void SetData(Transform transform, string path, uint langId)
        {
            if (null == descText)
            {
                descText = transform.Find("Text_Des").GetComponent<Text>();
                showRawimage = transform.Find("Image_Picture").GetComponent<RawImage>();
            }

            TextHelper.SetText(descText, langId);
            ImageHelper.SetTexture(showRawimage, path);
        }

        private void MHandle_Completed(AsyncOperationHandle<Texture> handle)
        {
            showRawimage.texture = handle.Result;
            showRawimage.enabled = true;
        }
    }
    public class UI_MagicBookDetailTask
    {
        private Text descText;
        private PropItem tipItem;
        private Button gotoBtn;
        private Button acceptBtn;
        private Button againBtn;
        private GameObject finishGo;
        private CSVChapterSysTask.Data sysTaskData;
        public void SetData(Transform transform, uint taskId)
        {
            if (null == descText)
            {
                descText = transform.Find("Text_Des").GetComponent<Text>();
                gotoBtn = transform.Find("Btn_Go").GetComponent<Button>();
                acceptBtn = transform.Find("Btn_Accept").GetComponent<Button>();
                againBtn = transform.Find("Object_Finish/Btn_Again").GetComponent<Button>();
                finishGo = transform.Find("Object_Finish/Image_Finish").gameObject;
                gotoBtn.onClick.AddListener(OnGotoClicked); // 前往
                acceptBtn?.onClick.AddListener(acceptBtnClick); // 领取
                againBtn?.onClick.AddListener(OnGotoClicked); // 再次体验
                ButtonHelper.AddButtonCtrl(acceptBtn);
                ButtonHelper.AddButtonCtrl(againBtn);
                ButtonHelper.AddButtonCtrl(gotoBtn);
                tipItem = new PropItem();
                tipItem.BindGameObject(transform.Find("Item").gameObject);
            }

            sysTaskData = CSVChapterSysTask.Instance.GetConfData(taskId);
            if (null != sysTaskData)
            {
                TextHelper.SetText(descText, sysTaskData.TaskNameLag);
                PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData(sysTaskData.MainDisplay, 0, false, false, false, false, false, false, false, true);
                tipItem.SetData(new MessageBoxEvt(EUIID.UI_MagicBook_Detail, showItem));
            }
            RefreshButtonState();
        }

        public void RefreshButtonState()
        {
            EMaigcBookTaskState taskState = Sys_MagicBook.Instance.GetTaskStateByTaskId(sysTaskData);
            acceptBtn.gameObject.SetActive(taskState == EMaigcBookTaskState.UnFinished);
            gotoBtn.gameObject.SetActive(taskState == EMaigcBookTaskState.Hand);
            againBtn.gameObject.SetActive(sysTaskData.TryAgain == 1 && taskState == EMaigcBookTaskState.Finished);
            againBtn.transform.parent.gameObject.SetActive(taskState == EMaigcBookTaskState.Finished);
            finishGo.SetActive(sysTaskData.TryAgain == 0 && taskState == EMaigcBookTaskState.Finished);
        }


        private void OnGotoClicked()
        {
            if (null != sysTaskData)
            {
                Sys_MagicBook.Instance.MagicBookTaskGo(sysTaskData);
            }
        }

        private void acceptBtnClick()
        {
            if (null != sysTaskData)
            {
                Sys_MagicBook.Instance.MagicDictGetSysTaskAwardReq(sysTaskData.id);
            }
        }

    }
    public class UI_MagicBook_DetailCeil
    {
        private CSVChapterFunctionList.Data currentSysData;
        private Image sysIconImage;
        private Text sysNameText;
        private Text sysDescText;
        private Button closeBtn;
        private Button jumpBtn;
        private Transform ImageParent;
        private Transform taskParent;
        List<UI_MagicBookDetailTask> taskCeil = new List<UI_MagicBookDetailTask>();
        public void Init(Transform transform)
        {
            if(null == sysIconImage)
            {
                closeBtn = transform.Find("View_Title/Btn_Close").GetComponent<Button>();
                closeBtn.onClick.AddListener(CloseUI);
                jumpBtn = transform.Find("View_Top/Btn_Look").GetComponent<Button>();
                jumpBtn.onClick.AddListener(JumpBtnClicked);
                sysIconImage = transform.Find("View_Top/Imagebg/Image_Icon").GetComponent<Image>();
                sysNameText = transform.Find("View_Top/Text_Name").GetComponent<Text>();
                sysDescText = transform.Find("View_Top/Text_Finish").GetComponent<Text>();
                ImageParent = transform.Find("View_Bottom/Object_Picture");
                taskParent = transform.Find("View_Bottom/Object_Task");
            }            
        }

        private void CloseUI()
        {
            UIManager.CloseUI(EUIID.UI_MagicBook_Detail);
        }

        private void JumpBtnClicked()
        {
            if(null != currentSysData)
            {
                if (!Sys_FunctionOpen.Instance.IsOpen(currentSysData.FunctionId, true))
                    return;

                if (!Sys_Hint.Instance.PushForbidOprationInFight())
                {
                    if (currentSysData.Type == 0)
                    {
                        if (null != currentSysData.JumpInterface && currentSysData.JumpInterface.Count >= 1)
                        {
                            uint Openid = currentSysData.JumpInterface[0];
                            CSVOpenUi.Data cSVOpenUiData = CSVOpenUi.Instance.GetConfData(Openid);
                            if (null != cSVOpenUiData)
                            {
                                uint para = cSVOpenUiData.ui_para;
                                if (para != 0)
                                {
                                    UIManager.OpenUI((EUIID)cSVOpenUiData.Uiid, false, para);
                                }
                                else
                                {
                                    UIManager.OpenUI((EUIID)cSVOpenUiData.Uiid);
                                }
                            }
                        }
                    }
                    else if (currentSysData.Type == 1)
                    {
                        if (null != currentSysData.JumpInterface && currentSysData.JumpInterface.Count >= 1)
                        {
                            MessageEx practiceEx = new MessageEx
                            {
                                messageState = (EPetMessageViewState)currentSysData.JumpInterface[0],
                            };
                            Sys_Pet.Instance.OnGetPetInfoReq(practiceEx, EPetUiType.UI_Message);
                        }
                    }
                    else if (currentSysData.Type == 2)
                    {
                        if (!Sys_FunctionOpen.Instance.IsOpen(30401, true)) return;

                        Sys_Family.Instance.OpenUI_Family();
                    }
                    else if (currentSysData.Type == 3)
                    {
                        if (!Sys_FunctionOpen.Instance.IsOpen(10304, true))
                            return;
                        if (null != currentSysData.JumpInterface && currentSysData.JumpInterface.Count >= 1)
                        {
                            Sys_Equip.UIEquipPrama data = new Sys_Equip.UIEquipPrama();
                            data.curEquip = null;
                            data.opType = (Sys_Equip.EquipmentOperations)currentSysData.JumpInterface[0];
                            UIManager.OpenUI(EUIID.UI_Equipment, false, data);
                        }
                    }
                    else if (currentSysData.Type == 4)
                    {
                        if (null != currentSysData.JumpInterface && currentSysData.JumpInterface.Count >= 3)
                        {
                            //配置似乎也没必要。
                            EUIID uiId = (EUIID)currentSysData.JumpInterface[0];
                            MallPrama param = new MallPrama();
                            param.mallId = currentSysData.JumpInterface[1];
                            param.shopId = currentSysData.JumpInterface[2];
                            UIManager.OpenUI(uiId, false, param);
                        }
                    }
                    else if (currentSysData.Type == 5) // 前往寻找npc
                    {
                        if (null != currentSysData.JumpInterface && currentSysData.JumpInterface.Count >= 1)
                        {
                            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(currentSysData.JumpInterface[0]);
                            UIManager.CloseUI(EUIID.UI_MagicBook);
                            UIManager.CloseUI(EUIID.UI_MagicBook_Detail);
                        }
                    }
                }
      
            }
        }

        public void SetData(CSVChapterFunctionList.Data sysData)
        {
            currentSysData = sysData;
            if (null != sysData)
            {
                ImageHelper.SetIcon(sysIconImage, sysData.SysIcon);
                TextHelper.SetText(sysNameText, sysData.SystemLanguage);
                TextHelper.SetText(sysDescText, sysData.InterfaceDes);
                if (null != sysData.Picture)
                {
                    int count = sysData.Picture.Count;
                    int descCount = sysData.PictureDescribe.Count;
                    FrameworkTool.CreateChildList(ImageParent, count);
                    for (int i = 0; i < count; i++)
                    {
                        Transform tran = ImageParent.GetChild(i);
                        UI_MagicBookDetailImage imageObj = new UI_MagicBookDetailImage();
                        if(descCount == count)
                        {
                            imageObj.SetData(tran, sysData.Picture[i], sysData.PictureDescribe[i]);
                        }
                        else
                        {
                            imageObj.SetData(tran, sysData.Picture[i], sysData.PictureDescribe[0]);
                        }
                    }
                }
                taskCeil.Clear();
                if (null != sysData.ShowRegionalTask)
                {
                    int count = sysData.ShowRegionalTask.Count;
                    FrameworkTool.CreateChildList(taskParent, count);
                    for (int i = 0; i < count; i++)
                    {
                        Transform tran = taskParent.GetChild(i);
                        UI_MagicBookDetailTask task = new UI_MagicBookDetailTask();
                        task.SetData(tran, sysData.ShowRegionalTask[i]);
                        taskCeil.Add(task);
                    }
                }
            }
        }

        public void RefreshData()
        {
            for (int i = 0; i < taskCeil.Count; i++)
            {
                taskCeil[i].RefreshButtonState();
            }            
        }
    }
    public class UI_MagicBook_Detail : UIBase
    {
        private CSVChapterFunctionList.Data currentData;

        private int index;

        private Button leftBtn;
        private Button rightBtn;

        private List<CSVChapterFunctionList.Data>  unLockSysIds;
        private List<CSVChapterFunctionList.Data>  AllSysIds;
        private UIScrollCenterEx uiCenterOnChild;
        private Dictionary<GameObject, UI_MagicBook_DetailCeil> dicCenters = new Dictionary<GameObject, UI_MagicBook_DetailCeil>();
        List<UI_MagicBook_DetailCeil> centers = new List<UI_MagicBook_DetailCeil>();

        protected override void OnLoaded()
        {
            leftBtn = transform.Find("Animator/Arrow_Left/Button_Left").GetComponent<Button>();
            leftBtn.onClick.AddListener(LeftBtnClick);
            rightBtn = transform.Find("Animator/Arrow_Right/Button_Right").GetComponent<Button>();
            rightBtn.onClick.AddListener(RightBtnClick);
            uiCenterOnChild = transform.Find("Animator/ScrollView").GetNeedComponent<UIScrollCenterEx>();
            uiCenterOnChild.m_itemSetHandler = UpdateChildrenCallback;
            uiCenterOnChild.cb_CenterChildSettle = OnCenter;;
            uiCenterOnChild.SetParam(null, 2);
            uiCenterOnChild.canDrag = false;
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_MagicBook.Instance.eventEmitter.Handle(Sys_MagicBook.EEvents.TaskReward, UpdateCeilUI, toRegister);
            Sys_MagicBook.Instance.eventEmitter.Handle(Sys_MagicBook.EEvents.OnNeedCheckItemRed, UpdateCeilUI, toRegister);
        }

        private void LeftBtnClick()
        {
            if(index - 1 >= 0)
            {
                currentData = unLockSysIds[index -= 1];
                uiCenterOnChild.SwitchIndex(index + 1, false, 5000);
            }                
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10762));
            }
        }

        private void RightBtnClick()
        {
            if (index + 1 < unLockSysIds.Count)
            {
                currentData = unLockSysIds[index += 1];
                uiCenterOnChild.SwitchIndex(index + 1, false, 5000);
            }
            else
            {
                if(unLockSysIds.Count == AllSysIds.Count)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10763));
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(Sys_MagicBook.Instance.GetUnlockText(AllSysIds[index + 1]));
                }                
            }
        }

        protected override void OnOpen(object arg)
        {
            uint currentShowSysID = Convert.ToUInt32(arg);
            currentData = CSVChapterFunctionList.Instance.GetConfData(currentShowSysID);
            if(null == AllSysIds)
            {
                AllSysIds = new List<CSVChapterFunctionList.Data>();
            }
            else
            {
                AllSysIds.Clear();
            }
            if (null != currentData)
            {
                unLockSysIds = Sys_MagicBook.Instance.GetUnlockSysIdBySubSysId(currentData.Chapter, ref AllSysIds);
            }           
        }

        protected override void OnShow()
        {
            UpdateUI();
        }

        private void UpdateChildrenCallback(int index, Transform tra)
        {
            if (index < 0 || index >= unLockSysIds.Count) return;
            if (dicCenters.ContainsKey(tra.gameObject))
            {
                UI_MagicBook_DetailCeil ceil = dicCenters[tra.gameObject];
                ceil.SetData(unLockSysIds[index]);
            }
            else
            {
                UI_MagicBook_DetailCeil ceil = new UI_MagicBook_DetailCeil();
                ceil.Init(tra);
                centers.Add(ceil);
                ceil.SetData(unLockSysIds[index]);
                dicCenters.Add(tra.gameObject, ceil);
            }
        }

        private void UpdateCeilUI()
        {
            for (int i = 0; i < centers.Count; i++)
            {
                centers[i].RefreshData();
            }
        }

        uint pageShowTime;
        public void OnCenter(int index)
        {
            this.index = index;
            currentData = unLockSysIds[index];
            pageShowTime = Sys_Time.Instance.GetServerTime();
            UIManager.HitPointShow(EUIID.UI_MagicBook_Detail, currentData.id.ToString());
        }

        private int GetIndex()
        {
            int count = unLockSysIds.Count;
            for (int i = 0; i < count; i++)
            {
                if(unLockSysIds[i].id == currentData.id)
                {
                    return i;
                }
            }
            return 0;
        }

        private void UpdateUI()
        {
            index = GetIndex();
            uiCenterOnChild.scrollOffset = 0;
            uiCenterOnChild.Init(unLockSysIds.Count);
            uiCenterOnChild.SwitchIndex(index + 1, true);
        }

        protected override void OnClose()
        {
            if (pageShowTime != 0)
            {
                UIManager.HitPointHide(EUIID.UI_MagicBook_Detail, pageShowTime, currentData.id.ToString());
            }
            pageShowTime = 0;
        }
    }
}
