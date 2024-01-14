using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Multi_Info_Layout
    {
        class  InfoItem: ToggleIntClickItem
        {
          
            private Text m_TexNum;
            private Text m_TexInfo;

            private Transform mSelect01Trans;
            private Transform mSelect02Trans;

            public void SetNumTex(string tex)
            {
                m_TexNum.text = tex;
            }

            public void SetInfoTex(string tex)
            {
                m_TexInfo.text = tex;
            }
            public override void Load(Transform root)
            {
                base.Load(root);

                m_TexNum = root.Find("Text01").GetComponent<Text>();
                m_TexInfo = root.Find("Text02").GetComponent<Text>();

                mSelect01Trans = root.Find("Image_Select/Image_Select (1)");
                mSelect02Trans = root.Find("Image_Select/Image_Select (2)");
            }

            protected override void OnClick(bool b)
            {
                base.OnClick(b);

                mSelect01Trans.gameObject.SetActive(b);
                mSelect02Trans.gameObject.SetActive(b);
            }
            public override ClickItem Clone()
            {
                return Clone<InfoItem>(this);
            }
        }
    }

    public partial class UI_Multi_Info_Layout
    {
        class InfoChildItem:ClickItem
        {
            private Text m_TexNum;
            private Text m_TexInfo;

            private List<Transform> m_TranStates = new List<Transform>();

            public void SetNumTex(string tex)
            {
                m_TexNum.text = tex;
            }

            public void SetInfoTex(string tex)
            {
                m_TexInfo.text = tex;
            }

            public void SetInfoColor(Color color)
            {
                m_TexNum.color = color;
                m_TexInfo.color = color;
            }
            public void SetState(int state) //0 锁 1 完成 2 现在
            {
                int count = m_TranStates.Count;

                if (state >= count)
                    return;

                for (int i = 0; i < count; i++)
                {
                    m_TranStates[i].gameObject.SetActive(i == state);
                }
            }

            public void SetActive(bool b)
            {
                mTransform.gameObject.SetActive(b);
            }
            public override void Load(Transform root)
            {
                base.Load(root);

                m_TexNum = root.Find("Text01").GetComponent<Text>();
                m_TexInfo = root.Find("Text02").GetComponent<Text>();

                m_TranStates.Add(root.Find("Image_Lock"));
                m_TranStates.Add(root.Find("Image_Hook"));
                m_TranStates.Add(root.Find("Image_Now"));
            }

            public override ClickItem Clone()
            {
                return Clone<InfoChildItem>(this);
            }
        }
    }

    public partial class UI_Multi_Info_Layout
    {

        private Button mBtnAgine;
        private Button mBtnGoOn;

        private Button mBtnClose;

        private ClickItemGroup<InfoItem> m_InfoGroup;
        private ClickItemGroup<InfoChildItem> m_InfoChildGroup;

        private IListener m_Listener;
        private Slider m_SliderProcess;
        private Text m_TexSlider;



        private Text m_TexTitle;

        private Button m_BtnFastTeam;
        public void Load(Transform root)
        {
            Transform infotrans = root.Find("Animator/View_Left/Scroll View01/Viewport/Content/RewardItem");

            InfoItem infoItem = new InfoItem();
            infoItem.Load(infotrans);
            m_InfoGroup = new ClickItemGroup<InfoItem>(infoItem);
            m_InfoGroup.SetAddChildListenter(AddInfoItem);

            Transform infochildtrans = root.Find("Animator/View_Right/Scroll View01/Viewport/Content/RewardItem");

            InfoChildItem infochildItem = new InfoChildItem();
            infochildItem.Load(infochildtrans);
            m_InfoChildGroup = new ClickItemGroup<InfoChildItem>(infochildItem);

            mBtnAgine = root.Find("Animator/View_Right/Btn_02").GetComponent<Button>();
            mBtnGoOn = root.Find("Animator/View_Right/Btn_01").GetComponent<Button>();

            m_SliderProcess = root.Find("Animator/View_Left/Slider_Exp").GetComponent<Slider>();

            m_TexSlider = root.Find("Animator/View_Left/Slider_Exp/Text_Percent").GetComponent<Text>();


            mBtnAgine.onClick.AddListener(OnClickAgain);
            mBtnGoOn.onClick.AddListener(OnClickGoOn);


            mBtnClose = root.Find("Animator/View_TipsBg03_Big/Btn_Close").GetComponent<Button>();
            mBtnClose.onClick.AddListener(OnClickClose);


            m_TexTitle = root.Find("Animator/View_TipsBg03_Big/Image_Title").GetComponent<Text>();


            m_BtnFastTeam = root.Find("Animator/View_Right/Btn_03").GetComponent<Button>();

        }

        public void setListener(IListener listener)
        {
            m_Listener = listener;

            m_BtnFastTeam.onClick.AddListener(listener.OnClickFastTeam);
        }

        private void AddInfoItem(InfoItem item)
        {
            item.clickItemEvent.AddListener(OnClickInfoItem);
        }
        private void OnClickInfoItem(int index)
        {
            m_Listener?.OnClickInfo(index);
        }
        public void SetInfoCount(int count)
        {
            m_InfoGroup.SetChildSize(count);
        }

        public void SetFastTeamActive(bool active)
        {
            if (m_BtnFastTeam.gameObject.activeSelf != active)
                m_BtnFastTeam.gameObject.SetActive(active);
        }
        public void SetInfo(int index, string num, string name)
        {
            var item = m_InfoGroup.getAt(index);

            if (item == null)
                return;

            item.SetNumTex(num);
            item.SetInfoTex(name);
        }

        public void SetInfoIndex(int index, int value)
        {
            var item = m_InfoGroup.getAt(index);

            if (item == null)
                return;

            item.Index = value;
           
        }

        public void SetInteractableInfo(int index,bool b)
        {
            var item = m_InfoGroup.getAt(index);

            if (item == null)
                return;

            item.Togg.interactable = b;
        }

        public void FocusInfo(int index)
        {
            var item = m_InfoGroup.getAt(index);

            if (item == null)
                return;

            item.Togg.isOn = true;
        }

        public bool getFocusInfo(int index)
        {
            var item = m_InfoGroup.getAt(index);

            if (item == null)
                return true;

            return item.Togg.isOn ;
        }
        public void SetInfoChildCount(int count)
        {
            m_InfoChildGroup.SetChildSize(count);
        }

        public void SetInfoChild(int index, string num, string name,Color color)
        {
            var item = m_InfoChildGroup.getAt(index);

            if (item == null)
                return;

            item.SetNumTex(num);
            item.SetInfoTex(name);
            item.SetInfoColor(color);
        }

        
        public void SetInfoChildActive(int index,bool b)
        {
            var item = m_InfoChildGroup.getAt(index);

            if (item == null)
                return;

            item.SetActive(b);
        }
        public void SetChildInfoState(int index, int state)
        {
            var item = m_InfoChildGroup.getAt(index);

            if (item == null)
                return;

            item.SetState(state);
        }

        public void SetProcessState(bool b)
        {
            m_SliderProcess.gameObject.SetActive(b);
        }
        public void SetProcess(int cur, int total)
        {
            m_SliderProcess.value = cur / (total * 1f);

            m_TexSlider.text = string.Format("{0}/{1}", cur, total);
        }


        public void SetTitle(string tex)
        {
            m_TexTitle.text = tex;
        }
        public void ShowGoOn(bool b)
        {
            mBtnGoOn.gameObject.SetActive(b);
        }

        private void OnClickAgain()
        {
            m_Listener?.Agine();
        }

        private void OnClickGoOn()
        {
            m_Listener?.GoOn();
        }

        private void OnClickClose()
        {
            m_Listener?.Close();
        }
        public interface IListener
        {
            void Agine();
            void GoOn();

            void OnClickInfo(int index);

            void Close();

            void OnClickFastTeam();
        }
    }
}
