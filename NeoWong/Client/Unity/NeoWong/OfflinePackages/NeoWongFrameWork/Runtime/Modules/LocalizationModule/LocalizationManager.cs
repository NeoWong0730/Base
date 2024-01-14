using System;
using System.Collections.Generic;

namespace NWFramework
{
    /// <summary>
    /// 本地化管理器
    /// </summary>
    internal sealed partial class LocalizationManager : ModuleImp, ILocalizationManager
    {
        private readonly Dictionary<string, string> m_Dictionary;
        private IResourceManager m_ResourceManager;
        private ILocalizationHelper m_LocalizationHelper;
        private Language m_Language;

        /// <summary>
        /// 初始化本地化管理器的新实例
        /// </summary>
        public LocalizationManager()
        {
            m_Dictionary = new Dictionary<string, string>(StringComparer.Ordinal);
            m_LocalizationHelper = null;
            m_Language = Language.Unspecified;
        }

        /// <summary>
        /// 获取或设置本地化语言
        /// </summary>
        public Language Language
        {
            get
            {
                return m_Language;
            }
            set
            {
                if (value == Language.Unspecified)
                {
                    throw new NWFrameworkException("Language is invalid.");
                }

                m_Language = value;
            }
        }

        /// <summary>
        /// 获取系统语言
        /// </summary>
        public Language SystemLanguage
        {
            get
            {
                if (m_LocalizationHelper == null)
                {
                    throw new NWFrameworkException("You must set localization helper first.");
                }

                return m_LocalizationHelper.SystemLanguage;
            }
        }

        /// <summary>
        /// 获取字典数量
        /// </summary>
        public int DictionaryCount
        {
            get
            {
                return m_Dictionary.Count;
            }
        }

        /// <summary>
        /// 本地化管理器轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位</param>
        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {            
        }

        /// <summary>
        /// 关闭并清理本地化管理器
        /// </summary>
        internal override void Shutdown()
        {
        }

        /// <summary>
        /// 设置资源管理器
        /// </summary>
        /// <param name="resourceManager">资源管理器</param>
        public void SetResourceManager(IResourceManager resourceManager)
        {
            m_ResourceManager = resourceManager;
        }

        /// <summary>
        /// 设置本地化辅助器
        /// </summary>
        /// <param name="localizationHelper">本地化辅助器</param>
        public void SetLocalizationHelper(ILocalizationHelper localizationHelper)
        {
            if (localizationHelper == null)
            {
                throw new NWFrameworkException("Localization helper is invalid.");
            }

            m_LocalizationHelper = localizationHelper;
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串
        /// </summary>
        /// <param name="key">字典主键</param>
        /// <returns>要获取的字典内容字符串</returns>
        public string GetString(string key)
        {
            string value = GetRawString(key);
            if (value == null)
            {
                return Utility.Text.Format("<NoKey>{0}", key);
            }

            return value;
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串
        /// </summary>
        /// <typeparam name="T">字典参数的类型</typeparam>
        /// <param name="key">字典主键</param>
        /// <param name="arg">字典参数</param>
        /// <returns>要获取的字典内容字符串</returns>
        public string GetString<T>(string key, T arg)
        {
            string value = GetRawString(key);
            if (value == null)
            {
                return Utility.Text.Format("<NoKey>{0}", key);
            }

            try
            {
                return Utility.Text.Format(value, arg);
            }
            catch (Exception exception)
            {
                return Utility.Text.Format("<Error>{0},{1},{2},{3}", key, value, arg, exception);
            }
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串
        /// </summary>
        /// <typeparam name="T1">字典参数 1 的类型</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型</typeparam>
        /// <param name="key">字典主键</param>
        /// <param name="arg1">字典参数 1</param>
        /// <param name="arg2">字典参数 2</param>
        /// <returns>要获取的字典内容字符串</returns>
        public string GetString<T1, T2>(string key, T1 arg1, T2 arg2)
        {
            string value = GetRawString(key);
            if (value == null)
            {
                return Utility.Text.Format("<NoKey>{0}", key);
            }

            try
            {
                return Utility.Text.Format(value, arg1, arg2);
            }
            catch (Exception exception)
            {
                return Utility.Text.Format("<Error>{0},{1},{2},{3},{4}", key, value, arg1, arg2, exception);
            }
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串
        /// </summary>
        /// <typeparam name="T1">字典参数 1 的类型</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型</typeparam>
        /// <param name="key">字典主键</param>
        /// <param name="arg1">字典参数 1</param>
        /// <param name="arg2">字典参数 2</param>
        /// <param name="arg3">字典参数 3</param>
        /// <returns>要获取的字典内容字符串</returns>
        public string GetString<T1, T2, T3>(string key, T1 arg1, T2 arg2, T3 arg3)
        {
            string value = GetRawString(key);
            if (value == null)
            {
                return Utility.Text.Format("<NoKey>{0}", key);
            }

            try
            {
                return Utility.Text.Format(value, arg1, arg2, arg3);
            }
            catch (Exception exception)
            {
                return Utility.Text.Format("<Error>{0},{1},{2},{3},{4},{5}", key, value, arg1, arg2, arg3, exception);
            }
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串
        /// </summary>
        /// <typeparam name="T1">字典参数 1 的类型</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型</typeparam>
        /// <param name="key">字典主键</param>
        /// <param name="arg1">字典参数 1</param>
        /// <param name="arg2">字典参数 2</param>
        /// <param name="arg3">字典参数 3</param>
        /// <param name="arg4">字典参数 4</param>
        /// <returns>要获取的字典内容字符串</returns>
        public string GetString<T1, T2, T3, T4>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            string value = GetRawString(key);
            if (value == null)
            {
                return Utility.Text.Format("<NoKey>{0}", key);
            }

            try
            {
                return Utility.Text.Format(value, arg1, arg2, arg3, arg4);
            }
            catch (Exception exception)
            {
                return Utility.Text.Format("<Error>{0},{1},{2},{3},{4},{5},{6}", key, value, arg1, arg2, arg3, arg4, exception);
            }
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串
        /// </summary>
        /// <typeparam name="T1">字典参数 1 的类型</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型</typeparam>
        /// <param name="key">字典主键</param>
        /// <param name="arg1">字典参数 1</param>
        /// <param name="arg2">字典参数 2</param>
        /// <param name="arg3">字典参数 3</param>
        /// <param name="arg4">字典参数 4</param>
        /// <param name="arg5">字典参数 5</param>
        /// <returns>要获取的字典内容字符串</returns>
        public string GetString<T1, T2, T3, T4, T5>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            string value = GetRawString(key);
            if (value == null)
            {
                return Utility.Text.Format("<NoKey>{0}", key);
            }

            try
            {
                return Utility.Text.Format(value, arg1, arg2, arg3, arg4, arg5);
            }
            catch (Exception exception)
            {
                return Utility.Text.Format("<Error>{0},{1},{2},{3},{4},{5},{6},{7}", key, value, arg1, arg2, arg3, arg4, arg5, exception);
            }
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串
        /// </summary>
        /// <typeparam name="T1">字典参数 1 的类型</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型</typeparam>
        /// <param name="key">字典主键</param>
        /// <param name="arg1">字典参数 1</param>
        /// <param name="arg2">字典参数 2</param>
        /// <param name="arg3">字典参数 3</param>
        /// <param name="arg4">字典参数 4</param>
        /// <param name="arg5">字典参数 5</param>
        /// <param name="arg6">字典参数 6</param>
        /// <returns>要获取的字典内容字符串</returns>
        public string GetString<T1, T2, T3, T4, T5, T6>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            string value = GetRawString(key);
            if (value == null)
            {
                return Utility.Text.Format("<NoKey>{0}", key);
            }

            try
            {
                return Utility.Text.Format(value, arg1, arg2, arg3, arg4, arg5, arg6);
            }
            catch (Exception exception)
            {
                return Utility.Text.Format("<Error>{0},{1},{2},{3},{4},{5},{6},{7},{8}", key, value, arg1, arg2, arg3, arg4, arg5, arg6, exception);
            }
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串
        /// </summary>
        /// <typeparam name="T1">字典参数 1 的类型</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型</typeparam>
        /// <param name="key">字典主键</param>
        /// <param name="arg1">字典参数 1</param>
        /// <param name="arg2">字典参数 2</param>
        /// <param name="arg3">字典参数 3</param>
        /// <param name="arg4">字典参数 4</param>
        /// <param name="arg5">字典参数 5</param>
        /// <param name="arg6">字典参数 6</param>
        /// <param name="arg7">字典参数 7</param>
        /// <returns>要获取的字典内容字符串</returns>
        public string GetString<T1, T2, T3, T4, T5, T6, T7>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            string value = GetRawString(key);
            if (value == null)
            {
                return Utility.Text.Format("<NoKey>{0}", key);
            }

            try
            {
                return Utility.Text.Format(value, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            }
            catch (Exception exception)
            {
                return Utility.Text.Format("<Error>{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", key, value, arg1, arg2, arg3, arg4, arg5, arg6, arg7, exception);
            }
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串
        /// </summary>
        /// <typeparam name="T1">字典参数 1 的类型</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型</typeparam>
        /// <typeparam name="T8">字典参数 8 的类型</typeparam>
        /// <param name="key">字典主键</param>
        /// <param name="arg1">字典参数 1</param>
        /// <param name="arg2">字典参数 2</param>
        /// <param name="arg3">字典参数 3</param>
        /// <param name="arg4">字典参数 4</param>
        /// <param name="arg5">字典参数 5</param>
        /// <param name="arg6">字典参数 6</param>
        /// <param name="arg7">字典参数 7</param>
        /// <param name="arg8">字典参数 8</param>
        /// <returns>要获取的字典内容字符串</returns>
        public string GetString<T1, T2, T3, T4, T5, T6, T7, T8>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            string value = GetRawString(key);
            if (value == null)
            {
                return Utility.Text.Format("<NoKey>{0}", key);
            }

            try
            {
                return Utility.Text.Format(value, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            }
            catch (Exception exception)
            {
                return Utility.Text.Format("<Error>{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", key, value, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, exception);
            }
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串
        /// </summary>
        /// <typeparam name="T1">字典参数 1 的类型</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型</typeparam>
        /// <typeparam name="T8">字典参数 8 的类型</typeparam>
        /// <typeparam name="T9">字典参数 9 的类型</typeparam>
        /// <param name="key">字典主键</param>
        /// <param name="arg1">字典参数 1</param>
        /// <param name="arg2">字典参数 2</param>
        /// <param name="arg3">字典参数 3</param>
        /// <param name="arg4">字典参数 4</param>
        /// <param name="arg5">字典参数 5</param>
        /// <param name="arg6">字典参数 6</param>
        /// <param name="arg7">字典参数 7</param>
        /// <param name="arg8">字典参数 8</param>
        /// <param name="arg9">字典参数 9</param>
        /// <returns>要获取的字典内容字符串</returns>
        public string GetString<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            string value = GetRawString(key);
            if (value == null)
            {
                return Utility.Text.Format("<NoKey>{0}", key);
            }

            try
            {
                return Utility.Text.Format(value, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            }
            catch (Exception exception)
            {
                return Utility.Text.Format("<Error>{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", key, value, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, exception);
            }
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串
        /// </summary>
        /// <typeparam name="T1">字典参数 1 的类型</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型</typeparam>
        /// <typeparam name="T8">字典参数 8 的类型</typeparam>
        /// <typeparam name="T9">字典参数 9 的类型</typeparam>
        /// <typeparam name="T10">字典参数 10 的类型</typeparam>
        /// <param name="key">字典主键</param>
        /// <param name="arg1">字典参数 1</param>
        /// <param name="arg2">字典参数 2</param>
        /// <param name="arg3">字典参数 3</param>
        /// <param name="arg4">字典参数 4</param>
        /// <param name="arg5">字典参数 5</param>
        /// <param name="arg6">字典参数 6</param>
        /// <param name="arg7">字典参数 7</param>
        /// <param name="arg8">字典参数 8</param>
        /// <param name="arg9">字典参数 9</param>
        /// <param name="arg10">字典参数 10</param>
        /// <returns>要获取的字典内容字符串</returns>
        public string GetString<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            string value = GetRawString(key);
            if (value == null)
            {
                return Utility.Text.Format("<NoKey>{0}", key);
            }

            try
            {
                return Utility.Text.Format(value, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            }
            catch (Exception exception)
            {
                return Utility.Text.Format("<Error>{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}", key, value, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, exception);
            }
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串
        /// </summary>
        /// <typeparam name="T1">字典参数 1 的类型</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型</typeparam>
        /// <typeparam name="T8">字典参数 8 的类型</typeparam>
        /// <typeparam name="T9">字典参数 9 的类型</typeparam>
        /// <typeparam name="T10">字典参数 10 的类型</typeparam>
        /// <typeparam name="T11">字典参数 11 的类型</typeparam>
        /// <param name="key">字典主键</param>
        /// <param name="arg1">字典参数 1</param>
        /// <param name="arg2">字典参数 2</param>
        /// <param name="arg3">字典参数 3</param>
        /// <param name="arg4">字典参数 4</param>
        /// <param name="arg5">字典参数 5</param>
        /// <param name="arg6">字典参数 6</param>
        /// <param name="arg7">字典参数 7</param>
        /// <param name="arg8">字典参数 8</param>
        /// <param name="arg9">字典参数 9</param>
        /// <param name="arg10">字典参数 10</param>
        /// <param name="arg11">字典参数 11</param>
        /// <returns>要获取的字典内容字符串</returns>
        public string GetString<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            string value = GetRawString(key);
            if (value == null)
            {
                return Utility.Text.Format("<NoKey>{0}", key);
            }

            try
            {
                return Utility.Text.Format(value, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
            }
            catch (Exception exception)
            {
                return Utility.Text.Format("<Error>{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}", key, value, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, exception);
            }
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串
        /// </summary>
        /// <typeparam name="T1">字典参数 1 的类型</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型</typeparam>
        /// <typeparam name="T8">字典参数 8 的类型</typeparam>
        /// <typeparam name="T9">字典参数 9 的类型</typeparam>
        /// <typeparam name="T10">字典参数 10 的类型</typeparam>
        /// <typeparam name="T11">字典参数 11 的类型</typeparam>
        /// <typeparam name="T12">字典参数 12 的类型</typeparam>
        /// <param name="key">字典主键</param>
        /// <param name="arg1">字典参数 1</param>
        /// <param name="arg2">字典参数 2</param>
        /// <param name="arg3">字典参数 3</param>
        /// <param name="arg4">字典参数 4</param>
        /// <param name="arg5">字典参数 5</param>
        /// <param name="arg6">字典参数 6</param>
        /// <param name="arg7">字典参数 7</param>
        /// <param name="arg8">字典参数 8</param>
        /// <param name="arg9">字典参数 9</param>
        /// <param name="arg10">字典参数 10</param>
        /// <param name="arg11">字典参数 11</param>
        /// <param name="arg12">字典参数 12</param>
        /// <returns>要获取的字典内容字符串</returns>
        public string GetString<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            string value = GetRawString(key);
            if (value == null)
            {
                return Utility.Text.Format("<NoKey>{0}", key);
            }

            try
            {
                return Utility.Text.Format(value, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
            }
            catch (Exception exception)
            {
                return Utility.Text.Format("<Error>{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}", key, value, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, exception);
            }
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串
        /// </summary>
        /// <typeparam name="T1">字典参数 1 的类型</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型</typeparam>
        /// <typeparam name="T8">字典参数 8 的类型</typeparam>
        /// <typeparam name="T9">字典参数 9 的类型</typeparam>
        /// <typeparam name="T10">字典参数 10 的类型</typeparam>
        /// <typeparam name="T11">字典参数 11 的类型</typeparam>
        /// <typeparam name="T12">字典参数 12 的类型</typeparam>
        /// <typeparam name="T13">字典参数 13 的类型</typeparam>
        /// <param name="key">字典主键</param>
        /// <param name="arg1">字典参数 1</param>
        /// <param name="arg2">字典参数 2</param>
        /// <param name="arg3">字典参数 3</param>
        /// <param name="arg4">字典参数 4</param>
        /// <param name="arg5">字典参数 5</param>
        /// <param name="arg6">字典参数 6</param>
        /// <param name="arg7">字典参数 7</param>
        /// <param name="arg8">字典参数 8</param>
        /// <param name="arg9">字典参数 9</param>
        /// <param name="arg10">字典参数 10</param>
        /// <param name="arg11">字典参数 11</param>
        /// <param name="arg12">字典参数 12</param>
        /// <param name="arg13">字典参数 13</param>
        /// <returns>要获取的字典内容字符串</returns>
        public string GetString<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            string value = GetRawString(key);
            if (value == null)
            {
                return Utility.Text.Format("<NoKey>{0}", key);
            }

            try
            {
                return Utility.Text.Format(value, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
            }
            catch (Exception exception)
            {
                return Utility.Text.Format("<Error>{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", key, value, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, exception);
            }
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串
        /// </summary>
        /// <typeparam name="T1">字典参数 1 的类型</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型</typeparam>
        /// <typeparam name="T8">字典参数 8 的类型</typeparam>
        /// <typeparam name="T9">字典参数 9 的类型</typeparam>
        /// <typeparam name="T10">字典参数 10 的类型</typeparam>
        /// <typeparam name="T11">字典参数 11 的类型</typeparam>
        /// <typeparam name="T12">字典参数 12 的类型</typeparam>
        /// <typeparam name="T13">字典参数 13 的类型</typeparam>
        /// <typeparam name="T14">字典参数 14 的类型</typeparam>
        /// <param name="key">字典主键</param>
        /// <param name="arg1">字典参数 1</param>
        /// <param name="arg2">字典参数 2</param>
        /// <param name="arg3">字典参数 3</param>
        /// <param name="arg4">字典参数 4</param>
        /// <param name="arg5">字典参数 5</param>
        /// <param name="arg6">字典参数 6</param>
        /// <param name="arg7">字典参数 7</param>
        /// <param name="arg8">字典参数 8</param>
        /// <param name="arg9">字典参数 9</param>
        /// <param name="arg10">字典参数 10</param>
        /// <param name="arg11">字典参数 11</param>
        /// <param name="arg12">字典参数 12</param>
        /// <param name="arg13">字典参数 13</param>
        /// <param name="arg14">字典参数 14</param>
        /// <returns>要获取的字典内容字符串</returns>
        public string GetString<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            string value = GetRawString(key);
            if (value == null)
            {
                return Utility.Text.Format("<NoKey>{0}", key);
            }

            try
            {
                return Utility.Text.Format(value, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
            }
            catch (Exception exception)
            {
                string args = Utility.Text.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
                return Utility.Text.Format("<Error>{0},{1},{2},{3}", key, value, args, exception);
            }
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串
        /// </summary>
        /// <typeparam name="T1">字典参数 1 的类型</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型</typeparam>
        /// <typeparam name="T8">字典参数 8 的类型</typeparam>
        /// <typeparam name="T9">字典参数 9 的类型</typeparam>
        /// <typeparam name="T10">字典参数 10 的类型</typeparam>
        /// <typeparam name="T11">字典参数 11 的类型</typeparam>
        /// <typeparam name="T12">字典参数 12 的类型</typeparam>
        /// <typeparam name="T13">字典参数 13 的类型</typeparam>
        /// <typeparam name="T14">字典参数 14 的类型</typeparam>
        /// <typeparam name="T15">字典参数 15 的类型</typeparam>
        /// <param name="key">字典主键</param>
        /// <param name="arg1">字典参数 1</param>
        /// <param name="arg2">字典参数 2</param>
        /// <param name="arg3">字典参数 3</param>
        /// <param name="arg4">字典参数 4</param>
        /// <param name="arg5">字典参数 5</param>
        /// <param name="arg6">字典参数 6</param>
        /// <param name="arg7">字典参数 7</param>
        /// <param name="arg8">字典参数 8</param>
        /// <param name="arg9">字典参数 9</param>
        /// <param name="arg10">字典参数 10</param>
        /// <param name="arg11">字典参数 11</param>
        /// <param name="arg12">字典参数 12</param>
        /// <param name="arg13">字典参数 13</param>
        /// <param name="arg14">字典参数 14</param>
        /// <param name="arg15">字典参数 15</param>
        /// <returns>要获取的字典内容字符串</returns>
        public string GetString<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            string value = GetRawString(key);
            if (value == null)
            {
                return Utility.Text.Format("<NoKey>{0}", key);
            }

            try
            {
                return Utility.Text.Format(value, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
            }
            catch (Exception exception)
            {
                string args = Utility.Text.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
                return Utility.Text.Format("<Error>{0},{1},{2},{3}", key, value, args, exception);
            }
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串
        /// </summary>
        /// <typeparam name="T1">字典参数 1 的类型</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型</typeparam>
        /// <typeparam name="T8">字典参数 8 的类型</typeparam>
        /// <typeparam name="T9">字典参数 9 的类型</typeparam>
        /// <typeparam name="T10">字典参数 10 的类型</typeparam>
        /// <typeparam name="T11">字典参数 11 的类型</typeparam>
        /// <typeparam name="T12">字典参数 12 的类型</typeparam>
        /// <typeparam name="T13">字典参数 13 的类型</typeparam>
        /// <typeparam name="T14">字典参数 14 的类型</typeparam>
        /// <typeparam name="T15">字典参数 15 的类型</typeparam>
        /// <typeparam name="T16">字典参数 16 的类型</typeparam>
        /// <param name="key">字典主键</param>
        /// <param name="arg1">字典参数 1</param>
        /// <param name="arg2">字典参数 2</param>
        /// <param name="arg3">字典参数 3</param>
        /// <param name="arg4">字典参数 4</param>
        /// <param name="arg5">字典参数 5</param>
        /// <param name="arg6">字典参数 6</param>
        /// <param name="arg7">字典参数 7</param>
        /// <param name="arg8">字典参数 8</param>
        /// <param name="arg9">字典参数 9</param>
        /// <param name="arg10">字典参数 10</param>
        /// <param name="arg11">字典参数 11</param>
        /// <param name="arg12">字典参数 12</param>
        /// <param name="arg13">字典参数 13</param>
        /// <param name="arg14">字典参数 14</param>
        /// <param name="arg15">字典参数 15</param>
        /// <param name="arg16">字典参数 16</param>
        /// <returns>要获取的字典内容字符串</returns>
        public string GetString<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            string value = GetRawString(key);
            if (value == null)
            {
                return Utility.Text.Format("<NoKey>{0}", key);
            }

            try
            {
                return Utility.Text.Format(value, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
            }
            catch (Exception exception)
            {
                string args = Utility.Text.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
                return Utility.Text.Format("<Error>{0},{1},{2},{3}", key, value, args, exception);
            }
        }

        /// <summary>
        /// 根据字典值取主键
        /// </summary>
        /// <param name="value">字典值</param>
        /// <returns></returns>
        public string GetKey(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            foreach (var item in m_Dictionary)
            {
                if (item.Value == value)
                {
                    return item.Key;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 是否存在字典
        /// </summary>
        /// <param name="key">字典主键</param>
        /// <returns>是否存在字典</returns>
        public bool HasRawString(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new NWFrameworkException("Key is invalid.");
            }

            return m_Dictionary.ContainsKey(key);
        }

        /// <summary>
        /// 根据字典主键获取字典值
        /// </summary>
        /// <param name="key">字典主键</param>
        /// <returns>字典值</returns>
        public string GetRawString(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new NWFrameworkException("Key is invalid.");
            }

            string value = null;
            if (m_Dictionary.TryGetValue(key, out value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// 增加字典
        /// </summary>
        /// <param name="key">字典主键</param>
        /// <param name="value">字典内容</param>
        /// <returns>是否增加字典成功</returns>
        public bool AddRawString(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new NWFrameworkException("Key is invalid.");
            }

            if (m_Dictionary.ContainsKey(key))
            {
                return false;
            }

            m_Dictionary.Add(key, value ?? string.Empty);
            return true;
        }

        /// <summary>
        /// 移除字典
        /// </summary>
        /// <param name="key">字典主键</param>
        /// <returns>是否移除字典成功</returns>
        public bool RemoveRawString(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new NWFrameworkException("Key is invalid.");
            }

            return m_Dictionary.Remove(key);
        }

        /// <summary>
        /// 清空所有字典
        /// </summary>
        public void RemoveAllRawStrings()
        {
            m_Dictionary.Clear();
        }
    }
}