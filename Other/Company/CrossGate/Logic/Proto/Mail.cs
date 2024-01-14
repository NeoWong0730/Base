// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: mail.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using scg = global::System.Collections.Generic;
namespace Packet {

  #region Enums
  public enum CmdMail {
    None = 0,
    /// <summary>
    /// 请求新增邮件
    /// </summary>
    QueryNewMailReq = 4601,
    /// <summary>
    /// 设置已读
    /// </summary>
    SetReadReq = 4602,
    /// <summary>
    /// 设置已读
    /// </summary>
    SetReadRes = 4603,
    /// <summary>
    /// 收取附件
    /// </summary>
    GetMailAttachReq = 4604,
    /// <summary>
    /// 一键收取附件(多封邮件)
    /// </summary>
    GetMulMailAttachReq = 4605,
    /// <summary>
    /// 一键收取附件(多封邮件)
    /// </summary>
    GetMulMailAttachRes = 4606,
    /// <summary>
    /// 删除邮件
    /// </summary>
    DelMailReq = 4607,
    /// <summary>
    /// 删除邮件
    /// </summary>
    DelMailRes = 4608,
    /// <summary>
    /// 新邮件通知
    /// </summary>
    NewMailNtf = 4609,
    /// <summary>
    /// 设置客户端邮件版本
    /// </summary>
    SetCliVer = 4610,
  }

  public enum ErrorMail {
    None = 0,
    /// <summary>
    /// 获取邮件失败
    /// </summary>
    GetMailErr = 4601,
    /// <summary>
    /// 不存在的邮件id
    /// </summary>
    MailIdNotExist = 4602,
    /// <summary>
    /// 删除邮件失败，邮件附件未领取
    /// </summary>
    AttachNotGet = 4603,
    /// <summary>
    /// 删除邮件失败，邮件附件未读
    /// </summary>
    NotRead = 4604,
    /// <summary>
    /// 附件已领取
    /// </summary>
    ItemHasGet = 4605,
    /// <summary>
    /// 没有可领取的附件
    /// </summary>
    NoAttach = 4606,
    /// <summary>
    /// 没有可领取的邮件
    /// </summary>
    NoCanGetMail = 4607,
    /// <summary>
    /// 找不到该角色的邮件
    /// </summary>
    NotFindRoleId = 4608,
    /// <summary>
    /// 该邮件已过期
    /// </summary>
    ExpiredMail = 4609,
    /// <summary>
    /// 请稍后重试
    /// </summary>
    WaitForMoment = 4610,
  }

  public enum MailActiveReason {
    None = 0,
    /// <summary>
    /// 收取附件
    /// </summary>
    GetAttach = 4601,
  }

  #endregion

  #region Messages
  public sealed class MailAttach : pb::IMessage {
    private static readonly pb::MessageParser<MailAttach> _parser = new pb::MessageParser<MailAttach>(() => new MailAttach());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<MailAttach> Parser { get { return _parser; } }

    /// <summary>Field number for the "itemId" field.</summary>
    public const int ItemIdFieldNumber = 1;
    private uint itemId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ItemId {
      get { return itemId_; }
      set {
        itemId_ = value;
      }
    }

    /// <summary>Field number for the "itemNum" field.</summary>
    public const int ItemNumFieldNumber = 2;
    private uint itemNum_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ItemNum {
      get { return itemNum_; }
      set {
        itemNum_ = value;
      }
    }

    /// <summary>Field number for the "templateId" field.</summary>
    public const int TemplateIdFieldNumber = 3;
    private uint templateId_;
    /// <summary>
    /// 宠物或装备的模板id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint TemplateId {
      get { return templateId_; }
      set {
        templateId_ = value;
      }
    }

    /// <summary>Field number for the "prohibitionSec" field.</summary>
    public const int ProhibitionSecFieldNumber = 4;
    private int prohibitionSec_;
    /// <summary>
    /// 禁售期(天), -1为永久禁售
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int ProhibitionSec {
      get { return prohibitionSec_; }
      set {
        prohibitionSec_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (ItemId != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(ItemId);
      }
      if (ItemNum != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(ItemNum);
      }
      if (TemplateId != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(TemplateId);
      }
      if (ProhibitionSec != 0) {
        output.WriteRawTag(32);
        output.WriteInt32(ProhibitionSec);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ItemId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ItemId);
      }
      if (ItemNum != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ItemNum);
      }
      if (TemplateId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(TemplateId);
      }
      if (ProhibitionSec != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(ProhibitionSec);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            ItemId = input.ReadUInt32();
            break;
          }
          case 16: {
            ItemNum = input.ReadUInt32();
            break;
          }
          case 24: {
            TemplateId = input.ReadUInt32();
            break;
          }
          case 32: {
            ProhibitionSec = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class MailData : pb::IMessage {
    private static readonly pb::MessageParser<MailData> _parser = new pb::MessageParser<MailData>(() => new MailData());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<MailData> Parser { get { return _parser; } }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private ulong id_;
    /// <summary>
    /// 邮件id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Id {
      get { return id_; }
      set {
        id_ = value;
      }
    }

    /// <summary>Field number for the "title" field.</summary>
    public const int TitleFieldNumber = 2;
    private pb::ByteString title_ = pb::ByteString.Empty;
    /// <summary>
    /// 标题
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pb::ByteString Title {
      get { return title_; }
      set {
        title_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "content" field.</summary>
    public const int ContentFieldNumber = 3;
    private pb::ByteString content_ = pb::ByteString.Empty;
    /// <summary>
    /// 内容
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pb::ByteString Content {
      get { return content_; }
      set {
        content_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "time" field.</summary>
    public const int TimeFieldNumber = 4;
    private uint time_;
    /// <summary>
    /// 添加邮件时间
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Time {
      get { return time_; }
      set {
        time_ = value;
      }
    }

    /// <summary>Field number for the "isRead" field.</summary>
    public const int IsReadFieldNumber = 5;
    private bool isRead_;
    /// <summary>
    /// 是否已读
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool IsRead {
      get { return isRead_; }
      set {
        isRead_ = value;
      }
    }

    /// <summary>Field number for the "isGet" field.</summary>
    public const int IsGetFieldNumber = 6;
    private bool isGet_;
    /// <summary>
    /// 是否已领取附件
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool IsGet {
      get { return isGet_; }
      set {
        isGet_ = value;
      }
    }

    /// <summary>Field number for the "attach" field.</summary>
    public const int AttachFieldNumber = 7;
    private static readonly pb::FieldCodec<global::Packet.MailAttach> _repeated_attach_codec
        = pb::FieldCodec.ForMessage(58, global::Packet.MailAttach.Parser);
    private readonly pbc::RepeatedField<global::Packet.MailAttach> attach_ = new pbc::RepeatedField<global::Packet.MailAttach>();
    /// <summary>
    /// 附件
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Packet.MailAttach> Attach {
      get { return attach_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Id != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(Id);
      }
      if (Title.Length != 0) {
        output.WriteRawTag(18);
        output.WriteBytes(Title);
      }
      if (Content.Length != 0) {
        output.WriteRawTag(26);
        output.WriteBytes(Content);
      }
      if (Time != 0) {
        output.WriteRawTag(32);
        output.WriteUInt32(Time);
      }
      if (IsRead != false) {
        output.WriteRawTag(40);
        output.WriteBool(IsRead);
      }
      if (IsGet != false) {
        output.WriteRawTag(48);
        output.WriteBool(IsGet);
      }
      attach_.WriteTo(output, _repeated_attach_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Id != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Id);
      }
      if (Title.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(Title);
      }
      if (Content.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(Content);
      }
      if (Time != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Time);
      }
      if (IsRead != false) {
        size += 1 + 1;
      }
      if (IsGet != false) {
        size += 1 + 1;
      }
      size += attach_.CalculateSize(_repeated_attach_codec);
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            Id = input.ReadUInt64();
            break;
          }
          case 18: {
            Title = input.ReadBytes();
            break;
          }
          case 26: {
            Content = input.ReadBytes();
            break;
          }
          case 32: {
            Time = input.ReadUInt32();
            break;
          }
          case 40: {
            IsRead = input.ReadBool();
            break;
          }
          case 48: {
            IsGet = input.ReadBool();
            break;
          }
          case 58: {
            attach_.AddEntriesFrom(input, _repeated_attach_codec);
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// 请求新增邮件
  /// </summary>
  public sealed class CmdMailQueryNewMailReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdMailQueryNewMailReq> _parser = new pb::MessageParser<CmdMailQueryNewMailReq>(() => new CmdMailQueryNewMailReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdMailQueryNewMailReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "cliVersion" field.</summary>
    public const int CliVersionFieldNumber = 1;
    private uint cliVersion_;
    /// <summary>
    /// 客户端邮件版本
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint CliVersion {
      get { return cliVersion_; }
      set {
        cliVersion_ = value;
      }
    }

    /// <summary>Field number for the "maxMailId" field.</summary>
    public const int MaxMailIdFieldNumber = 2;
    private ulong maxMailId_;
    /// <summary>
    /// 客户端本地已收取的最大邮件id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong MaxMailId {
      get { return maxMailId_; }
      set {
        maxMailId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (CliVersion != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(CliVersion);
      }
      if (MaxMailId != 0UL) {
        output.WriteRawTag(16);
        output.WriteUInt64(MaxMailId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (CliVersion != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(CliVersion);
      }
      if (MaxMailId != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(MaxMailId);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            CliVersion = input.ReadUInt32();
            break;
          }
          case 16: {
            MaxMailId = input.ReadUInt64();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// 设置已读
  /// </summary>
  public sealed class CmdMailSetReadReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdMailSetReadReq> _parser = new pb::MessageParser<CmdMailSetReadReq>(() => new CmdMailSetReadReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdMailSetReadReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private ulong id_;
    /// <summary>
    /// 邮件id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Id {
      get { return id_; }
      set {
        id_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Id != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(Id);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Id != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Id);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            Id = input.ReadUInt64();
            break;
          }
        }
      }
    }

  }

  public sealed class CmdMailSetReadRes : pb::IMessage {
    private static readonly pb::MessageParser<CmdMailSetReadRes> _parser = new pb::MessageParser<CmdMailSetReadRes>(() => new CmdMailSetReadRes());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdMailSetReadRes> Parser { get { return _parser; } }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private ulong id_;
    /// <summary>
    /// 邮件id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Id {
      get { return id_; }
      set {
        id_ = value;
      }
    }

    /// <summary>Field number for the "ver" field.</summary>
    public const int VerFieldNumber = 2;
    private uint ver_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Ver {
      get { return ver_; }
      set {
        ver_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Id != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(Id);
      }
      if (Ver != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Ver);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Id != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Id);
      }
      if (Ver != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Ver);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            Id = input.ReadUInt64();
            break;
          }
          case 16: {
            Ver = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// 收取附件
  /// </summary>
  public sealed class CmdMailGetMailAttachReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdMailGetMailAttachReq> _parser = new pb::MessageParser<CmdMailGetMailAttachReq>(() => new CmdMailGetMailAttachReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdMailGetMailAttachReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private ulong id_;
    /// <summary>
    /// 邮件id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Id {
      get { return id_; }
      set {
        id_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Id != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(Id);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Id != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Id);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            Id = input.ReadUInt64();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// 一键收取附件(多封邮件)
  /// </summary>
  public sealed class CmdMailGetMulMailAttachReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdMailGetMulMailAttachReq> _parser = new pb::MessageParser<CmdMailGetMulMailAttachReq>(() => new CmdMailGetMulMailAttachReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdMailGetMulMailAttachReq> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
        }
      }
    }

  }

  public sealed class CmdMailGetMulMailAttachRes : pb::IMessage {
    private static readonly pb::MessageParser<CmdMailGetMulMailAttachRes> _parser = new pb::MessageParser<CmdMailGetMulMailAttachRes>(() => new CmdMailGetMulMailAttachRes());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdMailGetMulMailAttachRes> Parser { get { return _parser; } }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private static readonly pb::FieldCodec<ulong> _repeated_id_codec
        = pb::FieldCodec.ForUInt64(10);
    private readonly pbc::RepeatedField<ulong> id_ = new pbc::RepeatedField<ulong>();
    /// <summary>
    /// 邮件id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<ulong> Id {
      get { return id_; }
    }

    /// <summary>Field number for the "ver" field.</summary>
    public const int VerFieldNumber = 2;
    private uint ver_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Ver {
      get { return ver_; }
      set {
        ver_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      id_.WriteTo(output, _repeated_id_codec);
      if (Ver != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Ver);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += id_.CalculateSize(_repeated_id_codec);
      if (Ver != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Ver);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10:
          case 8: {
            id_.AddEntriesFrom(input, _repeated_id_codec);
            break;
          }
          case 16: {
            Ver = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// 删除邮件
  /// </summary>
  public sealed class CmdMailDelMailReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdMailDelMailReq> _parser = new pb::MessageParser<CmdMailDelMailReq>(() => new CmdMailDelMailReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdMailDelMailReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private ulong id_;
    /// <summary>
    /// 邮件id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Id {
      get { return id_; }
      set {
        id_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Id != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(Id);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Id != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Id);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            Id = input.ReadUInt64();
            break;
          }
        }
      }
    }

  }

  public sealed class CmdMailDelMailRes : pb::IMessage {
    private static readonly pb::MessageParser<CmdMailDelMailRes> _parser = new pb::MessageParser<CmdMailDelMailRes>(() => new CmdMailDelMailRes());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdMailDelMailRes> Parser { get { return _parser; } }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private static readonly pb::FieldCodec<ulong> _repeated_id_codec
        = pb::FieldCodec.ForUInt64(10);
    private readonly pbc::RepeatedField<ulong> id_ = new pbc::RepeatedField<ulong>();
    /// <summary>
    /// 邮件id(支持多封)
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<ulong> Id {
      get { return id_; }
    }

    /// <summary>Field number for the "ver" field.</summary>
    public const int VerFieldNumber = 2;
    private uint ver_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Ver {
      get { return ver_; }
      set {
        ver_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      id_.WriteTo(output, _repeated_id_codec);
      if (Ver != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Ver);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += id_.CalculateSize(_repeated_id_codec);
      if (Ver != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Ver);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10:
          case 8: {
            id_.AddEntriesFrom(input, _repeated_id_codec);
            break;
          }
          case 16: {
            Ver = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// 新邮件通知
  /// </summary>
  public sealed class CmdMailNewMailNtf : pb::IMessage {
    private static readonly pb::MessageParser<CmdMailNewMailNtf> _parser = new pb::MessageParser<CmdMailNewMailNtf>(() => new CmdMailNewMailNtf());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdMailNewMailNtf> Parser { get { return _parser; } }

    /// <summary>Field number for the "clearCliMail" field.</summary>
    public const int ClearCliMailFieldNumber = 1;
    private bool clearCliMail_;
    /// <summary>
    /// 清空客户端存储邮件标记
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool ClearCliMail {
      get { return clearCliMail_; }
      set {
        clearCliMail_ = value;
      }
    }

    /// <summary>Field number for the "ver" field.</summary>
    public const int VerFieldNumber = 2;
    private uint ver_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Ver {
      get { return ver_; }
      set {
        ver_ = value;
      }
    }

    /// <summary>Field number for the "data" field.</summary>
    public const int DataFieldNumber = 3;
    private static readonly pb::FieldCodec<global::Packet.MailData> _repeated_data_codec
        = pb::FieldCodec.ForMessage(26, global::Packet.MailData.Parser);
    private readonly pbc::RepeatedField<global::Packet.MailData> data_ = new pbc::RepeatedField<global::Packet.MailData>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Packet.MailData> Data {
      get { return data_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (ClearCliMail != false) {
        output.WriteRawTag(8);
        output.WriteBool(ClearCliMail);
      }
      if (Ver != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Ver);
      }
      data_.WriteTo(output, _repeated_data_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ClearCliMail != false) {
        size += 1 + 1;
      }
      if (Ver != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Ver);
      }
      size += data_.CalculateSize(_repeated_data_codec);
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            ClearCliMail = input.ReadBool();
            break;
          }
          case 16: {
            Ver = input.ReadUInt32();
            break;
          }
          case 26: {
            data_.AddEntriesFrom(input, _repeated_data_codec);
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// 设置客户端邮件版本
  /// </summary>
  public sealed class CmdMailSetCliVer : pb::IMessage {
    private static readonly pb::MessageParser<CmdMailSetCliVer> _parser = new pb::MessageParser<CmdMailSetCliVer>(() => new CmdMailSetCliVer());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdMailSetCliVer> Parser { get { return _parser; } }

    /// <summary>Field number for the "cliVersion" field.</summary>
    public const int CliVersionFieldNumber = 1;
    private uint cliVersion_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint CliVersion {
      get { return cliVersion_; }
      set {
        cliVersion_ = value;
      }
    }

    /// <summary>Field number for the "cliMaxMailId" field.</summary>
    public const int CliMaxMailIdFieldNumber = 2;
    private ulong cliMaxMailId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong CliMaxMailId {
      get { return cliMaxMailId_; }
      set {
        cliMaxMailId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (CliVersion != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(CliVersion);
      }
      if (CliMaxMailId != 0UL) {
        output.WriteRawTag(16);
        output.WriteUInt64(CliMaxMailId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (CliVersion != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(CliVersion);
      }
      if (CliMaxMailId != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(CliMaxMailId);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            CliVersion = input.ReadUInt32();
            break;
          }
          case 16: {
            CliMaxMailId = input.ReadUInt64();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code