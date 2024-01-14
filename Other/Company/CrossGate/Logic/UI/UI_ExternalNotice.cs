using System.Collections.Generic;
using UnityEngine;
using Lib.Core;
using Logic.Core;
using UnityEngine.UI;
using UnityEngine.Networking;
using Framework;

namespace Logic
{
    public class UI_ExternalNotice_Layout
    {
        public class NoticeItem
        {
            public GameObject root;

            public GameObject darkRoot;
            public GameObject lightRoot;
            public Text darkText;
            public Text lightText;
            public CP_Toggle toggle;

            public Sys_ExternalNotice.NoticeData noticeData;

            public NoticeItem(GameObject gameObject, Sys_ExternalNotice.NoticeData _noticeData)
            {
                noticeData = _noticeData;
                root = gameObject;

                darkRoot = root.FindChildByName("Dark");
                lightRoot = root.FindChildByName("Light");
                darkText = root.FindChildByName("darkText").GetComponent<Text>();
                lightText = root.FindChildByName("lightText").GetComponent<Text>();
                toggle = root.GetComponent<CP_Toggle>();
                toggle.onValueChanged.AddListener(OnClickToggle);

                TextHelper.SetText(darkText, noticeData.Title);
                TextHelper.SetText(lightText, noticeData.Title);
            }

            void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    darkRoot.SetActive(false);
                    lightRoot.SetActive(true);

                    Sys_ExternalNotice.Instance.eventEmitter.Trigger<Sys_ExternalNotice.NoticeData>(Sys_ExternalNotice.EEvents.ChooseNoticeItem, noticeData);
                }
                else
                {
                    darkRoot.SetActive(true);
                    lightRoot.SetActive(false);
                }
            }
        }

        public Transform transform;
        public Button closeButton;
        public GameObject leftTabContent;
        public GameObject noticeItemPrefab;

        public GameObject rightRoot;
        public Image noticeImage;
        public Text noticeText;
        public Mask mask;

        public void Init(Transform transform)
        {
            this.transform = transform;

            closeButton = transform.gameObject.FindChildByName("Btn_Close").GetComponent<Button>();
            leftTabContent = transform.gameObject.FindChildByName("LeftContent");
            noticeItemPrefab = transform.gameObject.FindChildByName("TogglePrefab");

            rightRoot = transform.gameObject.FindChildByName("View_right");
            noticeImage = transform.gameObject.FindChildByName("Notice_banner").GetComponent<Image>();
            noticeText = transform.gameObject.FindChildByName("NoticeContent").GetComponent<Text>();
            mask = rightRoot.FindChildByName("Viewport").GetComponent<Mask>();
            mask.enabled = true;
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
        }

        public interface IListener
        {
            void OnClickCloseButton();
        }
    }

    public class UI_ExternalNotice : UIBase, UI_ExternalNotice_Layout.IListener
    {
        UI_ExternalNotice_Layout layout = new UI_ExternalNotice_Layout();

        List<UI_ExternalNotice_Layout.NoticeItem> noticeItems = new List<UI_ExternalNotice_Layout.NoticeItem>();

        UnityWebRequest unityWebRequest;

        public AssetDependencies assetDependencies;

        protected override void OnLoaded()
        {
            noticeItems.Clear();

            layout.Init(transform);
            layout.RegisterEvents(this);
            assetDependencies = layout.transform.GetComponent<AssetDependencies>();
        }

        protected override void ProcessEvents(bool toRegister)
        {            
            Sys_ExternalNotice.Instance.eventEmitter.Handle<Sys_ExternalNotice.NoticeData>(Sys_ExternalNotice.EEvents.ChooseNoticeItem, OnChooseNoticeItem, toRegister);
        }

        protected override void OnShow()
        {
            RefreshToggles();
            if (noticeItems != null && noticeItems.Count > 0)
            {
                noticeItems[0].toggle.SetSelected(true, true);
            }
        }

        protected override void OnHide()
        {
            layout.noticeImage.sprite = (Sprite)(assetDependencies.mCustomDependencies[0]);
            layout.noticeText.text = string.Empty;
        }

        void RefreshToggles()
        {
            layout.leftTabContent.DestoryAllChildren();
            noticeItems.Clear();

            for (int index = 0, len = Sys_ExternalNotice.Instance.noticeDatas.Count; index < len; index++)
            {
                GameObject noticeGo = GameObject.Instantiate(layout.noticeItemPrefab);
                noticeGo.SetActive(true);
                noticeGo.transform.SetParent(layout.leftTabContent.transform, false);
                UI_ExternalNotice_Layout.NoticeItem noticeItem = new UI_ExternalNotice_Layout.NoticeItem(noticeGo, Sys_ExternalNotice.Instance.noticeDatas[index]);
                noticeItems.Add(noticeItem);
            }
        }

        void OnChooseNoticeItem(Sys_ExternalNotice.NoticeData noticeData)
        {
            TextHelper.SetText(layout.noticeText, noticeData.Content);

            if (Sys_ExternalNotice.Instance.cacheTextures.ContainsKey(noticeData.ImageUrl))
            {
                Sprite sprite = Sprite.Create(Sys_ExternalNotice.Instance.cacheTextures[noticeData.ImageUrl], new Rect(0, 0, Sys_ExternalNotice.Instance.cacheTextures[noticeData.ImageUrl].width, Sys_ExternalNotice.Instance.cacheTextures[noticeData.ImageUrl].height), new Vector2(0, 0f));
                layout.noticeImage.sprite = sprite;
            }
            else
            {
                unityWebRequest?.Abort();
                unityWebRequest?.Dispose();
                unityWebRequest = UnityWebRequestTexture.GetTexture(noticeData.ImageUrl);
                UnityWebRequestAsyncOperation requestAsyncOperation = unityWebRequest.SendWebRequest();
                requestAsyncOperation.completed += OnGetTexture;
            }
        }

        void OnGetTexture(AsyncOperation operation)
        {
            if (unityWebRequest == null)
                return;

            if (unityWebRequest.isHttpError || unityWebRequest.isNetworkError)
            {
                Debug.LogError(unityWebRequest.error);
                layout.noticeImage.sprite = (Sprite)(assetDependencies.mCustomDependencies[0]);
            }
            else
            {
                if (unityWebRequest.isDone)
                {
                    Texture2D texture = ((DownloadHandlerTexture)unityWebRequest.downloadHandler).texture;
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0f));
                    layout.noticeImage.sprite = sprite;
                    Sys_ExternalNotice.Instance.cacheTextures[unityWebRequest.url] = texture;
                }
            }
        }

        public void OnClickCloseButton()
        {
            CloseSelf();
        }
    }
}
