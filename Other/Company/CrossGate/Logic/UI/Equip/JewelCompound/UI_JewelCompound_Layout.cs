using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_JewelCompound_Layout
    {
        public GameObject root;
        private Transform trans;
        private Button btnClose;

        public UI_JewelCompound_Right compoundRight;
        public UI_JewelCompound_Left compoundLeft;

        public GameObject goNone;

        private Animator animator;


        public void Parse(GameObject _root)
        {
            root = _root;
            trans = root.transform;

            btnClose = trans.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();

            compoundRight = new UI_JewelCompound_Right();
            compoundRight.Init(trans.Find("Animator/View_Right"));

            compoundLeft = new UI_JewelCompound_Left();
            compoundLeft.Init(trans.Find("Animator/View_Left"));

            goNone = trans.Find("Animator/View_None01").gameObject;

            animator = compoundRight.gameObject.GetComponent<Animator>();
        }

        public void RegisterEvents(IListener listener)
        {
            btnClose.onClick.AddListener(listener.OnClickClose);
        }

        public void SetAnimator(bool ten)
        {
            animator.enabled = true;
            string name = ten ? "Ten_Open" : "Once_Open";
            animator.speed = 1f;
            animator.Play(name, 0, 0f);
        }

        public interface IListener
        {
            void OnClickClose();
        }
    }
}


