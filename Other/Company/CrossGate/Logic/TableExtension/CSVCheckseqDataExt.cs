using System.Collections.Generic;
using Logic;
using Framework;
using Framework.Table;
using Lib.Core;

namespace Table
{
    public static class CSVCheckseqDataExt
    {
        private static List<int> _tempValues = new List<int>(4);

        static void Tip(this CSVCheckseq.Data data)
        {
            //if (CondiNoPasarTips != 0)
            //    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(CondiNoPasarTips));
        }

        public static bool IsValid(this CSVCheckseq.Data data)
        {
            bool result = false;
            if (data.CheckCondi2 == null)
            {
                result = IsConditionArrayValid(data.CheckCondi1);
                if (!result)
                    data.Tip();
                return result;
            }
            else
            {
                if (data.CheckCondi3 == null)
                {
                    result = IsConditionArrayValid(data.CheckCondi1) ||
                            IsConditionArrayValid(data.CheckCondi2);
                    if (!result)
                        data.Tip();
                    return result;
                }
                else
                {
                    if (data.CheckCondi4 == null)
                    {
                        result = IsConditionArrayValid(data.CheckCondi1) ||
                                IsConditionArrayValid(data.CheckCondi2) ||
                                IsConditionArrayValid(data.CheckCondi3);
                        if (!result)
                            data.Tip();
                        return result;
                    }
                    else
                    {
                        if (data.CheckCondi5 == null)
                        {
                            result = IsConditionArrayValid(data.CheckCondi1) ||
                                    IsConditionArrayValid(data.CheckCondi2) ||
                                    IsConditionArrayValid(data.CheckCondi3) ||
                                    IsConditionArrayValid(data.CheckCondi4);
                            if (!result)
                                data.Tip();
                            return result;
                        }
                        else
                        {
                            if (data.CheckCondi6 == null)
                            {
                                result = IsConditionArrayValid(data.CheckCondi1) ||
                                        IsConditionArrayValid(data.CheckCondi2) ||
                                        IsConditionArrayValid(data.CheckCondi3) ||
                                        IsConditionArrayValid(data.CheckCondi4) ||
                                        IsConditionArrayValid(data.CheckCondi5);
                                if (!result)
                                    data.Tip();
                                return result;
                            }
                            else
                            {
                                if (data.CheckCondi7 == null)
                                {
                                    result = IsConditionArrayValid(data.CheckCondi1) ||
                                            IsConditionArrayValid(data.CheckCondi2) ||
                                            IsConditionArrayValid(data.CheckCondi3) ||
                                            IsConditionArrayValid(data.CheckCondi4) ||
                                            IsConditionArrayValid(data.CheckCondi5) ||
                                            IsConditionArrayValid(data.CheckCondi6);
                                    if (!result)
                                        data.Tip();
                                    return result;
                                }
                                else
                                {
                                    if (data.CheckCondi8 == null)
                                    {
                                        result = IsConditionArrayValid(data.CheckCondi1) ||
                                                IsConditionArrayValid(data.CheckCondi2) ||
                                                IsConditionArrayValid(data.CheckCondi3) ||
                                                IsConditionArrayValid(data.CheckCondi4) ||
                                                IsConditionArrayValid(data.CheckCondi5) ||
                                                IsConditionArrayValid(data.CheckCondi6) ||
                                                IsConditionArrayValid(data.CheckCondi7);
                                        if (!result)
                                            data.Tip();
                                        return result;
                                    }
                                    else
                                    {
                                        if (data.CheckCondi9 == null)
                                        {
                                            result = IsConditionArrayValid(data.CheckCondi1) ||
                                                    IsConditionArrayValid(data.CheckCondi2) ||
                                                    IsConditionArrayValid(data.CheckCondi3) ||
                                                    IsConditionArrayValid(data.CheckCondi4) ||
                                                    IsConditionArrayValid(data.CheckCondi5) ||
                                                    IsConditionArrayValid(data.CheckCondi6) ||
                                                    IsConditionArrayValid(data.CheckCondi7) ||
                                                    IsConditionArrayValid(data.CheckCondi8);
                                            if (!result)
                                                data.Tip();
                                            return result;
                                        }
                                        else
                                        {
                                            if (data.CheckCondi10 == null)
                                            {
                                                result = IsConditionArrayValid(data.CheckCondi1) ||
                                                        IsConditionArrayValid(data.CheckCondi2) ||
                                                        IsConditionArrayValid(data.CheckCondi3) ||
                                                        IsConditionArrayValid(data.CheckCondi4) ||
                                                        IsConditionArrayValid(data.CheckCondi5) ||
                                                        IsConditionArrayValid(data.CheckCondi6) ||
                                                        IsConditionArrayValid(data.CheckCondi7) ||
                                                        IsConditionArrayValid(data.CheckCondi8) ||
                                                        IsConditionArrayValid(data.CheckCondi9);
                                                if (!result)
                                                    data.Tip();
                                                return result;
                                            }
                                            else
                                            {
                                                result = IsConditionArrayValid(data.CheckCondi1) ||
                                                    IsConditionArrayValid(data.CheckCondi2) ||
                                                    IsConditionArrayValid(data.CheckCondi3) ||
                                                    IsConditionArrayValid(data.CheckCondi4) ||
                                                    IsConditionArrayValid(data.CheckCondi5) ||
                                                    IsConditionArrayValid(data.CheckCondi6) ||
                                                    IsConditionArrayValid(data.CheckCondi7) ||
                                                    IsConditionArrayValid(data.CheckCondi8) ||
                                                    IsConditionArrayValid(data.CheckCondi9) ||
                                                    IsConditionArrayValid(data.CheckCondi10);
                                                if (!result)
                                                    data.Tip();
                                                return result;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static bool IsValid(this CSVCheckseq.Data data, ref Dictionary<int, int> marks)
        {
            if (data.CheckCondi2 == null)
            {
                return IsConditionArrayValid(data.CheckCondi1, ref marks);
            }
            else
            {
                if (data.CheckCondi3 == null)
                {
                    return IsConditionArrayValid(data.CheckCondi1, ref marks) ||
                            IsConditionArrayValid(data.CheckCondi2, ref marks);
                }
                else
                {
                    if (data.CheckCondi4 == null)
                    {
                        return IsConditionArrayValid(data.CheckCondi1, ref marks) ||
                                IsConditionArrayValid(data.CheckCondi2, ref marks) ||
                                IsConditionArrayValid(data.CheckCondi3, ref marks);
                    }
                    else
                    {
                        if (data.CheckCondi5 == null)
                        {
                            return IsConditionArrayValid(data.CheckCondi1, ref marks) ||
                                    IsConditionArrayValid(data.CheckCondi2, ref marks) ||
                                    IsConditionArrayValid(data.CheckCondi3, ref marks) ||
                                    IsConditionArrayValid(data.CheckCondi4, ref marks);
                        }
                        else
                        {
                            if (data.CheckCondi6 == null)
                            {
                                return IsConditionArrayValid(data.CheckCondi1, ref marks) ||
                                        IsConditionArrayValid(data.CheckCondi2, ref marks) ||
                                        IsConditionArrayValid(data.CheckCondi3, ref marks) ||
                                        IsConditionArrayValid(data.CheckCondi4, ref marks) ||
                                        IsConditionArrayValid(data.CheckCondi5, ref marks);
                            }
                            else
                            {
                                if (data.CheckCondi7 == null)
                                {
                                    return IsConditionArrayValid(data.CheckCondi1, ref marks) ||
                                            IsConditionArrayValid(data.CheckCondi2, ref marks) ||
                                            IsConditionArrayValid(data.CheckCondi3, ref marks) ||
                                            IsConditionArrayValid(data.CheckCondi4, ref marks) ||
                                            IsConditionArrayValid(data.CheckCondi5, ref marks) ||
                                            IsConditionArrayValid(data.CheckCondi6, ref marks);
                                }
                                else
                                {
                                    if (data.CheckCondi8 == null)
                                    {
                                        return IsConditionArrayValid(data.CheckCondi1, ref marks) ||
                                                IsConditionArrayValid(data.CheckCondi2, ref marks) ||
                                                IsConditionArrayValid(data.CheckCondi3, ref marks) ||
                                                IsConditionArrayValid(data.CheckCondi4, ref marks) ||
                                                IsConditionArrayValid(data.CheckCondi5, ref marks) ||
                                                IsConditionArrayValid(data.CheckCondi6, ref marks) ||
                                                IsConditionArrayValid(data.CheckCondi7, ref marks);
                                    }
                                    else
                                    {
                                        if (data.CheckCondi9 == null)
                                        {
                                            return IsConditionArrayValid(data.CheckCondi1, ref marks) ||
                                                    IsConditionArrayValid(data.CheckCondi2, ref marks) ||
                                                    IsConditionArrayValid(data.CheckCondi3, ref marks) ||
                                                    IsConditionArrayValid(data.CheckCondi4, ref marks) ||
                                                    IsConditionArrayValid(data.CheckCondi5, ref marks) ||
                                                    IsConditionArrayValid(data.CheckCondi6, ref marks) ||
                                                    IsConditionArrayValid(data.CheckCondi7, ref marks) ||
                                                    IsConditionArrayValid(data.CheckCondi8, ref marks);
                                        }
                                        else
                                        {
                                            if (data.CheckCondi10 == null)
                                            {
                                                return IsConditionArrayValid(data.CheckCondi1, ref marks) ||
                                                        IsConditionArrayValid(data.CheckCondi2, ref marks) ||
                                                        IsConditionArrayValid(data.CheckCondi3, ref marks) ||
                                                        IsConditionArrayValid(data.CheckCondi4, ref marks) ||
                                                        IsConditionArrayValid(data.CheckCondi5, ref marks) ||
                                                        IsConditionArrayValid(data.CheckCondi6, ref marks) ||
                                                        IsConditionArrayValid(data.CheckCondi7, ref marks) ||
                                                        IsConditionArrayValid(data.CheckCondi8, ref marks) ||
                                                        IsConditionArrayValid(data.CheckCondi9, ref marks);
                                            }
                                            else
                                            {
                                                return IsConditionArrayValid(data.CheckCondi1, ref marks) ||
                                                    IsConditionArrayValid(data.CheckCondi2, ref marks) ||
                                                    IsConditionArrayValid(data.CheckCondi3, ref marks) ||
                                                    IsConditionArrayValid(data.CheckCondi4, ref marks) ||
                                                    IsConditionArrayValid(data.CheckCondi5, ref marks) ||
                                                    IsConditionArrayValid(data.CheckCondi6, ref marks) ||
                                                    IsConditionArrayValid(data.CheckCondi7, ref marks) ||
                                                    IsConditionArrayValid(data.CheckCondi8, ref marks) ||
                                                    IsConditionArrayValid(data.CheckCondi9, ref marks) ||
                                                    IsConditionArrayValid(data.CheckCondi10, ref marks);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        static bool IsConditionValid(List<int> condition)
        {
            if (condition.Count < 1)
                return true;

            ConditionBase conditionBase = ConditionManager.CreateCondition((EConditionType)condition[0]);
            if (conditionBase == null)
                return true;

            _tempValues.Clear();
            _tempValues.AddRange(condition);
            _tempValues.RemoveAt(0);

            conditionBase.DeserializeObject(_tempValues);
            bool result = conditionBase.IsValid();
            conditionBase.Dispose();

            return result;
        }

        static bool IsConditionArrayValid(List<List<int>> conditions)
        {
            if (conditions == null)
                return true;

            for (int index = 0, len = conditions.Count; index < len; index++)
            {
                if (!IsConditionValid(conditions[index]))
                    return false;
            }

            return true;
        }        

        static bool IsConditionArrayValid(List<List<int>> conditions, ref Dictionary<int, int> marks)
        {
            if (conditions == null)
                return true;

            for (int index = 0, len = conditions.Count; index < len; index++)
            {
                if (marks != null)
                {
                    marks[conditions[index][0]] = conditions[index][0];
                }
                if (!IsConditionValid(conditions[index]))
                    return false;
            }

            return true;
        }
    }
}