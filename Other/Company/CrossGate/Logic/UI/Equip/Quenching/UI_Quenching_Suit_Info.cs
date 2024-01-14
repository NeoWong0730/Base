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
    public class UI_Quenching_Suit_Info : TipEquipInfoBase
    {
        private GameObject template;

        private ItemData item;

        protected override void Parse()
        {
            template = transform.Find("View_Suit_Property").gameObject;
            template.SetActive(false);
        }

        public override void OnDestroy()
        {

        }

        public void UpdateSuitInfo(ItemData _item, RepeatedField<uint> effectIds)
        {
            this.item = _item;

            Lib.Core.FrameworkTool.DestroyChildren(gameObject, template.name, lineStr);

            if (effectIds.Count == 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }

            if (effectIds.Count >= 1 && effectIds.Count <= 2)
            {
                CSVParam.Data paramData =  CSVParam.Instance.GetConfData(205);
                string[] strArr = paramData.str_value.Split('|');

                int index = effectIds.Count - 1;
                int paraValue = System.Convert.ToInt32(strArr[index]) * 100 / 10000;

                GameObject entryGo = GameObject.Instantiate<GameObject>(template, template.transform.parent);
                Text desText = entryGo.transform.Find("Text_Property").GetComponent<Text>();
                desText.text = LanguageHelper.GetTextContent(4083, paraValue.ToString());

                //计算特效概率，全局
                paramData = CSVParam.Instance.GetConfData(206);
                strArr = paramData.str_value.Split('|');
                paraValue = System.Convert.ToInt32(strArr[0]) * 100 / 10000;

                for (int i = 0; i < effectIds.Count; ++i)
                {
                    GameObject tempGo = GameObject.Instantiate<GameObject>(template, template.transform.parent);
                    tempGo.SetActive(true);

                    Text tempText = tempGo.transform.Find("Text_Property").GetComponent<Text>();

                    CSVEquipmentEffect.Data effectData = CSVEquipmentEffect.Instance.GetDataByEffectId(effectIds[i]);
                    if (effectData != null)
                    {
                        tempText.text = LanguageHelper.GetTextContent(4084, LanguageHelper.GetTextContent(effectData.name), paraValue.ToString());
                    }
                    else
                    {
                        Debug.LogErrorFormat("CSVEquipmentEffect 找不到 id = {0}", effectIds[i]);
                    }
                }
            }
        }
    }
}



