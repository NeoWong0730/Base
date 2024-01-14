using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Packet;
using Net;
using Table;
using Lib.Core;


namespace Logic
{
    /// <summary>
    /// 通用教程
    /// </summary>
    public class Sys_CommonCourse : SystemModuleBase<Sys_CommonCourse>
    {


        /// <summary> 教程ID对应一级标题列表 (一级标题已排序) </summary>
        private Dictionary<uint, List<CSVTutorialFirstHeading.Data>> dictCourse = new Dictionary<uint, List<CSVTutorialFirstHeading.Data>>();
        /// <summary> 一级标题ID对应二级标题列表 (二级标题列表已排序) </summary>
        private Dictionary<uint, List<CSVTutorialSecondHeading.Data>> dictFirstTitles = new Dictionary<uint, List<CSVTutorialSecondHeading.Data>>();

        #region 系统函数
        public override void Init()
        {

        }
        #endregion

        #region func
        /// <summary>
        /// 打开通用教程界面 (教程ID，选中1级标题ID，2级标题ID)
        /// </summary>
        public void OpenCommonCourse(uint courseId, uint firstTitleId = 0, uint secondTitleId = 0)
        {
            ParseCourseData(courseId);

            CommonCoursePrama param = new CommonCoursePrama();
            param.courseId = courseId;
            param.FirstTitleId = firstTitleId;
            param.SecondTitleId = secondTitleId;
            UIManager.OpenUI(EUIID.UI_CommonCourse, false, param);
        }
        /// <summary>
        /// 获取教程下的一级标题列表(已排序) 未找到返回null
        /// </summary>
        public List<CSVTutorialFirstHeading.Data>  GetFirstHeadingList(uint courseId)
        {
            if (dictCourse.TryGetValue(courseId, out List<CSVTutorialFirstHeading.Data>  value))
            {
                return value;
            }
            return null;
        }
        /// <summary>
        /// 获取一级标题下的二级标题列表(已排序) 未找到返回null
        /// </summary>
        public List<CSVTutorialSecondHeading.Data>  GetSecondHeadingList(uint firstHeadingId)
        {
            if (dictFirstTitles.TryGetValue(firstHeadingId, out List<CSVTutorialSecondHeading.Data>  value))
            {
                return value;
            }
            return null;
        }
        /// <summary>
        /// 解析当前教程表(已解析则跳过)
        /// </summary>
        public void ParseCourseData(uint courseId)
        {
            if (!dictCourse.TryGetValue(courseId, out List<CSVTutorialFirstHeading.Data>  firstDataList))
            {
                CSVTutorial.Data data = CSVTutorial.Instance.GetConfData(courseId);
                if (data != null)
                {
                    firstDataList = new List<CSVTutorialFirstHeading.Data>();
                    var firstTitles = data.firstlHeading_array;
                    for (int i = 0; i < firstTitles.Count; i++)
                    {
                        uint firstTitleId = firstTitles[i];
                        CSVTutorialFirstHeading.Data firstData = CSVTutorialFirstHeading.Instance.GetConfData(firstTitleId);
                        if (firstData != null)
                        {
                            //一级标题列表的插入排序
                            if (firstDataList.Count > 0)
                            {
                                var flag = false;
                                for (int j = 0; j < firstDataList.Count; j++)
                                {
                                    if (firstData.firstHeadingSrot < firstDataList[j].firstHeadingSrot)
                                    {
                                        firstDataList.Insert(j, firstData);
                                        flag = true;
                                        break;
                                    }
                                }
                                if (!flag)
                                {
                                    firstDataList.Add(firstData);
                                }
                            }
                            else
                            {
                                firstDataList.Add(firstData);
                            }
                            //解析一级标题下的二级标题
                            if (!dictFirstTitles.TryGetValue(firstTitleId, out List<CSVTutorialSecondHeading.Data>  secondDataList))
                            {
                                secondDataList = new List<CSVTutorialSecondHeading.Data>();
                                var secondTitles = firstData.secondHeading_array;
                                for (int j = 0; j < secondTitles.Count; j++)
                                {
                                    uint secondTitleId = secondTitles[j];
                                    CSVTutorialSecondHeading.Data secondData = CSVTutorialSecondHeading.Instance.GetConfData(secondTitleId);
                                    if (secondData != null)
                                    {
                                        //二级标题列表的插入排序
                                        if (secondDataList.Count > 0)
                                        {
                                            var flag = false;
                                            for (int k = 0; k < secondDataList.Count; k++)
                                            {
                                                if (secondData.secondHeadingSrot < secondDataList[k].secondHeadingSrot)
                                                {
                                                    secondDataList.Insert(k, secondData);
                                                    flag = true;
                                                    break;
                                                }
                                            }
                                            if (!flag)
                                            {
                                                secondDataList.Add(secondData);
                                            }
                                        }
                                        else
                                        {
                                            secondDataList.Add(secondData);
                                        }
                                    }
                                    else
                                    {
                                        DebugUtil.Log(ELogType.eNone, "找不到教程二级标题id对应的数据 " + secondTitleId);

                                    }
                                }
                                dictFirstTitles[firstTitleId] = secondDataList;
                            }
                        }
                        else
                        {
                            DebugUtil.Log(ELogType.eNone, "找不到教程一级标题id对应的数据 " + firstTitleId);
                        }
                    }
                    dictCourse[courseId] = firstDataList;
                }
                else
                {
                    DebugUtil.Log(ELogType.eNone, "找不到教程id对应的数据 " + courseId);
                }
            }
        }
        /// <summary> 获取当前选中的二级标题id </summary>
        public uint GetSecondTitleID(uint firstId,uint secondId)
        {
            if (secondId > 0)
            {
                return secondId;
            }
            var secondHeadingList = GetSecondHeadingList(firstId);
            return secondHeadingList[0].id;
        }
        /// <summary> 检测是否是第一个标题 </summary>
        public bool CheckIsFirstTitle(uint courseId, uint firstId, uint secondId)
        {
            var listFirstTitles = GetFirstHeadingList(courseId);
            if(listFirstTitles != null && listFirstTitles[0].id == firstId)
            {
                if(secondId == 0)
                {
                    return true;
                }
                var listSecondTitles = GetSecondHeadingList(firstId);
                if(listSecondTitles!=null && listSecondTitles[0].id == secondId)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary> 检测是否是最后一个标题 </summary>
        public bool CheckIsLastTitle(uint courseId, uint firstId, uint secondId)
        {
            var listFirstTitles = GetFirstHeadingList(courseId);
            if (listFirstTitles != null && listFirstTitles[listFirstTitles.Count - 1].id == firstId)
            {
                var listSecondTitles = GetSecondHeadingList(firstId);
                if (listSecondTitles != null && listSecondTitles[listSecondTitles.Count - 1].id == secondId)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary> 获取前一个标题参数 </summary>

        public CommonCoursePrama GetFrontTitleInfo(uint courseId, uint firstId, uint secondId, string keyword = "")
        {
            CommonCoursePrama param = new CommonCoursePrama();
            //if (keyword.Length > 0)
            //{
            //    //先找同标题下有效的二级标题
            //    var listSecondTitles = GetSecondHeadingList(firstId);
            //    List<uint> vaileSecondId = new List<uint>();
            //    for (int i = 0; i < listSecondTitles.Count; i++)
            //    {
            //        var secondData = listSecondTitles[i];
            //        if (secondData.id == secondId)
            //        {
            //            if (vaileSecondId.Count > 1)
            //            {
            //                //输出自己前一个有效的标题
            //                param.FirstTitleId = firstId;
            //                param.SecondTitleId = vaileSecondId[vaileSecondId.Count - 1];
            //                return param;
            //            }
            //            break;
            //        }
            //        if (LanguageHelper.GetTextContent(secondData.secondHeadingName).Contains(keyword))
            //        {
            //            vaileSecondId.Add(secondData.id);
            //        }
            //    }
            //}
            //else
            //{
            var listSecondTitles = GetSecondHeadingList(firstId);
            if (secondId == 0 || listSecondTitles[0].id == secondId)
            {
                //需要前一个一级标题的最后一个二级标题
                var listFirstTitles = GetFirstHeadingList(courseId);
                if (listFirstTitles[0].id == firstId)
                {
                    //已经是第一个一级标题
                    param.FirstTitleId = firstId;
                    param.SecondTitleId = 0;
                }
                else
                {
                    for (int i = 1; i < listFirstTitles.Count; i++)
                    {
                        if (listFirstTitles[i].id == firstId)
                        {
                            uint frontFirstId = listFirstTitles[i - 1].id;
                            var listFrontSecondTitles = GetSecondHeadingList(frontFirstId);
                            param.FirstTitleId = frontFirstId;
                            param.SecondTitleId = listFrontSecondTitles[listFrontSecondTitles.Count - 1].id;
                            break;
                        }
                    }
                }
            }
            else
            {
                for (int i = 1; i < listSecondTitles.Count; i++)
                {
                    if (listSecondTitles[i].id == secondId)
                    {
                        param.FirstTitleId = firstId;
                        param.SecondTitleId = listSecondTitles[i - 1].id;
                        break;
                    }
                }
            }
            //}
            return param;
        }
        /// <summary> 获取后一个标题参数 </summary>
        public CommonCoursePrama GetAfterTitleInfo(uint courseId, uint firstId, uint secondId, string keyword = "")
        {
            CommonCoursePrama param = new CommonCoursePrama();

            var listSecondTitles = GetSecondHeadingList(firstId);
            if (listSecondTitles[listSecondTitles.Count-1].id == secondId)
            {
                //需要后一个一级标题的第一个二级标题
                var listFirstTitles = GetFirstHeadingList(courseId);
                if (listFirstTitles[listFirstTitles.Count - 1].id == firstId)
                {
                    //已经是最后一个一级标题
                    param.FirstTitleId = firstId;
                    param.SecondTitleId = secondId;
                }
                else
                {
                    for (int i = 0; i < listFirstTitles.Count - 1; i++)
                    {
                        if (listFirstTitles[i].id == firstId)
                        {
                            var nextFirstId = listFirstTitles[i + 1].id;
                            param.FirstTitleId = nextFirstId;
                            var SecondTitles = GetSecondHeadingList(nextFirstId);
                            param.SecondTitleId = SecondTitles[0].id;
                            break;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < listSecondTitles.Count - 1; i++)
                {
                    if (listSecondTitles[i].id == secondId)
                    {
                        param.FirstTitleId = firstId;
                        param.SecondTitleId = listSecondTitles[i + 1].id;
                        break;
                    }
                }
            }
            return param;
        }
        /// <summary>
        /// 获取关键字高亮的文本
        /// </summary>
        public string GetHightLightText(string txt, string keyword)
        {
            if (txt.Contains(keyword))
            {
                string replaceStr = LanguageHelper.GetTextContent(3219000001, keyword);
                return txt.Replace(keyword, replaceStr);
            }
            return txt;
        }
        /// <summary>
        /// 获取有效的参数 (仅在有关键字时调用)
        /// </summary>
        public CommonCoursePrama GetValidTitleInfo(uint courseId, uint firstId, uint secondId, string keyword)
        {
            CommonCoursePrama param = new CommonCoursePrama();
            if (keyword.Length > 0)
            {
                //先验证有效性
                //CSVTutorialFirstHeading.Data nowFirstData = CSVTutorialFirstHeading.Instance.GetConfData(firstId);
                //if (nowFirstData != null && LanguageHelper.GetTextContent(nowFirstData.firstHeadingName).Contains(keyword))
                //{
                //    param.FirstTitleId = firstId;
                //    //一标题有效，继续验证二标题
                //    CSVTutorialSecondHeading.Data nowsecondData = CSVTutorialSecondHeading.Instance.GetConfData(secondId);
                //    if (nowsecondData != null && LanguageHelper.GetTextContent(nowsecondData.secondHeadingName).Contains(keyword))
                //    {
                //        param.SecondTitleId = secondId;
                //        return param;
                //    }
                //    else
                //    {
                //        param.SecondTitleId = GetValidSecondTitleId(firstId, keyword);
                //        return param;
                //    }
                //}
                //else
                //{
                //    //一标题无效再检测二标题(二标题可能有效)
                //    if (secondId == 0 && firstId > 0)
                //    {
                //        //选中一级标题时，secondId会传0,找第一个有效二级标题
                //        uint validSecondId = GetValidSecondTitleId(firstId, keyword);
                //        if (validSecondId > 0)
                //        {
                //            param.FirstTitleId = firstId;
                //            param.SecondTitleId = validSecondId;
                //            return param;
                //        }
                //    }
                //    CSVTutorialSecondHeading.Data nowsecondData = CSVTutorialSecondHeading.Instance.GetConfData(secondId);
                //    if (nowsecondData != null && LanguageHelper.GetTextContent(nowsecondData.secondHeadingName).Contains(keyword))
                //    {
                //        param.FirstTitleId = firstId;
                //        param.SecondTitleId = secondId;
                //        return param;
                //    }
                //    else
                //    {
                //二标题也无效，找符合条件的第一个二级标题
                var listFirstTitles = GetFirstHeadingList(courseId);
                for (int i = 0; i < listFirstTitles.Count; i++)
                {
                    var firstData = listFirstTitles[i];
                    var validSecondId = GetValidSecondTitleId(firstData.id, keyword);
                    if (validSecondId > 0)
                    {
                        param.FirstTitleId = firstData.id;
                        param.SecondTitleId = validSecondId;
                        return param;
                    }
                    else
                    {
                        string txtFirstTitle = LanguageHelper.GetTextContent(firstData.firstHeadingName);
                        if (txtFirstTitle.Contains(keyword))
                        {
                            param.FirstTitleId = firstData.id;
                            //找符合条件的第一个二级标题
                            param.SecondTitleId = GetValidSecondTitleId(firstData.id, keyword);
                            return param;
                        }
                    }
                }
                //    }
                //}
            }
            return param;
        }
        /// <summary>
        /// 获取有效的二标题ID
        /// </summary>
        public uint GetValidSecondTitleId(uint firstId, string keyword)
        {
            var listSecondTitles = GetSecondHeadingList(firstId);
            if (listSecondTitles.Count > 1)
            {
                for (int j = 0; j < listSecondTitles.Count; j++)
                {
                    var secondData = listSecondTitles[j];
                    string txtSecondTitle = LanguageHelper.GetTextContent(secondData.secondHeadingName);
                    if (txtSecondTitle.Contains(keyword))
                    {
                        return secondData.id;
                    }
                }
            }
            return 0;
        }
        #endregion
    }
}
