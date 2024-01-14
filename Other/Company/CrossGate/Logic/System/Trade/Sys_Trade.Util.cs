using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using Table;

namespace Logic
{
    public partial class Sys_Trade : SystemModuleBase<Sys_Trade>
    {
        private uint _priceRate; //限制定价浮动比例10%
        private uint _priceRateMax; //限制定价浮动上下限50%

        private uint _minLocalBooth; //限制定价最小摊位费(本服)
        private uint _maxLocalBooth; //限制定价最大摊位费(本服)
        private uint _localBoothRate; //摊位费费率(本服)
        private uint _crossBooth;      //摊位费(跨服)

        private uint _localTallegeRate; //税率(本服)
        private uint _crossTallegeRate; //税率(跨服)
        private uint _assignTallegeRate; //指定交易税率

        private uint _timeTreasureGood; //珍品单次摆摊时长 天
        private uint _timeNormalGood; //非珍品单次摆摊时长 天
        private uint _timePublicity; //公示时长 秒

        private uint _boothGridCount; //每人摊位数

        private uint _tradeRecordCount; //交易记录保存数

        private uint _timeNormalFreez;  //非珍品收买冻结期
        private uint _timeTreasureFreez; //珍品冻结期

        private uint _minPrice;         //定价下限
        private uint _watchCount;       //可关注商品数量
        private uint _assignMinPrice;   //制定交易最低价格
        private uint _hotPriceRate;     //一口价系数
        private uint _timePublicityCompete; //公示期竞价时间

        private uint _limitSaleLevel;   //限制定价商品上架等级限制
        private uint _freeSaleLevel;    //自由定价商品上架等级限制
        private uint _limitBuyLevel;    //限制定价商品购买等级限制
        private uint _freeBuyLevel;     //自由定价商品购买等级限制

        private uint _timeCheckGood;    //商品审核时间 小时

        private uint _timeAppeal;       //玩家申诉时间 天
        private uint _timeApppealSecond; //玩家申诉复审时间 天

        private uint _searchCostId;     //搜索消耗道具id
        private uint _searchCostNum;    //搜索消耗道具数量
        private uint _minCrossPrice;    //跨服最低定价

        private string _appealUrl;

        private void LoadTradePrama()
        {
            uint.TryParse(CSVTradeParam.Instance.GetConfData(1).str_value, out _priceRate);
            uint.TryParse(CSVTradeParam.Instance.GetConfData(2).str_value, out _priceRateMax);

            uint.TryParse(CSVTradeParam.Instance.GetConfData(3).str_value, out _minLocalBooth);
            uint.TryParse(CSVTradeParam.Instance.GetConfData(4).str_value, out _maxLocalBooth);
            uint.TryParse(CSVTradeParam.Instance.GetConfData(5).str_value, out _localBoothRate);
            uint.TryParse(CSVTradeParam.Instance.GetConfData(6).str_value, out _crossBooth);

            uint.TryParse(CSVTradeParam.Instance.GetConfData(7).str_value, out _localTallegeRate);
            uint.TryParse(CSVTradeParam.Instance.GetConfData(8).str_value, out _crossTallegeRate);
            uint.TryParse(CSVTradeParam.Instance.GetConfData(9).str_value, out _assignTallegeRate);

            uint.TryParse(CSVTradeParam.Instance.GetConfData(10).str_value, out _timeTreasureGood);
            uint.TryParse(CSVTradeParam.Instance.GetConfData(11).str_value, out _timeNormalGood);
            uint.TryParse(CSVTradeParam.Instance.GetConfData(12).str_value, out _timePublicity);

            uint.TryParse(CSVTradeParam.Instance.GetConfData(13).str_value, out _boothGridCount);

            uint.TryParse(CSVTradeParam.Instance.GetConfData(14).str_value, out _tradeRecordCount);

            uint.TryParse(CSVTradeParam.Instance.GetConfData(15).str_value, out _timeNormalFreez);
            uint.TryParse(CSVTradeParam.Instance.GetConfData(16).str_value, out _timeTreasureFreez);

            uint.TryParse(CSVTradeParam.Instance.GetConfData(17).str_value, out _minPrice);
            uint.TryParse(CSVTradeParam.Instance.GetConfData(18).str_value, out _watchCount);
            uint.TryParse(CSVTradeParam.Instance.GetConfData(19).str_value, out _assignMinPrice);
            uint.TryParse(CSVTradeParam.Instance.GetConfData(21).str_value, out _hotPriceRate);
            uint.TryParse(CSVTradeParam.Instance.GetConfData(22).str_value, out _timePublicityCompete);

            uint.TryParse(CSVTradeParam.Instance.GetConfData(27).str_value, out _limitSaleLevel);
            uint.TryParse(CSVTradeParam.Instance.GetConfData(28).str_value, out _freeSaleLevel);
            uint.TryParse(CSVTradeParam.Instance.GetConfData(29).str_value, out _limitBuyLevel);
            uint.TryParse(CSVTradeParam.Instance.GetConfData(30).str_value, out _freeBuyLevel);

            uint.TryParse(CSVTradeParam.Instance.GetConfData(31).str_value, out _timeCheckGood);

            uint.TryParse(CSVTradeParam.Instance.GetConfData(37).str_value, out _timeAppeal);
            uint.TryParse(CSVTradeParam.Instance.GetConfData(38).str_value, out _timeApppealSecond);
            uint.TryParse(CSVTradeParam.Instance.GetConfData(44).str_value, out _minCrossPrice);

            string[] arrCost = CSVTradeParam.Instance.GetConfData(40).str_value.Split('|');
            if (arrCost.Length >= 2)
            {
                uint.TryParse(arrCost[0], out _searchCostId);
                uint.TryParse(arrCost[1], out _searchCostNum);
            }

            //_appealUrl = CSVTradeParam.Instance.GetConfData(42).str_value;
        }

        /// <summary>
        /// 限制定价摊位费1000-10000
        /// </summary>
        /// <param name="boothPrice"></param>
        /// <returns></returns>
        public uint GetBoothPrice(uint boothPrice, bool isCross)
        {
            if (!isCross)
            {
                boothPrice = boothPrice < _minLocalBooth ? _minLocalBooth : boothPrice;
                boothPrice = boothPrice > _maxLocalBooth ? _maxLocalBooth : boothPrice;
            }
            else
            {
                boothPrice = _crossBooth;
            }

            return boothPrice;
        }

        /// <summary>
        /// 限制定价计算摊位费
        /// </summary>
        /// <param name="totalPrice"></param>
        /// <returns></returns>
        public uint CalBoothPrice(uint totalPrice)
        {
            return totalPrice * _localBoothRate;
        }

        /// <summary>
        /// 一口价
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public uint GetHotPrice(long price)
        {
            return (uint)(price * _hotPriceRate / 100L);
        }

        /// <summary>
        /// 自由定价，最小1000
        /// </summary>
        /// <returns></returns>
        public uint GetFreePricingMin(bool cross = false)
        {
            return cross ? _minCrossPrice : _minPrice;
        }

        /// <summary>
        /// 指定交易最小价格
        /// </summary>
        /// <returns></returns>
        public uint GetAssignPriceMin()
        {
            return _assignMinPrice;
        }

        /// <summary>
        /// 输入最大999,999,999
        /// </summary>
        /// <returns></returns>
        public uint GetInputValueMax()
        {
            return 999999999;
        }

        /// <summary>
        /// 指定交易最大天数
        /// </summary>
        /// <returns></returns>
        public uint GetAssignTimeMax()
        {
            return _timeTreasureGood; //珍品摆摊最大时长
        }

        /// <summary>
        /// 出售时间 (天)
        /// </summary>
        /// <param name="isTreasure"></param>
        /// <returns></returns>
        public uint SaleTime(bool isTreasure)
        {
            if (isTreasure)
                return _timeTreasureGood;
            else
                return _timeNormalGood;
        }

        /// <summary>
        /// 限制价格，浮动百分比限制
        /// </summary>
        /// <returns></returns>
        public int PriceFloatedLimit()
        {
            return (int)_priceRateMax; //百分比
        }

        /// <summary>
        /// 限制价格，浮动delta
        /// </summary>
        /// <returns></returns>
        public int PriceDelta()
        {
            return (int)_priceRate; //百分比
        }

        /// <summary>
        /// 公示期最后xx时间内，可以竞价
        /// </summary>
        /// <returns></returns>
        public uint PublicityRemainTime()
        {
            return _timePublicityCompete;
        }

        /// <summary>
        /// 计算比价
        /// </summary>
        /// <param name="price"></param>
        /// <param name="recommendPrice"></param>
        /// <returns></returns>
        public string CalPriceCompare(int percent)
        {
            if (percent == 0)
            {
                return LanguageHelper.GetTextContent(2011124);
            }
            else if (percent > 0)
            {
                return LanguageHelper.GetTextContent(2011066, percent.ToString());
            }
            else
            {
                return LanguageHelper.GetTextContent(2011067, UnityEngine.Mathf.Abs(percent).ToString());
            }
        }

        /// <summary>
        /// 计算单位
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public string CalUnit(uint param)
        {
            string unit = "";
            if (param >= 100000000) //亿
            {
                unit = LanguageHelper.GetTextContent(2011113);
            }
            else if (param >= 10000000) //千万
            {
                unit = LanguageHelper.GetTextContent(2011112);
            }
            else if (param >= 1000000) //百万
            {
                unit = LanguageHelper.GetTextContent(2011111);
            }
            else if (param >= 100000) //十万
            {
                unit = LanguageHelper.GetTextContent(2011110);
            }
            else if (param >= 10000) //万
            {
                unit = LanguageHelper.GetTextContent(2011109);
            }

            return unit;
        }

        /// <summary>
        /// 最多关注商品数量
        /// </summary>
        /// <returns></returns>
        public int GetWatchMaxNum()
        {
            return (int)_watchCount;
        }
        
        /// <summary>
        /// 计算摊位费规则说明
        /// </summary>
        /// <returns></returns>
        public UIRuleParam GetBoothPriceTip()
        {
            UIRuleParam param = new UIRuleParam();
            param.TitlelanId = 2011068u;
            param.StrContent = LanguageHelper.GetTextContent(2011129, _localBoothRate.ToString(), _minLocalBooth.ToString(), _maxLocalBooth.ToString(), _crossBooth.ToString());
            return param;
        }

        /// <summary>
        /// 公示时间秒
        /// </summary>
        /// <returns></returns>
        public uint GetPublicityTime()
        {
            return _timePublicity;
        }

        /// <summary>
        /// 审核时间，小时
        /// </summary>
        /// <returns></returns>
        public uint GetCheckTime()
        {
            return _timeCheckGood;
        }

        /// <summary>
        /// 交易最多保存记录
        /// </summary>
        /// <returns></returns>
        public uint GetTradeRecordCount()
        {
            return _tradeRecordCount;
        }

        /// <summary>
        /// 税率 (百分比)
        /// </summary>
        /// <param name="brief"></param>
        /// <returns></returns>
        public uint GetTradeRate(TradeBrief brief)
        {
            if (brief.TargetId != 0)
                return _assignTallegeRate;

            if (brief.BCross)
                return _crossTallegeRate;
            else
                return _localTallegeRate;
        }

        /// <summary>
        /// 商品,上架等级
        /// </summary>
        /// <param name="pricingType"></param>
        /// <returns></returns>
        public uint GoodSaleLevel(uint pricingType)
        {
            if (pricingType == 0u)   //限制定价
            {
                return _limitSaleLevel;
            }
            else if (pricingType == 1u) //自由定价
            {
                return _freeSaleLevel;
            }

            return 0u;
        }

        /// <summary>
        /// 商品,购买等级
        /// </summary>
        /// <param name="pricingType"></param>
        /// <returns></returns>
        public uint GoodBuyLevel(uint pricingType)
        {
            if (pricingType == 0u)   //限制定价
            {
                return _limitBuyLevel;
            }
            else if (pricingType == 1u) //自由定价
            {
                return _freeBuyLevel;
            }

            return 0u;
        }

        /// <summary>
        /// 获得指定交易税率
        /// </summary>
        /// <returns></returns>
        public uint GetAssignTradeRate()
        {
            return _assignTallegeRate;
        }

        /// <summary>
        /// 获得玩家申诉时间
        /// </summary>
        /// <returns></returns>
        public uint GetAppealTime()
        {
            return _timeAppeal;
        }

        public uint GetSearchCostId()
        {
            return _searchCostId;
        }

        public uint GetSearchCostNum()
        {
            return _searchCostNum;
        }
    }
}
