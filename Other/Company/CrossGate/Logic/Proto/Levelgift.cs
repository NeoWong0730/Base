// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: levelgift.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using scg = global::System.Collections.Generic;
namespace Packet {

  #region Enums
  /// <summary>
  /// 5300+
  /// </summary>
  public enum CmdLevelGift {
    None = 0,
    /// <summary>
    /// 等级礼包数据通知
    /// </summary>
    DataNtf = 5301,
    /// <summary>
    /// 领取礼包
    /// </summary>
    GetGiftReq = 5302,
    /// <summary>
    /// 领取礼包
    /// </summary>
    GetGiftRes = 5303,
  }

  public enum ErrorLevelGift {
    None = 0,
    /// <summary>
    /// 等级不足
    /// </summary>
    LevelErr = 5301,
    /// <summary>
    /// 礼包已领
    /// </summary>
    HaveGet = 5302,
    /// <summary>
    /// 背包已满
    /// </summary>
    BagIsFull = 5303,
    /// <summary>
    /// 不存在的礼包id
    /// </summary>
    GiftIdErr = 5304,
    /// <summary>
    /// 配置错误
    /// </summary>
    CfgErr = 5305,
  }

  public enum LvGiftItemReason {
    None = 0,
    /// <summary>
    /// 等级礼包
    /// </summary>
    Gift = 5301,
  }

  #endregion

  #region Messages
  public sealed class LevelGiftUnit : pb::IMessage {
    private static readonly pb::MessageParser<LevelGiftUnit> _parser = new pb::MessageParser<LevelGiftUnit>(() => new LevelGiftUnit());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<LevelGiftUnit> Parser { get { return _parser; } }

    /// <summary>Field number for the "giftId" field.</summary>
    public const int GiftIdFieldNumber = 1;
    private uint giftId_;
    /// <summary>
    /// 礼包id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint GiftId {
      get { return giftId_; }
      set {
        giftId_ = value;
      }
    }

    /// <summary>Field number for the "canGet" field.</summary>
    public const int CanGetFieldNumber = 2;
    private bool canGet_;
    /// <summary>
    /// 是否可领
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool CanGet {
      get { return canGet_; }
      set {
        canGet_ = value;
      }
    }

    /// <summary>Field number for the "isGet" field.</summary>
    public const int IsGetFieldNumber = 3;
    private bool isGet_;
    /// <summary>
    /// 是否已领
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool IsGet {
      get { return isGet_; }
      set {
        isGet_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (GiftId != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(GiftId);
      }
      if (CanGet != false) {
        output.WriteRawTag(16);
        output.WriteBool(CanGet);
      }
      if (IsGet != false) {
        output.WriteRawTag(24);
        output.WriteBool(IsGet);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (GiftId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(GiftId);
      }
      if (CanGet != false) {
        size += 1 + 1;
      }
      if (IsGet != false) {
        size += 1 + 1;
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
            GiftId = input.ReadUInt32();
            break;
          }
          case 16: {
            CanGet = input.ReadBool();
            break;
          }
          case 24: {
            IsGet = input.ReadBool();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// 等级礼包数据通知
  /// </summary>
  public sealed class CmdLevelGiftDataNtf : pb::IMessage {
    private static readonly pb::MessageParser<CmdLevelGiftDataNtf> _parser = new pb::MessageParser<CmdLevelGiftDataNtf>(() => new CmdLevelGiftDataNtf());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdLevelGiftDataNtf> Parser { get { return _parser; } }

    /// <summary>Field number for the "gifts" field.</summary>
    public const int GiftsFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Packet.LevelGiftUnit> _repeated_gifts_codec
        = pb::FieldCodec.ForMessage(10, global::Packet.LevelGiftUnit.Parser);
    private readonly pbc::RepeatedField<global::Packet.LevelGiftUnit> gifts_ = new pbc::RepeatedField<global::Packet.LevelGiftUnit>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Packet.LevelGiftUnit> Gifts {
      get { return gifts_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      gifts_.WriteTo(output, _repeated_gifts_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += gifts_.CalculateSize(_repeated_gifts_codec);
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
          case 10: {
            gifts_.AddEntriesFrom(input, _repeated_gifts_codec);
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// 领取礼包
  /// </summary>
  public sealed class CmdLevelGiftGetGiftReq : pb::IMessage {
    private static readonly pb::MessageParser<CmdLevelGiftGetGiftReq> _parser = new pb::MessageParser<CmdLevelGiftGetGiftReq>(() => new CmdLevelGiftGetGiftReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdLevelGiftGetGiftReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "giftId" field.</summary>
    public const int GiftIdFieldNumber = 1;
    private uint giftId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint GiftId {
      get { return giftId_; }
      set {
        giftId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (GiftId != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(GiftId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (GiftId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(GiftId);
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
            GiftId = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class CmdLevelGiftGetGiftRes : pb::IMessage {
    private static readonly pb::MessageParser<CmdLevelGiftGetGiftRes> _parser = new pb::MessageParser<CmdLevelGiftGetGiftRes>(() => new CmdLevelGiftGetGiftRes());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CmdLevelGiftGetGiftRes> Parser { get { return _parser; } }

    /// <summary>Field number for the "gift" field.</summary>
    public const int GiftFieldNumber = 1;
    private global::Packet.LevelGiftUnit gift_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Packet.LevelGiftUnit Gift {
      get { return gift_; }
      set {
        gift_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (gift_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Gift);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (gift_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Gift);
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
          case 10: {
            if (gift_ == null) {
              gift_ = new global::Packet.LevelGiftUnit();
            }
            input.ReadMessage(gift_);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code