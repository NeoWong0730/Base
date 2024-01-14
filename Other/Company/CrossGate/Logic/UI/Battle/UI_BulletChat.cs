using DG.Tweening;
using Google.Protobuf;
using Lib.Core;
using Logic.Core;
using Packet;
using System;
using System.Collections.Generic;
using System.Text;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_BulletChat : UIComponent
    {
        private Text text01;
        private Text text02;
        private Text text03;
        private Text textSelf;
        private RectTransform rect01;
        private RectTransform rect02;
        private RectTransform rect03;
        private RectTransform rectSelf;
        private List<ServerBullet> serverBulletList = new List<ServerBullet>();

        private StringBuilder strBuilder01;
        private StringBuilder strBuilder02;
        private StringBuilder strBuilder03;

        protected override void Loaded()
        {
            base.Loaded();
            rect01 = transform.Find("Text").GetComponent<RectTransform>();
            rect02 = transform.Find("Text (1)").GetComponent<RectTransform>();
            rect03 = transform.Find("Text (2)").GetComponent<RectTransform>();
            rectSelf = transform.Find("Text (3)").GetComponent<RectTransform>();

            text01 = rect01.GetComponent<Text>();
            text02 = rect02.GetComponent<Text>();
            text03 = rect03.GetComponent<Text>();
            textSelf = rectSelf.GetComponent<Text>();
        }

        public override void Show()
        {
            SetState(Sys_Video.Instance.isOpenBullet);
            rect01.anchoredPosition = new Vector2(0, rect01.anchoredPosition.y);
            rect02.anchoredPosition = new Vector2(50, rect02.anchoredPosition.y);
            rect03.anchoredPosition = new Vector2(100, rect03.anchoredPosition.y);
        }

        public override void Hide()
        {
            DefaultBullet();
            DefaultSelfBullet();
            serverBulletList.Clear();
            DefaultTempGo();
        }

        public void ProcessEventsRegiste(bool toRegister)
        {
            base.ProcessEventsForEnable(toRegister);

            Sys_Video.Instance.eventEmitter.Handle(Sys_Video.EEvents.OnReceiveBullet, OnReceiveBullet, toRegister);
            Sys_Video.Instance.eventEmitter.Handle<string>(Sys_Video.EEvents.OnPlaySelfBullet, OnPlaySelfBullet, toRegister);
        }

        private void OnPlaySelfBullet(string content)
        {
            if (Sys_Video.Instance.isOpenBullet)
            {
                float endPos = -(Screen.width + rectSelf.sizeDelta.x);
                if (rectSelf.anchoredPosition.x == 100)
                {
                    textSelf.text = content;
                    rectSelf.DOLocalMoveX(endPos, 15).SetEase(Ease.Linear).onComplete += () =>
                    {
                        rectSelf.anchoredPosition = new Vector2(100, rectSelf.anchoredPosition.y);
                    };
                }
                else
                {
                    GameObject go = FrameworkTool.CreateGameObject(rectSelf.gameObject, rectSelf.parent.gameObject);
                    go.transform.name = "TempSelf";
                    go.transform.GetComponent<Text>().text = content;
                    RectTransform rect = go.GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(100, rectSelf.anchoredPosition.y);
                    rect.DOLocalMoveX(endPos, 15).SetEase(Ease.Linear).onComplete += () =>
                    {
                        GameObject.Destroy(go);
                    };
                }
            }     
        }

        private void OnReceiveBullet()
        {
            SetContent();
            OpenBullent();
        }

        public void SetState(bool isOpen)
        { 
            DefaultBullet();
            DefaultSelfBullet();
            DefaultTempGo();
            if (isOpen)
            {
                SetContent();
                OpenBullent();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void SetBulletData()
        {
            var list = Net_Combat.Instance.GetServerBulletList();
            if (list == null || list.Count == 0)
            {
                return;
            }
            serverBulletList.Clear();
            for (int i = 0; i < list.Count; ++i)
            {
                serverBulletList.Add(list[i]);
            }
        }

        private void SetContent()
        {
            if (!Net_Combat.Instance.m_IsVideo || !Sys_Video.Instance.isOpenBullet)
            {
                return;
            }
            SetBulletData();
            strBuilder01 = StringBuilderPool.GetTemporary();
            strBuilder02 = StringBuilderPool.GetTemporary();
            strBuilder03 = StringBuilderPool.GetTemporary();

            for (int i = 0; i < serverBulletList.Count; ++i)
            {
                if (i % 3 == 1)
                {
                    strBuilder01.Append(serverBulletList[i].Context.ToStringUtf8());
                    strBuilder01.Append("          ");
                }
                else if (i % 3 == 2)
                {
                    strBuilder02.Append(serverBulletList[i].Context.ToStringUtf8());
                    strBuilder02.Append("          ");
                }
                else
                {
                    strBuilder03.Append(serverBulletList[i].Context.ToStringUtf8());
                    strBuilder03.Append("          ");
                }
            }
        }

        private void OpenBullent()
        {
            if (serverBulletList == null || serverBulletList.Count == 0)
            {
                return;
            }

            float duration = serverBulletList.Count < 3 ? 20 : serverBulletList.Count / 3 * 20;
            float endPos01 = -(Screen.width + rect01.sizeDelta.x);
            float endPos02 = -(Screen.width + rect02.sizeDelta.x);
            float endPos03 = -(Screen.width + rect03.sizeDelta.x);
            if (rect01.anchoredPosition.x == 0)
            {
                text01.text = StringBuilderPool.ReleaseTemporaryAndToString(strBuilder01);
                text02.text = StringBuilderPool.ReleaseTemporaryAndToString(strBuilder02);
                text03.text = StringBuilderPool.ReleaseTemporaryAndToString(strBuilder03);
                rect01.DOLocalMoveX(endPos01, duration).SetEase(Ease.Linear).onComplete += () =>
                {
                    rect01.anchoredPosition = new Vector2(0, rect01.anchoredPosition.y);
                };
                rect02.DOLocalMoveX(endPos02, duration).SetEase(Ease.Linear).onComplete += () =>
                {
                    rect02.anchoredPosition = new Vector2(50, rect02.anchoredPosition.y);
                };
                rect03.DOLocalMoveX(endPos03, duration).SetEase(Ease.Linear).onComplete += () =>
                {
                    rect03.anchoredPosition = new Vector2(100, rect03.anchoredPosition.y);
                };
            }
            else //上一回合还没播完 
            {
                GameObject go01 = FrameworkTool.CreateGameObject(rect01.gameObject, rect01.parent.gameObject);
                GameObject go02 = FrameworkTool.CreateGameObject(rect02.gameObject, rect02.parent.gameObject);
                GameObject go03 = FrameworkTool.CreateGameObject(rect03.gameObject, rect03.parent.gameObject);
                go01.transform.name = "Temp01";
                go02.transform.name = "Temp02";
                go03.transform.name = "Temp03";
                go01.GetComponent<Text>().text = StringBuilderPool.ReleaseTemporaryAndToString(strBuilder01);
                go02.GetComponent<Text>().text = StringBuilderPool.ReleaseTemporaryAndToString(strBuilder02);
                go03.GetComponent<Text>().text = StringBuilderPool.ReleaseTemporaryAndToString(strBuilder03);
                RectTransform rect1 = go01.GetComponent<RectTransform>();
                RectTransform rect2 = go02.GetComponent<RectTransform>();
                RectTransform rect3 = go03.GetComponent<RectTransform>();
                rect1.anchoredPosition = new Vector2(0, rect01.anchoredPosition.y);
                rect1.DOLocalMoveX(endPos01, duration).SetEase(Ease.Linear).onComplete += () =>
                {
                    GameObject.Destroy(go01);
                };
                rect2.anchoredPosition = new Vector2(50, rect02.anchoredPosition.y);
                rect2.DOLocalMoveX(endPos02, duration).SetEase(Ease.Linear).onComplete += () =>
                {
                    GameObject.Destroy(go02);
                };
                rect3.anchoredPosition = new Vector2(100, rect03.anchoredPosition.y);
                rect3.DOLocalMoveX(endPos03, duration).SetEase(Ease.Linear).onComplete += () =>
                {
                    GameObject.Destroy(go03);
                };
            }
        }       

        private void DefaultBullet()
        {
            DOTween.Kill(rect01);
            DOTween.Kill(rect02);
            DOTween.Kill(rect03);
            rect01.anchoredPosition = new Vector2(0, rect01.anchoredPosition.y);
            rect02.anchoredPosition = new Vector2(50, rect02.anchoredPosition.y);
            rect03.anchoredPosition = new Vector2(100, rect03.anchoredPosition.y);
        }

        private void DefaultSelfBullet()
        {
            DOTween.Kill(rectSelf);
            rectSelf.anchoredPosition = new Vector2(100, rectSelf.anchoredPosition.y);
        }

        private void DefaultTempGo()
        {
            FrameworkTool.DestroyChildren(gameObject,rect01.name,rect02.name,rect03.name,rectSelf.name);
        }
    }
}
