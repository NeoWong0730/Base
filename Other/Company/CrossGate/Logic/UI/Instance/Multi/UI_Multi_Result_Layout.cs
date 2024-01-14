using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Multi_Result_Layout
    {
        interface UI_IResult
        {
            void SetDesc(string tex);

            void SetName(string tex);
            void SetActive(bool b);


        }
        class UI_Cross: UI_IResult
        {
            private Transform mTrans;

            private Text mTexDesc;

            private Text mTexName;
            public void Load(Transform transform)
            {
                mTrans = transform;

                mTexDesc = transform.Find("Image_BG/Text_Desc").GetComponent<Text>();

                mTexName = transform.Find("Image_BG/Text_Name").GetComponent<Text>();
                SetActive(false);
            }

            public void SetActive(bool b)
            {
                mTrans.gameObject.SetActive(b);

            }

            public void SetDesc(string tex)
            {
                mTexDesc.text = tex;
            }

            public void SetName(string tex)
            {
                mTexName.text = tex;
            }
        }
    }

    public partial class UI_Multi_Result_Layout
    {
        class UI_Success: UI_IResult
        {
            private Transform mTrans;

            private Text mTexDesc;

            private Text mTexName;
            public void Load(Transform transform)
            {
                mTrans = transform;

                mTexDesc = transform.Find("Image_BG/Text_Desc").GetComponent<Text>();

                mTexName = transform.Find("Image_BG/Text_Name").GetComponent<Text>();

                SetActive(false);
            }

            public void SetActive(bool b)
            {

                mTrans.gameObject.SetActive(b);
            }

            public void SetDesc(string tex)
            {
                mTexDesc.text = tex;
            }

            public void SetName(string tex)
            {
                mTexName.text = tex;
            }
        }
    }

    public partial class UI_Multi_Result_Layout
    {
        class UI_Fail: UI_IResult
        {
            private Transform mTrans;

            private Text mTexDesc;

            private Text mTexName;
            public void Load(Transform transform)
            {
                mTrans = transform;

                mTexDesc = transform.Find("Image_BG/Text_Desc").GetComponent<Text>();

                mTexName = transform.Find("Image_BG/Text_Name").GetComponent<Text>();

                SetActive(false);
            }

            public void SetActive(bool b)
            {
                mTrans.gameObject.SetActive(b);

            }

            public void SetDesc(string tex)
            {
                mTexDesc.text = tex;
            }

            public void SetName(string tex)
            {
                mTexName.text = tex;
            }
        }
    }
    public partial class UI_Multi_Result_Layout
    {

        private Button mBtnClose;


        private UI_Cross mUICross = new UI_Cross();
        private UI_Success mUISuccess = new UI_Success();
        private UI_Fail mUIFail = new UI_Fail();

        private UI_IResult mIResut;

        private IListener m_Listener;

        public void Load(Transform root)
        {
            Transform infotrans = root.Find("Animator/Scroll View01/Viewport/Content/RewardItem");

            mBtnClose = root.Find("Image_Black").GetComponent<Button>();

            mUICross.Load(root.Find("Animator/View_Go"));
            mUISuccess.Load(root.Find("Animator/View_Success"));
            mUIFail.Load(root.Find("Animator/View_Fail"));

            mBtnClose.onClick.AddListener(OnClickClose);

        }

        public void setListener(IListener listener)
        {
            m_Listener = listener;
        }

        public void ShowMode(int mode) // 0 通关 1 胜利 2 失败
        {
            switch (mode)
            {
                case 0:
                    mIResut = mUICross;
                    break;
                case 1:
                    mIResut = mUISuccess;
                    break;
                case 2:
                    mIResut = mUIFail;
                    break;
                default:
                    break;
            }
        }


        public void SetDesc(string tex)
        {
            if (mIResut == null)
                return;
            mIResut.SetDesc(tex);
        }

        public void SetName(string tex)
        {
            if (mIResut == null)
                return;
            mIResut.SetName(tex);
        }

        public void SetActive(bool b)
        {
            if (mIResut == null)
                return;
            mIResut.SetActive(b);
        }
        private void OnClickClose()
        {
            m_Listener?.Close();
        }


        public interface IListener
        {
            void Close();
        }
    }
}
