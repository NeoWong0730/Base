// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: fightcommon.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using scg = global::System.Collections.Generic;
namespace Common {

  #region Messages
  public sealed class SkillPos : pb::IMessage {
    private static readonly pb::MessageParser<SkillPos> _parser = new pb::MessageParser<SkillPos>(() => new SkillPos());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SkillPos> Parser { get { return _parser; } }

    /// <summary>Field number for the "x" field.</summary>
    public const int XFieldNumber = 1;
    private uint x_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint X {
      get { return x_; }
      set {
        x_ = value;
      }
    }

    /// <summary>Field number for the "y" field.</summary>
    public const int YFieldNumber = 2;
    private uint y_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Y {
      get { return y_; }
      set {
        y_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (X != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(X);
      }
      if (Y != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Y);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (X != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(X);
      }
      if (Y != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Y);
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
            X = input.ReadUInt32();
            break;
          }
          case 16: {
            Y = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class SkillRelatedEntity : pb::IMessage {
    private static readonly pb::MessageParser<SkillRelatedEntity> _parser = new pb::MessageParser<SkillRelatedEntity>(() => new SkillRelatedEntity());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SkillRelatedEntity> Parser { get { return _parser; } }

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

    /// <summary>Field number for the "uid" field.</summary>
    public const int UidFieldNumber = 2;
    private uint uid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Uid {
      get { return uid_; }
      set {
        uid_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Type != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Type);
      }
      if (Uid != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Uid);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Type != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Type);
      }
      if (Uid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Uid);
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
            Type = input.ReadUInt32();
            break;
          }
          case 16: {
            Uid = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class SkillCommonColddown : pb::IMessage {
    private static readonly pb::MessageParser<SkillCommonColddown> _parser = new pb::MessageParser<SkillCommonColddown>(() => new SkillCommonColddown());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SkillCommonColddown> Parser { get { return _parser; } }

    /// <summary>Field number for the "groupid" field.</summary>
    public const int GroupidFieldNumber = 1;
    private uint groupid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Groupid {
      get { return groupid_; }
      set {
        groupid_ = value;
      }
    }

    /// <summary>Field number for the "colddown" field.</summary>
    public const int ColddownFieldNumber = 2;
    private ulong colddown_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Colddown {
      get { return colddown_; }
      set {
        colddown_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Groupid != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Groupid);
      }
      if (Colddown != 0UL) {
        output.WriteRawTag(16);
        output.WriteUInt64(Colddown);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Groupid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Groupid);
      }
      if (Colddown != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Colddown);
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
            Groupid = input.ReadUInt32();
            break;
          }
          case 16: {
            Colddown = input.ReadUInt64();
            break;
          }
        }
      }
    }

  }

  public sealed class SkillColddown : pb::IMessage {
    private static readonly pb::MessageParser<SkillColddown> _parser = new pb::MessageParser<SkillColddown>(() => new SkillColddown());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SkillColddown> Parser { get { return _parser; } }

    /// <summary>Field number for the "skillid" field.</summary>
    public const int SkillidFieldNumber = 1;
    private uint skillid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Skillid {
      get { return skillid_; }
      set {
        skillid_ = value;
      }
    }

    /// <summary>Field number for the "colddown" field.</summary>
    public const int ColddownFieldNumber = 2;
    private ulong colddown_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Colddown {
      get { return colddown_; }
      set {
        colddown_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Skillid != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Skillid);
      }
      if (Colddown != 0UL) {
        output.WriteRawTag(16);
        output.WriteUInt64(Colddown);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Skillid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Skillid);
      }
      if (Colddown != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Colddown);
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
            Skillid = input.ReadUInt32();
            break;
          }
          case 16: {
            Colddown = input.ReadUInt64();
            break;
          }
        }
      }
    }

  }

  public sealed class SkillColddownList : pb::IMessage {
    private static readonly pb::MessageParser<SkillColddownList> _parser = new pb::MessageParser<SkillColddownList>(() => new SkillColddownList());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SkillColddownList> Parser { get { return _parser; } }

    /// <summary>Field number for the "commons" field.</summary>
    public const int CommonsFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Common.SkillCommonColddown> _repeated_commons_codec
        = pb::FieldCodec.ForMessage(10, global::Common.SkillCommonColddown.Parser);
    private readonly pbc::RepeatedField<global::Common.SkillCommonColddown> commons_ = new pbc::RepeatedField<global::Common.SkillCommonColddown>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Common.SkillCommonColddown> Commons {
      get { return commons_; }
    }

    /// <summary>Field number for the "single" field.</summary>
    public const int SingleFieldNumber = 2;
    private global::Common.SkillColddown single_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Common.SkillColddown Single {
      get { return single_; }
      set {
        single_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      commons_.WriteTo(output, _repeated_commons_codec);
      if (single_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Single);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += commons_.CalculateSize(_repeated_commons_codec);
      if (single_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Single);
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
            commons_.AddEntriesFrom(input, _repeated_commons_codec);
            break;
          }
          case 18: {
            if (single_ == null) {
              single_ = new global::Common.SkillColddown();
            }
            input.ReadMessage(single_);
            break;
          }
        }
      }
    }

  }

  public sealed class SkillTarget : pb::IMessage {
    private static readonly pb::MessageParser<SkillTarget> _parser = new pb::MessageParser<SkillTarget>(() => new SkillTarget());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SkillTarget> Parser { get { return _parser; } }

    /// <summary>Field number for the "pos" field.</summary>
    public const int PosFieldNumber = 1;
    private global::Common.SkillPos pos_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Common.SkillPos Pos {
      get { return pos_; }
      set {
        pos_ = value;
      }
    }

    /// <summary>Field number for the "entity" field.</summary>
    public const int EntityFieldNumber = 2;
    private global::Common.SkillRelatedEntity entity_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Common.SkillRelatedEntity Entity {
      get { return entity_; }
      set {
        entity_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (pos_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Pos);
      }
      if (entity_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Entity);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (pos_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Pos);
      }
      if (entity_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Entity);
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
            if (pos_ == null) {
              pos_ = new global::Common.SkillPos();
            }
            input.ReadMessage(pos_);
            break;
          }
          case 18: {
            if (entity_ == null) {
              entity_ = new global::Common.SkillRelatedEntity();
            }
            input.ReadMessage(entity_);
            break;
          }
        }
      }
    }

  }

  public sealed class SkillTargetList : pb::IMessage {
    private static readonly pb::MessageParser<SkillTargetList> _parser = new pb::MessageParser<SkillTargetList>(() => new SkillTargetList());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SkillTargetList> Parser { get { return _parser; } }

    /// <summary>Field number for the "targets" field.</summary>
    public const int TargetsFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Common.SkillTarget> _repeated_targets_codec
        = pb::FieldCodec.ForMessage(10, global::Common.SkillTarget.Parser);
    private readonly pbc::RepeatedField<global::Common.SkillTarget> targets_ = new pbc::RepeatedField<global::Common.SkillTarget>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Common.SkillTarget> Targets {
      get { return targets_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      targets_.WriteTo(output, _repeated_targets_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += targets_.CalculateSize(_repeated_targets_codec);
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
            targets_.AddEntriesFrom(input, _repeated_targets_codec);
            break;
          }
        }
      }
    }

  }

  public sealed class SkillBuff : pb::IMessage {
    private static readonly pb::MessageParser<SkillBuff> _parser = new pb::MessageParser<SkillBuff>(() => new SkillBuff());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SkillBuff> Parser { get { return _parser; } }

    /// <summary>Field number for the "buffid" field.</summary>
    public const int BuffidFieldNumber = 1;
    private uint buffid_;
    /// <summary>
    /// TODO
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Buffid {
      get { return buffid_; }
      set {
        buffid_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Buffid != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Buffid);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Buffid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Buffid);
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
            Buffid = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class SkillBuffList : pb::IMessage {
    private static readonly pb::MessageParser<SkillBuffList> _parser = new pb::MessageParser<SkillBuffList>(() => new SkillBuffList());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SkillBuffList> Parser { get { return _parser; } }

    /// <summary>Field number for the "buffs" field.</summary>
    public const int BuffsFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Common.SkillBuff> _repeated_buffs_codec
        = pb::FieldCodec.ForMessage(10, global::Common.SkillBuff.Parser);
    private readonly pbc::RepeatedField<global::Common.SkillBuff> buffs_ = new pbc::RepeatedField<global::Common.SkillBuff>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Common.SkillBuff> Buffs {
      get { return buffs_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      buffs_.WriteTo(output, _repeated_buffs_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += buffs_.CalculateSize(_repeated_buffs_codec);
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
            buffs_.AddEntriesFrom(input, _repeated_buffs_codec);
            break;
          }
        }
      }
    }

  }

  public sealed class SkillEffect : pb::IMessage {
    private static readonly pb::MessageParser<SkillEffect> _parser = new pb::MessageParser<SkillEffect>(() => new SkillEffect());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SkillEffect> Parser { get { return _parser; } }

    /// <summary>Field number for the "entity" field.</summary>
    public const int EntityFieldNumber = 1;
    private global::Common.SkillRelatedEntity entity_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Common.SkillRelatedEntity Entity {
      get { return entity_; }
      set {
        entity_ = value;
      }
    }

    /// <summary>Field number for the "bufflist" field.</summary>
    public const int BufflistFieldNumber = 2;
    private global::Common.SkillBuffList bufflist_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Common.SkillBuffList Bufflist {
      get { return bufflist_; }
      set {
        bufflist_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (entity_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Entity);
      }
      if (bufflist_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Bufflist);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (entity_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Entity);
      }
      if (bufflist_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Bufflist);
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
            if (entity_ == null) {
              entity_ = new global::Common.SkillRelatedEntity();
            }
            input.ReadMessage(entity_);
            break;
          }
          case 18: {
            if (bufflist_ == null) {
              bufflist_ = new global::Common.SkillBuffList();
            }
            input.ReadMessage(bufflist_);
            break;
          }
        }
      }
    }

  }

  public sealed class SkillEffectList : pb::IMessage {
    private static readonly pb::MessageParser<SkillEffectList> _parser = new pb::MessageParser<SkillEffectList>(() => new SkillEffectList());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SkillEffectList> Parser { get { return _parser; } }

    /// <summary>Field number for the "effects" field.</summary>
    public const int EffectsFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Common.SkillEffect> _repeated_effects_codec
        = pb::FieldCodec.ForMessage(10, global::Common.SkillEffect.Parser);
    private readonly pbc::RepeatedField<global::Common.SkillEffect> effects_ = new pbc::RepeatedField<global::Common.SkillEffect>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Common.SkillEffect> Effects {
      get { return effects_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      effects_.WriteTo(output, _repeated_effects_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += effects_.CalculateSize(_repeated_effects_codec);
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
            effects_.AddEntriesFrom(input, _repeated_effects_codec);
            break;
          }
        }
      }
    }

  }

  public sealed class ModuleFight : pb::IMessage {
    private static readonly pb::MessageParser<ModuleFight> _parser = new pb::MessageParser<ModuleFight>(() => new ModuleFight());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ModuleFight> Parser { get { return _parser; } }

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