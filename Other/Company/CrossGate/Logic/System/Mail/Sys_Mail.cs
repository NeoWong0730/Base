using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using Table;
using System.Json;
using UnityEngine;
using System.IO;

namespace Logic
{
    public class Sys_Mail : SystemModuleBase<Sys_Mail>
    {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public readonly string MailMemoryPath = "MailMemory";

        private Dictionary<ulong, MailData> mailDataMaps = new Dictionary<ulong, MailData>();

        public MailInfo mailInfo = new MailInfo();

        public bool bMailShowing = false;            //玩家是否在阅览邮件

        public int mailEnterType;

        public uint vaildtime;

        public bool FirstMailAdd;


        public class MailInfo
        {
            public List<MailData> mailDatas = new List<MailData>();
            public uint version;
            public void DeserializeObject(JsonObject jo)
            {
                JsonHeler.DeserializeObject(jo, this);

                if (jo.ContainsKey("mailDatas"))
                {
                    mailDatas.Clear();
                    JsonArray ja = (JsonArray)jo["mailDatas"];
                    foreach (var item in ja)
                    {
                        MailData info = new MailData();
                        info.DeserializeObject((JsonObject)item);
                        mailDatas.Add(info);
                    }
                }
            }

            public bool Contains(ulong mailId)
            {
                for (int i = 0; i < mailDatas.Count; i++)
                {
                    if (mailDatas[i].id == mailId)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public class MailAttachData
        {
            public uint ItemId;
            public uint ItemNum;
            public uint TemplateId;
            public int prohibitionSec;

            public void DeserializeObject(JsonObject jo)
            {
                JsonHeler.DeserializeObject(jo, this);
            }
        }


        public class MailData
        {
            public ulong id;
            public bool read;
            public List<MailAttachData> attach = new List<MailAttachData>();
            public bool get;
            public uint time;
            public string title;
            public string content;

            public bool Attach
            {
                get
                {
                    return attach != null && attach.Count > 0 && attach[0] != null;
                }
            }

            public void DeserializeObject(JsonObject jo)
            {
                JsonHeler.DeserializeObject(jo, this);

                if (jo.ContainsKey("attach"))
                {
                    attach.Clear();
                    JsonArray ja = (JsonArray)jo["attach"];
                    foreach (var item in ja)
                    {
                        MailAttachData info = new MailAttachData();
                        info.DeserializeObject((JsonObject)item);
                        attach.Add(info);
                    }
                }
            }
        }

        public enum EEvents
        {
            OnAddMail,
            OnDeleteMail,
            OnNoticeMailAdd,    //告知玩家是否增加邮件(界面收取邮件通知提示)
            OnNoticeMailOver,
            OnGetAttach,        //领取附件
            OnGetAll,          //一键领取(一键领取之后需要一键已读)
        }

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdMail.SetReadReq, (ushort)CmdMail.SetReadRes, MailSetReadRes, CmdMailSetReadRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdMail.DelMailReq, (ushort)CmdMail.DelMailRes, OnDeleteMailRes, CmdMailDelMailRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdMail.GetMulMailAttachReq, (ushort)CmdMail.GetMulMailAttachRes, OnGetMulMailAttachRes, CmdMailGetMulMailAttachRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMail.NewMailNtf, OnNewMailNtf, CmdMailNewMailNtf.Parser);
            Sys_Net.Instance.eventEmitter.Handle<bool>(Sys_Net.EEvents.OnReconnectResult, OnReconnectResult, true);
            vaildtime = uint.Parse(CSVParam.Instance.GetConfData(815).str_value);
        }

        public override void Dispose()
        {
            Sys_Net.Instance.eventEmitter.Handle<bool>(Sys_Net.EEvents.OnReconnectResult, OnReconnectResult, false);
        }

        private void OnReconnectResult(bool result)
        {
            if (result)
            {
                LoadedMemory();
            }
        }

        public override void OnLogin()
        {
            LoadedMemory();
        }

        static string path = Path.GetFullPath(Framework.Consts.persistentDataPath) + "/{0}_{1}_{2}.json";

        public void LoadedMemory()
        {
            mailInfo.mailDatas.Clear();
            JsonObject json = FileStore.ReadJson(MailMemoryPath);
            if (json != null)
            {
                mailInfo.DeserializeObject(json);
                MapData();
                QueryNewMailReq(mailInfo.version, GetMaxMailId());
            }
            else
            {
                QueryNewMailReq(0, 0);
            }
        }

        public void SaveMemory()
        {
            FileStore.WriteJson(MailMemoryPath, mailInfo);
        }

        #region  Net

        private void QueryNewMailReq(uint version, ulong maxMailId)
        {
            CmdMailQueryNewMailReq cmdMailQueryNewMailReq = new CmdMailQueryNewMailReq();
            cmdMailQueryNewMailReq.CliVersion = version;
            cmdMailQueryNewMailReq.MaxMailId = maxMailId;
            NetClient.Instance.SendMessage((ushort)CmdMail.QueryNewMailReq, cmdMailQueryNewMailReq);
        }


        private void MailSetCliVer(uint version, ulong maxMailId)
        {
            CmdMailSetCliVer cmdMailSetCliVer = new CmdMailSetCliVer();
            cmdMailSetCliVer.CliVersion = version;
            cmdMailSetCliVer.CliMaxMailId = maxMailId;
            NetClient.Instance.SendMessage((ushort)CmdMail.SetCliVer, cmdMailSetCliVer);
        }

        /// <summary>
        /// 已读邮件
        /// </summary>
        /// <param name="id"></param>
        private void SendMailReadReq(ulong id)
        {
            CmdMailSetReadReq cmdMailSetReadReq = new CmdMailSetReadReq();
            cmdMailSetReadReq.Id = id;
            NetClient.Instance.SendMessage((ushort)CmdMail.SetReadReq, cmdMailSetReadReq);
        }

        private void MailSetReadRes(NetMsg netMsg)
        {
            CmdMailSetReadRes res = NetMsgUtil.Deserialize<CmdMailSetReadRes>(CmdMailSetReadRes.Parser, netMsg);
            mailInfo.version = res.Ver;
            MailSetCliVer(mailInfo.version, GetMaxMailId());
            SaveMemory();
        }

        public void GetMailAttachReq(ulong id)
        {
            CmdMailGetMailAttachReq cmdMailGetMailAttachReq = new CmdMailGetMailAttachReq();
            cmdMailGetMailAttachReq.Id = id;
            NetClient.Instance.SendMessage((ushort)CmdMail.GetMailAttachReq, cmdMailGetMailAttachReq);
        }

        /// <summary>
        /// 删除邮件
        /// </summary>
        /// <param name="id"></param>
        public void DeleteMailReq(ulong id)
        {
            CmdMailDelMailReq cmdMailDelMailReq = new CmdMailDelMailReq();
            cmdMailDelMailReq.Id = id;
            NetClient.Instance.SendMessage((ushort)CmdMail.DelMailReq, cmdMailDelMailReq);
        }


        private void OnDeleteMailRes(NetMsg netMsg)
        {
            CmdMailDelMailRes res = NetMsgUtil.Deserialize<CmdMailDelMailRes>(CmdMailDelMailRes.Parser, netMsg);
            for (int i = 0; i < res.Id.Count; i++)
            {
                ulong mailId = res.Id[i];
                if (mailDataMaps.TryGetValue(mailId, out MailData mailData))
                {
                    DebugUtil.LogError("删除邮件返回");
                    DeleteMail(mailData);
                }
            }
            mailInfo.version = res.Ver;
            MailSetCliVer(mailInfo.version, GetMaxMailId());
            SaveMemory();
        }

        public void DeleteAllReadAndNoAttachMail()
        {
            for (int i = 0; i < mailInfo.mailDatas.Count; i++)
            {
                MailData mailData = mailInfo.mailDatas[i];
                if ((!mailData.Attach && mailData.read) || (mailData.read && mailData.Attach && mailData.get))
                {
                    DeleteMailReq(mailData.id);
                }
            }
        }

        public void GetMulMailAttachReq()
        {
            CmdMailGetMulMailAttachReq cmdMailGetMulMailAttachReq = new CmdMailGetMulMailAttachReq();
            NetClient.Instance.SendMessage((ushort)CmdMail.GetMulMailAttachReq, cmdMailGetMulMailAttachReq);
        }

        private void OnGetMulMailAttachRes(NetMsg netMsg)
        {
            CmdMailGetMulMailAttachRes res = NetMsgUtil.Deserialize<CmdMailGetMulMailAttachRes>(CmdMailGetMulMailAttachRes.Parser, netMsg);
            for (int i = 0; i < res.Id.Count; i++)
            {
                ulong id = res.Id[i];
                MailData mailData = GetMailData(id);
                mailData.get = true;
                eventEmitter.Trigger<ulong>(EEvents.OnGetAttach, id);
            }
            mailInfo.version = res.Ver;
            MailSetCliVer(mailInfo.version, GetMaxMailId());
            SaveMemory();

            //一键已读
            if (res.Id.Count > 1)
            {
                eventEmitter.Trigger(EEvents.OnGetAll);
            }
        }


        private void OnNewMailNtf(NetMsg netMsg)
        {
            CmdMailNewMailNtf res = NetMsgUtil.Deserialize<CmdMailNewMailNtf>(CmdMailNewMailNtf.Parser, netMsg);
            if (res.ClearCliMail)
            {
                mailInfo.mailDatas.Clear();
                mailDataMaps.Clear();
            }
            mailInfo.version = res.Ver;
            for (int i = 0; i < res.Data.Count; i++)
            {
                if (mailInfo.Contains(res.Data[i].Id))
                    continue;
                MailData mailData = new MailData();
                mailData.id = res.Data[i].Id;
                mailData.read = res.Data[i].IsRead;
                mailData.get = res.Data[i].IsGet;
                for (int j = 0; j < res.Data[i].Attach.Count; j++)
                {
                    MailAttachData mailAttachData = new MailAttachData();
                    mailAttachData.ItemId = res.Data[i].Attach[j].ItemId;
                    mailAttachData.ItemNum = res.Data[i].Attach[j].ItemNum;
                    mailAttachData.TemplateId = res.Data[i].Attach[j].TemplateId;
                    mailAttachData.prohibitionSec = res.Data[i].Attach[j].ProhibitionSec;
                    mailData.attach.Add(mailAttachData);
                }
                mailData.content = res.Data[i].Content.ToStringUtf8();
                mailData.content = mailData.content.Replace("\\n", "\n");
                mailData.content = RegexHelper.Parse(RegexHelper.RegexType.E_Item, mailData.content, "<itemid>", "</itemid>");
                mailData.content = mailData.content.Replace(" ", "\u00A0");
                mailData.title = res.Data[i].Title.ToStringUtf8();
                mailData.time = res.Data[i].Time;
                mailInfo.mailDatas.Insert(0, mailData);
                MapData();

                eventEmitter.Trigger(EEvents.OnAddMail);
                if (!bMailShowing)
                {
                    eventEmitter.Trigger(EEvents.OnNoticeMailAdd);
                }
                //SyncMailSendStateReq(mailData.id);
            }

            SaveMemory();
            if (res.Data.Count > 0 && Sys_Role.Instance.FirstEnterGame)
            {
                FirstMailAdd = true;
            }
        }

        #endregion

        #region Util     

        public void DeleteMail(MailData mailData)
        {
            if (mailInfo.mailDatas.Remove(mailData))
            {
                mailDataMaps.Remove(mailData.id);

                eventEmitter.Trigger(EEvents.OnDeleteMail);
                SaveMemory();
            }
        }

        private void MapData()
        {
            for (int i = 0; i < mailInfo.mailDatas.Count; i++)
            {
                MailData mailData = mailInfo.mailDatas[i];
                ulong id = mailData.id;
                mailDataMaps[id] = mailData;
            }
        }

        private ulong GetMaxMailId()
        {
            ulong maxId = 0;
            for (int i = 0; i < mailInfo.mailDatas.Count; i++)
            {
                ulong id = mailInfo.mailDatas[i].id;
                if (maxId <= id)
                {
                    maxId = id;
                }
            }
            return maxId;
        }

        //TODO:排序优化
        public void SortMail()
        {
            List<MailData> readMails = new List<MailData>();    //已读邮件
            List<MailData> unreadMails = new List<MailData>();  //未读邮件

            for (int i = 0; i < mailInfo.mailDatas.Count; i++)
            {
                if (mailInfo.mailDatas[i].read)
                {
                    readMails.Add(mailInfo.mailDatas[i]);
                }
                else
                {
                    unreadMails.Add(mailInfo.mailDatas[i]);
                }
            }

            //先处理未读
            //unreadMails.Sort((x, y) => y.Attach.CompareTo(x.Attach));  //有附件 排在前面 降序
            List<MailData> unread_AttachMails = new List<MailData>();
            List<MailData> unread_UnAttachMails = new List<MailData>();

            for (int i = 0; i < unreadMails.Count; i++)
            {
                if (unreadMails[i].Attach)
                {
                    unread_AttachMails.Add(unreadMails[i]);
                }
                else
                {
                    unread_UnAttachMails.Add(unreadMails[i]);
                }
            }

            unreadMails.Clear();
            unread_AttachMails.Sort((x, y) => y.time.CompareTo(x.time));
            unread_UnAttachMails.Sort((x, y) => y.time.CompareTo(x.time));
            unreadMails.AddRange(unread_AttachMails);
            unreadMails.AddRange(unread_UnAttachMails);



            //处理已读
            List<MailData> readMail_Attach = new List<MailData>();
            List<MailData> readMail_UnAttach = new List<MailData>();

            for (int i = 0; i < readMails.Count; i++)
            {
                if (readMails[i].Attach)
                {
                    readMail_Attach.Add(readMails[i]);
                }
                else
                {
                    readMail_UnAttach.Add(readMails[i]);
                }
            }

            List<MailData> readMail_Attach_get = new List<MailData>();
            List<MailData> readMail_Attach_Unget = new List<MailData>();

            for (int i = 0; i < readMail_Attach.Count; i++)
            {
                if (readMail_Attach[i].get)
                {
                    readMail_Attach_get.Add(readMail_Attach[i]);
                }
                else
                {
                    readMail_Attach_Unget.Add(readMail_Attach[i]);
                }
            }
            readMail_Attach.Clear();
            //readMail_Attach_get.Sort((x, y) => x.time.CompareTo(y.time));
            readMail_Attach_Unget.Sort((x, y) => y.time.CompareTo(x.time));
            readMail_Attach.AddRange(readMail_Attach_Unget);
            //readMail_Attach.AddRange(readMail_Attach_get);

            //readMail_Attach.Sort((x, y) => x.get.CompareTo(y.get));  //未领取  排在前面 升序
            List<MailData> readMail_Attach_get_UnAttach = new List<MailData>();
            readMail_Attach_get_UnAttach.AddRange(readMail_Attach_get);
            readMail_Attach_get_UnAttach.AddRange(readMail_UnAttach);

            //readMail_UnAttach.Sort((x, y) => x.time.CompareTo(y.time));
            readMail_Attach_get_UnAttach.Sort((x, y) => y.time.CompareTo(x.time));

            //清理所有邮件
            mailInfo.mailDatas.Clear();
            mailInfo.mailDatas.AddRange(unreadMails);
            mailInfo.mailDatas.AddRange(readMail_Attach);
            mailInfo.mailDatas.AddRange(readMail_Attach_get_UnAttach);
        }

        public void CalExpirmeMail()
        {
            for (int i = 0; i < mailInfo.mailDatas.Count; i++)
            {
                MailData mailData = mailInfo.mailDatas[i];
                long overTime = ((long)Sys_Time.Instance.GetServerTime() - (long)mailData.time) / 86400;
                if (overTime >= vaildtime)
                {
                    DebugUtil.LogError("邮件过期==>主动删除邮件请求");
                    DeleteMail(mailData);
                }
            }
        }

        public MailData GetMailData(ulong mailId)
        {
            mailDataMaps.TryGetValue(mailId, out MailData mailData);
            return mailData;
        }

        public void ReadMail(ulong mailId)
        {
            if (!bMailShowing)
                return;
            MailData mailData = GetMailData(mailId);
            if (mailData == null)
            {
                DebugUtil.LogErrorFormat("未能找到邮件{0}", mailId);
                return;
            }

            SendMailReadReq(mailId);

            mailData.read = true;
            SaveMemory();
        }

        public void ReadAll()
        {
            for (int i = 0, count = mailInfo.mailDatas.Count; i < count; i++)
            {
                ReadMail(mailInfo.mailDatas[i].id);
            }
        }

        public bool ReadAllMail()
        {
            for (int i = 0, count = mailInfo.mailDatas.Count; i < count; i++)
            {
                if (!mailInfo.mailDatas[i].read)
                {
                    return false;
                }
            }
            return true;
        }

        public bool CanGetAttach()
        {
            for (int i = 0, count = mailInfo.mailDatas.Count; i < count; i++)
            {
                if (mailInfo.mailDatas[i].Attach && !mailInfo.mailDatas[i].get)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}


