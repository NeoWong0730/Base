using System;
using System.Runtime.InteropServices;

namespace AceSdk
{
    public enum AntiCheatExpertResult
    {
        ACE_OK = 0, // OK
        ACE_INVALID_ARGUMENT = 1, // Invalid argument
        ACE_DEPLOYMENT_ERROR = 2, // Some necessary files of ACE may be lost or can't be accessed
        ACE_NOT_SUPPORTED = 3, // Interface not supported
        ACE_INTERNAL_ERROR = 4, // Some internal errors in ACE
        ACE_ILLEGAL_INIT = 5, // init_ace_client can't be called more than once in one process
        ACE_NO_LAUNCHER = 6, // Process is not started by ACE-Launcher
        ACE_CONFIG_ERROR = 7, ///< The ACE-Base.dat is missing or corrupted
        ACE_SAFE_ERROR = 8, ///< The ACE-Safe.dll is missing or corrupted
        ACE_ROLE_ERROR = 9, ///< Role config has errors
        ACE_ILLEGAL_LOG_ON = 100, ///< Can't relogging on before loging off
    };

    /// <summary>
    /// Information required when initializing
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ClientInitInfo
    {
        /// <summary>
        /// Pid of the process which is launched by ACE-Launcher.
        /// </summary>
        public int first_process_pid;

        /// <summary>
        /// Set to -1 if there is not 2 or more processes integrated the SDK
        /// with the SAME file name when the game is running.
        /// Otherwise, assign a unique and fixed id for each process integrated the SDK.
        /// </summary>
        public int current_process_role_id;

        /// <summary>
        /// Set to non-null only if required by ACE team.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string base_dat_path;
    }

    /// <summary>
    /// The ACE Client Interface
    /// </summary>
    class AceClient
    {
        /// <summary>
        /// Load and initialize ACE Client interface.
        /// Developer should exit the game immediately when this function fails
        /// (which means it returned nullptr).
        /// </summary>
        /// <param name="init_info">see InitInfo</param>
        /// <param name="ace_base_path">relative path or full path of the Ace-BaseXX.dll. If it is null, ACE will use the default value.</param>
        /// <param name="ace_client">out object AceClient</param>
        /// <returns>the result</returns>
        /// <remarks>This interface can only be called once. It will return ACE_ILLEGAL_INIT if more than once.</remarks>
        public static AntiCheatExpertResult init(ref ClientInitInfo init_info, string ace_base_path, out AceClient ace_client)
        {
            if (ace_base_path == null)
            {
                if (IntPtr.Size == 8)
                {
                    ace_base_path = @"AntiCheatExpert\InGame\x64\ACE-Base64.dll";
                }
                else
                {
                    ace_base_path = @"AntiCheatExpert\InGame\x86\ACE-Base32.dll";
                }
            }

            ace_client = null;

            var mod = LoadLibrary(ace_base_path);
            if (mod == IntPtr.Zero)
            {
                return AntiCheatExpertResult.ACE_DEPLOYMENT_ERROR;
            }
            var func = GetProcAddress(mod, "InitAceClient4");
            if (func == IntPtr.Zero)
            {
                return AntiCheatExpertResult.ACE_DEPLOYMENT_ERROR;
            }
            var inner_init = (InitAceClient4Routine)Marshal.GetDelegateForFunctionPointer(func, typeof(InitAceClient4Routine));
            IntPtr client_ptr;
            var ret = inner_init(ref init_info, 6, out client_ptr);
            if (ret == AntiCheatExpertResult.ACE_OK)
            {
                ace_client = new AceClient(client_ptr);
            }

            return ret;
        }

        /// <summary>
        /// Developer should call this method when user successfully logged on.
        /// </summary>
        /// <param name="acc">the account information of user</param>
        /// <returns>the result</returns>
        public AntiCheatExpertResult log_on(ref AceAccountInfo acc)
        {
            return ace_.log_on(ace_.ace_client, ref acc);
        }

        /// <summary>
        /// Game developers should call it in the main loop of the game.
        /// </summary>
        public void tick()
        {
            ace_.tick(ace_.ace_client);
        }

        /// <summary>
        /// Developer should call this method when user log off the game.
        /// It should be called before exit_process when the last process of the game is exiting.
        /// </summary>
        public void log_off()
        {
            ace_.log_off(ace_.ace_client);
        }

        /// <summary>
        /// Developer should call this method when a process is exiting.
        /// </summary>
        public void exit_process()
        {
            ace_.exit_process(ace_.ace_client);
        }

        /// <summary>
        /// Get optional features supporting interface.
        /// </summary>
        /// <returns>ClientOptional</returns>
        public ClientOptional get_optional_interface()
        {
            return opt_;
        }

        AceClient(IntPtr ace_ptr)
        {
            ace_ = (WrapperAceClient)Marshal.PtrToStructure(ace_ptr, typeof(WrapperAceClient));
            opt_ = new ClientOptional(ace_.get_optional_interface(ace_.ace_client));
        }

        WrapperAceClient ace_;
        ClientOptional opt_;

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "LoadLibraryW")]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPWStr)] string lpLibFileName);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetProcAddress")]
        static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate AntiCheatExpertResult InitAceClient4Routine(ref ClientInitInfo info, uint v, out IntPtr client);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate AntiCheatExpertResult LogOnRoutine(IntPtr ace, ref AceAccountInfo acc);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void TickRoutine(IntPtr ace);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void LogOffRoutine(IntPtr ace);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ExitProcessRoutine(IntPtr ace);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate IntPtr GetOptRoutine(IntPtr ace);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct WrapperAceClient
        {
            public IntPtr ace_client;
            public LogOnRoutine log_on;
            public TickRoutine tick;
            public LogOffRoutine log_off;
            public ExitProcessRoutine exit_process;
            public GetOptRoutine get_optional_interface;
        };
    }
}

