using Lib.AssetLoader;
using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Logic.Core;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using Framework;
using Logic;

namespace Logic
{
    public class UI_ClueTaskWall_Layout
    {
        public GameObject gameObject;
        public Transform transform;

        public Button btnReturn;
        public Text clueTaskName;
        public CP_StarLevel starLevel;

        public Text taskName;
        public Text taskDesc;
        public Button btnGoto;
        public Text gotoText;

        public Transform View_Finish;
        public Text View_Finish_Text;
        public Transform View_None;
        public Text View_None_Text;
        public Transform View_Now;

        public GameObject leftProto;
        public Transform leftParent;

        public GameObject rightLeftProto;
        public Transform rightLeftParent;

        public GameObject rightRightProto;
        public Transform rightRightParent;

        public Transform rewardNode;

        public RawImage icon;
        public RawImageLoader rawImageLoader;
        public Cp_HorCoupleScrollRect horCoupleScrollRect;

        public void Parse(GameObject root)
        {
            gameObject = root;
            transform = gameObject.transform;

            btnReturn = transform.Find("Animator/View_Title03/Btn_Close").GetComponent<Button>();
            horCoupleScrollRect = transform.Find("Animator/View_Wall").GetComponent<Cp_HorCoupleScrollRect>();

            clueTaskName = transform.Find("Animator/View_Left/TaskTitle/TaskName").GetComponent<Text>();
            starLevel = transform.Find("Animator/View_Left/TaskTitle/StarNode").GetComponent<CP_StarLevel>();

            leftProto = transform.Find("Animator/View_Left/ScrollPhase/Content/Proto").gameObject;
            leftParent = transform.Find("Animator/View_Left/ScrollPhase/Content");

            rightLeftProto = transform.Find("Animator/View_Wall/ScrollLeft/Content/Proto").gameObject;
            rightLeftParent = transform.Find("Animator/View_Wall/ScrollLeft/Content");

            rightRightProto = transform.Find("Animator/View_Wall/ScrollRight/Content/Proto").gameObject;
            rightRightParent = transform.Find("Animator/View_Wall/ScrollRight/Content");

            taskName = transform.Find("Animator/View_Tips/View_Now/Text_Title").GetComponent<Text>();
            taskDesc = transform.Find("Animator/View_Tips/View_Now/Text_Title/Text_Tips").GetComponent<Text>();
            btnGoto = transform.Find("Animator/View_Tips/View_Now/Btn_01").GetComponent<Button>();
            gotoText = transform.Find("Animator/View_Tips/View_Now/Btn_01/Text_01").GetComponent<Text>();

            View_Now = transform.Find("Animator/View_Tips/View_Now");
            View_None = transform.Find("Animator/View_Tips/View_None");
            View_None_Text = transform.Find("Animator/View_Tips/View_None/Text_None").GetComponent<Text>();
            View_Finish = transform.Find("Animator/View_Tips/View_Finish");
            View_Finish_Text = transform.Find("Animator/View_Tips/View_Finish/Text_Finish/Text_Tips").GetComponent<Text>();

            rewardNode = transform.Find("Animator/View_Left/RewardList/RewardNode/Content");

            icon = transform.Find("Animator/View_Wall/Image_BG01").GetComponent<RawImage>();
            rawImageLoader = icon.GetComponent<RawImageLoader>();
        }
        public void RegisterEvents(IListener listener)
        {
            btnReturn.onClick.AddListener(listener.OnBtnReturnClicked);
            btnGoto.onClick.AddListener(listener.OnBtnGotoClicked);
        }

        public interface IListener
        {
            void OnBtnReturnClicked();
            void OnBtnGotoClicked();
        }
    }
}
