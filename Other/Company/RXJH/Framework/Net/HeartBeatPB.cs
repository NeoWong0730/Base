using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pb = global::Google.Protobuf;

namespace Lib.Core.Net {
    public sealed class SecondLogin_Req_KeepHeart : pb::IMessage {
        private static readonly pb::MessageParser<SecondLogin_Req_KeepHeart> _parser = new pb::MessageParser<SecondLogin_Req_KeepHeart>(() => new SecondLogin_Req_KeepHeart());

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pb::MessageParser<SecondLogin_Req_KeepHeart> Parser {
            get { return _parser; }
        }

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
                switch (tag) {
                    default:
                        input.SkipLastField();
                        break;
                }
            }
        }
    }

    public sealed class SecondLogin_Ntf_KeepHeart : pb::IMessage {
        private static readonly pb::MessageParser<SecondLogin_Ntf_KeepHeart> _parser = new pb::MessageParser<SecondLogin_Ntf_KeepHeart>(() => new SecondLogin_Ntf_KeepHeart());

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pb::MessageParser<SecondLogin_Ntf_KeepHeart> Parser {
            get { return _parser; }
        }

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
                switch (tag) {
                    default:
                        input.SkipLastField();
                        break;
                }
            }
        }
    }
}