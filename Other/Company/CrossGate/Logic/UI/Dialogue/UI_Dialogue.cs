using UnityEngine.UI;
using Logic.Core;
using UnityEngine;
using Lib.Core;
using Table;
using System;
using System.Collections.Generic;
using Framework;
using Packet;
using DG.Tweening;

namespace Logic
{
#if UNITY_EDITOR
    public class EditorDialogueActor
    {
        public Timer timer;

        public void Add(GameObject mRoot, GameObject Pos)
        {
            if (mRoot == null || Pos == null)
            {
                return;
            }

            ShowSceneInspector showSceneInspector = mRoot.GetNeedComponent<ShowSceneInspector>();

            showSceneInspector.SetTransformAction = (_pos, _rot, _scale) =>
            {
                if (Pos.transform.childCount > 0)
                {
                    Pos.transform.GetChild(0).localPosition = _pos;
                    Pos.transform.GetChild(0).localEulerAngles = _rot;
                    Pos.transform.GetChild(0).localScale = _scale;
                }
            };

            timer?.Cancel();
            timer = Timer.Register(1f, () =>
            {
                if (Pos == null)
                {
                    return;
                }
                if (Pos.transform.childCount > 0)
                {
                    showSceneInspector.pos = Pos.transform.GetChild(0).localPosition;
                    showSceneInspector.rot = Pos.transform.GetChild(0).localEulerAngles;
                    showSceneInspector.scale = Pos.transform.GetChild(0).localScale;
                }
            }, null, false, true);
        }

        public void Dispose()
        {
            timer?.Cancel();
            timer = null;
        }
    }
#endif

    public class onLoadedPlusEvt
    {
        public int index;
        public uint charID;
        public uint actionID;
        public uint weaponID;
        public uint status;
        public uint animID;
        public bool isLeft;
    }

    public class DialogueShowActor
    {
        public HeroLoader heroLoader;
        public DisplayControl<EHeroModelParts> displayControl;

        public void Dispose()
        {
            heroLoader?.Dispose();
            heroLoader = null;

            //if (displayControl != null)
            //{
            //    displayControl.onLoaded -= OnLoadedCallBack;
            //    displayControl.Dispose();
            //    displayControl = null;
            //}
            DisplayControl<EHeroModelParts>.Destory(ref displayControl);
        }

        public void OnLoadedCallBack(int index)
        {
            onLoadedPlus?.Invoke(new onLoadedPlusEvt()
            {
                index = 0,
                charID = CharID,
                actionID = ActionID,
                weaponID = WeaponID,
                status = Status,
                animID = AnimID,
                isLeft = IsLeft,
            });
        }

        public uint CharID
        {
            get;
            set;
        }

        public uint ActionID
        {
            get;
            set;
        }

        public uint WeaponID
        {
            get;
            set;
        }

        public uint Status
        {
            get;
            set;
        }

        public uint AnimID
        {
            get;
            set;
        }

        public bool IsLeft
        {
            get;
            set;
        }

        public Vector3 LeftOffset
        {
            get;
            set;
        }

        public Vector3 RightOffset
        {
            get;
            set;
        }

        public Vector3 LeftRotOffset
        {
            get;
            set;
        }

        public Vector3 RightRotOffset
        {
            get;
            set;
        }

        public Vector3 LeftScaleOffset
        {
            get;
            set;
        }

        public Vector3 RightScaleOffset
        {
            get;
            set;
        }

        public Action<onLoadedPlusEvt> onLoadedPlus;

        public void SetTransformOffset(bool isLeft)
        {
            GameObject obj = null;
            if (heroLoader != null)
            {
                obj = heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).gameObject;
            }
            else if (displayControl != null)
            {
                obj = displayControl.GetPart(EHeroModelParts.Main).gameObject;
            }

            if (obj == null)
                return;
            if (isLeft)
            {
                obj.transform.localPosition += LeftOffset;
                obj.transform.localEulerAngles += LeftRotOffset;
                obj.transform.localScale = new Vector3(obj.transform.localScale.x * LeftScaleOffset.x, obj.transform.localScale.y * LeftScaleOffset.y, obj.transform.localScale.z * LeftScaleOffset.z);

            }
            else
            {
                obj.transform.localPosition += RightOffset;
                obj.transform.localEulerAngles += RightRotOffset;
                obj.transform.localScale = new Vector3(obj.transform.localScale.x * RightScaleOffset.x, obj.transform.localScale.y * RightScaleOffset.y, obj.transform.localScale.z * RightScaleOffset.z);
            }
        }

        public GameObject GetGameObject(uint index)
        {
            if (heroLoader != null)
            {
                return heroLoader.heroDisplay.GetPart((EHeroModelParts)index).gameObject;
            }
            else if (displayControl != null)
            {
                return displayControl.GetPart((EHeroModelParts)index).gameObject;
            }
            return null;
        }
        /// <summary>
        /// 挂件镜像位恢复
        /// </summary>
        public void SetAccessoryMirrorReturn(bool isLeft)
        {
            if (!isLeft && heroLoader != null)
            {
                EHeroModelParts eHeroModelParts = EHeroModelParts.None;
                var dressDatas = Sys_Fashion.Instance.GetDressData();
                foreach (var id in dressDatas.Keys)
                {
                    if (!Sys_Fashion.Instance.parts.TryGetValue(id, out eHeroModelParts))
                    {
                        continue;
                    }
                    if (eHeroModelParts == EHeroModelParts.Jewelry_Back
                        || eHeroModelParts == EHeroModelParts.Jewelry_Face
                        || eHeroModelParts == EHeroModelParts.Jewelry_Head
                        || eHeroModelParts == EHeroModelParts.Jewelry_Waist)
                    {
                        var acceId = heroLoader.showParts[(int)eHeroModelParts];
                        CSVFashionAccessory.Data acceData = CSVFashionAccessory.Instance.GetConfData(acceId);
                        if (acceData != null && acceData.is_overturn == 1)
                        {
                            var goAcce = heroLoader.heroDisplay.GetPart(eHeroModelParts);
                            if (goAcce.eState == VirtualGameObject.EState.Loaded)
                            {
                                goAcce.transform.localEulerAngles = new Vector3(0, -180, 0);
                            }
                            else if (goAcce.eState == VirtualGameObject.EState.Loading)
                            {
                                goAcce.onLoaded += (vGO) =>
                                {
                                    vGO.transform.localEulerAngles = new Vector3(0, -180, 0);
                                };
                            }
                        }
                    }
                }
            }
        }
    }

    public class UI_Dialogue : UIBase
    {
        public class UI_Dialogue_Secret
        {
            public GameObject root;
            public InputField inputField;
            public Button sendButton;
            public Button closeButton;
            public Button cancelButton;

            public CSVCode.Data cSVCodeData;

            public UI_Dialogue_Secret(GameObject gameObject, uint id)
            {
                root = gameObject;
                cSVCodeData = CSVCode.Instance.GetConfData(id);

                inputField = root.gameObject.FindChildByName("InputField").GetComponent<InputField>();
                sendButton = root.gameObject.FindChildByName("Btn_01").GetComponent<Button>();
                sendButton.onClick.AddListener(OnClickSendButton);
                closeButton = root.gameObject.FindChildByName("Btn_Close").GetComponent<Button>();
                closeButton.onClick.AddListener(OnClickCloseButton);
                cancelButton = root.gameObject.FindChildByName("Btn_Cancel").GetComponent<Button>();
                cancelButton.onClick.AddListener(OnClickCancelButton);
            }

            void OnClickSendButton()
            {
                if (inputField.text == LanguageHelper.GetTextContent(cSVCodeData.CorrectCode))
                {
                    Sys_SecretMessage.Instance.eventEmitter.Trigger<uint, uint>(Sys_SecretMessage.EEvents.MessageRight, cSVCodeData.CorrectDialogue, cSVCodeData.id);
                    Sys_Chat.Instance.PushMessage(ChatType.Person, Sys_Chat.Instance.gSystemChatBaseInfo, LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(2008311)), Sys_Chat.EMessageProcess.None);
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(2008311)));
                }
                else
                {
                    bool clue = false;
                    if (cSVCodeData.ClueDialogue != null && cSVCodeData.ClueDialogue.Count > 0)
                    {
                        for (int index = 0, len = cSVCodeData.ClueDialogue.Count; index < len; index++)
                        {
                            if (inputField.text == LanguageHelper.GetTextContent(cSVCodeData.ClueDialogue[index][0]))
                            {
                                Sys_SecretMessage.Instance.eventEmitter.Trigger<uint, uint>(Sys_SecretMessage.EEvents.MessageClue, cSVCodeData.ClueDialogue[index][1], cSVCodeData.id);
                                clue = true;
                                break;
                            }
                        }
                    }
                    if (!clue)
                    {
                        Sys_SecretMessage.Instance.eventEmitter.Trigger<uint, uint>(Sys_SecretMessage.EEvents.MessageWrong, cSVCodeData.WrongCode, cSVCodeData.id);
                        Sys_Chat.Instance.PushMessage(ChatType.Person, Sys_Chat.Instance.gSystemChatBaseInfo, LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(2008312)), Sys_Chat.EMessageProcess.None);
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(2008312)));
                    }
                }

                //Sys_Chat.ChatBaseInfo chatBaseInfo = new Sys_Chat.ChatBaseInfo();
                //chatBaseInfo.nHeroID = Sys_Role.Instance.HeroId;
                //chatBaseInfo.sSenderName = Sys_Role.Instance.sRoleName;
                //chatBaseInfo.nRoleID = Sys_Role.Instance.RoleId;
                //chatBaseInfo.SenderHead = Sys_Head.Instance.clientHead.headId;
                //chatBaseInfo.SenderHeadFrame = Sys_Head.Instance.clientHead.headFrameId;
                //chatBaseInfo.SenderChatFrame = Sys_Head.Instance.clientHead.chatFrameId;
                //chatBaseInfo.SenderChatText = Sys_Head.Instance.clientHead.chatTextId;
                //Sys_Chat.Instance.PushMessage(ChatType.Local, chatBaseInfo, inputField.text, Sys_Chat.EMessageProcess.None, Sys_Chat.EExtMsgType.Normal, null, null, true);
                Sys_Chat.Instance.SendContent(ChatType.Local, inputField.text, Sys_Chat.EExtMsgType.Normal);
                inputField.text = string.Empty;
            }

            void OnClickCloseButton()
            {
                GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
                UIManager.CloseUI(EUIID.UI_Dialogue);
            }

            void OnClickCancelButton()
            {
                GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
                UIManager.CloseUI(EUIID.UI_Dialogue);
            }
        }

        Button skipButton;
        Button backGroundButton;
        Button backGroundTextButton;
        GameObject nextIcon;

        public GameObject dialogueRoot;
        GameObject titleLeftRoot;
        Text titleNameLeft;
        GameObject titleRightRoot;
        Text titleNameRight;
        EmojiText text;

        Text nextCD;

        int index = 0;

        Action callback;

        ResetDialogueDataEventData currentResetDialogueDataEventData;

        Timer nextTimer;

        public AssetDependencies assetDependencies;
        ShowSceneControl showSceneControlLeft;
        ShowSceneControl showSceneControlRight;
        RawImage rawImageLeft;
        RawImage rawImageRight;
        Dictionary<uint, DialogueShowActor> dialogueActors = new Dictionary<uint, DialogueShowActor>();

        static float grayValue;

        List<Transform> backgroundImages = new List<Transform>();

        GameObject secretRoot;
        GameObject secretPrefab;

        UI_Dialogue_Secret uI_Dialogue_Secret;

        private int charsPerSecond = 1;
        private Tweener tweener;

        protected override void OnLoaded()
        {
            assetDependencies = gameObject.GetComponent<AssetDependencies>();

            skipButton = gameObject.FindChildByName("SkipButton").GetComponent<Button>();
            skipButton.onClick.AddListener(OnClickSkipButton);
            backGroundButton = gameObject.FindChildByName("BackGround").GetComponent<Button>();
            backGroundButton.onClick.AddListener(OnClickNextButton);
            nextIcon = gameObject.FindChildByName("NextButton");
            backGroundTextButton = gameObject.FindChildByName("BackGroundText").GetComponent<Button>();
            backGroundTextButton.onClick.AddListener(OnClickQuickShowButton);

            dialogueRoot = transform.Find("Animator/DialogueRoot").gameObject;

            titleLeftRoot = gameObject.FindChildByName("DialogueRoot_01");
            titleNameLeft = titleLeftRoot.FindChildByName("TitleName").GetComponent<Text>();
            titleRightRoot = gameObject.FindChildByName("DialogueRoot_02");
            titleNameRight = titleRightRoot.FindChildByName("TitleName").GetComponent<Text>();
            text = gameObject.FindChildByName("ContentText").GetComponent<EmojiText>();
            text.text = string.Empty;
            charsPerSecond = int.Parse(CSVParam.Instance.GetConfData(127).str_value);

            nextCD = gameObject.FindChildByName("NextCD").GetComponent<Text>();

            rawImageLeft = gameObject.FindChildByName("RawImageLeft").GetComponent<RawImage>();
            rawImageRight = gameObject.FindChildByName("RawImageRight").GetComponent<RawImage>();

            grayValue = float.Parse(CSVParam.Instance.GetConfData(128).str_value);

            backgroundImages.Clear();
            Transform backGroundRoot = gameObject.FindChildByName("Object_Frame").transform;
            for (int index = 0, len = backGroundRoot.childCount; index < len; index++)
            {
                backgroundImages.Add(backGroundRoot.GetChild(index));
            }

            secretRoot = gameObject.FindChildByName("SecretRoot");
            secretPrefab = gameObject.FindChildByName("UI_Dialogue_Secret");
        }

        protected override void OnOpen(object arg)
        {
            currentResetDialogueDataEventData = arg as ResetDialogueDataEventData;
        }

        protected override void OnShow()
        {
            UnloadShowContent();

            LoadShowSceneLeft();
            LoadShowSceneRight();

            if (currentResetDialogueDataEventData != null)
            {
                OnResetDialogueData(currentResetDialogueDataEventData);
            }
        }

        protected override void OnHideStart()
        {
            nextTimer?.Cancel();
            nextTimer = null;
        }

        void LoadShowSceneLeft()
        {
            if (showSceneControlLeft == null)
            {
                showSceneControlLeft = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControlLeft.Parse(sceneModel);
            showSceneControlLeft.mRoot.name = "UI_Dialogue_showSceneControlLeft";

            //=====>>
            //ct ：Npc对话关闭多余的RT和绘制            
            showSceneControlLeft.mCamera.gameObject.SetActive(false);
            //rawImageLeft.texture = showSceneControlLeft.GetTemporary(0, 0, 16, RenderTextureFormat.ARGB32, 1f);
            //====<<
        }

        void LoadShowSceneRight()
        {
            if (showSceneControlRight == null)
            {
                showSceneControlRight = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[1] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControlRight.Parse(sceneModel);
            showSceneControlRight.mRoot.name = "UI_Dialogue_showSceneControlRight";

            //=====>>
            //ct ：Npc对话关闭多余的RT和绘制            
            showSceneControlRight.mCamera.gameObject.SetActive(false);
            //rawImageRight.texture = showSceneControlRight.GetTemporary(0, 0, 16, RenderTextureFormat.ARGB32, 1f);
            //====<<
        }

        //=====>>
        //ct ：Npc对话关闭多余的RT和绘制        
        void SetActiveShowSceneLeft(bool active)
        {
            if (showSceneControlLeft == null)
                return;

            if (active)
            {
                rawImageLeft.texture = showSceneControlLeft.GetTemporary(0, 0, 16, RenderTextureFormat.ARGB32, 1f);
            }
            else
            {
                rawImageLeft.texture = null;
                showSceneControlLeft.ReleaseTemporary();
            }

            showSceneControlLeft.mCamera.gameObject.SetActive(active);
        }

        void SetActiveShowSceneRight(bool active)
        {
            if (showSceneControlRight == null)
                return;

            if (active)
            {
                rawImageRight.texture = showSceneControlRight.GetTemporary(0, 0, 16, RenderTextureFormat.ARGB32, 1f);
            }
            else
            {
                rawImageRight.texture = null;
                showSceneControlRight.ReleaseTemporary();
            }

            showSceneControlRight.mCamera.gameObject.SetActive(active);
        }

        void RefreshShowScene()
        {
            //保证RT先释放再申请 重复利用

            bool useLeft = rawImageLeft.color.a > 0.01f;
            bool useRight = rawImageRight.color.a > 0.01f;

            if (!useLeft)
            {
                SetActiveShowSceneLeft(false);
            }

            if (!useRight)
            {
                SetActiveShowSceneRight(false);
            }

            if (useLeft)
            {
                SetActiveShowSceneLeft(true);
            }

            if (useRight)
            {
                SetActiveShowSceneRight(true);
            }
        }
        //====<<

        void UnloadShowContent()
        {
            rawImageLeft.texture = null;
            rawImageRight.texture = null;
            rawImageLeft.color = new Color(1f, 1f, 1f, 0f);
            rawImageRight.color = new Color(1f, 1f, 1f, 0f);
            foreach (var dialogueActor in dialogueActors.Values)
            {
                dialogueActor.Dispose();
            }
            dialogueActors.Clear();
            showSceneControlLeft?.Dispose();
            showSceneControlRight?.Dispose();
            lastLeftShowCharID = 49999999;
            lastRightShowCharID = 49999999;

            secretRoot.DestoryAllChildren();
            uI_Dialogue_Secret = null;

            Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnClearEmotion);
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_SecretMessage.Instance.eventEmitter.Handle<uint, uint>(Sys_SecretMessage.EEvents.MessageClue, OnMessageClue, toRegister);
            Sys_SecretMessage.Instance.eventEmitter.Handle<uint, uint>(Sys_SecretMessage.EEvents.MessageWrong, OnMessageWrong, toRegister);
            Sys_SecretMessage.Instance.eventEmitter.Handle<uint, uint>(Sys_SecretMessage.EEvents.MessageRight, OnMessageRight, toRegister);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Dialogue.Instance.eventEmitter.Handle<ResetDialogueDataEventData>(Sys_Dialogue.EEvents.OnResetDialogueData, OnResetDialogueData, toRegister);
        }

        void OnMessageRight(uint id, uint codeID)
        {
            secretRoot.SetActive(false);

            CSVDialogue.Data cSVDialogueData = CSVDialogue.Instance.GetConfData(id);
            List<Sys_Dialogue.DialogueDataWrap> datas = Sys_Dialogue.GetDialogueDataWraps(cSVDialogueData);
            ResetDialogueDataEventData resetDialogueDataEventData = PoolManager.Fetch(typeof(ResetDialogueDataEventData)) as ResetDialogueDataEventData;
            resetDialogueDataEventData.Init(datas, () =>
            {
                Sys_SecretMessage.Instance.ReqSecretMessage(codeID);
            }, cSVDialogueData);

            OnResetDialogueData(resetDialogueDataEventData);
        }

        void OnMessageClue(uint id, uint codeID)
        {
            secretRoot.SetActive(false);

            CSVDialogue.Data cSVDialogueData = CSVDialogue.Instance.GetConfData(id);
            List<Sys_Dialogue.DialogueDataWrap> datas = Sys_Dialogue.GetDialogueDataWraps(cSVDialogueData);
            ResetDialogueDataEventData resetDialogueDataEventData = PoolManager.Fetch(typeof(ResetDialogueDataEventData)) as ResetDialogueDataEventData;
            resetDialogueDataEventData.Init(datas, () =>
            {
                Sys_SecretMessage.Instance.OpenSecretMessage(CSVCode.Instance.GetConfData(codeID));
                secretRoot.SetActive(true);
            }, cSVDialogueData);
            resetDialogueDataEventData.holdDialogueOpen = true;

            OnResetDialogueData(resetDialogueDataEventData);
        }

        void OnMessageWrong(uint id, uint codeID)
        {
            secretRoot.SetActive(false);

            CSVDialogue.Data cSVDialogueData = CSVDialogue.Instance.GetConfData(id);
            List<Sys_Dialogue.DialogueDataWrap> datas = Sys_Dialogue.GetDialogueDataWraps(cSVDialogueData);
            ResetDialogueDataEventData resetDialogueDataEventData = PoolManager.Fetch(typeof(ResetDialogueDataEventData)) as ResetDialogueDataEventData;
            resetDialogueDataEventData.Init(datas, () =>
            {
                Sys_SecretMessage.Instance.OpenSecretMessage(CSVCode.Instance.GetConfData(codeID));
                secretRoot.SetActive(true);
            }, cSVDialogueData);
            resetDialogueDataEventData.holdDialogueOpen = true;

            OnResetDialogueData(resetDialogueDataEventData);
        }

        void OnResetDialogueData(ResetDialogueDataEventData resetDialogueDataEventData)
        {
            index = 0;
            currentResetDialogueDataEventData = resetDialogueDataEventData;
            callback = currentResetDialogueDataEventData.callback;

            // added by cuibinbin
            // -- start
            if (gameObject == null || !gameObject.activeInHierarchy)
            {
                return;
            }
            // -- end

            if (currentResetDialogueDataEventData.datas.Count > 0)
            {
                Refresh(currentResetDialogueDataEventData.datas[0]);
                if (Sys_Dialogue.Instance.ShowUIActorFlag)
                {
                    RefreshShowActors(currentResetDialogueDataEventData.datas[0]);
                }
                if (resetDialogueDataEventData.onlyOnePass && resetDialogueDataEventData.datas.Count == 1)
                {
                    OnClickNextButton();
                }
            }

            if (resetDialogueDataEventData.secret)
            {
                secretRoot.DestoryAllChildren();
                GameObject secretGo = GameObject.Instantiate(secretPrefab);
                secretGo.transform.SetParent(secretRoot.transform, false);
                secretGo.SetActive(true);

                uI_Dialogue_Secret = new UI_Dialogue_Secret(secretGo, resetDialogueDataEventData.cSVCodeDataID);

            }
        }

        void RefreshUIShowStatus()
        {
            nextIcon.SetActive(false);
            skipButton.gameObject.SetActive(!currentResetDialogueDataEventData.hideSkipButton);
        }

        void OnShowModelLoaded(onLoadedPlusEvt evt)
        {
            if (evt.index == 0)
            {
                GameObject go = dialogueActors[evt.charID].GetGameObject(0);
                go?.SetActive(false);

                if (dialogueActors[evt.charID].heroLoader != null)
                {
                    dialogueActors[evt.charID].heroLoader.heroDisplay.mAnimation.UpdateHoldingAnimations(evt.actionID, evt.weaponID, null, (EStateType)evt.animID, go);
                }
                else if (dialogueActors[evt.charID].displayControl != null)
                {
                    dialogueActors[evt.charID].displayControl.mAnimation.UpdateHoldingAnimations(evt.actionID, evt.weaponID, null, (EStateType)evt.animID, go);
                }

                if (evt.status == 0)
                {
                    if (evt.isLeft)
                    {
                        rawImageLeft.color = new Color(1f, 1f, 1f, 0f);
                    }
                    else
                    {
                        rawImageRight.color = new Color(1f, 1f, 1f, 0f);
                    }
                }
                else if (evt.status == 1)
                {
                    //变灰
                    if (evt.isLeft)
                    {
                        rawImageLeft.color = new Color(grayValue, grayValue, grayValue, 1);
                    }
                    else
                    {
                        rawImageRight.color = new Color(grayValue, grayValue, grayValue, 1);
                    }
                }
                else if (evt.status == 2)
                {
                    //变亮
                    if (evt.isLeft)
                    {
                        rawImageLeft.color = new Color(1f, 1f, 1f, 1);
                    }
                    else
                    {
                        rawImageRight.color = new Color(1f, 1f, 1f, 1);
                    }
                }

                dialogueActors[evt.charID].SetTransformOffset(evt.isLeft);
                dialogueActors[evt.charID].SetAccessoryMirrorReturn(evt.isLeft);
                
                //=====>>
                //ct ：Npc对话关闭多余的RT和绘制                
                RefreshShowScene();
                //=====<<
            }
        }

        protected override void OnHide()
        {
            UnloadShowContent();

#if UNITY_EDITOR && !ILRUNTIME_MODE
            GameCenter.ShowSceneRootInspectorFlag = false;
#endif            
        }

        void Refresh(Sys_Dialogue.DialogueDataWrap dialogueDataWrap)
        {
            if (dialogueDataWrap.LeftShowStatus == 2)
            {
                titleLeftRoot.SetActive(true);
                titleRightRoot.SetActive(false);
                if (dialogueDataWrap.ActorNameID != (uint)EDialogueActorType.Player)
                {
                    Refresh(dialogueDataWrap.ActorNameID, dialogueDataWrap.ContentID, titleNameLeft, dialogueDataWrap.BackGroundIndex);
                    if (GameCenter.uniqueNpcs.ContainsKey(dialogueDataWrap.CharID) && dialogueDataWrap.BubbleID != 0)
                    {
                        CreateNpcBubble(dialogueDataWrap.BubbleID, dialogueDataWrap.CharID);
                    }
                }
                else
                {
                    Refresh(Sys_Role.Instance.Role.Name.ToStringUtf8(), dialogueDataWrap.ContentID, titleNameLeft, dialogueDataWrap.BackGroundIndex);
                    if (dialogueDataWrap.BubbleID != 0)
                    {
                        CreateMainHeroBubble(dialogueDataWrap.BubbleID);
                    }
                }
            }
            else
            {
                titleLeftRoot.SetActive(false);
                titleRightRoot.SetActive(true);
                if (dialogueDataWrap.ActorNameID != (uint)EDialogueActorType.Player)
                {
                    Refresh(dialogueDataWrap.ActorNameID, dialogueDataWrap.ContentID, titleNameRight, dialogueDataWrap.BackGroundIndex);
                    if (GameCenter.uniqueNpcs.ContainsKey(dialogueDataWrap.CharID) && dialogueDataWrap.BubbleID != 0)
                    {
                        CreateNpcBubble(dialogueDataWrap.BubbleID, dialogueDataWrap.CharID);
                    }
                }
                else
                {
                    Refresh(Sys_Role.Instance.Role.Name.ToStringUtf8(), dialogueDataWrap.ContentID, titleNameRight, dialogueDataWrap.BackGroundIndex);
                    if (dialogueDataWrap.BubbleID != 0)
                    {
                        CreateMainHeroBubble(dialogueDataWrap.BubbleID);
                    }
                }
            }
        }

        void CreateMainHeroBubble(uint bubbleId)
        {
            Sys_HUD.Instance.OpenHud();
            CSVBubble.Data cSVBubbleData = CSVBubble.Instance.GetConfData(bubbleId);
            if (cSVBubbleData != null)
            {
                if (cSVBubbleData.Type == (uint)EBubbleType.PureWord)
                {
                    TriggerNpcBubbleEvt triggerNpcBubbleEvt = new TriggerNpcBubbleEvt();
                    triggerNpcBubbleEvt.ownerType = 1;
                    triggerNpcBubbleEvt.bubbleid = bubbleId;
                    triggerNpcBubbleEvt.npcobj = GameCenter.mainHero.gameObject;

                    Sys_HUD.Instance.eventEmitter.Trigger<TriggerNpcBubbleEvt>(Sys_HUD.EEvents.OnTriggerNpcBubble, triggerNpcBubbleEvt);
                }
                else if (cSVBubbleData.Type == (uint)EBubbleType.EmojiText)
                {
                    TriggerExpressionBubbleEvt triggerEmotionBubbleEvt = new TriggerExpressionBubbleEvt();
                    triggerEmotionBubbleEvt.ownerType = 1u;
                    CSVLanguage.Data cSVLanguageData = CSVLanguage.Instance.GetConfData(cSVBubbleData.BubbleText);
                    if (cSVLanguageData != null)
                    {
                        triggerEmotionBubbleEvt.content = cSVLanguageData.words;
                    }
                    else
                    {
                        DebugUtil.LogError($"CSVLanguage.Data is null id:{cSVBubbleData.BubbleText}");
                    }
                    triggerEmotionBubbleEvt.showTime = cSVBubbleData.BubbleTime;
                    triggerEmotionBubbleEvt.gameObject = GameCenter.mainHero.gameObject;
                    triggerEmotionBubbleEvt.bubbleId = bubbleId;

                    Sys_HUD.Instance.eventEmitter.Trigger<TriggerExpressionBubbleEvt>(Sys_HUD.EEvents.OnTriggerExpressionBubble, triggerEmotionBubbleEvt);
                }
                else if (cSVBubbleData.Type == (uint)EBubbleType.Pic)
                {
                    CreateEmotionEvt createEmotionEvt = new CreateEmotionEvt();
                    createEmotionEvt.gameObject = GameCenter.mainHero.gameObject;
                    createEmotionEvt.emtionId = cSVBubbleData.MoodId;
                    createEmotionEvt.actorId = GameCenter.mainHero.uID;
                    Sys_HUD.Instance.eventEmitter.Trigger<CreateEmotionEvt>(Sys_HUD.EEvents.OnCreateEmotion, createEmotionEvt);
                }
            }
            else
            {
                Lib.Core.DebugUtil.LogError($"CSVBubbleData表中没有Id：{bubbleId.ToString()}");
            }
        }

        void CreateNpcBubble(uint bubbleId, uint npcInfoID)
        {
            Sys_HUD.Instance.OpenHud();
            Npc npc;
            CSVBubble.Data cSVBubbleData = CSVBubble.Instance.GetConfData(bubbleId);
            if (cSVBubbleData != null)
            {
                if (cSVBubbleData.Type == (uint)EBubbleType.PureWord)
                {
                    if (GameCenter.uniqueNpcs.TryGetValue(npcInfoID, out npc))
                    {
                        TriggerNpcBubbleEvt triggerNpcBubbleEvt = new TriggerNpcBubbleEvt();
                        triggerNpcBubbleEvt.ownerType = 0;
                        triggerNpcBubbleEvt.npcid = npc.uID;
                        triggerNpcBubbleEvt.npcInfoId = npcInfoID;
                        triggerNpcBubbleEvt.bubbleid = bubbleId;
                        triggerNpcBubbleEvt.npcobj = npc.gameObject;

                        Sys_HUD.Instance.eventEmitter.Trigger<TriggerNpcBubbleEvt>(Sys_HUD.EEvents.OnTriggerNpcBubble, triggerNpcBubbleEvt);
                    }
                }
                else if (cSVBubbleData.Type == (uint)EBubbleType.EmojiText)
                {
                    if (GameCenter.uniqueNpcs.TryGetValue(npcInfoID, out npc))
                    {
                        TriggerExpressionBubbleEvt triggerEmotionBubbleEvt = new TriggerExpressionBubbleEvt();
                        triggerEmotionBubbleEvt.id = npc.uID;
                        triggerEmotionBubbleEvt.npcInfoId = npcInfoID;
                        triggerEmotionBubbleEvt.ownerType = 0;
                        CSVLanguage.Data cSVLanguageData = CSVLanguage.Instance.GetConfData(cSVBubbleData.BubbleText);
                        if (cSVLanguageData != null)
                        {
                            triggerEmotionBubbleEvt.content = cSVLanguageData.words;
                        }
                        else
                        {
                            DebugUtil.LogError($"CSVLanguage.Data is null id:{cSVBubbleData.BubbleText}");
                        }
                        triggerEmotionBubbleEvt.showTime = cSVBubbleData.BubbleTime;
                        triggerEmotionBubbleEvt.gameObject = npc.gameObject;
                        triggerEmotionBubbleEvt.bubbleId = bubbleId;
                        Sys_HUD.Instance.eventEmitter.Trigger<TriggerExpressionBubbleEvt>(Sys_HUD.EEvents.OnTriggerExpressionBubble, triggerEmotionBubbleEvt);
                    }
                }
                else if (cSVBubbleData.Type == (uint)EBubbleType.Pic)
                {
                    if (GameCenter.uniqueNpcs.TryGetValue(npcInfoID, out npc))
                    {
                        CreateEmotionEvt createEmotionEvt = new CreateEmotionEvt();
                        createEmotionEvt.actorId = npc.uID;
                        createEmotionEvt.gameObject = npc.gameObject;
                        createEmotionEvt.emtionId = cSVBubbleData.MoodId;
                        Sys_HUD.Instance.eventEmitter.Trigger<CreateEmotionEvt>(Sys_HUD.EEvents.OnCreateEmotion, createEmotionEvt);
                    }
                }
            }
            else
            {
                Lib.Core.DebugUtil.LogError($"CSVBubbleData表中没有Id：{bubbleId.ToString()}");
            }
        }

        void CheckBackGroundImage(uint backgroundIndex)
        {
            for (int index = 0, len = backgroundImages.Count; index < len; index++)
            {
                if (index == backgroundIndex)
                {
                    backgroundImages[index].gameObject.SetActive(true);
                }
                else
                {
                    backgroundImages[index].gameObject.SetActive(false);
                }
            }
        }

        void Refresh(uint npcNameID, uint contentID, Text titleName, uint backgroundIndex = 0)
        {
            CSVDialogueLanguage.Data cSVDialogueLanguageData = CSVDialogueLanguage.Instance.GetConfData(contentID);
            if (cSVDialogueLanguageData != null)
            {
                AudioUtil.PlayDubbing(cSVDialogueLanguageData.DubbingId, AudioUtil.EAudioType.NPCSound, false);
            }

            titleName.text = LanguageHelper.GetNpcTextContent(npcNameID);
            CheckBackGroundImage(backgroundIndex);
            string finalText = CheckText(contentID);
            if (!string.IsNullOrWhiteSpace(finalText))
            {
                backGroundButton.gameObject.SetActive(false);
                backGroundTextButton.gameObject.SetActive(true);
                tweener.Complete();
                text.text = string.Empty;
                tweener = text.DOText(finalText, charsPerSecond, true);
                tweener.SetSpeedBased(true).onComplete = () =>
                {
                    backGroundButton.gameObject.SetActive(!currentResetDialogueDataEventData.hideNextButton);
                    nextIcon.SetActive(!currentResetDialogueDataEventData.hideNextButton);
                    backGroundTextButton.gameObject.SetActive(false);
                    if (currentResetDialogueDataEventData.autoFlag)
                    {
                        if (Sys_Team.Instance.HaveTeam && !Sys_Team.Instance.isCaptain() && !Sys_Team.Instance.isPlayerLeave())
                        {

                        }
                        else
                        {
                            nextTimer?.Cancel();
                            float cdTime = CSVParam.Instance.UIDialogueAutoCD;
                            nextCD.text = cdTime.ToString();
                            nextTimer = Timer.Register(cdTime, () =>
                            {
                                OnClickNextButton();
                            }, (time) =>
                            {
                                if (nextCD != null)
                                    nextCD.text = ((int)(cdTime - time + 1)).ToString();
                            }, false, true);
                        }
                    }
                };
            }
            else
            {
                backGroundButton.gameObject.SetActive(true);
                backGroundTextButton.gameObject.SetActive(false);
                text.text = string.Empty;
            }

            RefreshUIShowStatus();
        }

        void Refresh(string playerName, uint contentID, Text titleName, uint backgroundIndex = 0)
        {
            CSVDialogueLanguage.Data cSVDialogueLanguageData = CSVDialogueLanguage.Instance.GetConfData(contentID);
            if (cSVDialogueLanguageData != null)
            {
                AudioUtil.PlayDubbing(cSVDialogueLanguageData.DubbingId, AudioUtil.EAudioType.NPCSound, false);
            }

            TextHelper.SetText(titleName, playerName);
            CheckBackGroundImage(backgroundIndex);
            string finalText = CheckText(contentID);
            if (!string.IsNullOrWhiteSpace(finalText))
            {
                backGroundButton.gameObject.SetActive(false);
                backGroundTextButton.gameObject.SetActive(true);
                tweener.Complete();
                text.text = string.Empty;
                tweener = text.DOText(finalText, charsPerSecond, true);
                tweener.SetSpeedBased(true).onComplete = () =>
                {
                    backGroundButton.gameObject.SetActive(!currentResetDialogueDataEventData.hideNextButton);
                    nextIcon.SetActive(!currentResetDialogueDataEventData.hideNextButton);
                    backGroundTextButton.gameObject.SetActive(false);
                    if (currentResetDialogueDataEventData.autoFlag)
                    {
                        if (Sys_Team.Instance.HaveTeam && !Sys_Team.Instance.isCaptain() && !Sys_Team.Instance.isPlayerLeave())
                        {

                        }
                        else
                        {
                            nextTimer?.Cancel();
                            float cdTime = CSVParam.Instance.UIDialogueAutoCD;
                            nextCD.text = cdTime.ToString();
                            nextTimer = Timer.Register(cdTime, () =>
                            {
                                OnClickNextButton();
                            }, (time) =>
                            {
                                if (nextCD != null)
                                    nextCD.text = ((int)(cdTime - time + 1)).ToString();
                            }, false, true);
                        }
                    }
                };
            }
            else
            {
                backGroundButton.gameObject.SetActive(true);
                backGroundTextButton.gameObject.SetActive(false);
                text.text = string.Empty;
            }

            RefreshUIShowStatus();
        }

        string CheckText(uint contentID)
        {
            string textContent = LanguageHelper.GetDialogueLanguageColorWords(contentID);

            string carrerStr = "游民";
            CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career);
            if (cSVCareerData != null)
            {
                CSVLanguage.Data cSVLanguageData = CSVLanguage.Instance.GetConfData(cSVCareerData.name);
                if (cSVLanguageData != null)
                {
                    carrerStr = cSVLanguageData.words;
                }
            }

            if (textContent != null)
            {
                return textContent.Replace("{0}", $"<color=#6fd364>{Sys_Role.Instance.sRoleName}</color>").Replace("{1}", carrerStr);
            }
            return textContent;
        }

        public void OnClickSkipButton()
        {
            try
            {
                if (null != currentResetDialogueDataEventData && !currentResetDialogueDataEventData.holdDialogueOpen)
                {
                    nextTimer?.Cancel();
                    nextTimer = null;
                    if (currentResetDialogueDataEventData != null)
                    {
                        currentResetDialogueDataEventData.Dispose();
                        currentResetDialogueDataEventData = null;
                    }
                    UnloadShowContent();

                    if (tweener != null)
                    {
                        tweener.onComplete = null;
                    }
                    CloseSelf();
                    Sys_Interactive.Instance.OverActing(() =>
                    {
                        GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
                        Sys_Interactive.Instance.ClearInteractiveVirtualActors();
                        callback?.Invoke();
                        callback = null;
                    });
                }
                else
                {
                    Sys_Interactive.Instance.ClearInteractiveVirtualActors();
                    callback?.Invoke();
                    callback = null;
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e.ToString());

                if (!currentResetDialogueDataEventData.holdDialogueOpen)
                {
                    nextTimer?.Cancel();
                    nextTimer = null;
                    if (currentResetDialogueDataEventData != null)
                    {
                        currentResetDialogueDataEventData.Dispose();
                        currentResetDialogueDataEventData = null;
                    }
                    UnloadShowContent();

                    if (tweener != null)
                    {
                        tweener.onComplete = null;
                    }
                    CloseSelf();
                    Sys_Interactive.Instance.OverActing(() =>
                    {
                        GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
                        Sys_Interactive.Instance.ClearInteractiveVirtualActors();
                        callback?.Invoke();
                        callback = null;
                    });
                }
                else
                {
                    nextTimer?.Cancel();
                    nextTimer = null;

                    GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
                    Sys_Interactive.Instance.ClearInteractiveVirtualActors();
                    callback?.Invoke();
                    callback = null;
                }
            }
        }

        void OnClickQuickShowButton()
        {
            backGroundTextButton.gameObject.SetActive(false);
            tweener?.Complete();
        }

        void OnClickNextButton()
        {
            try
            {
                if (null == currentResetDialogueDataEventData)
                {
                    return;
                }

                if (index == (currentResetDialogueDataEventData.datas.Count - 1))
                {
                    if (!currentResetDialogueDataEventData.holdDialogueOpen)
                    {
                        nextTimer?.Cancel();
                        nextTimer = null;
                        if (currentResetDialogueDataEventData != null)
                        {
                            currentResetDialogueDataEventData.Dispose();
                            currentResetDialogueDataEventData = null;
                        }
                        UnloadShowContent();

                        if (tweener != null)
                        {
                            tweener.onComplete = null;
                        }
                        CloseSelf();
                        Sys_Interactive.Instance.OverActing(() =>
                        {
                            Sys_Interactive.Instance.ClearInteractiveVirtualActors();
                            callback?.Invoke();
                            callback = null;
                        });
                    }
                    else
                    {
                        nextTimer?.Cancel();
                        nextTimer = null;

                        Sys_Interactive.Instance.ClearInteractiveVirtualActors();
                        callback?.Invoke();
                        callback = null;
                    }

                    return;
                }
                index++;
                Refresh(currentResetDialogueDataEventData.datas[index]);
                if (Sys_Dialogue.Instance.ShowUIActorFlag)
                {
                    RefreshShowActors(currentResetDialogueDataEventData.datas[index]);
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e.ToString());

                if (null == currentResetDialogueDataEventData)
                {
                    nextTimer?.Cancel();
                    nextTimer = null;
                    GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
                    Sys_Interactive.Instance.ClearInteractiveVirtualActors();
                    callback?.Invoke();
                    callback = null;
                    return;
                }

                if (!currentResetDialogueDataEventData.holdDialogueOpen)
                {
                    nextTimer?.Cancel();
                    nextTimer = null;
                    if (currentResetDialogueDataEventData != null)
                    {
                        currentResetDialogueDataEventData.Dispose();
                        currentResetDialogueDataEventData = null;
                    }
                    UnloadShowContent();

                    if (tweener != null)
                    {
                        tweener.onComplete = null;
                    }
                    CloseSelf();
                    Sys_Interactive.Instance.OverActing(() =>
                    {
                        GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
                        Sys_Interactive.Instance.ClearInteractiveVirtualActors();
                        callback?.Invoke();
                        callback = null;
                    });
                }
                else
                {
                    nextTimer?.Cancel();
                    nextTimer = null;

                    Sys_Interactive.Instance.ClearInteractiveVirtualActors();
                    callback?.Invoke();
                    callback = null;
                }
            }
        }

        uint lastLeftShowCharID = 49999999;
        uint lastRightShowCharID = 49999999;

        void RefreshShowActors(Sys_Dialogue.DialogueDataWrap dialogueDataWrap)
        {
            if (dialogueDataWrap.LeftShowCharID == lastLeftShowCharID && lastLeftShowCharID != 49999999)
            {
                UpdateShowActor(dialogueDataWrap.LeftShowActorType, dialogueDataWrap.LeftShowCharID, dialogueDataWrap.LeftShowStatus, dialogueDataWrap.LeftShowAnimID, true);
            }
            else
            {
                if (dialogueActors.ContainsKey(lastLeftShowCharID))
                {
                    dialogueActors[lastLeftShowCharID].Dispose();
                    dialogueActors.Remove(lastLeftShowCharID);
                }
                CreateShowActor(showSceneControlLeft, dialogueDataWrap.LeftShowActorType, dialogueDataWrap.LeftShowCharID, dialogueDataWrap.LeftShowStatus, dialogueDataWrap.LeftShowAnimID, true);
            }
            lastLeftShowCharID = dialogueDataWrap.LeftShowCharID;

            if (dialogueDataWrap.RightShowCharID == lastRightShowCharID && lastRightShowCharID != 49999999)
            {
                UpdateShowActor(dialogueDataWrap.RightShowActorType, dialogueDataWrap.RightShowCharID, dialogueDataWrap.RightShowStatus, dialogueDataWrap.RightShowAnimID, false);
            }
            else
            {
                if (dialogueActors.ContainsKey(lastRightShowCharID))
                {
                    dialogueActors[lastRightShowCharID].Dispose();
                    dialogueActors.Remove(lastRightShowCharID);
                }
                CreateShowActor(showSceneControlRight, dialogueDataWrap.RightShowActorType, dialogueDataWrap.RightShowCharID, dialogueDataWrap.RightShowStatus, dialogueDataWrap.RightShowAnimID, false);
            }
            lastRightShowCharID = dialogueDataWrap.RightShowCharID;
        }

        void UpdateShowActor(uint actorType, uint charID, uint status, uint animID, bool isLeft)
        {
            if (animID != (uint)EStateType.Idle)
            {
                if (actorType == (uint)EDialogueActorType.Player)
                {
                    if (dialogueActors.ContainsKey(Sys_Role.Instance.Role.HeroId))
                    {
                        if (dialogueActors[Sys_Role.Instance.Role.HeroId].heroLoader != null && dialogueActors[Sys_Role.Instance.Role.HeroId].heroLoader.heroDisplay != null && dialogueActors[Sys_Role.Instance.Role.HeroId].heroLoader.heroDisplay.mAnimation != null && !dialogueActors[Sys_Role.Instance.Role.HeroId].heroLoader.heroDisplay.mAnimation.IsPlaying(animID))
                        {
                            dialogueActors[Sys_Role.Instance.Role.HeroId].heroLoader.heroDisplay.mAnimation.CrossFade(animID, Constants.CORSSFADETIME, () =>
                            {
                                if (dialogueActors != null && dialogueActors.ContainsKey(Sys_Role.Instance.Role.HeroId))
                                {
                                    dialogueActors[Sys_Role.Instance.Role.HeroId].heroLoader.heroDisplay.mAnimation?.CrossFade((uint)EStateType.DialogueIdle, Constants.CORSSFADETIME);
                                }
                            });
                        }
                    }
                }
                else
                {
                    if (dialogueActors.ContainsKey(charID))
                    {
                        if (dialogueActors[charID].displayControl.mAnimation != null && !dialogueActors[charID].displayControl.mAnimation.IsPlaying(animID))
                        {
                            dialogueActors[charID].displayControl.mAnimation.CrossFade(animID, Constants.CORSSFADETIME, () =>
                            {
                                if (dialogueActors != null && dialogueActors.ContainsKey(charID))
                                {
                                    dialogueActors[charID].displayControl.mAnimation.CrossFade((uint)EStateType.DialogueIdle, Constants.CORSSFADETIME);
                                }
                            });
                        }
                    }
                }
            }
            if (status == 0)
            {
                if (isLeft)
                {
                    rawImageLeft.color = new Color(1f, 1f, 1f, 0f);
                }
                else
                {
                    rawImageRight.color = new Color(1f, 1f, 1f, 0f);
                }
            }
            else if (status == 1)
            {
                //变灰
                if (isLeft)
                {
                    rawImageLeft.color = new Color(grayValue, grayValue, grayValue, 1);
                }
                else
                {
                    rawImageRight.color = new Color(grayValue, grayValue, grayValue, 1);
                }
            }
            else if (status == 2)
            {
                //变亮
                if (isLeft)
                {
                    rawImageLeft.color = new Color(1f, 1f, 1f, 1);
                }
                else
                {
                    rawImageRight.color = new Color(1f, 1f, 1f, 1);
                }
            }

            //=====>>
            //ct ：Npc对话关闭多余的RT和绘制                
            RefreshShowScene();
            //=====<<
        }

        void CreateShowActor(ShowSceneControl showSceneControl, uint actorType, uint charID, uint status, uint animID, bool isLeft)
        {
            if (charID == 49999999)
                return;

            if (isLeft && actorType == (uint)EDialogueActorType.Player)
                return;

            DialogueShowActor dialogueActor = new DialogueShowActor();

            string _modelPath = string.Empty;
            if (actorType == (uint)EDialogueActorType.NPC)
            {
                dialogueActor.displayControl = DisplayControl<EHeroModelParts>.Create((int)EHeroModelParts.Count);
                dialogueActor.displayControl.eLayerMask = ELayerMask.ModelShow;
                dialogueActor.CharID = charID;
                var csvNpcData = CSVNpc.Instance.GetConfData(charID);
                if (csvNpcData != null)
                {
                    dialogueActor.ActionID = csvNpcData.action_show_id;
                    dialogueActor.WeaponID = Constants.UMARMEDID;
                    dialogueActor.LeftOffset = new Vector3(csvNpcData.LeftLocationX / 10000f, csvNpcData.LeftLocationY / 10000f, csvNpcData.LeftLocationZ / 10000f);
                    dialogueActor.RightOffset = new Vector3(csvNpcData.RightLocationX / 10000f, csvNpcData.RightLocationY / 10000f, csvNpcData.RightLocationZ / 10000f);
                    dialogueActor.LeftRotOffset = new Vector3(csvNpcData.LeftLocationRotateX / 10000f, csvNpcData.LeftLocationRotateY / 10000f, csvNpcData.LeftLocationRotateZ / 10000f);
                    dialogueActor.RightRotOffset = new Vector3(csvNpcData.RightLocationRotateX / 10000f, csvNpcData.RightLocationRotateY / 10000f, csvNpcData.RightLocationRotateZ / 10000f);
                    dialogueActor.LeftScaleOffset = new Vector3(csvNpcData.LeftLocationMirrorImage / 10000f, 1f, 1f);
                    dialogueActor.RightScaleOffset = new Vector3(csvNpcData.RightLocationMirrorImage / 10000f, 1f, 1f);
                    _modelPath = csvNpcData.model_show;
                }
                else
                {
                    DebugUtil.LogError($"Can not find npcID : {charID}");
                }
            }
            else if (actorType == (uint)EDialogueActorType.Parnter)
            {
                dialogueActor.displayControl = DisplayControl<EHeroModelParts>.Create((int)EHeroModelParts.Count);
                dialogueActor.displayControl.eLayerMask = ELayerMask.ModelShow;

                dialogueActor.CharID = charID;
                var csvPartnerData = CSVPartner.Instance.GetConfData(dialogueActor.CharID);
                if (csvPartnerData != null)
                {
                    dialogueActor.ActionID = dialogueActor.CharID + 100;
                    dialogueActor.WeaponID = Constants.UMARMEDID;
                    dialogueActor.LeftOffset = new Vector3(csvPartnerData.LeftLocationX / 10000f, csvPartnerData.LeftLocationY / 10000f, csvPartnerData.LeftLocationZ / 10000f);
                    dialogueActor.RightOffset = new Vector3(csvPartnerData.RightLocationX / 10000f, csvPartnerData.RightLocationY / 10000f, csvPartnerData.RightLocationZ / 10000f);
                    dialogueActor.LeftRotOffset = new Vector3(csvPartnerData.LeftLocationRotateX / 10000f, csvPartnerData.LeftLocationRotateY / 10000f, csvPartnerData.LeftLocationRotateZ / 10000f);
                    dialogueActor.RightRotOffset = new Vector3(csvPartnerData.RightLocationRotateX / 10000f, csvPartnerData.RightLocationRotateY / 10000f, csvPartnerData.RightLocationRotateZ / 10000f);
                    dialogueActor.LeftScaleOffset = new Vector3(csvPartnerData.LeftLocationMirrorImage / 10000f, 1f, 1f);
                    dialogueActor.RightScaleOffset = new Vector3(csvPartnerData.RightLocationMirrorImage / 10000f, 1f, 1f);
                    _modelPath = csvPartnerData.model_show;
                }
                else
                {
                    DebugUtil.LogError($"Can not find parnterID : {charID}");
                }
            }
            else if (actorType == (uint)EDialogueActorType.Player)
            {
                dialogueActor.heroLoader = HeroLoader.Create(true);

                dialogueActor.CharID = Sys_Role.Instance.Role.HeroId;
                var cSVCharacterData = CSVCharacter.Instance.GetConfData(dialogueActor.CharID);
                if (cSVCharacterData != null)
                {
                    dialogueActor.ActionID = Hero.GetMainHeroHighModelAnimationID();
                    dialogueActor.WeaponID = Constants.UMARMEDID;
                    dialogueActor.LeftOffset = new Vector3(cSVCharacterData.LeftLocationX / 10000f, cSVCharacterData.LeftLocationY / 10000f, cSVCharacterData.LeftLocationZ / 10000f);
                    dialogueActor.RightOffset = new Vector3(cSVCharacterData.RightLocationX / 10000f, cSVCharacterData.RightLocationY / 10000f, cSVCharacterData.RightLocationZ / 10000f);
                    dialogueActor.LeftRotOffset = new Vector3(cSVCharacterData.LeftLocationRotateX / 10000f, cSVCharacterData.LeftLocationRotateY / 10000f, cSVCharacterData.LeftLocationRotateZ / 10000f);
                    dialogueActor.RightRotOffset = new Vector3(cSVCharacterData.RightLocationRotateX / 10000f, cSVCharacterData.RightLocationRotateY / 10000f, cSVCharacterData.RightLocationRotateZ / 10000f);
                    dialogueActor.LeftScaleOffset = new Vector3(cSVCharacterData.LeftLocationMirrorImage / 10000f, 1f, 1f);
                    dialogueActor.RightScaleOffset = new Vector3(cSVCharacterData.RightLocationMirrorImage / 10000f, 1f, 1f);
                    _modelPath = cSVCharacterData.model_show;
                }
                else
                {
                    DebugUtil.LogError($"Can not find CSVCharacterID : {charID}");
                }
            }
            dialogueActor.Status = status;
            if (animID == 0)
                animID = 4114;
            dialogueActor.AnimID = animID;
            dialogueActor.IsLeft = isLeft;

            dialogueActors[dialogueActor.CharID] = dialogueActor;
            dialogueActors[dialogueActor.CharID].onLoadedPlus = OnShowModelLoaded;

            try
            {
                if (dialogueActor != null)
                {
                    if (actorType == (uint)EDialogueActorType.Player)
                    {
                        dialogueActor.heroLoader.showWeapon = false;
                        dialogueActor.heroLoader.LoadHero(Sys_Role.Instance.Role.HeroId, GameCenter.mainHero.weaponComponent.CurWeaponID, ELayerMask.ModelShow, Sys_Fashion.Instance.GetDressData(), (go) =>
                        {
                            dialogueActor.heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).SetParent(showSceneControl.mModelPos, null);
                            dialogueActor.onLoadedPlus?.Invoke(new onLoadedPlusEvt()
                            {
                                index = 0,
                                charID = dialogueActor.CharID,
                                actionID = dialogueActor.ActionID,
                                weaponID = dialogueActor.WeaponID,
                                status = dialogueActor.Status,
                                animID = dialogueActor.AnimID,
                                isLeft = dialogueActor.IsLeft,
                            });
                        });
                    }
                    else
                    {
                        dialogueActor.displayControl.onLoaded += dialogueActor.OnLoadedCallBack;
                        dialogueActor.displayControl.LoadMainModel(EHeroModelParts.Main, _modelPath, EHeroModelParts.None, null);
                        if (showSceneControl != null && showSceneControl.mModelPos != null)
                        {
                            dialogueActor.displayControl.GetPart(EHeroModelParts.Main).SetParent(showSceneControl.mModelPos, null);
                        }
                        else
                        {
                            dialogueActor.displayControl.GetPart(EHeroModelParts.Main).Dispose();
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                DebugUtil.LogError($"{_modelPath} can not find, id: {dialogueActor.CharID}, type: {actorType}");
            }
        }

        protected override void OnClose()
        {
            //UIManager.OpenUI(EUIID.UI_PerForm);
        }
    }
}