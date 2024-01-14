using System.Collections;
using Logic.Core;
using System.Collections.Generic;
using UnityEngine;
using Table;
using UnityEngine.UI;
using Lib.Core;
using System;
using Framework;
using UnityEngine.EventSystems;
using System.Text;

namespace Logic
{
    public class UI_Earser : UIBase
    {
        private Button closeBtn;
        private RawImage petModel_rawImage;
        private RawImage rawImage;
        private Image eventImage;
        private Image areaimage;
        private Text petName;
        private Text series;
        private Text area;
        private Text growthText;
        private Text cardlv;
        private Image cardQuality;
        //private Button active;
        private GameObject actived;
        private EraserArea eraserArea;
        private Animator animator;
        private int brushSize = 60;
        private int brushLerpSize = 10;
        private int rate = 60;

        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;
        private AssetDependencies assetDependencies;

        private CSVPetNew.Data CurrentPetData = null;
        private UI_Pet_AttributeReview uI_Pet_AttributeReview;

        private uint _taskId;
        private uint _gameId;
        private Vector2 offest;
        private Vector3 scale;
        private float m_AutoUseTime;
        private float m_AutoUseTimer;
        private bool b_End;
        private bool b_ActiveReqed;

        protected override void OnOpen(object arg)
        {
            Tuple<uint, object> tuple = arg as Tuple<uint, object>;
            if (tuple != null)
            {
                _taskId = tuple.Item1;
                _gameId = Convert.ToUInt32(tuple.Item2);
            }
        }


        protected override void OnLoaded()
        {
            petModel_rawImage = transform.Find("Animator/View_Message/View_PetNubber/Image_Pet").GetComponent<RawImage>();
            closeBtn = transform.Find("Animator/Button_Off").GetComponent<Button>();
            rawImage = transform.Find("Animator/RawImage").GetComponent<RawImage>();
            eventImage = transform.Find("Animator/View_Message/View_PetNubber/eventImage").GetComponent<Image>();
            petName = transform.Find("Animator/View_Message/View_PetNubber/Image_Title/Text_Name").GetComponent<Text>();
            series = transform.Find("Animator/View_Message/View_PetNubber/Image_Title/Image_Left/Text").GetComponent<Text>();
            area = transform.Find("Animator/View_Message/View_PetNubber/Text_Area/Text_Area2").GetComponent<Text>();
            cardlv = transform.Find("Animator/View_Message/View_PetNubber/Text_Area/Image_No/Text").GetComponent<Text>();
            cardQuality = transform.Find("Animator/View_Message/View_PetNubber/Text_Area/Image_No").GetComponent<Image>();
            growthText = transform.Find("Animator/View_Message/View_PetNubber/Panel_Property/Image_Grow/Text_number").GetComponent<Text>();
            //active = transform.Find("Animator/View_Message/View_PetNubber/Button_Active").GetComponent<Button>();
            actived = transform.Find("Animator/View_Message/View_PetNubber/Image_Actives").gameObject;
            areaimage = transform.Find("Animator/View_Message/View_PetNubber/Image_Title/Image_Left/Image_Left2/Image").GetComponent<Image>();
            animator = transform.Find("Animator/Guide").GetComponent<Animator>();
            assetDependencies = transform.GetComponent<AssetDependencies>();
            uI_Pet_AttributeReview = AddComponent<UI_Pet_AttributeReview>(transform.Find("Animator/View_Message/View_PetNubber/Panel_Property"));

            //active.onClick.AddListener(Active);
            closeBtn.onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_Eraser);
                if (_taskId != 0)
                {
                    Sys_Task.Instance.ReqStepGoalFinishEx(_taskId);
                }
            });
            eraserArea = rawImage.gameObject.AddComponent<EraserArea>();
            rate = (int)CSVNubber.Instance.GetConfData(_gameId).proportion;
            offest = new Vector2(CSVNubber.Instance.GetConfData(_gameId).model_position[0] / 100000f, CSVNubber.Instance.GetConfData(_gameId).model_position[1] / 100000f);
            scale = new Vector3(CSVNubber.Instance.GetConfData(_gameId).zooming / 10000f, CSVNubber.Instance.GetConfData(_gameId).zooming / 10000f, CSVNubber.Instance.GetConfData(_gameId).zooming / 10000f);
            eraserArea.SetData(brushSize, brushLerpSize, rate, EndEraser, StartEraser);
            m_AutoUseTime = (float)(CSVNubber.Instance.GetConfData(_gameId).auto_use);
            m_AutoUseTimer = m_AutoUseTime;
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);

        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnPetActivate, OnActive, toRegister);
            Sys_Net.Instance.eventEmitter.Handle<bool>(Sys_Net.EEvents.OnReconnectResult, OnReconnectResult, toRegister);
        }

        private void OnReconnectResult(bool res)
        {
            UIManager.CloseUI(EUIID.UI_Eraser);
        }

        protected override void OnShow()
        {
            b_End = false;
            CSVNubber.Data cSVNubberData = CSVNubber.Instance.GetConfData(_gameId);
            if (cSVNubberData != null)
            {
                CurrentPetData = CSVPetNew.Instance.GetConfData(CSVNubber.Instance.GetConfData(_gameId).para_id);
            }
            else
            {
                DebugUtil.LogErrorFormat("橡皮擦游戏{0}配置错误", _gameId);
            }
            rawImage.gameObject.SetActive(true);
            actived.SetActive(false);
            eraserArea.InitData();
            RefreshPetData();
            _LoadShowScene();
            _LoadShowModel();
            animator.gameObject.SetActive(true);
            animator.enabled = true;
            animator.Play("Guide", -1, 0);
        }

        private void RefreshPetData()
        {
            if (CurrentPetData == null)
            {
                DebugUtil.LogErrorFormat("宠物id不存在{0}", CSVNubber.Instance.GetConfData(_gameId).para_id);
                return;
            }
            uI_Pet_AttributeReview.SetData(CurrentPetData.id, 0);
            growthText.text = Sys_Pet.Instance.GetGrowthValue(CurrentPetData.id, 0);
            TextHelper.SetText(petName, CurrentPetData.name);
            TextHelper.SetText(cardlv, CurrentPetData.card_lv.ToString());
            ImageHelper.SetIcon(cardQuality, Sys_Pet.Instance.SetPetQuality(CurrentPetData.card_type));
            //string areaSrc = string.Empty;
          
            List<uint> data = new List<uint>();
            if (CurrentPetData.activity != null)
            {
                data.AddRange(CurrentPetData.activity);
            }
            if (data.Count > 0)
            {
                StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
                for (int i = 0; i < data.Count; i++)
                {
                    string des = i == data.Count - 1 ? LanguageHelper.GetTextContent(data[i]) : LanguageHelper.GetTextContent(data[i]) + "、";
                    stringBuilder.Append(des);
                }
                string rlt = StringBuilderPool.ReleaseTemporaryAndToString(stringBuilder);
                TextHelper.SetText(area, rlt);
            }

            CSVGenus.Data cSVGenusData = CSVGenus.Instance.GetConfData(CurrentPetData.race);
            if (cSVGenusData != null)
            {
                TextHelper.SetText(series, cSVGenusData.rale_name);
                ImageHelper.SetIcon(areaimage, cSVGenusData.rale_icon);
            }
            else
            {
                DebugUtil.LogErrorFormat("cSVGenusData  id不存在{0}", CurrentPetData.race);
            }
            bool isPetActive = Sys_Pet.Instance.GetPetIsActive(CurrentPetData.id);
            if (isPetActive)
            {
                OnActive();
            }
            else
            {
                closeBtn.gameObject.SetActive(false);
                actived.SetActive(false);
                //active.gameObject.SetActive(true);
            }
        }

        protected override void OnHide()
        {
            eraserArea.Dispose();
            _UnloadShowContent();
        }

        protected override void OnUpdate()
        {
            m_AutoUseTimer -= deltaTime;
            if (m_AutoUseTimer <= 0 && !b_End)
            {
                m_AutoUseTimer = m_AutoUseTime;
                EndEraser();
                b_End = true;
                animator.enabled = false;
                animator.gameObject.SetActive(false);
                Active();
            }
        }


        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(sceneModel);
            //设置RenderTexture纹理到RawImage
            petModel_rawImage.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);

            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                petDisplay.onLoaded = OnShowModelLoaded;
            }
            DebugUtil.Log(ELogType.eModelShow, "_LoadShowScene");
        }

        private void _LoadShowModel()
        {
            DebugUtil.Log(ELogType.eModelShow, CurrentPetData.id.ToString());
            string _modelPath = CurrentPetData.model_show;
            petDisplay.eLayerMask = ELayerMask.ModelShow;
            petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);
            petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            showSceneControl.mModelPos.transform.Rotate(new Vector3(CurrentPetData.angle1, CurrentPetData.angle2, CurrentPetData.angle3));
            showSceneControl.mModelPos.transform.localScale = new Vector3(CurrentPetData.size, CurrentPetData.size, CurrentPetData.size);
            showSceneControl.mModelPos.transform.localPosition = new Vector3(showSceneControl.mModelPos.transform.localPosition.x + CurrentPetData.translation, CurrentPetData.height,
                showSceneControl.mModelPos.transform.localPosition.z);
            showSceneControl.mModelPos.transform.localPosition += new Vector3(offest.x, offest.y, 0);
            showSceneControl.mModelPos.transform.localScale = scale;
            DebugUtil.Log(ELogType.eModelShow, "_LoadShowModel");
        }


        private void OnShowModelLoaded(int obj)
        {
            DebugUtil.Log(ELogType.eModelShow, "onShowModelLoaded");
            if (obj == 0)
            {
                //uint highId = CurrentPetData.id;
                //petDisplay.mAnimation.UpdateHoldingAnimations(CurrentPetData.action_id_show, CurrentPetData.weapon, Constants.PetShowAnimationClip);
            }
        }


        private void _UnloadShowContent()
        {
            petModel_rawImage.texture = null;
            //petDisplay?.Dispose();
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            showSceneControl?.Dispose();
        }


        private void StartEraser()
        {
            animator.enabled = false;
            animator.gameObject.SetActive(false);
        }

        private void EndEraser()
        {
            eraserArea.Dispose();
            rawImage.gameObject.SetActive(false);
            Active();
            closeBtn.gameObject.SetActive(true);
        }

        public void OnDrag(BaseEventData eventData)
        {
            PointerEventData ped = eventData as PointerEventData;
            Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
            AddEulerAngles(angle);
        }

        public void AddEulerAngles(Vector3 angle)
        {
            Vector3 ilrTemoVector3 = angle;
            if (showSceneControl.mModelPos.transform != null)
            {
                showSceneControl.mModelPos.transform.Rotate(ilrTemoVector3.x, ilrTemoVector3.y, ilrTemoVector3.z);
            }
        }

        private void Active()
        {
            if (b_ActiveReqed)
            {
                return;
            }
            b_ActiveReqed = true;
            Sys_Pet.Instance.OnPetActivateReq(CurrentPetData.id);
            petDisplay.mAnimation.UpdateHoldingAnimations(CurrentPetData.action_id_show, CurrentPetData.weapon, Constants.PetShowAnimationClipHashSet);
        }

        private void OnActive()
        {
            //active.gameObject.SetActive(false);
            actived.SetActive(true);
            //closeBtn.gameObject.SetActive(true);
        }

        public class UI_PetReodel_AttrReview : UIComponent
        {
            private uint attrid;
            private uint petid;
            private uint remodelcount;
            private float maxgrade;
            private float maxgradeLow;
            private Text attrname;
            private GameObject stargo;

            public UI_PetReodel_AttrReview(Transform tran, uint _attrid) : base()
            {
                attrid = _attrid;
                Init(tran);
            }

            protected override void Loaded()
            {
                attrname = transform.Find("Text").GetComponent<Text>();
                stargo = transform.Find("Grid_Grade/Image_GradeBG01").gameObject;
            }

            public void RefreshShow(uint _petId, uint _remodelcount)
            {
                attrname.text = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(attrid).name);

                petid = _petId;
                remodelcount = _remodelcount;
                GetMaxGrade();
                AddStar();
            }

            private void GetMaxGrade()
            {
                CSVPetNew.Data petnew = CSVPetNew.Instance.GetConfData(petid);
                if (null != petnew)
                {
                    if (attrid == 5)
                    {
                        maxgrade = petnew.endurance;
                    }
                    else if (attrid == 7)
                    {
                        maxgrade = petnew.strength;
                    }
                    else if (attrid == 9)
                    {
                        maxgrade = petnew.strong;
                    }
                    else if (attrid == 11)
                    {
                        maxgrade = petnew.speed;
                    }
                    else if (attrid == 13)
                    {
                        maxgrade = petnew.magic;
                    }
                    maxgradeLow = petnew.max_lost_gear;
                }
            }

            private void AddStar()
            {
                float minValue = maxgrade;
                if (maxgradeLow != 0)
                {
                    minValue = maxgrade - Math.Min(Sys_Pet.Instance.MaxGradeLo, maxgradeLow);

                }
                stargo.SetActive(true);
                for (int i = 0; i < stargo.transform.parent.transform.childCount; i++)
                {
                    if (i >= 1) GameObject.Destroy(stargo.transform.parent.transform.GetChild(i).gameObject);
                }
                float fullStar = Mathf.Floor((maxgrade + 4) / 10);
                bool haveCut = 0 < (maxgrade % 10) && (maxgrade % 10) < 6;

                for (int i = 0; i < fullStar; ++i)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(stargo, stargo.transform.parent);
                    go.transform.GetChild(0).gameObject.SetActive(true);
                    go.transform.GetChild(0).GetComponent<Image>().fillAmount = 1;
                }
                if (haveCut)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(stargo, stargo.transform.parent);
                    go.transform.GetChild(0).gameObject.SetActive(true);
                    go.transform.GetChild(0).GetComponent<Image>().fillAmount = 0.5f;
                }
                stargo.SetActive(false);
            }
        }
        public class UI_Pet_AttributeReview : UIComponent
        {
            private uint currentPetId;
            private uint remodelCount;
            private List<UI_PetReodel_AttrReview> uI_PetReodel_AttrReviews = new List<UI_PetReodel_AttrReview>();
            protected override void Loaded()
            {
                uI_PetReodel_AttrReviews.Clear();
                uI_PetReodel_AttrReviews.Add(new UI_PetReodel_AttrReview(transform.Find("Image_Con"), 5));
                uI_PetReodel_AttrReviews.Add(new UI_PetReodel_AttrReview(transform.Find("Image_Pow"), 7));
                uI_PetReodel_AttrReviews.Add(new UI_PetReodel_AttrReview(transform.Find("Image_Str"), 9));
                uI_PetReodel_AttrReviews.Add(new UI_PetReodel_AttrReview(transform.Find("Image_Dex"), 11));
                uI_PetReodel_AttrReviews.Add(new UI_PetReodel_AttrReview(transform.Find("Image_Mp"), 13));
            }

            public override void SetData(params object[] arg)
            {
                if (arg.Length >= 2)
                {
                    currentPetId = (uint)arg[0];
                    remodelCount = System.Convert.ToUInt32(arg[1]);
                }
                RefreshValue();
            }

            public void RefreshValue()
            {
                for (int i = 0; i < uI_PetReodel_AttrReviews.Count; i++)
                {
                    uI_PetReodel_AttrReviews[i].RefreshShow(currentPetId, remodelCount);
                }
            }
        }
    }
}


