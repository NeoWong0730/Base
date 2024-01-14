using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lib.Core;
using Table;
using Packet;
using Google.Protobuf.Collections;

namespace Logic
{
    public class UI_Quenching_SpecialEffect_Info : TipEquipInfoBase
    {
        private GameObject template;

        private ItemData item;

        protected override void Parse()
        {
            template = transform.Find("View_Special_Property").gameObject;
            template.SetActive(false);
        }

        public override void OnDestroy()
        {

        }

        public void UpdateSpecialEffectInfo(ItemData _item, Essence essence)
        {
            this.item = _item;

            Lib.Core.FrameworkTool.DestroyChildren(gameObject, template.name, lineStr);

            if (essence.SuitType == 0u && essence.OriginEffectID.Count == 0 )
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }

            //Debug.LogErrorFormat("suitType == {0}", essence.SuitType);
            if (essence.SuitType != 0u)
            {
                //装备淬炼获得套装的描述打造装备时有{0}的概率继承{1}套装 
                CSVParam.Data paramData = CSVParam.Instance.GetConfData(207);
                float rate = System.Convert.ToInt32(paramData.str_value) / 100f;

                CSVSuit.Data suitData = CSVSuit.Instance.GetSuitData(essence.SuitType);

                GameObject entryGo = GameObject.Instantiate<GameObject>(template, template.transform.parent);
                entryGo.gameObject.SetActive(true);
                Text desText = entryGo.transform.Find("Text_Property").GetComponent<Text>();
                desText.text = LanguageHelper.GetTextContent(4210, string.Format("{0}%", rate), LanguageHelper.GetTextContent(suitData.suit_name));
            }

            if (essence.OriginEffectID.Count >= 1 && essence.OriginEffectID.Count <= 2)
            {
                CSVParam.Data paramData =  CSVParam.Instance.GetConfData(205);
                string[] strArr = paramData.str_value.Split('|');

                int index = essence.OriginEffectID.Count - 1;
                int paraValue = System.Convert.ToInt32(strArr[index]) * 100 / 10000;

                GameObject entryGo = GameObject.Instantiate<GameObject>(template, template.transform.parent);
                entryGo.gameObject.SetActive(true);
                Text desText = entryGo.transform.Find("Text_Property").GetComponent<Text>();
                desText.text = LanguageHelper.GetTextContent(4083, paraValue.ToString());

                //计算特效概率，全局
                paramData = CSVParam.Instance.GetConfData(206);
                strArr = paramData.str_value.Split('|');
                paraValue = System.Convert.ToInt32(strArr[0]) * 100 / 10000;

                for (int i = 0; i < essence.OriginEffectID.Count; ++i)
                {
                    GameObject tempGo = GameObject.Instantiate<GameObject>(template, template.transform.parent);
                    tempGo.SetActive(true);

                    Text tempText = tempGo.transform.Find("Text_Property").GetComponent<Text>();

                    CSVEquipmentEffect.Data effectData = CSVEquipmentEffect.Instance.GetDataByEffectId(essence.OriginEffectID[i]);
                    if (effectData != null)
                    {
                        tempText.text = LanguageHelper.GetTextContent(4084, LanguageHelper.GetTextContent(effectData.name), paraValue.ToString());
                    }
                    else
                    {
                        Debug.LogErrorFormat("CSVEquipmentEffect 找不到 id = {0}", essence.OriginEffectID[i]);
                    }
                }
            }
        }
    }
}



