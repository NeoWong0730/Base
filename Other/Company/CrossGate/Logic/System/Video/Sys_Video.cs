using Packet;
using Logic.Core;
using System.Collections.Generic;
using Net;
using Lib.Core;
using Table;
using Google.Protobuf;
using System;
using Framework;

namespace Logic
{
    public enum EVideoViewType
    {
        None = 0,
        MonthVideoView=1,      //本月最佳
        RecentlyVideoView=2,    //最近录像
        PersonalCenterView=3,     //个人中心
        Max =4,
    }

    public class ClientVideo
    {
        public ulong video;
        public uint function;
        public uint time;
        public VideoWhere where;
        public VideoBaseBrief baseBrif;
        public VideoAuthorBrief authorBrif=new VideoAuthorBrief ();
        public VideoMutualBrief muntalBrif;
        public VideoPlayerList players;
        public float score;

        public void SetScore()
        {
            if (CSVParam.Instance.TryGetValue(1322, out CSVParam.Data data)&& muntalBrif!=null)
            {
                string[] str = data.str_value.Split('|');
                float.TryParse(str[0], out float likeNum);
                float.TryParse(str[1], out float playNum);
                float.TryParse(str[2], out float bulletNum);
                score = muntalBrif.Bullet * bulletNum + muntalBrif.Like * likeNum + muntalBrif.Play * playNum;
            }
        }
    }

    public partial class Sys_Video : SystemModuleBase<Sys_Video>
    {
        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public VideoPersonCenter videoPersonCenter; 
        public MineUploadList mineUploadList;
        public MineCollectList mineCollectList;
        public MineLocalList mineLocalList;
        public MonthBestList monthBestList;
        public LastUploadList lastUploadList;
        public uint personalCenterExpireTime;
        public uint localExpireTime;
        public uint uploadExpireTime;
        public uint collectExpireTime;
        public uint nextShareTime; //下次分享非世界频道到期时间
        public uint nextWorldTime; //下次分享世界频道到期时间

        public bool isOpenBullet=true;
        public List<ClientVideo> uploadList = new List<ClientVideo>();
        public List<ClientVideo> unuploadList = new List<ClientVideo>();
        public List<ClientVideo> collectList = new List<ClientVideo>();
        public List<ClientVideo> recentlyList = new List<ClientVideo>();
        public List<ClientVideo> monthList = new List<ClientVideo>();
        public List<ClientVideo> tempList = new List<ClientVideo>();
        public List<ClientVideo> likeList = new List<ClientVideo>();

        public List<uint> challengeTypeList = new List<uint>();
        public List<uint> pkTypeList = new List<uint>();
        
        public float likeNum;
        public float playNum;
        public float bulletNum;
        private int personCenterMaxCount = 20;
        private int monthTypeMaxCount = 30;
        public VideoButton curVideoType = VideoButton.Local;

        public enum EEvents : int
        {
            OnSelectViewType,
            OnOpenPersonCenter,
            OnOpenMonthBest,
            OnLastUpload,
            OnSelectVideoType,    //分类筛选
            OnUpdateLocalVideo,
            OnUpdateUploadVideo,
            OnUpdateCollectVideo,
            OnVideoBaseDetail,   //基础详情更新
            OnVideoMutualDetail, //交互详情更新
            OnMineMutualInfo, // 个人交互信息更新
            OnChangeBulletState,  //弹幕开启关闭更新
            OnPlayVideoSuccess,   //成功播放录像
            OnReceiveBullet,     //收到弹幕
            OnSendBulletSuccess,    //成功发送弹幕
            OnPlaySelfBullet,    //播放自己刚发送的弹幕
        }

        public override void OnLogin()
        { 
            base.OnLogin();
            SetTypeListData();

            Sys_Video.Instance.OpenPersonCenterReq(Sys_Role.Instance.RoleId);
            Sys_Video.Instance.MonthBestListReq(0, 0);
            Sys_Video.Instance.LastUploadListReq(0, 0);
            if (Sys_Video.Instance.unuploadList.Count != 0)
            {
                Sys_Video.Instance.VideoBaseDetailReq(VideoButton.Local);
            }
        }

        public override void OnLogout()
        {
            base.OnLogout();
            curVideoType = VideoButton.Local;
            DefaultData();
        }

        #region 协议部分
        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.OpenPersonCenterReq, (ushort)CmdVideo.OpenPersonCenterRes, OnOpenPersonCenterRes, CmdVideoOpenPersonCenterRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.UploadVideoReq, (ushort)CmdVideo.UploadVideoRes, OnUploadVideoRes, CmdVideoUploadVideoRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.MineUploadListReq, (ushort)CmdVideo.MineUploadListRes, OnMineUploadListRes, CmdVideoMineUploadListRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.MineCollectListReq, (ushort)CmdVideo.MineCollectListRes, OnMineCollectListRes, CmdVideoMineCollectListRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.MineLocalListReq, (ushort)CmdVideo.MineLocalListRes, OnMineLocalListRes, CmdVideoMineLocalListRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.MonthBestListReq, (ushort)CmdVideo.MonthBestListRes, OnMonthBestListRes, CmdVideoMonthBestListRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.LastUploadListReq, (ushort)CmdVideo.LastUploadListRes, OnLastUploadListRes, CmdVideoLastUploadListRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.ShareChannelReq, (ushort)CmdVideo.ShareChannelRes, OnShareChannelRes, CmdVideoShareChannelRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.PlayVideoReq, (ushort)CmdVideo.PlayVideoRes, OnPlayVideoRes, CmdVideoPlayVideoRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.CollectVideoReq, (ushort)CmdVideo.CollectVideoRes, OnCollectVideoRes, CmdVideoCollectVideoRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.CancelCollectReq, (ushort)CmdVideo.CancelCollectRes, OnCancelCollectRes, CmdVideoCancelCollectRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.LikeVideoReq, (ushort)CmdVideo.LikeVideoRes, OnLikeVideoRes, CmdVideoLikeVideoRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.CancelLikeReq, (ushort)CmdVideo.CancelLikeRes, OnCancelLikeRes, CmdVideoCancelLikeRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.OpenBulletReq, (ushort)CmdVideo.OpenBulletRes, OnOpenBulletRes, CmdVideoOpenBulletRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.CloseBulletReq, (ushort)CmdVideo.CloseBulletRes, OnCloseBulletRes, CmdVideoCloseBulletRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.SendBulletReq, (ushort)CmdVideo.SendBulletRes, OnSendBulletRes, CmdVideoSendBulletRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.DeleteLocalReq, (ushort)CmdVideo.DeleteLocalRes, OnDeleteLocalRes, CmdVideoDeleteLocalRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.DeleteUploadReq, (ushort)CmdVideo.DeleteUploadRes, OnDeleteUploadRes, CmdVideoDeleteUploadRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.MineMutualInfoReq, (ushort)CmdVideo.MineMutualInfoRes, OnMineMutualInfoRes, CmdVideoMineMutualInfoRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.VideoBaseDetailReq, (ushort)CmdVideo.VideoBaseDetailRes, OnVideoBaseDetailRes, CmdVideoVideoBaseDetailRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.VideoMutualDetailReq, (ushort)CmdVideo.VideoMutualDetailRes, OnVideoMutualDetailRes, CmdVideoVideoMutualDetailRes.Parser);
        }

        public void OpenPersonCenterReq(ulong roleId)
        {
            CmdVideoOpenPersonCenterReq req = new CmdVideoOpenPersonCenterReq();
            req.RoleId = roleId;
            NetClient.Instance.SendMessage((ushort)CmdVideo.OpenPersonCenterReq, req);
        }

        private void OnOpenPersonCenterRes(NetMsg msg)
        {
            CmdVideoOpenPersonCenterRes res = NetMsgUtil.Deserialize<CmdVideoOpenPersonCenterRes>(CmdVideoOpenPersonCenterRes.Parser, msg);
            personalCenterExpireTime = res.ExpireTime;
            videoPersonCenter = res.Person;
            localExpireTime = res.Person.Local.TokenExpire;
            uploadExpireTime = res.Person.Upload.TokenExpire;
            collectExpireTime = res.Person.Collect.TokenExpire;
            nextWorldTime = res.Person.WorldTime;
            isOpenBullet = res.Person.BulletSwitch;
            for (int i = 0; i < videoPersonCenter.Upload.Videos.Count; ++i)
            {
                bool isInList = videoPersonCenter.Upload.Videos[i].Abrief != null && VideoIsInUpload(videoPersonCenter.Upload.Videos[i].VideoId, videoPersonCenter.Upload.Videos[i].Abrief.Author);
                if (!isInList)
                {
                    ClientVideo clientVideo = new ClientVideo();
                    clientVideo.where = VideoWhere.Upload;
                    clientVideo.video = videoPersonCenter.Upload.Videos[i].VideoId;
                    clientVideo.function = videoPersonCenter.Upload.Videos[i].Function;
                    clientVideo.baseBrif = videoPersonCenter.Upload.Videos[i].Bbrief;
                    clientVideo.authorBrif = videoPersonCenter.Upload.Videos[i].Abrief;
                    clientVideo.authorBrif.Author = Sys_Role.Instance.Role.RoleId;
                    clientVideo.muntalBrif = videoPersonCenter.Upload.Videos[i].Mbrief;
                    clientVideo.players = videoPersonCenter.Upload.Videos[i].Players;
                    clientVideo.time = (uint)(videoPersonCenter.Upload.Videos[i].VideoId >> 32);
                    uploadList.Add(clientVideo);
                }
            }
            for (int i = 0; i < videoPersonCenter.Local.Videos.Count; ++i)
            {
                bool isInList = VideoIsInLocalList(videoPersonCenter.Local.Videos[i].VideoId);
                if (!isInList)
                {
                    ClientVideo clientVideo = new ClientVideo();
                    clientVideo.where = VideoWhere.Local;
                    clientVideo.video = videoPersonCenter.Local.Videos[i].VideoId;
                    clientVideo.baseBrif = videoPersonCenter.Local.Videos[i].Bbrief;
                    clientVideo.time = (uint)(videoPersonCenter.Local.Videos[i].VideoId >> 32);
                    clientVideo.players = videoPersonCenter.Local.Videos[i].Players;
                    clientVideo.authorBrif = new VideoAuthorBrief();
                    clientVideo.authorBrif.Author = 0;
                    unuploadList.Add(clientVideo);
                }
            }
            for (int i = 0; i < videoPersonCenter.Collect.Videos.Count; ++i)
            {
                bool isInList = videoPersonCenter.Collect.Videos[i].Abrief!=null&& VideoIsInCollect(videoPersonCenter.Collect.Videos[i].VideoId, videoPersonCenter.Collect.Videos[i].Abrief.Author);
                if (!isInList)
                {
                    ClientVideo clientVideo = new ClientVideo();
                    clientVideo.where = VideoWhere.Collect;
                    clientVideo.video = videoPersonCenter.Collect.Videos[i].VideoId;
                    clientVideo.function = videoPersonCenter.Collect.Videos[i].Function;
                    clientVideo.baseBrif = videoPersonCenter.Collect.Videos[i].Bbrief;
                    clientVideo.time = (uint)(videoPersonCenter.Collect.Videos[i].VideoId >> 32);
                    clientVideo.authorBrif.Author = videoPersonCenter.Collect.Videos[i].Author;
                    if (videoPersonCenter.Collect.Videos[i].Abrief != null)
                    {
                        clientVideo.authorBrif = videoPersonCenter.Collect.Videos[i].Abrief;
                    }
                    clientVideo.muntalBrif = videoPersonCenter.Collect.Videos[i].Mbrief;
                    clientVideo.players = videoPersonCenter.Collect.Videos[i].Players;
                    collectList.Add(clientVideo);
                }
            }
            SetUploadVideoSort();
            SetLocalVideoSort();
            SetCollectVideoSort();
            eventEmitter.Trigger(EEvents.OnOpenPersonCenter);
            OpenBulletReq(Sys_Role.Instance.RoleId);
        }

        public void MonthBestListReq(ulong roleId, uint type)
        {
            CmdVideoMonthBestListReq req = new CmdVideoMonthBestListReq();
            req.RoleId = roleId;
            req.VideoType = type;
            NetClient.Instance.SendMessage((ushort)CmdVideo.MonthBestListReq, req);
        }

        private void OnMonthBestListRes(NetMsg msg)
        {
            CmdVideoMonthBestListRes res = NetMsgUtil.Deserialize<CmdVideoMonthBestListRes>(CmdVideoMonthBestListRes.Parser, msg);
            monthBestList = res.Mbest;
            monthList.Clear();
            for (int i = 0; i < monthBestList.Videos.Count; ++i)
            {
                ClientVideo clientVideo = new ClientVideo();
                clientVideo.time = monthBestList.Videos[i].Abrief == null ? 0 : monthBestList.Videos[i].Abrief.UploadTime;
                if (clientVideo.time != 0 && clientVideo.time < Sys_Time.Instance.GetServerTime())
                {
                    uint timepass = Sys_Time.Instance.GetServerTime() - clientVideo.time;
                    float days = timepass / 86400f;
                    if (days <= 30)
                    {
                        clientVideo.where = VideoWhere.Mbest;
                        clientVideo.video = monthBestList.Videos[i].VideoId;
                        clientVideo.function = monthBestList.Videos[i].Function;
                        clientVideo.baseBrif = monthBestList.Videos[i].Bbrief;
                        clientVideo.authorBrif = monthBestList.Videos[i].Abrief;
                        clientVideo.muntalBrif = monthBestList.Videos[i].Mbrief;
                        clientVideo.players = monthBestList.Videos[i].Players;
                        clientVideo.SetScore();
                        monthList.Add(clientVideo);
                    }
                    if ((monthBestList.Videos[i].Function == 2 || monthBestList.Videos[i].Function == 6) && !isLiked(monthBestList.Videos[i].VideoId, monthBestList.Videos[i].Abrief.Author))
                    {
                        likeList.Add(clientVideo);
                    }
                }
            }
            SetRank();
            if (res.VideoType != 0 && Sys_Video.Instance.monthList.Count > monthTypeMaxCount)
            {
                int deleteCount = Sys_Video.Instance.monthList.Count - monthTypeMaxCount;
                Sys_Video.Instance.monthList.RemoveRange(monthTypeMaxCount, deleteCount);
            }
            eventEmitter.Trigger(EEvents.OnOpenMonthBest);
        }

        public void LastUploadListReq(ulong roleId,uint type)
        {
            CmdVideoLastUploadListReq req = new CmdVideoLastUploadListReq();
            req.RoleId = roleId;
            req.VideoType = type;
            NetClient.Instance.SendMessage((ushort)CmdVideo.LastUploadListReq, req);
        }

        private void OnLastUploadListRes(NetMsg msg)
        {
            CmdVideoLastUploadListRes res = NetMsgUtil.Deserialize<CmdVideoLastUploadListRes>(CmdVideoLastUploadListRes.Parser, msg);
            lastUploadList = res.Lupload;
            recentlyList.Clear();
            for (int i = 0; i < lastUploadList.Videos.Count; ++i)
            {
                ClientVideo clientVideo = new ClientVideo();
                clientVideo.where = VideoWhere.Lastup;
                clientVideo.video = lastUploadList.Videos[i].VideoId;
                clientVideo.function = lastUploadList.Videos[i].Function;
                clientVideo.baseBrif = lastUploadList.Videos[i].Bbrief;
                clientVideo.time = lastUploadList.Videos[i].Abrief == null ? 0 : lastUploadList.Videos[i].Abrief.UploadTime;
                clientVideo.authorBrif = lastUploadList.Videos[i].Abrief;
                clientVideo.muntalBrif = lastUploadList.Videos[i].Mbrief;
                clientVideo.players = lastUploadList.Videos[i].Players;
                recentlyList.Add(clientVideo);
                if ((lastUploadList.Videos[i].Function == 2 || lastUploadList.Videos[i].Function == 6) && !isLiked(lastUploadList.Videos[i].VideoId, lastUploadList.Videos[i].Abrief.Author))
                {
                    likeList.Add(clientVideo);
                }
            }
            SetRecentVideoSort();
            int maxCount = 30;
            if (res.VideoType!=0&& Sys_Video.Instance.recentlyList.Count > maxCount)
            {
                int deleteCount = Sys_Video.Instance.recentlyList.Count - maxCount;
                Sys_Video.Instance.recentlyList.RemoveRange(maxCount, deleteCount);
            }
            eventEmitter.Trigger(EEvents.OnLastUpload);
        }

        public void ShareChannelReq(ulong roleId, ClientShareVideo clientShareVideo, List<ulong> firends=null)
        {
            bool canShareOtherChannel =(clientShareVideo.Channel != ShareChannelType.ShareChannelWorld && (nextShareTime == 0 || Sys_Time.Instance.GetServerTime() >= nextShareTime));
            bool canShareWorldChannel = (clientShareVideo.Channel == ShareChannelType.ShareChannelWorld &&Sys_Time.Instance.GetServerTime() >= nextWorldTime);

            if (canShareOtherChannel|| canShareWorldChannel)
            {
                CmdVideoShareChannelReq req = new CmdVideoShareChannelReq();
                req.RoleId = roleId;
                req.Video = clientShareVideo;
                if (firends != null)
                {
                    req.Need = new ShareNeedInfo();
                    for (int i = 0; i < firends.Count; ++i)
                    {
                        req.Need.FriendIds.Add(firends[i]);
                    }
                }
                NetClient.Instance.SendMessage((ushort)CmdVideo.ShareChannelReq, req);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(591000028));
            }
        }

        private void OnShareChannelRes(NetMsg msg)
        {
            CmdVideoShareChannelRes res = NetMsgUtil.Deserialize<CmdVideoShareChannelRes>(CmdVideoShareChannelRes.Parser, msg);
            nextShareTime = res.Video.NextShareTime;
            nextWorldTime = res.Video.NextWorldTime;
            OnShare((uint)res.Video.Video.Channel, res);
        }

        private string ParseFriendsChatString(ServerShareVideo serverShareVideo)
        {
            uint colorIndex = 0;
            string color = Constants.gChatColors_Item[colorIndex];

            ulong videoId = serverShareVideo.Video.VideoId;
            ulong authorId = serverShareVideo.Video.Author;
            System.Text.StringBuilder tempStringBuilder = Lib.Core.StringBuilderPool.GetTemporary();
            tempStringBuilder.Append(EmojiTextHelper.gColorStart);
            tempStringBuilder.Append(Constants.gChatColors_Item[colorIndex]);
            tempStringBuilder.Append(EmojiTextHelper.gVideo);
            tempStringBuilder.Append(string.Format("{0}|{1}", videoId, authorId));
            tempStringBuilder.Append('>');
            tempStringBuilder.Append('[');
            if(CSVVideoType.Instance.TryGetValue(serverShareVideo.Video.VideoType,out CSVVideoType.Data data)&&data!=null)
            {
                tempStringBuilder.Append(LanguageHelper.GetTextContent(data.Name));
                tempStringBuilder.Append(" - ");
            }
            tempStringBuilder.Append(serverShareVideo.Video.Title.ToStringUtf8());
            tempStringBuilder.Append(EmojiTextHelper.gColorEnd);
            string rlt = Lib.Core.StringBuilderPool.ReleaseTemporaryAndToString(tempStringBuilder);
            return rlt;
        }

        private string ParseChatString(ServerShareVideo serverShareVideo)
        {
            uint colorIndex = 0;
            string color = Constants.gChatColors_Item[colorIndex];

            ulong videoId = serverShareVideo.Video.VideoId;
            ulong authorId = serverShareVideo.Video.Author;

            System.Text.StringBuilder tempStringBuilder = Lib.Core.StringBuilderPool.GetTemporary();
            tempStringBuilder.Append(EmojiTextHelper.gColorStart);
            tempStringBuilder.Append(Constants.gChatColors_Item[colorIndex]);
            tempStringBuilder.Append(EmojiTextHelper.gVideo);
            tempStringBuilder.Append(string.Format("{0}|{1}", videoId, authorId));
            tempStringBuilder.Append('>');
            tempStringBuilder.Append('[');
            tempStringBuilder.Append(LanguageHelper.GetTextContent(591000060, serverShareVideo.ShareName.ToStringUtf8()));
            if (CSVVideoType.Instance.TryGetValue(serverShareVideo.Video.VideoType, out CSVVideoType.Data data) && data != null)
            {
                tempStringBuilder.Append(LanguageHelper.GetTextContent(data.Name));
                tempStringBuilder.Append(" - ");
            }
            tempStringBuilder.Append(serverShareVideo.Video.Title.ToStringUtf8());
            tempStringBuilder.Append(EmojiTextHelper.gColorEnd);
            string rlt = Lib.Core.StringBuilderPool.ReleaseTemporaryAndToString(tempStringBuilder);
            return rlt;
        }

        //ShareId senderRoleID
        //FriendIds targetID
        private void OnShare(uint channel, CmdVideoShareChannelRes res)
        {
            //检测聊天功能是否开启
            Sys_Chat.ChatChannelData data = Sys_Chat.Instance.GetChannelData((ChatType)channel);
            if (data == null)
                return;

            if (data.nLvLimit > Sys_Role.Instance.Role.Level)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10984, data.nLvLimit.ToString()));
                return;
            }
            string content = ParseChatString(res.Video);
            // 1-好友  2-家族  3-世界 4-队伍
            switch (channel)
            {
                case 1:
                    string contentFriends = ParseFriendsChatString(res.Video);
                    Sys_Society.Instance.InsertShareVideo(res.Video.ShareId, res.Need.FriendIds, contentFriends, Sys_Time.Instance.GetServerTime(), res.Video.ChatFrame, res.Video.ChatText);
                    break;
                case 2:
                    Sys_Chat.Instance.PushMessage(ChatType.Guild, null, content);
                    if (res.Video.ShareId == Sys_Role.Instance.Role.RoleId)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011214));
                    }
                    break;
                case 3:
                    Sys_Chat.Instance.PushMessage(ChatType.World, null, content);
                    if (res.Video.ShareId == Sys_Role.Instance.Role.RoleId)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011212));
                    }
                    break;
                case 4:
                    Sys_Chat.Instance.PushMessage(ChatType.Team, null, content);
                    if (res.Video.ShareId == Sys_Role.Instance.Role.RoleId)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5880));
                    }
                    break;
                default:
                    break;
            }
        }

        public void PlayVideoReq(ulong roleId, ulong videoId, ulong authorId)
        {
            CmdVideoPlayVideoReq req = new CmdVideoPlayVideoReq();
            req.RoleId = roleId;
            req.VideoId = videoId;
            req.AuthorId = authorId;
            NetClient.Instance.SendMessage((ushort)CmdVideo.PlayVideoReq, req);
        }

        private void OnPlayVideoRes(NetMsg msg)
        {
            CmdVideoPlayVideoRes res = NetMsgUtil.Deserialize<CmdVideoPlayVideoRes>(CmdVideoPlayVideoRes.Parser, msg);
        }

        public void CollectVideoReq(ulong roleId, ulong videoId, ulong authorId, VideoWhere videoWhere)
        {
            CmdVideoCollectVideoReq req = new CmdVideoCollectVideoReq();
            req.RoleId = roleId;
            req.VideoId = videoId;
            req.AuthorId = authorId;
            req.Where = videoWhere;
            NetClient.Instance.SendMessage((ushort)CmdVideo.CollectVideoReq, req);
        }

        private void OnCollectVideoRes(NetMsg msg)
        {
            CmdVideoCollectVideoRes res = NetMsgUtil.Deserialize<CmdVideoCollectVideoRes>(CmdVideoCollectVideoRes.Parser, msg);
            if (res.VideoId != 0 && res.RoleId == Sys_Role.Instance.Role.RoleId&&!isCollected(res.VideoId,res.AuthorId))
            {
                ClientVideo video = new ClientVideo();
                video.video = res.VideoId;
                video.time = (uint)( res.VideoId >> 32);
                video.where = VideoWhere.Collect;
                video.authorBrif.Author = res.AuthorId;
                collectList.Add(video);
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(591000011));
                eventEmitter.Trigger(EEvents.OnUpdateCollectVideo);
                SetCollectVideoSort();
            }
        }

        public void CancelCollectReq(ulong roleId, ulong videoId, ulong authorId)
        {
            CmdVideoCancelCollectReq req = new CmdVideoCancelCollectReq();
            req.RoleId = roleId;
            req.VideoId = videoId;
            req.AuthorId = authorId;
            NetClient.Instance.SendMessage((ushort)CmdVideo.CancelCollectReq, req);
        }

        private void OnCancelCollectRes(NetMsg msg)
        {
            CmdVideoCancelCollectRes res = NetMsgUtil.Deserialize<CmdVideoCancelCollectRes>(CmdVideoCancelCollectRes.Parser, msg);
            if (res.VideoId != 0 && res.RoleId == Sys_Role.Instance.Role.RoleId)
            {
                for(int i=0;i< collectList.Count; ++i)
                {
                    if(collectList[i].video== res.VideoId&& collectList[i].authorBrif!=null&& collectList[i].authorBrif.Author == res.AuthorId)
                    {
                        collectList.Remove(collectList[i]);
                        break;
                    }
                }
                eventEmitter.Trigger(EEvents.OnUpdateCollectVideo);
            }
        }

        public void LikeVideoReq(ulong roleId, ulong videoId, ulong authorId, VideoWhere videoWhere)
        {
            CmdVideoLikeVideoReq req = new CmdVideoLikeVideoReq();
            req.RoleId = roleId;
            req.VideoId = videoId;
            req.AuthorId = authorId;
            req.Where = videoWhere;
            NetClient.Instance.SendMessage((ushort)CmdVideo.LikeVideoReq, req);
        }

        private void OnLikeVideoRes(NetMsg msg)
        {
            CmdVideoLikeVideoRes res = NetMsgUtil.Deserialize<CmdVideoLikeVideoRes>(CmdVideoLikeVideoRes.Parser, msg);
            if (res.VideoId != 0 && res.RoleId == Sys_Role.Instance.Role.RoleId && !isLiked(res.VideoId, res.AuthorId))
            {
                ClientVideo video = new ClientVideo();
                video.video = res.VideoId;
                video.authorBrif = new VideoAuthorBrief();
                video.authorBrif.Author = res.AuthorId;
                likeList.Add(video);
            }
        }

        public void CancelLikeReq(ulong roleId, ulong videoId, ulong authorId)
        {
            CmdVideoCancelLikeReq req = new CmdVideoCancelLikeReq();
            req.RoleId = roleId;
            req.VideoId = videoId;
            req.AuthorId = authorId;
            NetClient.Instance.SendMessage((ushort)CmdVideo.CancelLikeReq, req);
        }

        private void OnCancelLikeRes(NetMsg msg)
        {
            CmdVideoCancelLikeRes res = NetMsgUtil.Deserialize<CmdVideoCancelLikeRes>(CmdVideoCancelLikeRes.Parser, msg);
            if (res.VideoId != 0 && res.RoleId == Sys_Role.Instance.Role.RoleId)
            {
                for (int i = 0; i < likeList.Count; ++i)
                {
                    if (likeList[i].video == res.VideoId && likeList[i].authorBrif != null && likeList[i].authorBrif.Author == res.AuthorId)
                    {
                        likeList.Remove(likeList[i]);
                        break;
                    }
                }
            }
        }

        public void OpenBulletReq(ulong roleId)
        {
            CmdVideoOpenBulletReq req = new CmdVideoOpenBulletReq();
            req.RoleId = roleId;
            NetClient.Instance.SendMessage((ushort)CmdVideo.OpenBulletReq, req);
        }

        private void OnOpenBulletRes(NetMsg msg)
        {
            CmdVideoOpenBulletRes res = NetMsgUtil.Deserialize<CmdVideoOpenBulletRes>(CmdVideoOpenBulletRes.Parser, msg);
            if (Sys_Role.Instance.Role.RoleId == res.RoleId)
            {
                isOpenBullet = res.Status;
                eventEmitter.Trigger(EEvents.OnChangeBulletState);
            }
        }

        public void CloseBulletReq(ulong roleId)
        {
            CmdVideoCloseBulletReq req = new CmdVideoCloseBulletReq();
            req.RoleId = roleId;
            NetClient.Instance.SendMessage((ushort)CmdVideo.CloseBulletReq, req);
        }

        private void OnCloseBulletRes(NetMsg msg)
        {
            CmdVideoCloseBulletRes res = NetMsgUtil.Deserialize<CmdVideoCloseBulletRes>(CmdVideoCloseBulletRes.Parser, msg);
            if (Sys_Role.Instance.Role.RoleId == res.RoleId)
            {
                isOpenBullet = res.Status;
                eventEmitter.Trigger(EEvents.OnChangeBulletState);
            }
        }

        public void SendBulletReq(ulong authorId, ulong videoId, ClientBullet bullet)
        {
            CmdVideoSendBulletReq req = new CmdVideoSendBulletReq();
            req.AuthorId = authorId;
            req.VideoId = videoId;
            req.Bullet = bullet;
            NetClient.Instance.SendMessage((ushort)CmdVideo.SendBulletReq, req);
        }

        private void OnSendBulletRes(NetMsg msg)
        {
            CmdVideoSendBulletRes res = NetMsgUtil.Deserialize<CmdVideoSendBulletRes>(CmdVideoSendBulletRes.Parser, msg);
           if(res.Flag == true)
            {
                eventEmitter.Trigger(EEvents.OnSendBulletSuccess);
            }
        }

        public void UploadVideoReq(ulong roleId, string title, ulong videoId)
        {
            CmdVideoUploadVideoReq req = new CmdVideoUploadVideoReq();
            req.RoleId = roleId;
            req.VideoId = videoId;
            req.Title = ByteString.CopyFromUtf8(title);
            NetClient.Instance.SendMessage((ushort)CmdVideo.UploadVideoReq, req);
        }

        private void OnUploadVideoRes(NetMsg msg)
        {
            CmdVideoUploadVideoRes res = NetMsgUtil.Deserialize<CmdVideoUploadVideoRes>(CmdVideoUploadVideoRes.Parser, msg);
            ClientVideo video = new ClientVideo();
            for(int i = 0; i < unuploadList.Count; ++i)
              {
                if (unuploadList[i].video == res.VideoId)
                {
                    video = unuploadList[i];
                }
            }
            video.authorBrif.Author = Sys_Role.Instance.Role.RoleId;
            video.where = VideoWhere.Upload;
            unuploadList.Remove(video);
            uploadList.Add(video);
            SetUploadVideoSort();
            videoPersonCenter.Recv.Video = (uint) uploadList.Count;
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(591000017));
            eventEmitter.Trigger(EEvents.OnUpdateLocalVideo);
        }

        public void DeleteUploadReq(ulong roleId, ulong videoId)
        {
            CmdVideoDeleteUploadReq req = new CmdVideoDeleteUploadReq();
            req.RoleId = roleId;
            req.VideoId = videoId;
            NetClient.Instance.SendMessage((ushort)CmdVideo.DeleteUploadReq, req);
        }

        private void OnDeleteUploadRes(NetMsg msg)
        {
            CmdVideoDeleteUploadRes res = NetMsgUtil.Deserialize<CmdVideoDeleteUploadRes>(CmdVideoDeleteUploadRes.Parser, msg);
            ClientVideo video = new ClientVideo();
            for (int i = 0; i < uploadList.Count; ++i)
            {
                if (uploadList[i].video == res.VideoId)
                {
                    video = uploadList[i];
                }
            }
            uploadList.Remove(video);
            videoPersonCenter.Recv.Video = (uint)uploadList.Count;
            eventEmitter.Trigger(EEvents.OnUpdateUploadVideo);
        }

        public void DeleteLocalReq(ulong roleId, ulong videoId)
        {
            CmdVideoDeleteLocalReq req = new CmdVideoDeleteLocalReq();
            req.RoleId = roleId;
            req.VideoId = videoId;
            NetClient.Instance.SendMessage((ushort)CmdVideo.DeleteLocalReq, req);
        }

        private void OnDeleteLocalRes(NetMsg msg)
        {
            CmdVideoDeleteLocalRes res = NetMsgUtil.Deserialize<CmdVideoDeleteLocalRes>(CmdVideoDeleteLocalRes.Parser, msg);
            ClientVideo video = new ClientVideo();
            for (int i = 0; i < unuploadList.Count; ++i)
            {
                if (unuploadList[i].video == res.VideoId)
                {
                    video = unuploadList[i];
                }
            }
            unuploadList.Remove(video);
            eventEmitter.Trigger(EEvents.OnUpdateLocalVideo);
        }

        public void MineMutualInfoReq(ulong roleId)
        {
            CmdVideoMineMutualInfoReq req = new CmdVideoMineMutualInfoReq();
            req.RoleId = roleId;
            NetClient.Instance.SendMessage((ushort)CmdVideo.MineMutualInfoReq, req);
        }

        private void OnMineMutualInfoRes(NetMsg msg)
        {
            CmdVideoMineMutualInfoRes res = NetMsgUtil.Deserialize<CmdVideoMineMutualInfoRes>(CmdVideoMineMutualInfoRes.Parser, msg);
            videoPersonCenter.Recv = res.Recv;
            videoPersonCenter.Send = res.Send;        
            eventEmitter.Trigger(EEvents.OnMineMutualInfo);
        }

        public void MineUploadListReq(ulong roleId)
        {
            if (uploadExpireTime != 0 && Sys_Time.Instance.GetServerTime() < uploadExpireTime)
            {
                return;
            }
            CmdVideoMineUploadListReq req = new CmdVideoMineUploadListReq();
            req.RoleId = roleId;
            NetClient.Instance.SendMessage((ushort)CmdVideo.MineUploadListReq, req);
        }

        private void OnMineUploadListRes(NetMsg msg)
        {
            CmdVideoMineUploadListRes res = NetMsgUtil.Deserialize<CmdVideoMineUploadListRes>(CmdVideoMineUploadListRes.Parser, msg);
            mineUploadList = res.Upload;
            uploadExpireTime = mineUploadList.TokenExpire;
            uploadList.Clear();
            for (int i = 0; i < mineUploadList.Videos.Count; ++i)
            {
                ClientVideo clientVideo = new ClientVideo();
                clientVideo.where = VideoWhere.Upload;
                clientVideo.video = mineUploadList.Videos[i].VideoId;
                clientVideo.baseBrif = mineUploadList.Videos[i].Bbrief;
                clientVideo.time = (uint)(mineUploadList.Videos[i].VideoId >> 32);
                clientVideo.players = mineUploadList.Videos[i].Players;
                clientVideo.muntalBrif = mineUploadList.Videos[i].Mbrief;
                clientVideo.function = mineUploadList.Videos[i].Function;
                clientVideo.authorBrif = mineUploadList.Videos[i].Abrief;
                uploadList.Add(clientVideo);
            }
            SetUploadVideoSort();
            eventEmitter.Trigger(EEvents.OnUpdateUploadVideo);
        }

        public void MineLocalListReq(ulong roleId)
        {
            if (localExpireTime != 0 && Sys_Time.Instance.GetServerTime() < localExpireTime)
            {
                return;
            }
            CmdVideoMineLocalListReq req = new CmdVideoMineLocalListReq();
            req.RoleId = roleId;
            NetClient.Instance.SendMessage((ushort)CmdVideo.MineLocalListReq, req);
        }

        private void OnMineLocalListRes(NetMsg msg)
        {
            CmdVideoMineLocalListRes res = NetMsgUtil.Deserialize<CmdVideoMineLocalListRes>(CmdVideoMineLocalListRes.Parser, msg);
            mineLocalList = res.Local;
            localExpireTime = mineLocalList.TokenExpire;
            unuploadList.Clear();
            for (int i = 0; i < mineLocalList.Videos.Count; ++i)
            {
                ClientVideo clientVideo = new ClientVideo();
                clientVideo.where = VideoWhere.Local;
                clientVideo.video = mineLocalList.Videos[i].VideoId;
                clientVideo.baseBrif = mineLocalList.Videos[i].Bbrief;
                clientVideo.time = (uint)(mineLocalList.Videos[i].VideoId >> 32);
                clientVideo.players = mineLocalList.Videos[i].Players;
                unuploadList.Add(clientVideo);
            }
            SetLocalVideoSort();
            eventEmitter.Trigger(EEvents.OnUpdateLocalVideo);
        }

        public void MineCollectListReq(ulong roleId)
        {
            if(collectExpireTime!=0&& Sys_Time.Instance.GetServerTime()< collectExpireTime)
            {
                return;
            }
            CmdVideoMineCollectListReq req = new CmdVideoMineCollectListReq();
            req.RoleId = roleId;
            NetClient.Instance.SendMessage((ushort)CmdVideo.MineCollectListReq, req);
        }

        private void OnMineCollectListRes(NetMsg msg)
        {
            CmdVideoMineCollectListRes res = NetMsgUtil.Deserialize<CmdVideoMineCollectListRes>(CmdVideoMineCollectListRes.Parser, msg);
            mineCollectList = res.Collect;
            collectExpireTime = mineCollectList.TokenExpire;
            collectList.Clear();
            for (int i = 0; i < mineCollectList.Videos.Count; ++i)
            {
                if (mineCollectList.Videos[i].Abrief != null && mineCollectList.Videos[i].Abrief.Author != 0)
                {
                    ClientVideo clientVideo = new ClientVideo();
                    clientVideo.where = VideoWhere.Collect;
                    clientVideo.video = res.Collect.Videos[i].VideoId;
                    clientVideo.function = res.Collect.Videos[i].Function;
                    clientVideo.baseBrif = res.Collect.Videos[i].Bbrief;
                    clientVideo.time = (uint)(res.Collect.Videos[i].VideoId >> 32);
                    clientVideo.authorBrif.Author = res.Collect.Videos[i].Author;
                    if (res.Collect.Videos[i].Abrief != null)
                    {
                        clientVideo.authorBrif = res.Collect.Videos[i].Abrief;
                    }
                    clientVideo.muntalBrif = res.Collect.Videos[i].Mbrief;
                    clientVideo.players = res.Collect.Videos[i].Players;
                    collectList.Add(clientVideo);
                }
            }
            SetCollectVideoSort();
            eventEmitter.Trigger(EEvents.OnUpdateCollectVideo);
        }

        public void VideoBaseDetailReq(VideoButton type)
        {
            List<VideoUniqueInfo> list = new List<VideoUniqueInfo>();
            if (type == VideoButton.Upload)
            {
                for(int i = 0; i < uploadList.Count; ++i)
                {
                    if(uploadList[i].baseBrif==null|| uploadList[i].players == null|| uploadList[i].players.Players.Count==0)
                    {
                        VideoUniqueInfo item = new VideoUniqueInfo();
                        item.VideoId = uploadList[i].video;
                        item.AuthorId = Sys_Role.Instance.Role.RoleId;
                        list.Add(item);
                    }
                }
            }
            else if (type == VideoButton.Local)
            {
                for (int i = 0; i < unuploadList.Count; ++i)
                {
                    if (unuploadList[i].baseBrif == null || unuploadList[i].players == null || unuploadList[i].players.Players.Count == 0)
                    {
                        VideoUniqueInfo item = new VideoUniqueInfo();
                        item.VideoId = unuploadList[i].video;
                        list.Add(item);
                    }
                }
            }
            else if (type == VideoButton.Collect)
            {
                for (int i = 0; i < collectList.Count; ++i)
                {
                    if (collectList[i].baseBrif == null || collectList[i].authorBrif.Author == 0 || collectList[i].players == null || collectList[i].players.Players.Count == 0)
                    {
                        VideoUniqueInfo item = new VideoUniqueInfo();
                        item.VideoId = collectList[i].video;
                        item.AuthorId = collectList[i].authorBrif.Author;
                        list.Add(item);
                    }
                }
            }
            if (list.Count == 0)
            {
                return;
            }
            CmdVideoVideoBaseDetailReq req = new CmdVideoVideoBaseDetailReq();
            req.InfoList = new VideoUniqueInfoList();
            for (int i = 0; i < list.Count; ++i)
            {
                req.InfoList.List.Add(list[i]);
            }
            NetClient.Instance.SendMessage((ushort)CmdVideo.VideoBaseDetailReq, req);
        }

        private void OnVideoBaseDetailRes(NetMsg msg)
        {
            CmdVideoVideoBaseDetailRes res = NetMsgUtil.Deserialize<CmdVideoVideoBaseDetailRes>(CmdVideoVideoBaseDetailRes.Parser, msg);
            if (res.DetailList.List.Count == 0)
            {
                return;
            }
            for (int j = 0; j < unuploadList.Count; ++j)
            {
                if (unuploadList[j].video == res.DetailList.List[0].VideoId && 0 == res.DetailList.List[0].AuthorId)
                {
                    if (res.DetailList.List[0].Bbrief == null)
                    {
                        unuploadList.Remove(unuploadList[j]);
                        eventEmitter.Trigger(EEvents.OnUpdateLocalVideo);
                    }
                    else
                    {
                        unuploadList[j].baseBrif = res.DetailList.List[0].Bbrief;
                        unuploadList[j].players = res.DetailList.List[0].Players;
                        SetLocalVideoSort();
                        eventEmitter.Trigger(EEvents.OnVideoBaseDetail, unuploadList[j]);
                    }
                    break;
                }           
            }
            for (int j = 0; j < uploadList.Count; ++j)
            {
                if (uploadList[j].video == res.DetailList.List[0].VideoId && uploadList[j].authorBrif.Author== res.DetailList.List[0].AuthorId)
                {
                    if (res.DetailList.List[0].Bbrief == null)
                    {
                        uploadList.Remove(uploadList[j]);
                        eventEmitter.Trigger(EEvents.OnUpdateUploadVideo);
                    }
                    else
                    {
                        uploadList[j].baseBrif = res.DetailList.List[0].Bbrief;
                        uploadList[j].players = res.DetailList.List[0].Players;
                        eventEmitter.Trigger(EEvents.OnVideoBaseDetail, uploadList[j]);
                    }
                    break;
                } 
            }
            for (int j = 0; j < collectList.Count; ++j)
            {
                if (collectList[j].video == res.DetailList.List[0].VideoId&& collectList[j].authorBrif.Author== res.DetailList.List[0].AuthorId)
                {
                    if (res.DetailList.List[0].Bbrief == null)
                    {
                        collectList.Remove(collectList[j]);
                        eventEmitter.Trigger(EEvents.OnUpdateCollectVideo);
                    }
                    else
                    {
                        collectList[j].baseBrif = res.DetailList.List[0].Bbrief;
                        collectList[j].players = res.DetailList.List[0].Players;
                        eventEmitter.Trigger(EEvents.OnVideoBaseDetail, collectList[j]);
                    }
                    break;
                }
            }
        }

        public void VideoMutualDetailReq(VideoUniqueInfo info)
        {
            CmdVideoVideoMutualDetailReq req = new CmdVideoVideoMutualDetailReq();
            req.InfoList = new VideoUniqueInfoList();
            req.InfoList.List.Add(info);
            NetClient.Instance.SendMessage((ushort)CmdVideo.VideoMutualDetailReq, req);
        }

        private void OnVideoMutualDetailRes(NetMsg msg)
        {
            CmdVideoVideoMutualDetailRes res = NetMsgUtil.Deserialize<CmdVideoVideoMutualDetailRes>(CmdVideoVideoMutualDetailRes.Parser, msg);
            ClientVideo clientVideo = new ClientVideo();
            clientVideo.muntalBrif = res.DetailList.List[0].Mbrief;

            clientVideo.authorBrif = res.DetailList.List[0].Abrief;
            clientVideo.authorBrif.Author = res.DetailList.List[0].AuthorId;
            clientVideo.video = res.DetailList.List[0].VideoId;
            eventEmitter.Trigger(EEvents.OnVideoMutualDetail, clientVideo);
        }
        #endregion

        #region 接口

        public void DefaultData()
        {
            videoPersonCenter = null;
            mineUploadList = null;
            mineCollectList = null;
            mineLocalList = null;
            monthBestList = null;
            lastUploadList = null;
            uploadList.Clear();
            unuploadList.Clear();
            collectList.Clear();
            recentlyList.Clear();
            monthList.Clear();
            personalCenterExpireTime = 0;
            likeList.Clear();
            isOpenBullet = true;
        }

        private void SetRank()
        {
            if (Sys_Video.Instance.monthList.Count == 0)
            {
                return;
            }
            else
            {              
                monthList.Sort(SortMonthBest);
            }
        }

        private int SortMonthBest(ClientVideo a, ClientVideo b)
        {
            if (a.score.CompareTo(b.score) != 0)
            {
                return b.score.CompareTo(a.score);
            }
            else
            {
                return b.time.CompareTo(a.time);
            }
        }

        private void SetLocalVideoSort()
        {
            if (Sys_Video.Instance.unuploadList.Count == 0)
            {
                return;
            }
            else
            {
                unuploadList.Sort(SortLocal);
            }
            if (Sys_Video.Instance.unuploadList.Count > personCenterMaxCount)
            {
                int deleteCount = Sys_Video.Instance.unuploadList.Count - personCenterMaxCount;
                unuploadList.RemoveRange(personCenterMaxCount, deleteCount);
            }
        }

        private void SetUploadVideoSort()
        {
            if (Sys_Video.Instance.uploadList.Count == 0)
            {
                return;
            }
            else
            {
                uploadList.Sort(SortLocal);
            }
            if (Sys_Video.Instance.uploadList.Count > personCenterMaxCount)
            {
                int deleteCount = Sys_Video.Instance.uploadList.Count - personCenterMaxCount;
                uploadList.RemoveRange(personCenterMaxCount, deleteCount);
            }
        }

        private void SetCollectVideoSort()
        {
            if (Sys_Video.Instance.collectList.Count == 0)
            {
                return;
            }
            else
            {
                collectList.Sort(SortLocal);
            }
        }

        private void SetRecentVideoSort()
        {
            if (Sys_Video.Instance.recentlyList.Count == 0)
            {
                return;
            }
            else
            {
                recentlyList.Sort(SortLocal);
            }
        }

        private int SortLocal(ClientVideo a,ClientVideo b)
        {
            if (a.time.CompareTo(b.time) != 0)
            {
                return b.time.CompareTo(a.time);
            }
            else
            {
                return b.video.CompareTo(a.video);
            }
        }

        private void SetTypeListData()
        {
            challengeTypeList.Clear();
            pkTypeList.Clear();
            foreach(var item in CSVVideoType.Instance.GetAll())
            {
                if (item.Type == 1)
                {
                    challengeTypeList.Add(item.id);
                }
               else if (item.Type == 2)
                {
                    pkTypeList.Add(item.id);
                }
            }
        }

        public List<ClientVideo> SearchVideoListByAuthorId(ulong authorId, EVideoViewType type)
        {
            tempList.Clear();
            if (type == EVideoViewType.MonthVideoView)
            {
                for (int i = 0; i < monthList.Count; ++i)
                {
                    if (monthList[i].authorBrif.Author == authorId)
                    {
                        tempList.Add(monthList[i]);
                    }
                }
            }
            else if (type == EVideoViewType.RecentlyVideoView)
            {
                for (int i = 0; i < recentlyList.Count; ++i)
                {
                    if (recentlyList[i].authorBrif.Author == authorId)
                    {
                        tempList.Add(monthList[i]);
                    }
                }
            }
            return tempList;
        }

        public ClientVideo SearchVideoListById(ulong video,ulong authorId)
        {
            for (int i = 0; i < monthList.Count; ++i)
            {
                if (monthList[i].authorBrif.Author == authorId&& monthList[i].video==video)
                {
                    return monthList[i];
                }
            }
            for (int i = 0; i < recentlyList.Count; ++i)
            {
                if (recentlyList[i].authorBrif.Author == authorId && recentlyList[i].video == video)
                {
                    return recentlyList[i];
                }
            }
            for (int i = 0; i < uploadList.Count; ++i)
            {
                if (uploadList[i].authorBrif.Author == authorId && uploadList[i].video == video)
                {
                    return uploadList[i];
                }
            }
            return null;
        }

        public bool VideoIsInLocalList(ulong video)
        {
            for (int i = 0; i < unuploadList.Count; ++i)
            {
                if (unuploadList[i].video == video)
                {
                    return true;
                }
            }
            return false;
        }

        public bool VideoIsInUpload(ulong video, ulong authorId)
        {
            for (int i = 0; i < uploadList.Count; ++i)
            {
                if (uploadList[i].authorBrif.Author == authorId && uploadList[i].video == video)
                {
                    return true;
                }
            }
            return false;
        }

        public bool VideoIsInCollect(ulong video, ulong authorId)
        {
            for (int i = 0; i < collectList.Count; ++i)
            {
                if (collectList[i].authorBrif.Author == authorId && collectList[i].video == video)
                {
                    return true;
                }
            }
            return false;
        }

        public bool isCollected(ulong videoId,ulong authorId)
        {
            for( int i = 0; i < collectList.Count; ++i)
            {
                if (collectList[i].video == videoId&& collectList[i].authorBrif.Author== authorId)
                {
                    return true;
                }
            }
            return false;
        }

        public bool isLiked(ulong videoId, ulong authorId)
        {
            for (int i = 0; i < likeList.Count; ++i)
            {
                if (likeList[i].video == videoId && likeList[i].authorBrif.Author == authorId)
                {
                    return true;
                }
            }
            return false;
        }


        #endregion
    }
}
