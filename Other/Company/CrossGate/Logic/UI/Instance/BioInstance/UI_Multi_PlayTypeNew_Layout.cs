using Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Table;
namespace Logic
{
    public partial class UI_Multi_PlayTypeNew_Layout
    {
        class ChapterInfoItem : ToggleIntClickItem
        {

            private Text m_TexChapterName;
            private Text m_TexChapterNum;


            // 锁定状态
            private Transform m_LockTrans;
            private Text m_TexLockLastName;
            private Text m_TexLockLevel;

            private Image m_ImageBack;

            private Text m_TexLevel;

            private Transform m_TransScore;
            private Text m_TexScore;

            //private AsyncOperationHandle<Sprite> m_Handle;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_ImageBack = root.Find("Mask/Card").GetComponent<Image>();

                m_TexChapterName = root.Find("Image_Name/Text").GetComponent<Text>();
                m_TexChapterNum = root.Find("Image_Name/Text_Chapter").GetComponent<Text>();

                m_LockTrans = root.Find("View_Lock");
                m_TexLockLastName = root.Find("View_Lock/Image_Lock01/Text").GetComponent<Text>();
                m_TexLockLevel = root.Find("View_Lock/Image_Lock02/Text").GetComponent<Text>();

                m_TexLevel = root.Find("Image_LV/Text").GetComponent<Text>();

                m_TransScore = root.Find("Image_Score");
                m_TexScore = root.Find("Image_Score/Text_Num").GetComponent<Text>();
            }

            public override ClickItem Clone()
            {
                var item = Clone<ChapterInfoItem>(this);

                item.Togg.isOn = false;

                return item;
            }

            protected override void OnClick(bool b)
            {
                base.OnClick(b);
            }
            public void setCharpterName(string name)
            {
                m_TexChapterName.text = name;
            }

            public void SetCharpterNum(int num)
            {           
                TextHelper.SetText(m_TexChapterNum, (uint)(2022911 + num));
            }
            public void SetBg(string path)
            {
                ImageHelper.SetIcon(m_ImageBack, path);

            }
            public void SetCharpterLock( bool state)
            {
                m_TexLevel.gameObject.SetActive(!state);

                m_LockTrans.gameObject.SetActive(state);

                m_TransScore.gameObject.SetActive(!state);
            }

            public void SetCharpterLockLastLeve(uint name, uint langungeID)
            {
                if (name == 0)
                {
                    m_TexLockLastName.text = string.Empty;
                    return;
                }

                TextHelper.SetText(m_TexLockLastName, langungeID, LanguageHelper.GetTextContent(name));
            }

            public void SetCharpterLockLevel( int level, uint langungeID)
            {
                //m_TexLockLevel.text = level.ToString();

                TextHelper.SetText(m_TexLockLevel, langungeID, level.ToString());
            }


            public void SetLevel(int level)
            {
                TextHelper.SetText(m_TexLevel, 2022903, level.ToString());
            }

            public void SetScore(uint score, bool isGreen)
            {
                uint styleid = isGreen ? 74u : 156u;
                TextHelper.SetText(m_TexScore, score.ToString(), CSVWordStyle.Instance.GetConfData(styleid));
            }

        }
    }

    public partial class UI_Multi_PlayTypeNew_Layout
    {
        public class  CardItem:ButtonIntClickItem
        {
            private Image m_IIcon;
            public Transform m_FocusTrans;


            public override void Load(Transform root)
            {
                mTransform = root;

                Btn = mTransform.Find("Btn_Card").GetComponent<Button>();

                Btn.onClick.AddListener(OnClick);

                m_IIcon = root.Find("Image_Icon").GetComponent<Image>();
                m_FocusTrans = root.Find("Image_Select");
            }

            public override ClickItem Clone()
            {
                return Clone<CardItem>(this);
            }

            public void setSelectIcon(bool state)
            {
                m_FocusTrans.gameObject.SetActive(state);
            }

            public void SetIcon(uint icon)
            {
                ImageHelper.SetIcon(m_IIcon, icon);
            }
        }
    }

    public partial class UI_Multi_PlayTypeNew_Layout
    {

        private Button m_BtnClose;
        //private Button m_BtnReward;

        private Text m_TexCur;
        private Text m_TexTotal;

        private Text m_TexName;

        private ClickItemGroup<ChapterInfoItem> m_ChapterInfoGroup;
        private ClickItemGroup<CardItem> m_CardGroup;


        private RawImage m_IModelShow;
        private IListener m_Listener;

        private AssetDependencies mInfoAssetDependencies;

        private Text m_TexHaveRewardTimes;
        private Button m_BtnHaveRewardTimes;

        private Transform m_TransRule;
        private Button m_BtnRuleClose;
        public void Load(Transform root)
        {

            mInfoAssetDependencies = root.GetComponent<AssetDependencies>();

            Transform infotrans = root.Find("Animator/View_Right/Scroll View01/Content/CopyItem");


            ChapterInfoItem infoItem = new ChapterInfoItem();
            infoItem.Load(infotrans);
            m_ChapterInfoGroup = new ClickItemGroup<ChapterInfoItem>(infoItem);
            m_ChapterInfoGroup.SetAddChildListenter(AddInfoItem);

            Transform infochildtrans = root.Find("Animator/View_Left/Image_BG/Scroll View01/Viewport/Content/IconItem");

            CardItem infochildItem = new CardItem();
            infochildItem.Load(infochildtrans);
            m_CardGroup = new ClickItemGroup<CardItem>(infochildItem);
            m_CardGroup.SetAddChildListenter(AddCardItem);


           // m_BtnReward = root.Find("Animator/View_Left/Btn_Reward").GetComponent<Button>();
            m_BtnClose = root.Find("Animator/View_Title07/Image_Titlebg04/Btn_Close").GetComponent<Button>();


            m_IModelShow = root.Find("Animator/View_Left/Image_BG/Charapter").GetComponent<RawImage>();

            m_TexCur = root.Find("Animator/View_Left/Image_BG/Image_Process/Image_Process_01/Text1").GetComponent<Text>();
            m_TexTotal = root.Find("Animator/View_Left/Image_BG/Image_Process/Image_Process_01/Text2").GetComponent<Text>();

            m_TexName = root.Find("Animator/View_Left/Image_BG/Image_Title/Text").GetComponent<Text>();

            m_TexHaveRewardTimes = root.Find("Animator/View_Right/Image_Tips/Text").GetComponent<Text>();
            m_BtnHaveRewardTimes = root.Find("Animator/View_Right/Image_Tips/Button").GetComponent<Button>();


            m_TransRule = root.Find("Animator/UI_Rule1");

            m_BtnRuleClose = m_TransRule.Find("Black").GetComponent<Button>();

        }

        public void setListener(IListener listener)
        {
            m_Listener = listener;

           // m_BtnReward.onClick.AddListener(m_Listener.OnClickReward);
            m_BtnClose.onClick.AddListener(m_Listener.Close);

            m_BtnHaveRewardTimes.onClick.AddListener(m_Listener.OnClickHaveRewardTimes);

            m_BtnRuleClose.onClick.AddListener(listener.OnClickCloseRule);
        }

        public void SetRuleActive(bool active)
        {
            if (m_TransRule.gameObject.activeSelf != active)
                m_TransRule.gameObject.SetActive(active);
        }
        public void Close()
        {
            if (mShowSceneControl != null)
                mShowSceneControl.Dispose();

            //if (mDisplayControl != null)
            //    mDisplayControl.Dispose();
            DisplayControl<EHeroModelParts>.Destory(ref mDisplayControl);
        }
        private void AddInfoItem(ChapterInfoItem item)
        {
            item.clickItemEvent.AddListener(OnClickInfoItem);
        }

        private void AddCardItem(CardItem item)
        {
            item.clickItemEvent.AddListener(OnClickCardItem);
        }

        private void OnClickInfoItem(int index)
        {
            m_Listener?.OnClickChapterInfo(index);
        }

        private void OnClickCardItem(int index)
        {
            m_Listener?.OnClickChardInfo(index);

            int count = m_CardGroup.Count;

            for (int i = 0; i < count; i++)
            {
                var card = m_CardGroup.getAt(i);
                if (card != null)
                {
                    card.m_FocusTrans.gameObject.SetActive(i == index);
                }
            }
        }


        public void SetBtnRewardActive(bool b)
        {
            //m_BtnReward.gameObject.SetActive(b);
        }

        #region 章节信息设置
        public void SetCharpterInfoCount(int count)
        {
            m_ChapterInfoGroup.SetChildSize(count);
        }

        private ChapterInfoItem getCharpterItem(int index)
        {
            return m_ChapterInfoGroup.getAt(index);
        }
        public void setCharpterIndex(int index, int value)
        {
            var item = getCharpterItem(index);
            item.Index = value;
        }
        public void setCharpterName(int index,string text)
        {
            var item = getCharpterItem(index);

            item.setCharpterName(text);

            item.SetCharpterNum(index);
        }

        //public void setCharpterRewardState(int index, bool state)
        //{
        //    var item = getCharpterItem(index);

        //    item.setCharpterRewardState(state);
        //}

        //public void SetCharpterRewardProc(int index, int cur, int total)
        //{
        //    var item = getCharpterItem(index);
        //    item.SetCharpterRewardProc(cur, total);
        //}

        public void SetHaveRewardTimes(uint times)
        {
            TextHelper.SetText(m_TexHaveRewardTimes, 2022902, times.ToString());
        }
        public void SetCharpterBg(int index, string path)
        {
            var item = getCharpterItem(index);

            item.SetBg(path);
        }

        public void SetCharpterLock(int index, bool state)
        {
            var item = getCharpterItem(index);
            item.SetCharpterLock(state);
        }

        public void SetCharpterLockLastLeve(int index, uint name, uint langungeID)
        {
            var item = getCharpterItem(index);
            item.SetCharpterLockLastLeve(name, langungeID);
        }

        public void SetCharpterLockLevel(int index, int level,uint langungeID)
        {
            var item = getCharpterItem(index);
            item.SetCharpterLockLevel(level, langungeID);
        }


        public void SetCharpterFocus(int index)
        {
            var item = getCharpterItem(index);

            item.Togg.isOn = true;
        }

        public void SetCharpterMinLevel(int index, int minlevel)
        {
            var item = getCharpterItem(index);

            if (item == null)
                return;

            item.SetLevel(minlevel);
        }

        public void SetCharpterScore(int index, uint score,bool isgreen)
        {
            var item = getCharpterItem(index);

            if (item == null)
                return;

            item.SetScore(score,isgreen);
        }
        //public void SetRewardTexActive(int index, bool b)
        //{
        //    var item = getCharpterItem(index);

        //    item.SetRewardTexActive(b);
        //}
        #endregion

        #region

        public void SetCurCardName(string name)
        {
            m_TexName.text = name;
        }
        public void SetCardInfoCount(int count)
        {
            m_CardGroup.SetChildSize(count);
        }

        private CardItem getCardItem(int index)
        {
            return m_CardGroup.getAt(index);
        }
        public void setCardIndex(int index, int value)
        {
            var item = getCardItem(index);
            item.Index = value;
        }

        public void setCardSelect(int index, bool state)
        {
            var item = getCardItem(index);
            item.setSelectIcon(state);
           
        }

        public void setCardIcon(int index, uint iconID)
        {
            var item = getCardItem(index);
            item.SetIcon(iconID);

        }

        public void setFocusIndex(int index, int total)
        {
            m_TexCur.text = (index + 1).ToString();
            m_TexTotal.text = total.ToString();
        }
        #endregion

        Vector3 showModelPosition = Vector3.zero;

        ShowSceneControl mShowSceneControl = null;
        DisplayControl<EHeroModelParts> mDisplayControl = null;

        uint mModelID;
        public void SetModelActor( ulong roleID, string modelPath)
        {
            if (mShowSceneControl != null)
                mShowSceneControl.Dispose();

            //if(mDisplayControl != null)
            //    mDisplayControl.Dispose();
            DisplayControl<EHeroModelParts>.Destory(ref mDisplayControl);

            // Vector2 size = m_IModelShow.rectTransform.sizeDelta;

            Vector3 pos = showModelPosition;

            ShowSceneControl secneControl = CreateShowScene(mInfoAssetDependencies.mCustomDependencies[0] as GameObject, pos);

            mShowSceneControl = secneControl;

            mDisplayControl = LoadModelForShow(secneControl, modelPath);

            int size0 = 512;

            var targetTexture = secneControl.GetTemporary(size0, size0, 0, RenderTextureFormat.ARGB32, 1, false,4);
            m_IModelShow.texture = targetTexture;
    
            mShowSceneControl.mRoot.gameObject.transform.position = pos;

        }

        private ShowSceneControl CreateShowScene(GameObject origin, Vector3 pos)
        {
            GameObject scene = GameObject.Instantiate<GameObject>(origin);

            ShowSceneControl showSceneControl = new ShowSceneControl();

            scene.transform.position = pos;

            scene.transform.SetParent(GameCenter.sceneShowRoot.transform);

            showSceneControl.Parse(scene);

            return showSceneControl;
        }

        private DisplayControl<EHeroModelParts> LoadModelForShow(ShowSceneControl sceneControl, string modelPath)
        {


            //DisplayControl<EHeroModelParts> control = DisplayControl<EHeroModelParts>.Create(2);
            DisplayControl<EHeroModelParts> control = DisplayControl<EHeroModelParts>.Create((int)EHeroModelParts.Count);

            control.onLoaded = DisplayControlLoaded;

            control.eLayerMask = ELayerMask.ModelShow;

            control.LoadMainModel(EHeroModelParts.Main, modelPath, EHeroModelParts.None, null);
            //control.LoadMainModel(EHeroModelParts.Weapon, cSVEquipmentData.show_model, EHeroModelParts.Main, cSVEquipmentData.equip_pos);
            control.GetPart(EHeroModelParts.Main).SetParent(sceneControl.mModelPos, null);

            return control;
        }

        private void DisplayControlLoaded(int mode)
        {
            if (mDisplayControl.bMainPartFinished == false)
                return;

            m_Listener?.ShowModleFinish(mShowSceneControl,mDisplayControl);
         //   CSVCharacter.Data cSVCharacterData = CSVCharacter.Instance.GetConfData(mModelID);

         //   if (cSVCharacterData == null)
         //       return;



         //   mDisplayControl?.mAnimation.UpdateHoldingAnimations(ModelID + 100, cSVCharacterData.show_weapon_id,
         //Constants.IdleAndRunAnimationClip);
        }

        public interface IListener
        {
            void OnClickReward();

            void OnClickChapterInfo(int index);
            void OnClickChardInfo(int index);

            void OnClickCloseRule();
            void OnClickHaveRewardTimes();
            void Close();

            void ShowModleFinish(ShowSceneControl control, DisplayControl<EHeroModelParts>  displayControl);
        }
    }
}
