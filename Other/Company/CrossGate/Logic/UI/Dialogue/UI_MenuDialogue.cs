using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Framework;
using System;

namespace Logic
{     
    public class Sys_MenuDialogue : SystemModuleBase<Sys_MenuDialogue>
    {
        public class ResetMenuDialogueDataEventData
        {
            public List<MenuDialogueDataWrap> datas;

            public void Dispose()
            {
                datas?.Clear();
                datas = null;
                PoolManager.Recycle(this);
            }

            public void Init(List<MenuDialogueDataWrap> _datas)
            {
                datas = _datas;
            }
        }

        public struct MenuDialogueDataWrap
        {
            public uint ActorType;
            public uint ActorNameID;
            public uint ContentID;
            public uint ShowCharID;
            public uint ShowActorAnimID;
            public bool NeedShowActorAndTitle;
            
            public void Dispose()
            {
                ActorType = 0;
                ActorNameID = 0;
                ContentID = 0;
                ShowCharID = 49999999;
                ShowActorAnimID = 0;
                NeedShowActorAndTitle = true;
            }
        }

        public ResetMenuDialogueDataEventData currentResetMenuDialogueDataEventData;

        public enum EEvents
        {
            OnResetMenuDialogueData,
        }

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public static List<MenuDialogueDataWrap> GetMenuDialogueDataWrap(CSVInterfaceBubble.Data cSVInterfaceBubbleData)
        {
            List<MenuDialogueDataWrap> datas = new List<MenuDialogueDataWrap>();
            for (int index = 0, len = cSVInterfaceBubbleData.ModleId.Count; index < len; index++)
            {
                //TODO:结构体将原先的池去掉直接创建
                MenuDialogueDataWrap data = new MenuDialogueDataWrap();
                data.ActorType = cSVInterfaceBubbleData.ModleId[index][0];
                if (data.ActorType == (uint)EDialogueActorType.NPC)
                {
                    CSVNpc.Data cSVNpcData = CSVNpc.Instance.GetConfData(cSVInterfaceBubbleData.ModleId[index][1]);
                    if (cSVNpcData != null)
                    {
                        data.ShowCharID = cSVNpcData.id;
                        data.ActorNameID = cSVNpcData.name;
                    }
                    else
                    {
                        DebugUtil.LogError($"cSVNpcData is null, id: {cSVInterfaceBubbleData.ModleId[index][1]}");
                    }
                }
                else if (data.ActorType == (uint)EDialogueActorType.Parnter)
                {
                    CSVPartner.Data cSVPartnerData = CSVPartner.Instance.GetConfData(cSVInterfaceBubbleData.ModleId[index][1]);
                    if (cSVPartnerData != null)
                    {
                        data.ShowCharID = cSVPartnerData.id;
                        data.ActorNameID = cSVPartnerData.name;
                    }
                    else
                    {
                        DebugUtil.LogError($"CSVPartner.Data is null, id: {cSVInterfaceBubbleData.ModleId[index][1]}");
                    }
                }
                if (cSVInterfaceBubbleData.ModleId[index][2] == 1)
                {
                    data.NeedShowActorAndTitle = true;
                }
                else
                {
                    data.NeedShowActorAndTitle = false;
                }
                data.ShowActorAnimID = cSVInterfaceBubbleData.ModleId[index][3];
                data.ContentID = cSVInterfaceBubbleData.ModleId[index][4];

                datas.Add(data);
            }

            return datas;
        }

        public void OpenMenuDialogue(ResetMenuDialogueDataEventData resetMenuDialogueDataEventData)
        {
            currentResetMenuDialogueDataEventData = resetMenuDialogueDataEventData;
            if (!UIManager.IsOpen(EUIID.UI_MenuDialogue))
            {
                UIManager.OpenUI(EUIID.UI_MenuDialogue, true, resetMenuDialogueDataEventData);
            }
            else
            {
                eventEmitter.Trigger<ResetMenuDialogueDataEventData>(EEvents.OnResetMenuDialogueData, currentResetMenuDialogueDataEventData);
            }
        }

        public override void Dispose()
        {
            currentResetMenuDialogueDataEventData = null;

            base.Dispose();
        }

        public override void OnLogin()
        {
            base.OnLogin();

            cacheActivedNpcs.Clear();
        }

        public override void OnLogout()
        {
            base.OnLogout();

            cacheActivedNpcs.Clear();
        }

        List<ulong> cacheActivedNpcs = new List<ulong>();

        public void AddActiveNpc(ulong npcUID)
        {
            cacheActivedNpcs.Add(npcUID);
        }

        public bool CanActivedNpcFunction(ulong npcUID)
        {
            return !cacheActivedNpcs.Contains(npcUID);
        }
    }

    public class UI_MenuDialogue : UIBase
    {
        public class onLoadedPlusEvt
        {
            public int index;
            public uint charID;
            public uint actionID;
            public uint weaponID;
            public uint animID;
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
                    animID = AnimID
                });
            }

            public uint CharID { get; set; }
            public uint ActionID { get; set; }
            public uint WeaponID { get; set; }
            public uint AnimID { get; set; }
            public Vector3 Offset { get; set; }
            public Vector3 RotOffset { get; set; }
            public Vector3 scaleOffset { get; set; }

            public Action<onLoadedPlusEvt> onLoadedPlus;

            public void SetTransformOffset()
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

                if (null == obj)
                    return;

                obj.transform.localPosition += Offset;
                obj.transform.localEulerAngles += RotOffset;
                obj.transform.localScale = new Vector3(obj.transform.localScale.x * scaleOffset.x, obj.transform.localScale.y * scaleOffset.y, obj.transform.localScale.z * scaleOffset.z);
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
        }

        Button backGroundButton;
        Button backGroundTextButton;
        GameObject titleLeftRoot;
        Text titleNameLeft;
        Text text;
        int index = 0;
        Sys_MenuDialogue.ResetMenuDialogueDataEventData currentResetMenuDialogueDataEventData;
        Timer nextTimer;
        public AssetDependencies assetDependencies;
        ShowSceneControl showSceneControl;
        RawImage rawImage;
        TypewriterEffect typewriterEffect;
        Dictionary<uint, DialogueShowActor> dialogueActors = new Dictionary<uint, DialogueShowActor>();
        uint lastShowCharID = 49999999;

        protected override void ProcessEvents(bool toRegister)
        {            
            Sys_MenuDialogue.Instance.eventEmitter.Handle<Sys_MenuDialogue.ResetMenuDialogueDataEventData>(Sys_MenuDialogue.EEvents.OnResetMenuDialogueData, OnResetMenuDialogueData, toRegister);
        }

        protected override void OnLoaded()
        {         
            assetDependencies = gameObject.GetComponent<AssetDependencies>();

            backGroundButton = gameObject.FindChildByName("BackGround").GetComponent<Button>();
            backGroundButton.onClick.AddListener(OnClickNextButton);
            backGroundTextButton = gameObject.FindChildByName("BackGroundText").GetComponent<Button>();
            backGroundTextButton.onClick.AddListener(OnClickQuickShowButton);

            titleLeftRoot = gameObject.FindChildByName("Image2");
            titleNameLeft = gameObject.FindChildByName("Text_Name").GetComponent<Text>();
            text = gameObject.FindChildByName("Text").GetComponent<Text>();
            text.text = string.Empty;
            typewriterEffect = text.gameObject.GetNeedComponent<TypewriterEffect>();
            typewriterEffect.charsPerSecond = int.Parse(CSVParam.Instance.GetConfData(127).str_value);

            rawImage = gameObject.FindChildByName("RawImage").GetComponent<RawImage>();
        }

        protected override void OnOpen(object arg)
        {            
            currentResetMenuDialogueDataEventData = arg as Sys_MenuDialogue.ResetMenuDialogueDataEventData;
        }

        protected override void OnShow()
        {         
            UnloadShowContent();
            LoadShowScene();

            if (currentResetMenuDialogueDataEventData != null)
            {
                OnResetMenuDialogueData(currentResetMenuDialogueDataEventData);
            }
        }

        protected override void OnHide()
        {
            nextTimer?.Cancel();
            nextTimer = null;

            UnloadShowContent();

            if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.Auto && Sys_Task.Instance.currentTaskId != 0)
            {
                Sys_Task.Instance.TryContinueDoCurrentTask();
            }
        }

        void UnloadShowContent()
        {
            rawImage.texture = null;
            rawImage.color = new Color(1f, 1f, 1f, 0f);
            foreach (var dialogueActor in dialogueActors.Values)
            {
                dialogueActor.Dispose();
            }
            dialogueActors.Clear();
            showSceneControl?.Dispose();
            lastShowCharID = 49999999;
        }

        void LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(sceneModel);
            showSceneControl.mRoot.name = "UI_MenuDialogue_showSceneControl";

            rawImage.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);
        }

        void OnResetMenuDialogueData(Sys_MenuDialogue.ResetMenuDialogueDataEventData resetMenuDialogueDataEventData)
        {
            index = 0;
            currentResetMenuDialogueDataEventData = resetMenuDialogueDataEventData;

            if (currentResetMenuDialogueDataEventData.datas.Count > 0)
            {
                Refresh(currentResetMenuDialogueDataEventData.datas[0]);
                RefreshShowActor(currentResetMenuDialogueDataEventData.datas[0]);
            }

            if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.Auto && Sys_Task.Instance.currentTaskId != 0)
            {
                Sys_Task.Instance.TryContinueDoCurrentTask();
            }
        }

        void Refresh(Sys_MenuDialogue.MenuDialogueDataWrap menuDialogueDataWrap)
        {
            if (menuDialogueDataWrap.ActorNameID != (uint)EDialogueActorType.Player)
            {
                Refresh(menuDialogueDataWrap.ActorNameID, menuDialogueDataWrap.ContentID, menuDialogueDataWrap.NeedShowActorAndTitle);
            }
            else
            {
                Refresh(Sys_Role.Instance.Role.Name.ToStringUtf8(), menuDialogueDataWrap.ContentID, menuDialogueDataWrap.NeedShowActorAndTitle);
            }
        }

        void Refresh(uint npcNameID, uint contentID, bool showFlag)
        {
            titleLeftRoot.gameObject.SetActive(showFlag);
            TextHelper.SetText(titleNameLeft, LanguageHelper.GetNpcTextContent(npcNameID));
            string finalText = LanguageHelper.GetLanguageColorWordsFormat(contentID);
            if (!string.IsNullOrWhiteSpace(finalText))
            {
                backGroundButton.gameObject.SetActive(false);
                backGroundTextButton.gameObject.SetActive(true);
                typewriterEffect.WordByWord(finalText);
                typewriterEffect.onFinished.AddListener(() =>
                {
                    backGroundButton.gameObject.SetActive(true);
                    backGroundTextButton.gameObject.SetActive(false);

                    nextTimer?.Cancel();
                    nextTimer = Timer.Register(CSVParam.Instance.UIMenuDialogueAutoCD, () =>
                    {
                        OnClickNextButton();
                    }, null, false, true);
                });
            }
            else
            {
                backGroundButton.gameObject.SetActive(true);
                backGroundTextButton.gameObject.SetActive(false);
                text.text = string.Empty;
            }
        }

        void Refresh(string playerName, uint contentID, bool showFlag)
        {
            titleLeftRoot.gameObject.SetActive(showFlag);
            TextHelper.SetText(titleNameLeft, playerName);
            string finalText = LanguageHelper.GetLanguageColorWordsFormat(contentID);
            if (!string.IsNullOrWhiteSpace(finalText))
            {
                backGroundButton.gameObject.SetActive(false);
                backGroundTextButton.gameObject.SetActive(true);
                typewriterEffect.WordByWord(finalText);
                typewriterEffect.onFinished.AddListener(() =>
                {
                    backGroundButton.gameObject.SetActive(true);
                    backGroundTextButton.gameObject.SetActive(false);

                    nextTimer?.Cancel();
                    nextTimer = Timer.Register(CSVParam.Instance.UIMenuDialogueAutoCD, () =>
                    {
                        OnClickNextButton();
                    }, null, false, true);
                });
            }
            else
            {
                backGroundButton.gameObject.SetActive(true);
                backGroundTextButton.gameObject.SetActive(false);
                text.text = string.Empty;
            }
        }

        void RefreshShowActor(Sys_MenuDialogue.MenuDialogueDataWrap menuDialogueDataWrap)
        {
            if (!menuDialogueDataWrap.NeedShowActorAndTitle)
            {
                if (dialogueActors.ContainsKey(lastShowCharID))
                {
                    dialogueActors[lastShowCharID].Dispose();
                    dialogueActors.Remove(lastShowCharID);
                    lastShowCharID = 49999999;
                }
                return;
            }

            if (menuDialogueDataWrap.ShowCharID == lastShowCharID && lastShowCharID != 49999999)
            {
                UpdateShowActor(menuDialogueDataWrap.ActorType, menuDialogueDataWrap.ShowCharID, menuDialogueDataWrap.ActorNameID);
            }
            else
            {
                if (dialogueActors.ContainsKey(lastShowCharID))
                {
                    dialogueActors[lastShowCharID].Dispose();
                    dialogueActors.Remove(lastShowCharID);
                }
                CreateShowActor(showSceneControl, menuDialogueDataWrap.ActorType, menuDialogueDataWrap.ShowCharID, menuDialogueDataWrap.ShowActorAnimID);
            }          
        }

        void UpdateShowActor(uint actorType, uint charID, uint animID)
        {
            if (animID != (uint)EStateType.Idle)
            {
                if (actorType == (uint)EDialogueActorType.Player)
                {
                    if (dialogueActors.ContainsKey(Sys_Role.Instance.Role.HeroId))
                    {
                        if (!dialogueActors[Sys_Role.Instance.Role.HeroId].heroLoader.heroDisplay.mAnimation.IsPlaying(animID))
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
                        if (!dialogueActors[charID].displayControl.mAnimation.IsPlaying(animID))
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
            lastShowCharID = charID;
        }

        void CreateShowActor(ShowSceneControl showSceneControl, uint actorType, uint charID, uint animID)
        {
            //if (charID == 49999999)
            //    return;

            DialogueShowActor dialogueShowActor = new DialogueShowActor();

            string _modelPath = string.Empty;
            if (actorType == (int)EDialogueActorType.NPC)
            {
                dialogueShowActor.displayControl = DisplayControl<EHeroModelParts>.Create((int)EHeroModelParts.Count);
                dialogueShowActor.displayControl.eLayerMask = ELayerMask.ModelShow;
                dialogueShowActor.CharID = charID;
                var csvNpcData = CSVNpc.Instance.GetConfData(charID);
                if (csvNpcData != null)
                {
                    dialogueShowActor.ActionID = csvNpcData.action_show_id;
                    dialogueShowActor.WeaponID = Constants.UMARMEDID;
                    _modelPath = csvNpcData.model_show;

                    dialogueShowActor.Offset = new Vector3(csvNpcData.BubbleLocationX / 10000f, csvNpcData.BubbleLocationY / 10000f, csvNpcData.BubbleLocationZ / 10000f);
                    dialogueShowActor.RotOffset = new Vector3(csvNpcData.BubbleLocationRotateX / 10000f, csvNpcData.BubbleLocationRotateY / 10000f, csvNpcData.BubbleLocationRotateZ / 10000f);
                    dialogueShowActor.scaleOffset = new Vector3(csvNpcData.BubbleLocationMirrorImage / 10000f, 1f, 1f);
                }
                else
                {
                    DebugUtil.LogError($"Can not find npcID : {charID}");
                }
            }
            else if (actorType == (uint)EDialogueActorType.Parnter)
            {
                dialogueShowActor.displayControl = DisplayControl<EHeroModelParts>.Create((int)EHeroModelParts.Count);
                dialogueShowActor.displayControl.eLayerMask = ELayerMask.ModelShow;
                dialogueShowActor.CharID = charID;
                var csvPartnerData = CSVPartner.Instance.GetConfData(dialogueShowActor.CharID);
                if (csvPartnerData != null)
                {
                    dialogueShowActor.ActionID = dialogueShowActor.CharID + 100;
                    dialogueShowActor.WeaponID = Constants.UMARMEDID;
                    _modelPath = csvPartnerData.model_show;

                    dialogueShowActor.Offset = new Vector3(csvPartnerData.BubbleLocationX / 10000f, csvPartnerData.BubbleLocationY / 10000f, csvPartnerData.BubbleLocationZ / 10000f);
                    dialogueShowActor.RotOffset = new Vector3(csvPartnerData.BubbleLocationRotateX / 10000f, csvPartnerData.BubbleLocationRotateY / 10000f, csvPartnerData.BubbleLocationRotateZ / 10000f);
                    dialogueShowActor.scaleOffset = new Vector3(csvPartnerData.BubbleLocationMirrorImage / 10000f, 1f, 1f);
                }
                else
                {
                    DebugUtil.LogError($"Can not find parnterID : {charID}");
                }
            }
            else if (actorType == (uint)EDialogueActorType.Player)
            {
                dialogueShowActor.heroLoader = HeroLoader.Create(true);
                dialogueShowActor.CharID = Sys_Role.Instance.Role.HeroId;

                var cSVCharacterData = CSVCharacter.Instance.GetConfData(dialogueShowActor.CharID);
                if (cSVCharacterData != null)
                {
                    dialogueShowActor.ActionID = Hero.GetMainHeroHighModelAnimationID();
                    dialogueShowActor.WeaponID = Constants.UMARMEDID;
                    _modelPath = cSVCharacterData.model_show;

                    dialogueShowActor.Offset = new Vector3(cSVCharacterData.BubbleLocationX / 10000f, cSVCharacterData.BubbleLocationY / 10000f, cSVCharacterData.BubbleLocationZ / 10000f);
                    dialogueShowActor.RotOffset = new Vector3(cSVCharacterData.BubbleLocationRotateX / 10000f, cSVCharacterData.BubbleLocationRotateY / 10000f, cSVCharacterData.BubbleLocationRotateZ / 10000f);
                    //dialogueShowActor.scaleOffset = new Vector3(cSVCharacterData.BubbleLocationMirrorImage / 10000f, 1f, 1f);
                    dialogueShowActor.scaleOffset = Vector3.one;
                }
                else
                {
                    DebugUtil.LogError($"Can not find CSVCharacterID : {charID}");
                }
            }
            dialogueShowActor.AnimID = animID;

            dialogueActors[dialogueShowActor.CharID] = dialogueShowActor;
            dialogueActors[dialogueShowActor.CharID].onLoadedPlus = OnShowModelLoaded;
            lastShowCharID = dialogueShowActor.CharID;
            try
            {
                if (dialogueShowActor != null)
                {
                    if (actorType == (uint)EDialogueActorType.Player)
                    {
                        dialogueShowActor.heroLoader.showWeapon = false;
                        dialogueShowActor.heroLoader.LoadHero(Sys_Role.Instance.Role.HeroId, GameCenter.mainHero.weaponComponent.CurWeaponID, ELayerMask.ModelShow, Sys_Fashion.Instance.GetDressData(), (go) =>
                        {
                            dialogueShowActor.heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).SetParent(showSceneControl.mModelPos, null);
                            dialogueShowActor.onLoadedPlus?.Invoke(new onLoadedPlusEvt()
                            {
                                index = 0,
                                charID = dialogueShowActor.CharID,
                                actionID = dialogueShowActor.ActionID,
                                weaponID = dialogueShowActor.WeaponID,
                                animID = dialogueShowActor.AnimID,
                            });
                        });
                    }
                    else
                    {
                        dialogueShowActor.displayControl.onLoaded += dialogueShowActor.OnLoadedCallBack;
                        dialogueShowActor.displayControl.LoadMainModel(EHeroModelParts.Main, _modelPath, EHeroModelParts.None, null);
                        if (showSceneControl != null && showSceneControl.mModelPos != null)
                        {
                            dialogueShowActor.displayControl.GetPart(EHeroModelParts.Main).SetParent(showSceneControl.mModelPos, null);
                        }
                        else
                        {
                            dialogueShowActor.displayControl.GetPart(EHeroModelParts.Main).Dispose();
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                DebugUtil.LogError($"{_modelPath} can not find, id: {dialogueShowActor.CharID}, type: {actorType}");
            }
        }

        void OnShowModelLoaded(onLoadedPlusEvt evt)
        {
            if (evt.index == 0)
            {
                GameObject go = dialogueActors[evt.charID].GetGameObject(0);
                go?.SetActive(false);

                if (dialogueActors[evt.charID].heroLoader != null)
                {
                    dialogueActors[evt.charID].heroLoader.heroDisplay.mAnimation.UpdateHoldingAnimations(evt.actionID, evt.weaponID, Constants.IdleAnimationClipHashSet, (EStateType)evt.animID, go);
                }
                else if (dialogueActors[evt.charID].displayControl != null)
                {
                    dialogueActors[evt.charID].displayControl.mAnimation.UpdateHoldingAnimations(evt.actionID, evt.weaponID, Constants.IdleAnimationClipHashSet, (EStateType)evt.animID, go);
                }

                rawImage.color = Color.white;

                dialogueActors[evt.charID].SetTransformOffset();
            }
        }

        void OnClickNextButton()
        {
            try
            {
                if (null == currentResetMenuDialogueDataEventData)
                    return;

                if (index == (currentResetMenuDialogueDataEventData.datas.Count - 1))
                {
                    nextTimer?.Cancel();
                    nextTimer = null;

                    currentResetMenuDialogueDataEventData?.Dispose();
                    currentResetMenuDialogueDataEventData = null;

                    UnloadShowContent();

                    typewriterEffect.onFinished.RemoveAllListeners();
                    CloseSelf();

                    return;
                }

                index++;
                Refresh(currentResetMenuDialogueDataEventData.datas[index]);
                RefreshShowActor(currentResetMenuDialogueDataEventData.datas[index]);
            }
            catch (System.Exception e)
            {
                nextTimer?.Cancel();
                nextTimer = null;

                currentResetMenuDialogueDataEventData?.Dispose();
                currentResetMenuDialogueDataEventData = null;

                UnloadShowContent();

                typewriterEffect.onFinished.RemoveAllListeners();
                CloseSelf();
            }
        }

        void OnClickQuickShowButton()
        {
            backGroundTextButton.gameObject.SetActive(false);
            typewriterEffect.QuickShow();
        }
    }
}
