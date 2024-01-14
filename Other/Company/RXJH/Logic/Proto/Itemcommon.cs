// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: itemcommon.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using scg = global::System.Collections.Generic;
namespace Common {

  #region Enums
  public enum CurrencyType {
    CurrencyNone = 0,
    CurrencySilver = 1,
    CurrencyGold = 2,
  }

  public enum ItemReason {
    None = 0,
    RmvBagUpgrade = 1,
    AddBatDecompose = 2,
    RmvBatDecompose = 3,
    AddBatSale = 4,
    RmvBatSale = 5,
    RmvCommit = 6,
    AddAuthenticate = 7,
    RmvAuthenticate = 8,
    RmvUse = 9,
    AddTaskReward = 10,
  }

  #endregion

  #region Messages
  public sealed class BagItem : pb::IMessage {
    private static readonly pb::MessageParser<BagItem> _parser = new pb::MessageParser<BagItem>(() => new BagItem());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<BagItem> Parser { get { return _parser; } }

    /// <summary>Field number for the "pos" field.</summary>
    public const int PosFieldNumber = 1;
    private uint pos_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Pos {
      get { return pos_; }
      set {
        pos_ = value;
      }
    }

    /// <summary>Field number for the "item" field.</summary>
    public const int ItemFieldNumber = 2;
    private global::Common.Item item_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Common.Item Item {
      get { return item_; }
      set {
        item_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Pos != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Pos);
      }
      if (item_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Item);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Pos != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Pos);
      }
      if (item_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Item);
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
            Pos = input.ReadUInt32();
            break;
          }
          case 18: {
            if (item_ == null) {
              item_ = new global::Common.Item();
            }
            input.ReadMessage(item_);
            break;
          }
        }
      }
    }

  }

  public sealed class Bag : pb::IMessage {
    private static readonly pb::MessageParser<Bag> _parser = new pb::MessageParser<Bag>(() => new Bag());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Bag> Parser { get { return _parser; } }

    /// <summary>Field number for the "type" field.</summary>
    public const int TypeFieldNumber = 1;
    private uint type_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Type {
      get { return type_; }
      set {
        type_ = value;
      }
    }

    /// <summary>Field number for the "stage" field.</summary>
    public const int StageFieldNumber = 2;
    private uint stage_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Stage {
      get { return stage_; }
      set {
        stage_ = value;
      }
    }

    /// <summary>Field number for the "sortcd" field.</summary>
    public const int SortcdFieldNumber = 3;
    private uint sortcd_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Sortcd {
      get { return sortcd_; }
      set {
        sortcd_ = value;
      }
    }

    /// <summary>Field number for the "items" field.</summary>
    public const int ItemsFieldNumber = 4;
    private static readonly pb::FieldCodec<global::Common.BagItem> _repeated_items_codec
        = pb::FieldCodec.ForMessage(34, global::Common.BagItem.Parser);
    private readonly pbc::RepeatedField<global::Common.BagItem> items_ = new pbc::RepeatedField<global::Common.BagItem>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Common.BagItem> Items {
      get { return items_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Type != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Type);
      }
      if (Stage != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Stage);
      }
      if (Sortcd != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(Sortcd);
      }
      items_.WriteTo(output, _repeated_items_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Type != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Type);
      }
      if (Stage != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Stage);
      }
      if (Sortcd != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Sortcd);
      }
      size += items_.CalculateSize(_repeated_items_codec);
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
            Type = input.ReadUInt32();
            break;
          }
          case 16: {
            Stage = input.ReadUInt32();
            break;
          }
          case 24: {
            Sortcd = input.ReadUInt32();
            break;
          }
          case 34: {
            items_.AddEntriesFrom(input, _repeated_items_codec);
            break;
          }
        }
      }
    }

  }

  public sealed class ItemBase : pb::IMessage {
    private static readonly pb::MessageParser<ItemBase> _parser = new pb::MessageParser<ItemBase>(() => new ItemBase());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ItemBase> Parser { get { return _parser; } }

    /// <summary>Field number for the "uid" field.</summary>
    public const int UidFieldNumber = 1;
    private uint uid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Uid {
      get { return uid_; }
      set {
        uid_ = value;
      }
    }

    /// <summary>Field number for the "tid" field.</summary>
    public const int TidFieldNumber = 2;
    private uint tid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Tid {
      get { return tid_; }
      set {
        tid_ = value;
      }
    }

    /// <summary>Field number for the "count" field.</summary>
    public const int CountFieldNumber = 3;
    private uint count_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Count {
      get { return count_; }
      set {
        count_ = value;
      }
    }

    /// <summary>Field number for the "bind" field.</summary>
    public const int BindFieldNumber = 4;
    private uint bind_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Bind {
      get { return bind_; }
      set {
        bind_ = value;
      }
    }

    /// <summary>Field number for the "expire" field.</summary>
    public const int ExpireFieldNumber = 5;
    private uint expire_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Expire {
      get { return expire_; }
      set {
        expire_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Uid != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Uid);
      }
      if (Tid != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Tid);
      }
      if (Count != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(Count);
      }
      if (Bind != 0) {
        output.WriteRawTag(32);
        output.WriteUInt32(Bind);
      }
      if (Expire != 0) {
        output.WriteRawTag(40);
        output.WriteUInt32(Expire);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Uid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Uid);
      }
      if (Tid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Tid);
      }
      if (Count != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Count);
      }
      if (Bind != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Bind);
      }
      if (Expire != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Expire);
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
            Uid = input.ReadUInt32();
            break;
          }
          case 16: {
            Tid = input.ReadUInt32();
            break;
          }
          case 24: {
            Count = input.ReadUInt32();
            break;
          }
          case 32: {
            Bind = input.ReadUInt32();
            break;
          }
          case 40: {
            Expire = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class ItemEquip : pb::IMessage {
    private static readonly pb::MessageParser<ItemEquip> _parser = new pb::MessageParser<ItemEquip>(() => new ItemEquip());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ItemEquip> Parser { get { return _parser; } }

    /// <summary>Field number for the "equiptid" field.</summary>
    public const int EquiptidFieldNumber = 1;
    private uint equiptid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Equiptid {
      get { return equiptid_; }
      set {
        equiptid_ = value;
      }
    }

    /// <summary>Field number for the "quality" field.</summary>
    public const int QualityFieldNumber = 2;
    private uint quality_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Quality {
      get { return quality_; }
      set {
        quality_ = value;
      }
    }

    /// <summary>Field number for the "durability" field.</summary>
    public const int DurabilityFieldNumber = 3;
    private uint durability_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Durability {
      get { return durability_; }
      set {
        durability_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Equiptid != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Equiptid);
      }
      if (Quality != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Quality);
      }
      if (Durability != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(Durability);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Equiptid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Equiptid);
      }
      if (Quality != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Quality);
      }
      if (Durability != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Durability);
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
            Equiptid = input.ReadUInt32();
            break;
          }
          case 16: {
            Quality = input.ReadUInt32();
            break;
          }
          case 24: {
            Durability = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class Item : pb::IMessage {
    private static readonly pb::MessageParser<Item> _parser = new pb::MessageParser<Item>(() => new Item());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Item> Parser { get { return _parser; } }

    /// <summary>Field number for the "base" field.</summary>
    public const int BaseFieldNumber = 1;
    private global::Common.ItemBase base_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Common.ItemBase Base {
      get { return base_; }
      set {
        base_ = value;
      }
    }

    /// <summary>Field number for the "equip" field.</summary>
    public const int EquipFieldNumber = 2;
    private global::Common.ItemEquip equip_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Common.ItemEquip Equip {
      get { return equip_; }
      set {
        equip_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (base_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Base);
      }
      if (equip_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Equip);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (base_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Base);
      }
      if (equip_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Equip);
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
            if (base_ == null) {
              base_ = new global::Common.ItemBase();
            }
            input.ReadMessage(base_);
            break;
          }
          case 18: {
            if (equip_ == null) {
              equip_ = new global::Common.ItemEquip();
            }
            input.ReadMessage(equip_);
            break;
          }
        }
      }
    }

  }

  public sealed class ItemAdd : pb::IMessage {
    private static readonly pb::MessageParser<ItemAdd> _parser = new pb::MessageParser<ItemAdd>(() => new ItemAdd());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ItemAdd> Parser { get { return _parser; } }

    /// <summary>Field number for the "bag" field.</summary>
    public const int BagFieldNumber = 1;
    private uint bag_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Bag {
      get { return bag_; }
      set {
        bag_ = value;
      }
    }

    /// <summary>Field number for the "item" field.</summary>
    public const int ItemFieldNumber = 2;
    private global::Common.BagItem item_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Common.BagItem Item {
      get { return item_; }
      set {
        item_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Bag != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Bag);
      }
      if (item_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Item);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Bag != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Bag);
      }
      if (item_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Item);
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
            Bag = input.ReadUInt32();
            break;
          }
          case 18: {
            if (item_ == null) {
              item_ = new global::Common.BagItem();
            }
            input.ReadMessage(item_);
            break;
          }
        }
      }
    }

  }

  public sealed class ItemUpdate : pb::IMessage {
    private static readonly pb::MessageParser<ItemUpdate> _parser = new pb::MessageParser<ItemUpdate>(() => new ItemUpdate());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ItemUpdate> Parser { get { return _parser; } }

    /// <summary>Field number for the "uid" field.</summary>
    public const int UidFieldNumber = 1;
    private uint uid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Uid {
      get { return uid_; }
      set {
        uid_ = value;
      }
    }

    /// <summary>Field number for the "count" field.</summary>
    public const int CountFieldNumber = 2;
    private uint count_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Count {
      get { return count_; }
      set {
        count_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Uid != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Uid);
      }
      if (Count != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Count);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Uid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Uid);
      }
      if (Count != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Count);
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
            Uid = input.ReadUInt32();
            break;
          }
          case 16: {
            Count = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class ItemMove : pb::IMessage {
    private static readonly pb::MessageParser<ItemMove> _parser = new pb::MessageParser<ItemMove>(() => new ItemMove());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ItemMove> Parser { get { return _parser; } }

    /// <summary>Field number for the "uid" field.</summary>
    public const int UidFieldNumber = 1;
    private uint uid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Uid {
      get { return uid_; }
      set {
        uid_ = value;
      }
    }

    /// <summary>Field number for the "bag" field.</summary>
    public const int BagFieldNumber = 2;
    private uint bag_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Bag {
      get { return bag_; }
      set {
        bag_ = value;
      }
    }

    /// <summary>Field number for the "pos" field.</summary>
    public const int PosFieldNumber = 3;
    private uint pos_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Pos {
      get { return pos_; }
      set {
        pos_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Uid != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Uid);
      }
      if (Bag != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Bag);
      }
      if (Pos != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(Pos);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Uid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Uid);
      }
      if (Bag != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Bag);
      }
      if (Pos != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Pos);
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
            Uid = input.ReadUInt32();
            break;
          }
          case 16: {
            Bag = input.ReadUInt32();
            break;
          }
          case 24: {
            Pos = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class ItemOperations : pb::IMessage {
    private static readonly pb::MessageParser<ItemOperations> _parser = new pb::MessageParser<ItemOperations>(() => new ItemOperations());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ItemOperations> Parser { get { return _parser; } }

    /// <summary>Field number for the "removes" field.</summary>
    public const int RemovesFieldNumber = 1;
    private static readonly pb::FieldCodec<uint> _repeated_removes_codec
        = pb::FieldCodec.ForUInt32(10);
    private readonly pbc::RepeatedField<uint> removes_ = new pbc::RepeatedField<uint>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<uint> Removes {
      get { return removes_; }
    }

    /// <summary>Field number for the "updates" field.</summary>
    public const int UpdatesFieldNumber = 2;
    private static readonly pb::FieldCodec<global::Common.ItemUpdate> _repeated_updates_codec
        = pb::FieldCodec.ForMessage(18, global::Common.ItemUpdate.Parser);
    private readonly pbc::RepeatedField<global::Common.ItemUpdate> updates_ = new pbc::RepeatedField<global::Common.ItemUpdate>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Common.ItemUpdate> Updates {
      get { return updates_; }
    }

    /// <summary>Field number for the "moves" field.</summary>
    public const int MovesFieldNumber = 3;
    private static readonly pb::FieldCodec<global::Common.ItemMove> _repeated_moves_codec
        = pb::FieldCodec.ForMessage(26, global::Common.ItemMove.Parser);
    private readonly pbc::RepeatedField<global::Common.ItemMove> moves_ = new pbc::RepeatedField<global::Common.ItemMove>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Common.ItemMove> Moves {
      get { return moves_; }
    }

    /// <summary>Field number for the "adds" field.</summary>
    public const int AddsFieldNumber = 4;
    private static readonly pb::FieldCodec<global::Common.ItemAdd> _repeated_adds_codec
        = pb::FieldCodec.ForMessage(34, global::Common.ItemAdd.Parser);
    private readonly pbc::RepeatedField<global::Common.ItemAdd> adds_ = new pbc::RepeatedField<global::Common.ItemAdd>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Common.ItemAdd> Adds {
      get { return adds_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      removes_.WriteTo(output, _repeated_removes_codec);
      updates_.WriteTo(output, _repeated_updates_codec);
      moves_.WriteTo(output, _repeated_moves_codec);
      adds_.WriteTo(output, _repeated_adds_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += removes_.CalculateSize(_repeated_removes_codec);
      size += updates_.CalculateSize(_repeated_updates_codec);
      size += moves_.CalculateSize(_repeated_moves_codec);
      size += adds_.CalculateSize(_repeated_adds_codec);
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
            removes_.AddEntriesFrom(input, _repeated_removes_codec);
            break;
          }
          case 18: {
            updates_.AddEntriesFrom(input, _repeated_updates_codec);
            break;
          }
          case 26: {
            moves_.AddEntriesFrom(input, _repeated_moves_codec);
            break;
          }
          case 34: {
            adds_.AddEntriesFrom(input, _repeated_adds_codec);
            break;
          }
        }
      }
    }

  }

  public sealed class Currency : pb::IMessage {
    private static readonly pb::MessageParser<Currency> _parser = new pb::MessageParser<Currency>(() => new Currency());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Currency> Parser { get { return _parser; } }

    /// <summary>Field number for the "currencyid" field.</summary>
    public const int CurrencyidFieldNumber = 1;
    private uint currencyid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Currencyid {
      get { return currencyid_; }
      set {
        currencyid_ = value;
      }
    }

    /// <summary>Field number for the "value" field.</summary>
    public const int ValueFieldNumber = 2;
    private long value_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long Value {
      get { return value_; }
      set {
        value_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Currencyid != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Currencyid);
      }
      if (Value != 0L) {
        output.WriteRawTag(16);
        output.WriteInt64(Value);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Currencyid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Currencyid);
      }
      if (Value != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(Value);
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
            Currencyid = input.ReadUInt32();
            break;
          }
          case 16: {
            Value = input.ReadInt64();
            break;
          }
        }
      }
    }

  }

  public sealed class ItemColddown : pb::IMessage {
    private static readonly pb::MessageParser<ItemColddown> _parser = new pb::MessageParser<ItemColddown>(() => new ItemColddown());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ItemColddown> Parser { get { return _parser; } }

    /// <summary>Field number for the "cdgroupid" field.</summary>
    public const int CdgroupidFieldNumber = 1;
    private uint cdgroupid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Cdgroupid {
      get { return cdgroupid_; }
      set {
        cdgroupid_ = value;
      }
    }

    /// <summary>Field number for the "expire" field.</summary>
    public const int ExpireFieldNumber = 2;
    private uint expire_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Expire {
      get { return expire_; }
      set {
        expire_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Cdgroupid != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Cdgroupid);
      }
      if (Expire != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Expire);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Cdgroupid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Cdgroupid);
      }
      if (Expire != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Expire);
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
            Cdgroupid = input.ReadUInt32();
            break;
          }
          case 16: {
            Expire = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class ModuleItem : pb::IMessage {
    private static readonly pb::MessageParser<ModuleItem> _parser = new pb::MessageParser<ModuleItem>(() => new ModuleItem());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ModuleItem> Parser { get { return _parser; } }

    /// <summary>Field number for the "bags" field.</summary>
    public const int BagsFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Common.Bag> _repeated_bags_codec
        = pb::FieldCodec.ForMessage(10, global::Common.Bag.Parser);
    private readonly pbc::RepeatedField<global::Common.Bag> bags_ = new pbc::RepeatedField<global::Common.Bag>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Common.Bag> Bags {
      get { return bags_; }
    }

    /// <summary>Field number for the "currencies" field.</summary>
    public const int CurrenciesFieldNumber = 2;
    private static readonly pb::FieldCodec<global::Common.Currency> _repeated_currencies_codec
        = pb::FieldCodec.ForMessage(18, global::Common.Currency.Parser);
    private readonly pbc::RepeatedField<global::Common.Currency> currencies_ = new pbc::RepeatedField<global::Common.Currency>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Common.Currency> Currencies {
      get { return currencies_; }
    }

    /// <summary>Field number for the "cds" field.</summary>
    public const int CdsFieldNumber = 3;
    private static readonly pb::FieldCodec<global::Common.ItemColddown> _repeated_cds_codec
        = pb::FieldCodec.ForMessage(26, global::Common.ItemColddown.Parser);
    private readonly pbc::RepeatedField<global::Common.ItemColddown> cds_ = new pbc::RepeatedField<global::Common.ItemColddown>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Common.ItemColddown> Cds {
      get { return cds_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      bags_.WriteTo(output, _repeated_bags_codec);
      currencies_.WriteTo(output, _repeated_currencies_codec);
      cds_.WriteTo(output, _repeated_cds_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += bags_.CalculateSize(_repeated_bags_codec);
      size += currencies_.CalculateSize(_repeated_currencies_codec);
      size += cds_.CalculateSize(_repeated_cds_codec);
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
            bags_.AddEntriesFrom(input, _repeated_bags_codec);
            break;
          }
          case 18: {
            currencies_.AddEntriesFrom(input, _repeated_currencies_codec);
            break;
          }
          case 26: {
            cds_.AddEntriesFrom(input, _repeated_cds_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code