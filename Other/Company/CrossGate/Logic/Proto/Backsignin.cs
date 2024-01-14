// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: backsignin.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using scg = global::System.Collections.Generic;
namespace Packet {

  #region Enums
  /// <summary>
  /// 9000+
  /// </summary>
  public enum CmdBackSignIn {
    None = 0,
    /// <summary>
    ///获取回归签到数据
    /// </summary>
    GetBackSignInDataReq = 9001,
    GetBackSignInDataRes = 9002,
    /// <summary>
    ///领取回归签到奖励
    /// </summary>
    GetAwardReq = 9003,
    GetAwardRes = 9004,
    /// <summary>
    ///红点提示(仅仅在第一次登录时发送)
    /// </summary>
    RedTipsNtf = 9010,
  }

  public enum ErrorBackSignIn {
    None = 0,
    /// <summary>
    ///获取表失败
    /// </summary>
    Csvnull = 9001,
    /// <summary>
    ///签到天数错误
    /// </summary>
    SignDayIndex = 9002,
    /// <summary>
    ///已经领取过了
    /// </summary>
    AwardGeted = 9003,
    /// <summary>
    ///表配置错误
    /// </summary>
    DropId = 9004,
    /// <summary>
    ///不是回归用户
    /// </summary>
    NotActivityReturn = 9005,
  }

  /// <summary>
  ///回归奖励分组
  /// </summary>
  public enum BackSignInGroup {
    None = 0,
    /// <summary>
    ///旅人组
    /// </summary>
    Lvren = 1,
    /// <summary>
    ///勇者组
    /// </summary>
    Yongzhe = 2,
    /// <summary>
    ///宗师组
    /// </summary>
    Zongshi = 3,
  }

  public enum BackSignInActiveReason {
    None = 0,
    /// <summary>
    ///领取签到奖励
    /// </summary>
    SignAward = 9001,
  }

  #endregion

  #region Messages
  /// <summary>
  ///CmdBackSignIn_GetBackSignInDataReq = 9001; //获取回归签到数据
  /// </summary>
  public sealed class CmdBackSignInGetBackSignInDataReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdBackSignInGetBackSignInDataReq> _parser = new pb::MessageParser<CmdBackSignInGetBackSignInDataReq>(() => new CmdBackSignInGetBackSignInDataReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdBackSignInGetBackSignInDataReq> Parser { get { return _parser; } }

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
  ///CmdBackSignIn_GetBackSignInDataRes = 9002;
  /// </summary>
  public sealed class CmdBackSignInGetBackSignInDataRes : pb::IMessage {
    private static readonly pb::MessageParser<CmdBackSignInGetBackSignInDataRes> _parser = new pb::MessageParser<CmdBackSignInGetBackSignInDataRes>(() => new CmdBackSignInGetBackSignInDataRes());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdBackSignInGetBackSignInDataRes> Parser { get { return _parser; } }

    /// <summary>Field number for the "totalLogin" field.</summary>
    public const int TotalLoginFieldNumber = 1;
    private uint totalLogin_;
    /// <summary>
    ///回归期间内总的登录次数
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint TotalLogin {
      get { return totalLogin_; }
      set {
        totalLogin_ = value;
      }
    }

    /// <summary>Field number for the "awardGeted" field.</summary>
    public const int AwardGetedFieldNumber = 2;
    private uint awardGeted_;
    /// <summary>
    /// 奖励领取情况  bit位操作:第7天|第6天.....|第2天|第1天. bit位低位 1 领取过了 0 可领取
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint AwardGeted {
      get { return awardGeted_; }
      set {
        awardGeted_ = value;
      }
    }

    /// <summary>Field number for the "group" field.</summary>
    public const int GroupFieldNumber = 3;
    private uint group_;
    /// <summary>
    ///所属分组 BackSignInGroup
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Group {
      get { return group_; }
      set {
        group_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (TotalLogin != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(TotalLogin);
      }
      if (AwardGeted != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(AwardGeted);
      }
      if (Group != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(Group);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (TotalLogin != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(TotalLogin);
      }
      if (AwardGeted != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(AwardGeted);
      }
      if (Group != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Group);
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
            TotalLogin = input.ReadUInt32();
            break;
          }
          case 16: {
            AwardGeted = input.ReadUInt32();
            break;
          }
          case 24: {
            Group = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  ///CmdBackSignIn_GetAwardReq = 9003; //领取回归签到奖励
  /// </summary>
  public sealed class CmdBackSignInGetAwardReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdBackSignInGetAwardReq> _parser = new pb::MessageParser<CmdBackSignInGetAwardReq>(() => new CmdBackSignInGetAwardReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdBackSignInGetAwardReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "daySeqIndex" field.</summary>
    public const int DaySeqIndexFieldNumber = 1;
    private uint daySeqIndex_;
    /// <summary>
    ///第几天 0 1 2 3 .... 6
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint DaySeqIndex {
      get { return daySeqIndex_; }
      set {
        daySeqIndex_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (DaySeqIndex != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(DaySeqIndex);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (DaySeqIndex != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(DaySeqIndex);
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
            DaySeqIndex = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  ///CmdBackSignIn_GetAwardRes = 9004;
  /// </summary>
  public sealed class CmdBackSignInGetAwardRes : pb::IMessage {
    private static readonly pb::MessageParser<CmdBackSignInGetAwardRes> _parser = new pb::MessageParser<CmdBackSignInGetAwardRes>(() => new CmdBackSignInGetAwardRes());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdBackSignInGetAwardRes> Parser { get { return _parser; } }

    /// <summary>Field number for the "daySeqIndex" field.</summary>
    public const int DaySeqIndexFieldNumber = 1;
    private uint daySeqIndex_;
    /// <summary>
    ///第几天 0 1 2 3 .... 6
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint DaySeqIndex {
      get { return daySeqIndex_; }
      set {
        daySeqIndex_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (DaySeqIndex != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(DaySeqIndex);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (DaySeqIndex != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(DaySeqIndex);
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
            DaySeqIndex = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  ///CmdBackSignIn_RedTipsNtf = 9010; //红点提示
  /// </summary>
  public sealed class CmdBackSignInRedTipsNtf : pb::IMessage {
    private static readonly pb::MessageParser<CmdBackSignInRedTipsNtf> _parser = new pb::MessageParser<CmdBackSignInRedTipsNtf>(() => new CmdBackSignInRedTipsNtf());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdBackSignInRedTipsNtf> Parser { get { return _parser; } }

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
