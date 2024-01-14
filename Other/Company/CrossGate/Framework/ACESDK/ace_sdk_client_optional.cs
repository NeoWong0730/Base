/**
 * @copyright:  2020 AntiCheatExpert team. All Rights Reserved.
 * @filename:   ace_sdk_client_optional.cs
 * @version:    1.0.0
 * @brief:      This header file provides the definition of ClientOptional interface.
 * @details:
 */

using System;
using System.Runtime.InteropServices;

namespace AceSdk
{
    /// <summary>
    /// This callback will be called when ACE check a cheat and need to exit the game.
    /// </summary>
    /// <param name="ctx"></param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ClientExitingCallback(IntPtr ctx);

    class ClientOptional
    {
        public ClientOptional(IntPtr ptr)
        {
            opt_ = (WrappedOptional)Marshal.PtrToStructure(ptr, typeof(WrappedOptional));
            antibot_ = new TssAntibot(opt_.get_tss_antibot(opt_.opt));
        }

        /// <summary>
        /// Game developers should call this interface if they intend to use Tss AntiBot
        /// </summary>
        /// <returns>TssAntibot</returns>
        public TssAntibot get_tss_antibot()
        {
            return antibot_;
        }

        /// <summary>
        /// Set an exiting callback which will be called when ACE check a cheat and need to exit the game.
        /// </summary>
        /// <param name="cb">the callback</param>
        /// <param name="ctx">the optional context data which will be passed to the callback</param>
        public void set_exiting_callback(ClientExitingCallback cb, IntPtr ctx)
        {
            opt_.set_exiting_callback(opt_.opt, cb, ctx);
        }

        WrappedOptional opt_;
        TssAntibot antibot_;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate IntPtr GetTssAntibotRoutine(IntPtr opt);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate AntiCheatExpertResult SetExitingCallbackRoutine(IntPtr opt, ClientExitingCallback cb, IntPtr ctx);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate IntPtr GetCustomInterfaceRoutine(IntPtr opt, int inter_type);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct WrappedOptional
        {
            public IntPtr opt;
            public GetTssAntibotRoutine get_tss_antibot;
            public SetExitingCallbackRoutine set_exiting_callback;
            public GetCustomInterfaceRoutine get_custom_interface;
        }
    }
}
