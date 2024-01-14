using UnityEngine;

namespace Logic
{
    public class UI_Tips_Equipment_Layout
    {
        public GameObject mRoot { get; private set; }
        public Transform mTrans { get; private set; }

        public TipEquipBtnRoot btnRoot;
        public TipEquipMessage message;

        public TipEquipRightCompare rightCompare;
        public TipEquipLeftCompare leftCompare;

        public void Parse(GameObject root)
        {
            mRoot = root;
            mTrans = root.transform;

            btnRoot = new TipEquipBtnRoot();
            btnRoot.Init(mTrans.Find("View_Tips/Button_Root"));

            message = new TipEquipMessage();
            message.Init(mTrans.Find("View_Tips/Message"));

            rightCompare = new TipEquipRightCompare();
            rightCompare.Init(mTrans.Find("View_Tips/Tips01"));

            leftCompare = new TipEquipLeftCompare();
            leftCompare.Init(mTrans.Find("View_Tips/Tips02"));
        }

        public void OnDestroy()
        {
            btnRoot.OnDestroy();
            message.OnDestroy();
            rightCompare.OnDestroy();
            leftCompare.OnDestroy();
        }

        public void RegisterEvents(IListener listener)
        {
            btnRoot.btnIntensify.onClick.AddListener(listener.OnIntensifyBtnClicked);
            btnRoot.btnReplace.onClick.AddListener(listener.OnRepalceBtnClicked);
            btnRoot.btnDecompose.onClick.AddListener(listener.OnDecomposeBtnClicked);
            btnRoot.btnSale.onClick.AddListener(listener.OnSaleBtnClicked);
            btnRoot.btnTrade.onClick.AddListener(listener.OnTradeBtnClicked);
        }

        public interface IListener
        {
            void OnIntensifyBtnClicked();
            void OnRepalceBtnClicked();
            void OnDecomposeBtnClicked();

            void OnSaleBtnClicked();
            void OnTradeBtnClicked();
        }
    }
}


