using Framework;
using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class PetBookListPar
    {
        public List<uint> showPetList;
        public uint petId;
        public bool showChangeBtn
        {
            get;
            set;
        } = true;
        public EPetReviewViewType eviewType
        {
            get;
            set;
        } = EPetReviewViewType.Book;

        public EPetBookPageType ePetReviewPageType
        {
            get;
            set;
        } = EPetBookPageType.Seal;
    }
    public enum EPetBookPageType
    {
        Seal,
        Friend,
        BackGroud,
    }

    public enum EPetReviewViewType
    {
        Book,
        Friend,
    }

    public enum ESkillClientType
    {
        Noral,
        Unique,//专属
        Build,//改造
        Mount,//骑术技能
        DemonSpiritSkill,//魔魂技能技能
    }

    public class SkillClientEx
    {
        public uint skillId;
        public ESkillClientType eSkillOutType = ESkillClientType.Noral;
        public SkillClientEx(uint _skillId, ESkillClientType _eSkillOutType)
        {
            skillId = _skillId;
            eSkillOutType = _eSkillOutType;
        }
    }

    public class UI_Pet_BookReview_Layout
    {
        public Transform transform;
        public Button closeButton;

        public Button leftBtn;
        public Button rightBtn;

        public GameObject leftView;
        public GameObject rightView;
        
        public void Init(Transform transform)
        {
            this.transform = transform;
            closeButton = transform.Find("Animator/View_FullTitle02_New/Btn_Close").GetComponent<Button>();

            leftView = transform.Find("Animator/View_Left").gameObject;
            rightView = transform.Find("Animator/View_Messag/View_Right").gameObject;

            leftBtn = transform.Find("Animator/View_Left/Btn_Left").GetComponent<Button>();
            rightBtn = transform.Find("Animator/View_Left/Btn_Right").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnCloseBtnClicked);
            leftBtn.onClick.AddListener(listener.OnLeftClicked);
            rightBtn.onClick.AddListener(listener.OnRightClicked);
        }

        public interface IListener
        {
            void OnCloseBtnClicked();
            void OnLeftClicked();
            void OnRightClicked();
        }
    }

    public class UI_Pet_BookReview : UIBase, UI_Pet_BookReview_Layout.IListener
    {
        private UI_Pet_BookReview_Layout layout = new UI_Pet_BookReview_Layout();
        private UI_Pet_BookReview_LeftView leftview;
        private UI_Pet_BookReview_RightView rightView;
        private List<uint> petList;
        private int index;
        private uint currentShowPetId;
        private PetBookListPar petBookListPar;
        private EPetReviewViewType ePetReviewViewType = EPetReviewViewType.Book;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            leftview = AddComponent<UI_Pet_BookReview_LeftView>(layout.leftView.transform);
            rightView = AddComponent<UI_Pet_BookReview_RightView>(layout.rightView.transform);
            leftview.assetDependencies = transform.GetComponent<AssetDependencies>();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnPetActivate, OnPetActivate, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnPlayerCloseSeal, OnCloseBtnClicked, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnPetStroyClose, OnPetStroyClose, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnPetActivateStory, OnPetActivateStory, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnPetLoveExpUp, OnPetLoveExpUp, toRegister);
        }

        private void OnPetActivate()
        {
            rightView.ActiveSeal();
        }

        private void OnPetActivateStory(uint stroyid)
        {
            rightView.RefreshStroy(stroyid);
        }

        private void OnPetStroyClose()
        {
            rightView.BackGroundStroyClose();
        }

        private void OnPetLoveExpUp()
        {
            rightView.OnLoveUpExpUp();
        }

        protected override void OnOpen(object arg)
        {
            if (null != arg)
            {
                //TODO 后面统一下外部调用-H
                if(arg is uint)
                {
                    petList = Sys_Pet.Instance.GetBookList(0);
                    currentShowPetId = Convert.ToUInt32(arg);
                }
                else if(arg is Tuple<uint, object>)
                {
                    petList = Sys_Pet.Instance.GetBookList(0);
                    Tuple<uint, object> tuple = arg as Tuple<uint, object>;
                    currentShowPetId = Convert.ToUInt32(tuple.Item2);
                }
                else
                {
                    petBookListPar = arg as PetBookListPar;
                    if (null != petBookListPar.showPetList)
                        petList = petBookListPar.showPetList;
                    else
                        petList = Sys_Pet.Instance.GetBookList(0);
                    currentShowPetId = (uint)petBookListPar.petId;
                    ePetReviewViewType = petBookListPar.eviewType;
                }
               
            }
            else
            {
                petList = Sys_Pet.Instance.GetBookList(0);
                if (petList.Count > 0)
                {
                    currentShowPetId = petList[0];
                }
            }
            index = petList.IndexOf(currentShowPetId);
        }

        protected override void OnShow()
        {            
            InitData();
        }

        protected override void OnDestroy()
        {
            petBookListPar = null;
            petList.Clear();
            petList = null;
        }

        protected override void OnUpdate()
        {
            leftview?.ExecUpdate();
        }

        private void InitData()
        {
            layout.leftBtn.gameObject.SetActive(index > 0 && petList.Count != 0 && (petBookListPar == null ? true : petBookListPar.showChangeBtn));
            layout.rightBtn.gameObject.SetActive(index < petList.Count - 1 && petList.Count != 0 && (petBookListPar == null ? true : petBookListPar.showChangeBtn));
            leftview.SetData(currentShowPetId);
            leftview.Show();
            if(null != petBookListPar)
            {
                InitRightView();
            }
            else
            {
                RefreshRightView();
            }            
        }

        private void InitRightView()
        {
            rightView.RefreshData(currentShowPetId);
            rightView.InitShow(petBookListPar.ePetReviewPageType);
        }

        private void ResetData()
        {
            layout.leftBtn.gameObject.SetActive(index > 0 && petList.Count != 0);
            layout.rightBtn.gameObject.SetActive(index < petList.Count - 1 && petList.Count != 0);
            leftview.SetData(currentShowPetId);
            leftview.Show();
            RefreshRightView();
        }

        private void RefreshRightView()
        {
            rightView.RefreshData(currentShowPetId);
            rightView.ShowEx();
        }

        protected override void OnHide()
        {
            petBookListPar = null;
            leftview.Hide();
            rightView.Hide();
        }

        public void OnCloseBtnClicked()
        {
            CloseSelf();
        }

        private bool clickTag = true;

        private void SetClickTag()
        {
            clickTag = false;
            currentShowPetId = petList[index];
            ResetData();
            Timer.Register(0.3f, () =>
            {
                clickTag = true;
            });
        }

        public void OnLeftClicked()
        {
            if(clickTag)
            {
                index--;
                SetClickTag();
            }
            
        }

        public void OnRightClicked()
        {
            if (clickTag)
            {
                index++;
                SetClickTag();
            }
        }
    }
}
