/**
 * @copyright:  2020 AntiCheatExpert team. All Rights Reserved.
 * @filename:   tss_antibot.cs
 * @version:    1.0.0
 * @brief:      This header file provides the definition of TssAntibot interface.
 * @details:
 */

using System;
using System.Runtime.InteropServices;

namespace AceSdk
{
    class TssAntibot
    {
        public TssAntibot(IntPtr ptr)
        {
            antibot_ = (WrappedTssAntibot)Marshal.PtrToStructure(ptr, typeof(WrappedTssAntibot));
        }

        public byte[] GetReportData()
        {
            var data_ptr = antibot_.get_report_anti_data(antibot_.antibot);
            if (data_ptr == IntPtr.Zero)
            {
                return null;
            }
            var anti_data = (TssAntiData)Marshal.PtrToStructure(data_ptr, typeof(TssAntiData));
            if (anti_data.data_len == 0 || anti_data.data == IntPtr.Zero)
            {
                return null;
            }
            var buf = new byte[anti_data.data_len];
            Marshal.Copy(anti_data.data, buf, 0, (int)anti_data.data_len);
            antibot_.del_report_anti_data(antibot_.antibot, data_ptr);
            return buf;
        }

        public void OnRecvAntiData(byte[] data)
        {
            if (data.Length > data_buf_size_)
            {
                return;
            }
            var buf = get_data_buff();
            Marshal.Copy(data, 0, buf, data.Length);
            TssAntiData anti_data;
            anti_data.data = buf;
            anti_data.data_len = (uint)data.Length;
            antibot_.on_recv_anti_data(antibot_.antibot, ref anti_data);
        }

        public void Release()
        {
        }

        IntPtr get_data_buff()
        {
            if (data_buf_ == IntPtr.Zero)
            {
                data_buf_ = Marshal.AllocHGlobal(data_buf_size_);
            }
            return data_buf_;
        }

        WrappedTssAntibot antibot_;
        [ThreadStatic] IntPtr data_buf_;
        const int data_buf_size_ = 65536;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void DeprecatedRoutine(IntPtr antibot, IntPtr arg1);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate IntPtr GetReportAntiDataRoutine(IntPtr antibot);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void DelReportAntiDataRoutine(IntPtr antibot, IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void OnRecvAntiDataRoutine(IntPtr antibot, ref TssAntiData data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void DeprecatedRoutine2(IntPtr antibot);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct WrappedTssAntibot
        {
            public IntPtr antibot;
            public DeprecatedRoutine deprecated;
            public GetReportAntiDataRoutine get_report_anti_data;
            public DelReportAntiDataRoutine del_report_anti_data;
            public OnRecvAntiDataRoutine on_recv_anti_data;
            public DeprecatedRoutine2 deprecated2;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TssAntiData
        {
            public uint data_len;
            public IntPtr data;
        }
    }
}
