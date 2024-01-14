// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: compete.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using scg = global::System.Collections.Generic;
namespace Packet {

  #region Enums
  /// <summary>
  /// 3000+
  /// </summary>
  public enum CmdCompete {
    None = 0,
    /// <summary>
    ///邀请人(队长)发起邀请切磋请求
    /// </summary>
    InviteReq = 3001,
    /// <summary>
    ///邀请人(队长)收到请求恢复
    /// </summary>
    InviteRes = 3002,
    /// <summary>
    ///被邀请人回复切磋请求
    /// </summary>
    InviteOpReq = 3003,
    /// <summary>
    ///被邀请人回复切磋请求应答
    /// </summary>
    InviteOpRes = 3004,
    /// <summary>
    ///邀请人方 被邀请方接受邀请状态通知(在发起邀请时只给双方发送一次)
    /// </summary>
    InviteNtf = 3005,
    /// <summary>
    ///取消邀请切磋
    /// </summary>
    CancelInviteReq = 3006,
    /// <summary>
    ///取消邀请切磋应答
    /// </summary>
    CancelInviteRes = 3007,
    /// <summary>
    ///邀请切磋过程超时
    /// </summary>
    TimeOutCancelReq = 3008,
    /// <summary>
    ///邀请状态通知
    /// </summary>
    InviteStateNtf = 3009,
  }

  public enum ErrorCompete {
    None = 0,
    /// <summary>
    ///获取表失败
    /// </summary>
    Csvnull = 3001,
    /// <summary>
    ///切磋状态错误
    /// </summary>
    State = 3002,
    /// <summary>
    ///被邀请方角色不在线
    /// </summary>
    BeInviteRoleNotExsit = 3003,
    /// <summary>
    ///角色不是队长
    /// </summary>
    RoleNotLeader = 3004,
    /// <summary>
    ///当前地图不允许切磋
    /// </summary>
    MapCanNotPk = 3005,
    /// <summary>
    ///当前地图场景id错误
    /// </summary>
    MapSceneId = 3006,
    /// <summary>
    ///创建战斗错误
    /// </summary>
    CreateBattle = 3007,
    /// <summary>
    ///角色正在处于邀请中
    /// </summary>
    RoleIsInviting = 3008,
    /// <summary>
    ///在客户端设置里面设置拒绝邀请
    /// </summary>
    SetInviteRefuse = 3009,
    /// <summary>
    ///不在同一张地图中
    /// </summary>
    NotInSameMap = 3010,
    /// <summary>
    ///不在相互视野中
    /// </summary>
    NotInSameView = 3011,
    /// <summary>
    ///不能切磋自己或者同队
    /// </summary>
    CompeteOneSelf = 3012,
    /// <summary>
    ///自己已取消切磋
    /// </summary>
    InviteCancel = 3013,
    /// <summary>
    ///自己或者对方已经处于战斗中
    /// </summary>
    InBattle = 3014,
    /// <summary>
    ///邀请切磋或者回复邀请操作超时
    /// </summary>
    TimeOut = 3015,
    /// <summary>
    ///邀请方已经取消过邀请了
    /// </summary>
    HaveCanceled = 3016,
    /// <summary>
    ///主动邀请方角色不在线
    /// </summary>
    InviteRoleNotExsit = 3017,
  }

  /// <summary>
  ///邀请切磋状态
  /// </summary>
  public enum CompeteState {
    /// <summary>
    ///角色正常状态
    /// </summary>
    CompeteNormal = 0,
    /// <summary>
    ///邀请切磋请求中状态
    /// </summary>
    CompeteInviting = 1,
    /// <summary>
    ///邀请切磋同意状态
    /// </summary>
    CompeteInviteAgree = 2,
    /// <summary>
    ///邀请切磋拒绝状态
    /// </summary>
    CompeteInviteRefuse = 3,
    /// <summary>
    ///邀请切磋超时状态
    /// </summary>
    CompeteInviteTimeOut = 4,
    /// <summary>
    ///取消邀请切磋状态
    /// </summary>
    CompeteCancel = 5,
    /// <summary>
    ///切磋战斗状态
    /// </summary>
    CompeteFighting = 6,
    /// <summary>
    ///在客户端设置里面设置拒绝邀请
    /// </summary>
    CompeteSetInviteRefuse = 7,
  }

  #endregion

  #region Messages
  /// <summary>
  ///邀请人(队长)发起邀请切磋请求
  /// </summary>
  public sealed class CmdCompeteInviteReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdCompeteInviteReq> _parser = new pb::MessageParser<CmdCompeteInviteReq>(() => new CmdCompeteInviteReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdCompeteInviteReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "beInviteRoleId" field.</summary>
    public const int BeInviteRoleIdFieldNumber = 1;
    private ulong beInviteRoleId_;
    /// <summary>
    ///被邀请者角色id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong BeInviteRoleId {
      get { return beInviteRoleId_; }
      set {
        beInviteRoleId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (BeInviteRoleId != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(BeInviteRoleId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (BeInviteRoleId != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(BeInviteRoleId);
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
            BeInviteRoleId = input.ReadUInt64();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  ///邀请人(队长)收到请求恢复
  /// </summary>
  public sealed class CmdCompeteInviteRes : pb::IMessage {
    private static readonly pb::MessageParser<CmdCompeteInviteRes> _parser = new pb::MessageParser<CmdCompeteInviteRes>(() => new CmdCompeteInviteRes());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdCompeteInviteRes> Parser { get { return _parser; } }

    /// <summary>Field number for the "memNum" field.</summary>
    public const int MemNumFieldNumber = 1;
    private uint memNum_;
    /// <summary>
    ///对方的玩家当前实际组队人数
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint MemNum {
      get { return memNum_; }
      set {
        memNum_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (MemNum != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(MemNum);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (MemNum != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(MemNum);
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
            MemNum = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  ///邀请人方 被邀请方接受邀请状态通知(在发起邀请时只给双方发送一次)
  /// </summary>
  public sealed class CmdCompeteInviteNtf : pb::IMessage {
    private static readonly pb::MessageParser<CmdCompeteInviteNtf> _parser = new pb::MessageParser<CmdCompeteInviteNtf>(() => new CmdCompeteInviteNtf());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdCompeteInviteNtf> Parser { get { return _parser; } }

    /// <summary>Field number for the "inviteId" field.</summary>
    public const int InviteIdFieldNumber = 1;
    private ulong inviteId_;
    /// <summary>
    ///邀请方(队长)id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong InviteId {
      get { return inviteId_; }
      set {
        inviteId_ = value;
      }
    }

    /// <summary>Field number for the "beInviteId" field.</summary>
    public const int BeInviteIdFieldNumber = 2;
    private ulong beInviteId_;
    /// <summary>
    ///被邀请方(队长)id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong BeInviteId {
      get { return beInviteId_; }
      set {
        beInviteId_ = value;
      }
    }

    /// <summary>Field number for the "state" field.</summary>
    public const int StateFieldNumber = 3;
    private uint state_;
    /// <summary>
    ///切磋状态
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint State {
      get { return state_; }
      set {
        state_ = value;
      }
    }

    /// <summary>Field number for the "memNum" field.</summary>
    public const int MemNumFieldNumber = 4;
    private uint memNum_;
    /// <summary>
    ///对方的玩家当前实际组队人数
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint MemNum {
      get { return memNum_; }
      set {
        memNum_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (InviteId != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(InviteId);
      }
      if (BeInviteId != 0UL) {
        output.WriteRawTag(16);
        output.WriteUInt64(BeInviteId);
      }
      if (State != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(State);
      }
      if (MemNum != 0) {
        output.WriteRawTag(32);
        output.WriteUInt32(MemNum);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (InviteId != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(InviteId);
      }
      if (BeInviteId != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(BeInviteId);
      }
      if (State != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(State);
      }
      if (MemNum != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(MemNum);
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
            InviteId = input.ReadUInt64();
            break;
          }
          case 16: {
            BeInviteId = input.ReadUInt64();
            break;
          }
          case 24: {
            State = input.ReadUInt32();
            break;
          }
          case 32: {
            MemNum = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  ///邀请状态通知
  /// </summary>
  public sealed class CmdCompeteInviteStateNtf : pb::IMessage {
    private static readonly pb::MessageParser<CmdCompeteInviteStateNtf> _parser = new pb::MessageParser<CmdCompeteInviteStateNtf>(() => new CmdCompeteInviteStateNtf());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdCompeteInviteStateNtf> Parser { get { return _parser; } }

    /// <summary>Field number for the "state" field.</summary>
    public const int StateFieldNumber = 1;
    private uint state_;
    /// <summary>
    ///切磋状态
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint State {
      get { return state_; }
      set {
        state_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (State != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(State);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (State != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(State);
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
            State = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  ///被邀请人回复切磋请求
  /// </summary>
  public sealed class CmdCompeteInviteOpReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdCompeteInviteOpReq> _parser = new pb::MessageParser<CmdCompeteInviteOpReq>(() => new CmdCompeteInviteOpReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdCompeteInviteOpReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "state" field.</summary>
    public const int StateFieldNumber = 1;
    private uint state_;
    /// <summary>
    ///切磋状态
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint State {
      get { return state_; }
      set {
        state_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (State != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(State);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (State != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(State);
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
            State = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  ///被邀请人回复切磋请求应答
  /// </summary>
  public sealed class CmdCompeteInviteOpRes : pb::IMessage {
    private static readonly pb::MessageParser<CmdCompeteInviteOpRes> _parser = new pb::MessageParser<CmdCompeteInviteOpRes>(() => new CmdCompeteInviteOpRes());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdCompeteInviteOpRes> Parser { get { return _parser; } }

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

  /// <summary>
  ///取消邀请切磋
  /// </summary>
  public sealed class CmdCompeteCancelInviteReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdCompeteCancelInviteReq> _parser = new pb::MessageParser<CmdCompeteCancelInviteReq>(() => new CmdCompeteCancelInviteReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdCompeteCancelInviteReq> Parser { get { return _parser; } }

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

  /// <summary>
  ///取消邀请切磋应答
  /// </summary>
  public sealed class CmdCompeteCancelInviteRes : pb::IMessage {
    private static readonly pb::MessageParser<CmdCompeteCancelInviteRes> _parser = new pb::MessageParser<CmdCompeteCancelInviteRes>(() => new CmdCompeteCancelInviteRes());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdCompeteCancelInviteRes> Parser { get { return _parser; } }

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

  /// <summary>
  ///邀请切磋过程超时
  /// </summary>
  public sealed class CmdCompeteTimeOutCancelReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdCompeteTimeOutCancelReq> _parser = new pb::MessageParser<CmdCompeteTimeOutCancelReq>(() => new CmdCompeteTimeOutCancelReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdCompeteTimeOutCancelReq> Parser { get { return _parser; } }

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

  #endregion

}

#endregion Designer generated code