using GME;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Table;
using UnityEngine;

namespace Logic
{
    public partial class Sys_Chat : SystemModuleBase<Sys_Chat>
    {
        public const float gNoticeCD = 30;
        public static string gRecordPath = Framework.Consts.persistentDataPath + "/voiceTemp.silk";

        public readonly string sSpeechLanguage = "cmn-Hans-CN";
        public readonly string sTranslatelanguage = "cmn-Hans-CN";

        public class VoiceFileInfo
        {
            public string fileID;
            public string path;
            public int state;
        }

        public enum ERecodeVoice
        {
            None,
            /// <summary>
            /// 正在录音
            /// </summary>
            Recording,
            /// <summary>
            /// 等待录音完成
            /// </summary>
            WaitRecordComplete,
            /// <summary>
            /// 等待上传录音完成
            /// </summary>
            WaitUploadComplete,
            /// <summary>
            /// 等待文本转换完成 或 者审核结果
            /// </summary>
            WaitSpeechToText,
            /// <summary>
            /// 等待发起审核
            /// </summary>
            WaitAudit,
        }

        public enum EPlayVoice
        {
            None,
            /// <summary>
            /// 等待语音下载完成
            /// </summary>
            WaitDownloadFileComplete,
            /// <summary>
            /// 语音播放中
            /// </summary>
            Playing,
        }

        /// <summary>
        /// 语音的现在等级
        /// </summary>
        private uint nLvLimit = 0;
        /// <summary>
        /// 最大的录制时间
        /// </summary>
        public float fMaxRecordTime = 60;
        /// <summary>
        /// 语音的文本长度限制
        /// </summary>
        private int nVoiceContentLengthLimit = 100;
        public Queue<ChatContent> mVoiceFileIDs = new Queue<ChatContent>(256);

        public List<ulong> mInRoomRoles = new List<ulong>(5);
        public Dictionary<ulong, bool> mInRoomRoleInfos = new Dictionary<ulong, bool>(5);

        private Dictionary<string, VoiceFileInfo> _voiceFileInfos = new Dictionary<string, VoiceFileInfo>(32);
        private VoiceFileInfo[] _voiceFileInfosPool = new VoiceFileInfo[32];
        private int _nextFileIndex = 0;

        public bool isRoomEntered { get { return ITMGContext.GetInstance().IsRoomEntered(); } }
        public bool isWaitRoomEnterOrExit;


        public float fStartRecordTime;

        private ERecodeVoice _eRecordState;
        public ERecodeVoice eRecordState
        {
            get { return _eRecordState; }
            set
            {
                if (_eRecordState != value)
                {
                    _eRecordState = value;
                    RefreshBGMValue();
                    eventEmitter.Trigger(EEvents.VoiceRecordStateChange);
                }
            }
        }

        private bool _isVaildRecord;
        public bool IsVaildRecord
        {
            get { return _isVaildRecord; }
            set
            {
                if (_isVaildRecord != value)
                {
                    _isVaildRecord = value;
                    eventEmitter.Trigger(EEvents.VoiceRecordStateChange);
                }
            }
        }

        private string _currentFileID = null;
        public string sCurrentFileID
        {
            get
            {
                return _currentFileID;
            }

            set
            {
                if (_currentFileID != value)
                {
                    _currentFileID = value;
                    RefreshBGMValue();
                    eventEmitter.Trigger(EEvents.VoicePlayStateChange);
                }
            }
        }

        private float fLastNoticeTime = 0f;

        private ChatType eRecordChatType = ChatType.World;

        public bool IsSDKInited { get; private set; }

        /// <summary>
        /// 用于等待发起审核的计时器
        /// </summary>
        private Timer waitAuditTimer;

        /// <summary>
        /// 等待发起审核的语音id
        /// </summary>
        private string waitAuditFileid;

        /// <summary>
        /// 发起审核的次数
        /// </summary>
        private int auditCount;

        /// <summary>
        /// 当前录制的语音的时长
        /// </summary>
        private int waitAuditFileLength;

        private void _Init()
        {
            //聊天等级限制
            if (CSVParam.Instance.TryGetValue(931, out CSVParam.Data csvLvLimit))
            {
                uint.TryParse(csvLvLimit.str_value, out nLvLimit);
            }

            //语音最大录制时间
            if (CSVParam.Instance.TryGetValue(919, out CSVParam.Data csvMaxRecordTimeLimit))
            {
                List<float> csvRecordTimeLimits = ReadHelper.ReadArray_ReadFloat(csvMaxRecordTimeLimit.str_value, '|');
                if (csvRecordTimeLimits.Count > 1)
                {
                    fMaxRecordTime = csvRecordTimeLimits[1] * 0.001f;
                }
            }

            //语音转换的文本最大长度
            if (CSVParam.Instance.TryGetValue(1112, out CSVParam.Data csvVoiceContentLengthLimit))
            {
                int.TryParse(csvVoiceContentLengthLimit.str_value, out nVoiceContentLengthLimit);
            }

            EventDispatcher.Instance.AddEventListener((ushort)CmdTeam.GmeauthReq, (ushort)CmdTeam.GmeauthAck, OnGmeauthAck, CmdTeamGMEAuthAck.Parser);//鉴权
            EventDispatcher.Instance.AddEventListener((ushort)CmdTeam.GmenoticeReq, (ushort)CmdTeam.GmenoticeNtf, OnGmenoticeNtf, CmdTeamGMENoticeNtf.Parser);//推送
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.GmeroomInfoNtf, OnGmeroomInfoNtf, CmdTeamGMERoomInfoNtf.Parser);//广播房间信息
            OptionManager.Instance.eventEmitter.Handle<int>(OptionManager.EEvents.OptionFinalChange, OnOptionChange, true);
            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.TeamClear, OnTeamClear, true);
        }

        private void OnTeamClear()
        {
            ExitRoom();
        }

        private void OnGmeroomInfoNtf(NetMsg msg)
        {
            CmdTeamGMERoomInfoNtf ntf = NetMsgUtil.Deserialize<CmdTeamGMERoomInfoNtf>(CmdTeamGMENoticeNtf.Parser, msg);

            int oldRoleCount = mInRoomRoles.Count;
            bool hasChange = false;

            //先检统计离开的, 并且去除没有动的
            for (int i = mInRoomRoles.Count - 1; i >= 0; --i)
            {
                ulong id = mInRoomRoles[i];
                if (!ntf.RoleId.Contains(id))
                {
                    mInRoomRoles.RemoveAt(i);
                    TeamMem teamMem = Sys_Team.Instance.getTeamMem(id);
                    if (teamMem != null)
                    {
                        PushMessage(ChatType.Team, null, LanguageHelper.GetTextContent(10893, teamMem.Name.ToStringUtf8()));//string.Format("{0} 离开了队伍语音", teamMem.Name.ToStringUtf8())
                    }
                    hasChange = true;
                }
                else
                {
                    ntf.RoleId.Remove(id);
                }
            }

            //剩下加入的
            for (int i = 0; i < ntf.RoleId.Count; ++i)
            {
                ulong id = ntf.RoleId[i];
                mInRoomRoles.Add(id);
                TeamMem teamMem = Sys_Team.Instance.getTeamMem(id);
                if (teamMem != null)
                {
                    PushMessage(ChatType.Team, null, LanguageHelper.GetTextContent(10894, teamMem.Name.ToStringUtf8()));//string.Format("{0} 加入了队伍语音", teamMem.Name.ToStringUtf8())
                }
                hasChange = true;
            }

            //从无到有的时候弹出邀请窗口
            if (oldRoleCount == 0 && mInRoomRoles.Count > 0)
            {
                eventEmitter.Trigger(EEvents.EnterRoom);

                TeamMem teamMem = Sys_Team.Instance.getTeamMem(mInRoomRoles[0]);
                if (teamMem != null)
                {
                    PushMessage(ChatType.Team, null, LanguageHelper.GetTextContent(10895, teamMem.Name.ToStringUtf8()));//string.Format("{0} 开启了即时语音", teamMem.Name.ToStringUtf8())
                }
                _OnGmenoticeNtf();
            }

            if (hasChange)
            {
                eventEmitter.Trigger(EEvents.EndpointsUpdate);
            }
        }

        private void OnOptionChange(int optionID)
        {
            switch ((OptionManager.EOptionID)optionID)
            {
                case OptionManager.EOptionID.PlayerVoiceValue:
                case OptionManager.EOptionID.PlayerVoice:
                    {
                        ITMGContext.GetInstance().GetAudioCtrl().SetSpeakerVolume(OptionManager.Instance.GetBoolean(OptionManager.EOptionID.PlayerVoice) ? (int)Mathf.Lerp(0, 200, OptionManager.Instance.GetFloat(OptionManager.EOptionID.PlayerVoiceValue)) : 0);
                        ITMGContext.GetInstance().GetPttCtrl().SetSpeakerVolume(OptionManager.Instance.GetBoolean(OptionManager.EOptionID.PlayerVoice) ? (int)Mathf.Lerp(0, 200, OptionManager.Instance.GetFloat(OptionManager.EOptionID.PlayerVoiceValue)) : 0);
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnGmeauthAck(NetMsg msg)
        {
            CmdTeamGMEAuthAck info = NetMsgUtil.Deserialize<CmdTeamGMEAuthAck>(CmdTeamGMEAuthAck.Parser, msg);
            if (info.IsTeam)
            {
                Sys_Team.Instance.TeamInfo.Gmeauthbuffer = info.Authbuffer;
            }
            else
            {
                _InitSDK(info.Authbuffer.ToByteArray());
            }
        }

        private void _OnGmenoticeNtf()
        {
            fLastNoticeTime = Time.unscaledTime;

            //CmdTeamGMENoticeNtf info = NetMsgUtil.Deserialize<CmdTeamGMENoticeNtf>(CmdTeamGMENoticeNtf.Parser, msg);
            if (isRoomEntered || isWaitRoomEnterOrExit)
            {
                return;
            }
            else
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(10892); //"组队实时语音已开启";
                PromptBoxParameter.Instance.SetConfirm(true, EnterRoom);
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
            }
        }

        private void OnGmenoticeNtf(NetMsg msg)
        {
            _OnGmenoticeNtf();
        }

        private void _OnLogin()
        {
            CmdTeamGMEAuthReq req = new CmdTeamGMEAuthReq();
            req.IsTeam = false;
            NetClient.Instance.SendMessage((ushort)CmdTeam.GmeauthReq, req);
        }

        private void _InitSDK(byte[] authBuffer)
        {
            if (SDKManager.InitGMESDK(Sys_Role.Instance.RoleId) == 0)
            {
                IsSDKInited = true;

                //byte[] authBuffer = QAVAuthBuffer.GenAuthBuffer(1400525929, null, Sys_Role.Instance.RoleId.ToString(), "nPlhOoxqSoujIadP");                
                int ret = ITMGContext.GetInstance().GetPttCtrl().ApplyPTTAuthbuffer(authBuffer);
                if (ret != 0)
                {
                    DebugUtil.LogErrorFormat("ApplyPTTAuthbuffer Fail errorCode = {0}", ret.ToString());
                }

                ITMGContext.GetInstance().GetPttCtrl().SetMaxMessageLength((int)fMaxRecordTime * 1000);
                ITMGContext.GetInstance().GetPttCtrl().SetMicVolume(100);
                ITMGContext.GetInstance().GetPttCtrl().SetSpeakerVolume(OptionManager.Instance.GetBoolean(OptionManager.EOptionID.PlayerVoice) ? (int)Mathf.Lerp(0, 200, OptionManager.Instance.GetFloat(OptionManager.EOptionID.PlayerVoiceValue)) : 0);

                int retCode = (int)ITMGContext.GetInstance().CheckMicPermission();
                Debug.Log(string.Format("Check permission Code is {0}", retCode));

                ITMGContext.GetInstance().OnEnterRoomCompleteEvent += QAVEnterRoomComplete;
                ITMGContext.GetInstance().OnExitRoomCompleteEvent += QAVExitRoomComplete;
                ITMGContext.GetInstance().OnEndpointsUpdateInfoEvent += QAVEndpointsUpdateInfo;

                ITMGContext.GetInstance().GetPttCtrl().OnRecordFileComplete += OnRecordFileComplete;
                ITMGContext.GetInstance().GetPttCtrl().OnUploadFileComplete += OnUploadFileComplete;
                ITMGContext.GetInstance().GetPttCtrl().OnDownloadFileComplete += OnDownloadFileComplete;
                ITMGContext.GetInstance().GetPttCtrl().OnPlayFileComplete += OnPlayFileComplete;
                //ITMGContext.GetInstance().GetPttCtrl().OnSpeechToTextComplete += OnSpeechToTextComplete;
                ITMGContext.GetInstance().GetPttCtrl().OnSpeechToTextAuditComplete += OnSpeechToTextComplete;

                //ITMGContext.GetInstance().GetAudioCtrl().OnDeviceStateChangedEvent += OnDeviceStateChangedEvent;

            }
            else
            {
                IsSDKInited = false;
            }
        }

        private void _UninitSDK()
        {
            if (IsSDKInited)
            {
                ITMGContext.GetInstance().OnEnterRoomCompleteEvent -= QAVEnterRoomComplete;
                ITMGContext.GetInstance().OnExitRoomCompleteEvent -= QAVExitRoomComplete;
                ITMGContext.GetInstance().OnEndpointsUpdateInfoEvent -= QAVEndpointsUpdateInfo;

                ITMGContext.GetInstance().GetPttCtrl().OnRecordFileComplete -= OnRecordFileComplete;
                ITMGContext.GetInstance().GetPttCtrl().OnUploadFileComplete -= OnUploadFileComplete;
                ITMGContext.GetInstance().GetPttCtrl().OnDownloadFileComplete -= OnDownloadFileComplete;
                ITMGContext.GetInstance().GetPttCtrl().OnPlayFileComplete -= OnPlayFileComplete;
                //ITMGContext.GetInstance().GetPttCtrl().OnSpeechToTextComplete -= OnSpeechToTextComplete;
                ITMGContext.GetInstance().GetPttCtrl().OnSpeechToTextAuditComplete -= OnSpeechToTextComplete;
                //ITMGContext.GetInstance().GetAudioCtrl().OnDeviceStateChangedEvent -= OnDeviceStateChangedEvent;                
            }
            SDKManager.UninitGMESDK();
        }

        public void EnterRoom()
        {
            //if (!SDKManager.GetRealNameStatus())
            //{
            //    SDKManager.GetRealNameWebRequest();
            //    return;
            //}

            if (Sys_Role.Instance.Role.Level < nLvLimit)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10984, nLvLimit.ToString()));// 角色等级需到达{0}级，可使用该功能
                return;
            }

            if (!IsSDKInited)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000982));
                return;
            }

            if (isWaitRoomEnterOrExit)
                return;

            if (ITMGContext.GetInstance().IsRoomEntered())
                return;

            if (Sys_Team.Instance.HaveTeam)
            {
                isWaitRoomEnterOrExit = true;
                //byte[] authBuffer = QAVAuthBuffer.GenAuthBuffer(1400525929, Sys_Team.Instance.teamID.ToString(), Sys_Role.Instance.RoleId.ToString(), "nPlhOoxqSoujIadP");
                byte[] authBuffer = Sys_Team.Instance.TeamInfo.Gmeauthbuffer.ToByteArray();
                int ret = ITMGContext.GetInstance().EnterRoom(Sys_Team.Instance.teamID.ToString(), ITMGRoomType.ITMG_ROOM_TYPE_STANDARD, authBuffer);
                if (0 != ret)
                {
                    DebugUtil.LogErrorFormat("EnterRoom Fail {0}", ret);
                    isWaitRoomEnterOrExit = false;
                }
            }
            else
            {
                Sys_Chat.Instance.PushErrorTip(Chat_Error_NotHasTeam);
            }
        }

        public void ExitRoom()
        {
            if (isWaitRoomEnterOrExit)
            {
                return;
            }

            ITMGContext.GetInstance().ExitRoom();
        }

        public void Notice()
        {
            if (isRoomEntered)
            {
                if (Time.unscaledTime >= fLastNoticeTime + gNoticeCD)
                {
                    fLastNoticeTime = Time.unscaledTime;
                    //CmdTeamGMENoticeReq req = new CmdTeamGMENoticeReq();
                    NetClient.Instance.SendMessage((ushort)CmdTeam.GmenoticeReq, null);
                }
                else
                {
                    int t = Mathf.CeilToInt(fLastNoticeTime + gNoticeCD - Time.unscaledTime);
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10896, t.ToString()));//string.Format("刚才已有发起一轮邀请, {0}秒后可以发起邀请", t.ToString())
                }
            }
        }

        private void QAVEnterRoomComplete(int result, string error_info)
        {
            if (!Sys_Team.Instance.HaveTeam)
            {
                ITMGContext.GetInstance().ExitRoom();
                return;
            }

            isWaitRoomEnterOrExit = false;
            if (result == 0)
            {
                CmdTeamGMERoomEnterExitRpt rpt = new CmdTeamGMERoomEnterExitRpt();
                rpt.BEnter = true;
                NetClient.Instance.SendMessage((ushort)CmdTeam.GmeroomEnterExitRpt, rpt);

                ITMGContext.GetInstance().GetAudioCtrl().SetSpeakerVolume(OptionManager.Instance.GetBoolean(OptionManager.EOptionID.PlayerVoice) ? (int)Mathf.Lerp(0, 200, OptionManager.Instance.GetFloat(OptionManager.EOptionID.PlayerVoiceValue)) : 0);
                ITMGContext.GetInstance().GetAudioCtrl().EnableSpeaker(true);
                ITMGContext.GetInstance().GetAudioCtrl().EnableMic(true);

                RefreshBGMValue();
                eventEmitter.Trigger(EEvents.EnterRoom);
            }
            else
            {
                //可能是 鉴权过期
                if (result == QAVError.AV_ERR_AUTH_FIALD)
                {
                    CmdTeamGMEAuthReq req = new CmdTeamGMEAuthReq();
                    req.IsTeam = true;
                    NetClient.Instance.SendMessage((ushort)CmdTeam.GmeauthReq, req);
                }

                DebugUtil.LogErrorFormat("GME SDK Enter Room Fail errorCode({0}) {1}", result.ToString(), error_info);
            }
        }

        private void QAVExitRoomComplete()
        {
            CmdTeamGMERoomEnterExitRpt rpt = new CmdTeamGMERoomEnterExitRpt();
            rpt.BEnter = false;
            NetClient.Instance.SendMessage((ushort)CmdTeam.GmeroomEnterExitRpt, rpt);

            //信息置空
            mInRoomRoleInfos.Clear();

            RefreshBGMValue();
            eventEmitter.Trigger(EEvents.ExitRoom);
        }

        private void QAVEndpointsUpdateInfo(int eventID, int count, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] string[] openIdList)
        {
            //if (eventID == ITMGContext.EVENT_ID_ENDPOINT_ENTER)
            //{
            //    for (int i = 0; i < openIdList.Length; ++i)
            //    {
            //        if (!ulong.TryParse(openIdList[i], out ulong id))
            //            continue;
            //
            //        if (mInRoomRoles.Contains(id))
            //            continue;
            //
            //        mInRoomRoles.Add(id);
            //    }
            //    //当自己是第一个进入聊天室的时候 则广播消息给队员
            //    if (mInRoomRoles.Count == 1)
            //    {
            //        Notice();
            //    }
            //
            //}
            //else if (eventID == ITMGContext.EVENT_ID_ENDPOINT_EXIT)
            //{
            //    for (int i = 0; i < openIdList.Length; ++i)
            //    {
            //        if (ulong.TryParse(openIdList[i], out ulong id))
            //        {
            //            mInRoomRoles.Remove(id);
            //            mInRoomRoleInfos.Remove(id);
            //        }
            //    }
            //}


            if (eventID == ITMGContext.EVENT_ID_ENDPOINT_HAS_AUDIO)
            {
                for (int i = 0; i < openIdList.Length; ++i)
                {
                    if (ulong.TryParse(openIdList[i], out ulong id))
                    {
                        mInRoomRoleInfos[id] = true;
                    }
                }
                eventEmitter.Trigger(EEvents.EndpointsUpdate);
            }
            else if (eventID == ITMGContext.EVENT_ID_ENDPOINT_NO_AUDIO)
            {
                for (int i = 0; i < openIdList.Length; ++i)
                {
                    if (ulong.TryParse(openIdList[i], out ulong id))
                    {
                        mInRoomRoleInfos[id] = false;
                    }
                }
                eventEmitter.Trigger(EEvents.EndpointsUpdate);
            }

            //eventEmitter.Trigger(EEvents.EndpointsUpdate);
        }

        public int StartRecode(ChatType chatType)
        {
            if (eRecordState != ERecodeVoice.None)
                return 1;

            eRecordChatType = chatType;

            int ret = ITMGContext.GetInstance().GetPttCtrl().StartRecording(gRecordPath);
            if (0 == ret)
            {
                IsVaildRecord = true;
                eRecordState = ERecodeVoice.Recording;
                fStartRecordTime = Time.unscaledTime;

            }
            else
            {
                eRecordState = ERecodeVoice.None;
                DebugUtil.LogErrorFormat("GME SDK StartRecode Fail errorCode({0})", ret.ToString());
            }
            return ret;
        }

        public int StopRecode()
        {
            if (ERecodeVoice.Recording != eRecordState)
                return 0;

            if (!_isVaildRecord)
            {
                int ret = ITMGContext.GetInstance().GetPttCtrl().CancelRecording();
                if (0 != ret)
                {
                    DebugUtil.LogErrorFormat("GME SDK StopRecode Fail errorCode({0})", ret.ToString());
                }
                eRecordState = ERecodeVoice.None;
            }
            else
            {
                int ret = ITMGContext.GetInstance().GetPttCtrl().StopRecording();
                if (0 == ret)
                {
                    eRecordState = ERecodeVoice.WaitRecordComplete;
                }
                else
                {
                    eRecordState = ERecodeVoice.None;
                    DebugUtil.LogErrorFormat("GME SDK StopRecode Fail errorCode({0})", ret.ToString());
                }
            }
            return 0;
        }

        private void OnRecordFileComplete(int code, string filepath)
        {
            DebugUtil.LogFormat(ELogType.eChat, "OnRecordFileComplete({0}, {1})", code.ToString(), filepath);

            if (code == QAVError.OK)
            {
                if (_isVaildRecord)
                {
                    int ret = ITMGContext.GetInstance().GetPttCtrl().UploadRecordedFile(filepath);
                    if (0 == ret)
                    {
                        eRecordState = ERecodeVoice.WaitUploadComplete;
                    }
                    else
                    {
                        eRecordState = ERecodeVoice.None;
                        DebugUtil.LogErrorFormat("GME SDK UploadRecordedFile Fail errorCode({0})", ret.ToString());
                    }
                }
                else
                {
                    eRecordState = ERecodeVoice.None;
                }
            }
            else if (code == QAVError.ERR_VOICE_RECORD_AUDIO_TOO_SHORT)
            {
                eRecordState = ERecodeVoice.None;
                PushErrorTip(Chat_Voice_ToShort);
            }
            else
            {
                eRecordState = ERecodeVoice.None;
                DebugUtil.LogErrorFormat("GME SDK OnRecordFileComplete Fail errorCode({0}) {1}", code.ToString(), filepath);
            }
        }

        private void OnUploadFileComplete(int code, string filepath, string fileid)
        {
            DebugUtil.LogFormat(ELogType.eChat, "OnUploadFileComplete({0}, {1}, {2})", code.ToString(), filepath, fileid);

            if (0 == code)
            {
                eRecordState = ERecodeVoice.WaitAudit;
                waitAuditFileLength = ITMGContext.GetInstance().GetPttCtrl().GetVoiceFileDuration(gRecordPath) / 1000;
                waitAuditFileid = fileid;
                auditCount = 0;

                //根据语音时长计算首次发起验证的等待时间
                float firstWaitTime = (waitAuditFileLength / 5.0f) * 2;
                waitAuditTimer = Timer.Register(firstWaitTime, OnWaitAudit);
            }
            else
            {
                eRecordState = ERecodeVoice.None;
                DebugUtil.LogErrorFormat("GME SDK OnUploadFileComplete Fail errorCode({0}) {1} {2}", code.ToString(), filepath, fileid);
            }
        }

        private void OnDownloadFileComplete(int code, string filepath, string fileid)
        {
            if (0 == code)
            {
                //如果还在缓存列表中则更新状态，否则删除文件
                if (_voiceFileInfos.TryGetValue(fileid, out VoiceFileInfo voiceFileInfo))
                {
                    voiceFileInfo.state = 2;
                }
                else
                {
                    if (System.IO.File.Exists(filepath))
                    {
                        System.IO.File.Delete(filepath);
                    }
                }

                if (string.Equals(fileid, sCurrentFileID, StringComparison.Ordinal))
                {
                    int ret = ITMGContext.GetInstance().GetPttCtrl().PlayRecordedFile(filepath);
                    if (ret != 0)
                    {
                        DebugUtil.LogErrorFormat("GME SDK PlayRecordedFile Fail errorCode({0}) {1} {2}", ret.ToString(), filepath, fileid);
                    }
                }
            }
            else
            {
                if (string.Equals(fileid, sCurrentFileID, StringComparison.Ordinal))
                {
                    sCurrentFileID = null;
                }
                DebugUtil.LogErrorFormat("GME SDK OnDownloadFileComplete Fail errorCode({0}) {1} {2}", code.ToString(), filepath, fileid);
            }
        }

        private void OnPlayFileComplete(int code, string filepath)
        {
            sCurrentFileID = null;

            PlayNext();
        }

        private void OnWaitAudit()
        {
            Timer.Cancel(waitAuditTimer);
            waitAuditTimer = null;

            string fileid = waitAuditFileid;

            ++auditCount;
            DebugUtil.LogFormat(ELogType.eChat, "audit count {0} {1}", auditCount, fileid);

            if(auditCount > 10)
            {
                //TODO: 这个由策划决定下 提示消息
                eRecordState = ERecodeVoice.None;
                DebugUtil.LogError("OnWaitAudit() auditCount > 10");
                return;
            }

            int ret = ITMGContext.GetInstance().GetPttCtrl().SpeechToText(fileid, sSpeechLanguage, sTranslatelanguage);
            if (ret == 0)
            {
                eRecordState = ERecodeVoice.WaitSpeechToText;
            }
            else
            {
                //TODO: 这个由策划决定下 提示消息
                eRecordState = ERecodeVoice.None;
                DebugUtil.LogFormat(ELogType.eChat, "GME SDK SpeechToText Fail errorCode({0}) {1}", ret.ToString(), fileid);
            }
        }

        /// <summary>
        /// 语音转文本回调
        /// </summary>
        /// <param name="code">错误码</param>
        /// <param name="fileid">语音文件id</param>
        /// <param name="result">文本结果</param>
        /// <param name="auditResult">
        /// 
        /// user_Id	    Integer	    业务方用户 ID
        /// audit_res   Integer     审核状态，对应如下：
        ///     1：审核完成
        ///     2：审核中
        ///     3：语言不支持
        ///     4：没有勾选该语言送
        /// DataId      Integer     数据唯一标识
        /// HitFlag     Boolean     是否违规：true 违规，false 不违规
        /// Label       string      审核结果标签，对应如下：
        ///     normal：     正常文本
        ///     porn：       色情
        ///     abuse：      谩骂
        ///     ad：         广告
        ///     contraband： 违禁
        ///     customized： 自定义词库
        ///     RLanguages： 小语种
        ///     moan:        呻吟/娇喘
        /// AsrText     string      语音转文本结果
        /// ScanDetail  Array of ScanDetai      包含语音消息的审核结果和违规内容
        /// RequestId   Integer     唯一请求ID，用于跟踪查询问题
        /// 
        /// </param>

        private void OnSpeechToTextComplete(int code, string fileid, string result, string auditResult)
        {
            try
            {
                DebugUtil.LogFormat(ELogType.eChat, "OnSpeechToTextComplete({0}, {1}, {2}, {3})", code.ToString(), fileid, result, auditResult);

                if (0 != code)
                {
                    eRecordState = ERecodeVoice.None;
                    DebugUtil.LogErrorFormat("GME SDK OnSpeechToTextComplete({0}, {1}, {2}, {3}) Fail!", code.ToString(), fileid, result, auditResult);
                    return;
                }

                if (waitAuditFileid != fileid)
                {
                    eRecordState = ERecodeVoice.None;
                    DebugUtil.LogErrorFormat("GME SDK OnSpeechToTextComplete fileid({0}) !=  waitAuditFileid({1})", fileid, waitAuditFileid);
                    return;
                }

                if (result.Length > nVoiceContentLengthLimit)
                {
                    eRecordState = ERecodeVoice.None;
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11982));// ("语音内容过长");
                                                                                               //TODO 提示语音过长
                    return;
                }

                if (string.IsNullOrWhiteSpace(auditResult))
                {                    
                    eRecordState = ERecodeVoice.WaitAudit;
                    waitAuditTimer = Timer.Register(1, OnWaitAudit);
                    return;
                }

                LitJson.JsonData jsonData = LitJson.JsonMapper.ToObject(auditResult);
                if (jsonData == null)
                {
                    eRecordState = ERecodeVoice.None;
                    DebugUtil.LogErrorFormat("未收到审核结果");
                    return;
                }

                int audit_res = (int)jsonData["audit_res"];
                switch (audit_res)
                {
                    case 2: //审核中
                        {
                            eRecordState = ERecodeVoice.WaitAudit;
                            waitAuditTimer = Timer.Register(1, OnWaitAudit);
                        }
                        break;

                    case 1: //审核完成
                    case 3: //语言不支持             //TODO: 这个由策划决定下
                    case 4: //没有勾选该语言送        //TODO: 这个由策划决定下
                        {
                            auditCount = 0;

                            bool HitFlag = (bool)jsonData["HitFlag"];
                            SendVoice(eRecordChatType, result, waitAuditFileid, waitAuditFileLength, HitFlag ? 1 : 0);
                            eRecordState = ERecodeVoice.None;
                        }
                        break;
                    default:
                        {
                            eRecordState = ERecodeVoice.None;
                            DebugUtil.LogErrorFormat("GME SDK OnSpeechToTextComplete unknown audit_res（{0}）", audit_res.ToString());
                        }
                        break;
                }
            }
            catch(Exception e)
            {
                eRecordState = ERecodeVoice.None;
                Timer.Cancel(waitAuditTimer);
                waitAuditTimer = null;

                DebugUtil.LogException(e);
                DebugUtil.LogErrorFormat("OnSpeechToTextComplete({0}, {1}, {2})", code.ToString(), fileid, auditResult);
            }
        }

        private VoiceFileInfo GetVoiceFileInfo(string fileID)
        {
            if (string.IsNullOrWhiteSpace(fileID))
                return null;

            if (!_voiceFileInfos.TryGetValue(fileID, out VoiceFileInfo voiceFileInfo))
            {
                voiceFileInfo = _voiceFileInfosPool[_nextFileIndex];
                if (null == voiceFileInfo)
                {
                    voiceFileInfo = new VoiceFileInfo();
                    _voiceFileInfosPool[_nextFileIndex] = voiceFileInfo;

                    voiceFileInfo.path = Application.persistentDataPath + "/TempFilesVoice" + _nextFileIndex.ToString() + ".silk";
                }
                else
                {
                    if (voiceFileInfo.state == 2)
                    {
                        if (System.IO.File.Exists(voiceFileInfo.path))
                        {
                            System.IO.File.Delete(voiceFileInfo.path);
                        }
                        voiceFileInfo.state = 0;
                    }

                    _voiceFileInfos.Remove(voiceFileInfo.fileID);
                }

                voiceFileInfo.fileID = fileID;
                _voiceFileInfos.Add(fileID, voiceFileInfo);

                ++_nextFileIndex;
                if (_nextFileIndex >= _voiceFileInfosPool.Length)
                {
                    _nextFileIndex = 0;
                }
            }

            return voiceFileInfo;
        }

        public int PlayFile(string fileid, bool isAutoPlay = false)
        {
            if (!IsSDKInited)
                return -1;

            if (isAutoPlay == false)
            {
                while (mVoiceFileIDs.Count > 0)
                {
                    ChatContent content = mVoiceFileIDs.Dequeue();
                    RemoveChatContentReference(content, EChatContentReference.VoiceQueue);
                }
                //mVoiceFileIDs.Clear();
            }

            if (sCurrentFileID != null)
            {
                ITMGContext.GetInstance().GetPttCtrl().StopPlayFile();
            }

            int ret = 0;
            sCurrentFileID = fileid;
            VoiceFileInfo fileInfo = GetVoiceFileInfo(sCurrentFileID);
            if (fileInfo == null)
                return -2;

            if (fileInfo.state == 0)
            {
                DebugUtil.LogFormat(ELogType.eChat, "DownloadRecordedFile({0}, {1})", sCurrentFileID, fileInfo.path);
                ret = ITMGContext.GetInstance().GetPttCtrl().DownloadRecordedFile(sCurrentFileID, fileInfo.path);
            }
            else if (fileInfo.state == 2)
            {
                DebugUtil.LogFormat(ELogType.eChat, "PlayRecordedFile({0})", fileInfo.path);
                ret = ITMGContext.GetInstance().GetPttCtrl().PlayRecordedFile(fileInfo.path);
            }

            return ret;
        }

        public bool EnableMic(bool isEnabled)
        {
            int ret = ITMGContext.GetInstance().GetAudioCtrl().EnableMic(isEnabled);
            return ret == 0;
        }

        public bool GetMicState()
        {
            int ret = ITMGContext.GetInstance().GetAudioCtrl().GetMicState();
            return ret == 1;
        }

        public int GetRecvStreamLevel(string openID)
        {
            return ITMGContext.GetInstance().GetAudioCtrl().GetRecvStreamLevel(openID);
        }

        public void EnqueueVoiceContent(ChatContent content)
        {
            if (!string.IsNullOrWhiteSpace(content.sFileID) && CheckAutoPlay(content.eChatType))
            {
                if (content.mBaseInfo != null && Sys_Role.Instance.IsSelfRole(content.mBaseInfo.nRoleID))
                {
                    return;
                }

                AddChatContentReference(content, EChatContentReference.VoiceQueue);
                mVoiceFileIDs.Enqueue(content);
                PlayNext();
            }
        }

        private bool CheckAutoPlay(ChatType chatType)
        {
            if (OptionManager.Instance.GetBoolean(OptionManager.EOptionID.AutoPlayIfWifi) && !NetworkHelper.IsWifi())
            {
                return false;
            }

            switch (chatType)
            {
                case ChatType.World:
                    if (OptionManager.Instance.GetBoolean(OptionManager.EOptionID.AutoPlayWorld))
                    {
                        return true;
                    }
                    break;
                case ChatType.Guild:
                    if (OptionManager.Instance.GetBoolean(OptionManager.EOptionID.AutoPlayGuild))
                    {
                        return true;
                    }
                    break;
                case ChatType.Team:
                    if (OptionManager.Instance.GetBoolean(OptionManager.EOptionID.AutoPlayTeam))
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }

            return false;
        }

        private void PlayNext()
        {
            if (!string.IsNullOrWhiteSpace(_currentFileID))
                return;

            while (mVoiceFileIDs.Count > 0)
            {
                ChatContent content = mVoiceFileIDs.Dequeue();
                RemoveChatContentReference(content, EChatContentReference.VoiceQueue);

                if (CheckAutoPlay(content.eChatType))
                {
                    content.bPlayed = true;
                    if (!content.bHasIllegalWord)
                    {
                        PlayFile(content.sFileID, true);
                    }

                    //自动播放不提示
                    //else
                    //{
                    //    PushErrorTip(Chat_HasIllegalWord_Error);
                    //}
                    break;
                }
            }
        }

        private void RefreshBGMValue()
        {
            if (isRoomEntered
                || eRecordState == ERecodeVoice.Recording
                || !string.IsNullOrWhiteSpace(_currentFileID))
            {
                OptionManager.Instance.SetBoolean(OptionManager.EOptionID.BGM, false, true);
            }
            else
            {
                OptionManager.Instance.CancelOverride(OptionManager.EOptionID.BGM);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (waitAuditTimer != null)
            {
                Timer.Cancel(waitAuditTimer);
                waitAuditTimer = null;
            }
        }
    }
}