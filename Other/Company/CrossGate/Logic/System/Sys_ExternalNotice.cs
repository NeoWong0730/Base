using Lib.AssetLoader;
using Logic.Core;
using Net;
using Packet;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using Lib.Core;

namespace Logic
{
    public class Sys_ExternalNotice : SystemModuleBase<Sys_ExternalNotice>
    {
        public class NoticeData
        {
            public string ID
            {
                get;
                set;
            }

            public string Title
            {
                get;
                set;
            }

            public string Content
            {
                get;
                set;
            }

            public string ImageUrl
            {
                get;
                set;
            }

            public uint Weight
            {
                get;
                set;
            }

            public uint StartTime
            {
                get;
                set;
            }

            public uint EndTime
            {
                get;
                set;
            }
        }

        public List<NoticeData> noticeDatas = new List<NoticeData>();

        private UnityWebRequest externalNoticeWebRequest;

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public Dictionary<string, Texture2D> cacheTextures = new Dictionary<string, Texture2D>();

        public enum EEvents
        {
            ChooseNoticeItem,
        }

        public void GetExternalNotice()
        {
            string url = string.Format("{0}?appId={1}&channel={2}&channel_id={3}", VersionHelper.DirNoticeUrl, SDKManager.GetAppid(), SDKManager.GetChannel(), SDKManager.GetPublishAppMarket());
            //string url = "http://192.168.1.15:7777/api/getloginnotice?appId=appId_test&channel=C&channel_id=channel_id_test07";
            Debug.Log($"url: {url}");

            externalNoticeWebRequest = UnityWebRequest.Get(url);
            UnityWebRequestAsyncOperation requestAsyncOperation = externalNoticeWebRequest.SendWebRequest();
            requestAsyncOperation.completed += this.OnExternalNoticeGot;
        }

        void OnExternalNoticeGot(AsyncOperation operation)
        {
            if (externalNoticeWebRequest.isHttpError || externalNoticeWebRequest.isNetworkError)
            {
                Debug.LogError(externalNoticeWebRequest.error);
            }
            else
            {
                noticeDatas.Clear();

                NetMsgUtil.TryDeserialize(LoginNoticeRes.Parser, externalNoticeWebRequest.downloadHandler.data, out LoginNoticeRes logicNoticeRes);
                if (logicNoticeRes != null)
                {
                    for (int index = 0, len = logicNoticeRes.NoticeList.Count; index < len; index++)
                    {
                        NoticeData noticeData = new NoticeData();
                        noticeData.ID = logicNoticeRes.NoticeList[index].NoticeId.ToStringUtf8();
                        noticeData.Title = logicNoticeRes.NoticeList[index].Title.ToStringUtf8();
                        noticeData.Content = logicNoticeRes.NoticeList[index].Content.ToStringUtf8();
                        noticeData.ImageUrl = logicNoticeRes.NoticeList[index].ImgUrl.ToStringUtf8();
                        noticeData.StartTime = logicNoticeRes.NoticeList[index].StartTime;
                        noticeData.EndTime = logicNoticeRes.NoticeList[index].EndTime;
                        noticeData.Weight = logicNoticeRes.NoticeList[index].Weight;
                        noticeDatas.Add(noticeData);
                    }
                }

                noticeDatas.Sort((a, b) =>
                {
                    if (a.Weight < b.Weight)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                });

                UIManager.CloseUI(EUIID.UI_BlockClickHttp);
                UIManager.OpenUI(EUIID.UI_ExternalNotice);
            }
        }
    }
}
