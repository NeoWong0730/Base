/**
 * @copyright:  2020 AntiCheatExpert team. All Rights Reserved.
 * @filename:   ace_sdk_account_busi.cs
 * @version:    1.0.0
 * @brief:      This header file provides the definition of ACE_SDK account.
 * @details:
 */

using System.Runtime.InteropServices;

namespace AceSdk
{
    // Summary:
    //     The enumeration of ACE_SDK account type
    public enum AceAccountType
    {
        ACEACCOUNT_TYPE_QQ = 1,                                 // QQ
        ACEACCOUNT_TYPE_WECHAT = 2,                             // WeChat openid
        ACEACCOUNT_TYPE_BAIDU = 3,                              // Baidu account
        ACEACCOUNT_TYPE_QQ_OPENID = 4,                          // QQ openid
        ACEACCOUNT_TYPE_QQ_FAKE = 5,                            // fake QQ
        ACEACCOUNT_TYPE_MAIL_START = 100,                       // The beginning of email-like account
        ACEACCOUNT_TYPE_GOOGLE = 101,                           // Google account
        ACEACCOUNT_TYPE_MAIL_END = 199,                         // The end of email-like account
        ACEACCOUNT_TYPE_INT_START = 200,                        // The beginning of 32-bit integer account
        ACEACCOUNT_TYPE_4399 = 201,                             // 4399 account
        ACEACCOUNT_TYPE_GARENA = 202,                           // garena account
        ACEACCOUNT_TYPE_INT_END = 299,                          // The end of 32-bit integer account
        ACEACCOUNT_TYPE_INT64_START = 300,                      // The beginning of 64-bit integer account
        ACEACCOUNT_TYPE_WEGAME = 301,                           // WeGame account
        ACEACCOUNT_TYPE_STEAM = 302,                            // Steam account
        ACEACCOUNT_TYPE_WEGAME_COMMON_ID = 308,                 // WeGame common ID
        ACEACCOUNT_TYPE_INT64_END = 399,                        // The end of 64-bit integer account
        ACEACCOUNT_TYPE_COMMON_ID = 601,                        // common user identification
        ACEACCOUNT_TYPE_OTHER_START = 1000,                     // The beginning of other account
        ACEACCOUNT_TYPE_FACEBOOK = 1001,                        // facebook account
        ACEACCOUNT_TYPE_SUPERCELL = 1002,                       // supercell account
        ACEACCOUNT_TYPE_PHONE_OPENID = 1014,                    // telephone openid
    };

    // Summary:
    //     The enumeration of ACE_SDK plat id
    public enum AceAccountPlatId
    {
        ACEPLAT_ID_IOS = 0,                                     // iOS
        ACEPLAT_ID_ANDROID = 1,                                 // Android
        ACEPLAT_ID_RESERVE = 2,                                 // reserved
        ACEPLAT_ID_PC_CLIENT = 3,                               // PC client
        ACEPLAT_ID_MICRO_WEB = 4,                               // Micro web
        ACEPLAT_ID_MICRO_CLIENT = 5,                            // Micro client
        ACEPLAT_ID_SWITCH = 6,                                  // Switch client
    };

    // Summary:
    //     The struct of basic account info
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AceAccountId
    {
        /// <summary>
        /// The account of user.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 65)]
        public string account_;
        /// <summary>
        /// The account type of user, bind to AceAccountType.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public ushort account_type_;
    };

    // Summary:
    //     The struct of user account info
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AceAccountInfo
    {
        /// <summary>
        /// The basic account info of user.
        /// </summary>
        [MarshalAs(UnmanagedType.Struct)]
        public AceAccountId account_id_;
        /// <summary>
        /// The plat info, bind to AceAccountPlatId.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public ushort plat_id_;
        /// <summary>
        /// The game ID, assigned by ACE security team.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint game_id_;
        /// <summary>
        /// The world id of user.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint world_id_;
        /// <summary>
        /// The channel id of user.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint channel_id_;
        /// <summary>
        /// The role id of user.
        /// </summary>
        [MarshalAs(UnmanagedType.U8)]
        public ulong role_id_;
        /// <summary>
        /// reserved data.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string reserve_buf_;
    };

    // Summary:
    //     The struct of user extend data
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AceSdkUserExtData
    {
        /// <summary>
        /// The extend data of user.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string user_ext_data_;
        /// <summary>
        /// The length of extend data.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint ext_data_len_;
    };
}
