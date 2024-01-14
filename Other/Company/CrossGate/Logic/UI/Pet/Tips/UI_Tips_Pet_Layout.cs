using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Tips_Pet_Layout
    {
        public Transform transform;
        public Text textPublicityTime;
        public Image imgPrice;
        public Text textPrice;

        public Button btnRelation;
        public Button btnShare;
        public UI_Trade_MessageBox_Share shareList;

        public Transform leftArrow;
        public Button btnLeft;
        public Transform rightArrow;
        public Button btnRight;

        public void Init(Transform transform)
        {
            this.transform = transform;
            textPublicityTime = transform.Find("Animator/View_Left/Detail/Text_Time").GetComponent<Text>();
            imgPrice = transform.Find("Animator/View_Left/Detail/Image_Property/Image_Icon").GetComponent<Image>();
            textPrice = transform.Find("Animator/View_Left/Detail/Image_Property/Text_Number").GetComponent<Text>();

            btnRelation = transform.Find("Animator/View_Button/Button_1").GetComponent<Button>();
            bool isCross = Sys_Trade.Instance.IsCrossServer();
            btnRelation.gameObject.SetActive(!isCross);

            btnShare = transform.Find("Animator/View_Button/Button_2").GetComponent<Button>();

            shareList = new UI_Trade_MessageBox_Share();
            shareList.Init(transform.Find("Animator/GroupList"));

            leftArrow = transform.Find("Animator/View_PageTurn/Arrow_Left");
            btnLeft = transform.Find("Animator/View_PageTurn/Arrow_Left/Button_Left").GetComponent<Button>();
            rightArrow = transform.Find("Animator/View_PageTurn/Arrow_Right");
            btnRight = transform.Find("Animator/View_PageTurn/Arrow_Right/Button_Right").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            btnRelation.onClick.AddListener(listener.OnClickRelation);
            btnShare.onClick.AddListener(listener.OnClickShare);
            btnLeft.onClick.AddListener(listener.OnClickPageLeft);
            btnRight.onClick.AddListener(listener.OnClickPageRight);
        }

        public interface IListener
        {
            void OnClickRelation();
            void OnClickShare();

            void OnClickPageLeft();
            void OnClickPageRight();
        }
    }
}
