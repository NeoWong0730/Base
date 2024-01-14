using UnityEngine;

namespace Logic
{
    public class UI_Tips_Ornament_Layout
    {
        public GameObject mRoot { get; private set; }
        public Transform mTrans { get; private set; }
        public Transform viewTips { get; private set; }

        public TipOrnamentBtnRoot btnRoot;
        public TipOrnamentMessage message;

        public TipOrnamentRightCompare rightCompare;
        public TipOrnamentLeftCompare leftCompare;
        public TipOrnamentSourceView viewSource;

        public void Parse(GameObject root)
        {
            mRoot = root;
            mTrans = root.transform;
            viewTips = mTrans.Find("View_Tips");

            btnRoot = new TipOrnamentBtnRoot();
            btnRoot.Init(mTrans.Find("View_Tips/Button_Root"));

            message = new TipOrnamentMessage();
            message.Init(mTrans.Find("View_Tips/Message"));

            rightCompare = new TipOrnamentRightCompare();
            rightCompare.Init(mTrans.Find("View_Tips/Tips01"));

            leftCompare = new TipOrnamentLeftCompare();
            leftCompare.Init(mTrans.Find("View_Tips/Tips02"));

            viewSource = new TipOrnamentSourceView();
            viewSource.Init(mTrans.Find("View_Tips/View_Right"));
        }

        public void OnDestroy()
        {
            btnRoot.OnDestroy();
            message.OnDestroy();
            rightCompare.OnDestroy();
            leftCompare.OnDestroy();
            viewSource.OnDestroy();
        }

        public void RegisterEvents(IListener listener)
        {
            btnRoot.btnIntensify.onClick.AddListener(listener.OnIntensifyBtnClicked);
            btnRoot.btnRecast.onClick.AddListener(listener.OnRecastBtnClicked);
            btnRoot.btnReplace.onClick.AddListener(listener.OnRepalceBtnClicked);
            btnRoot.btnDecompose.onClick.AddListener(listener.OnDecomposeBtnClicked);
            btnRoot.btnSale.onClick.AddListener(listener.OnSaleBtnClicked);
            btnRoot.btnTrade.onClick.AddListener(listener.OnTradeBtnClicked);
            btnRoot.btnBagSource.onClick.AddListener(listener.OnBagSourceBtnClicked);
            rightCompare.tipInfo.btnSource.onClick.AddListener(listener.OnSourceBtnClicked);
        }

        public interface IListener
        {
            void OnIntensifyBtnClicked();
            void OnRecastBtnClicked();
            void OnRepalceBtnClicked();
            void OnDecomposeBtnClicked();

            void OnSaleBtnClicked();
            void OnTradeBtnClicked();

            void OnBagSourceBtnClicked();
            void OnSourceBtnClicked();

        }
    }
}