using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Make_Success : UIBase
    {
        private EquipItem leftEquip;
        private Text textLeftName;

        private EquipItem rightEquip;
        private Text textRightName;

        private UI_Equipment_Make_Right_Property propGo;

        private Sys_Equip.MakeEquipmentResult result;

        protected override void OnLoaded()
        {            
            Lib.Core.EventTrigger.AddEventListener(transform.Find("Black").gameObject, EventTriggerType.PointerClick, (eventData) =>
            {
                UIManager.CloseUI(EUIID.UI_Make_Success);
            });

            leftEquip = new EquipItem();
            leftEquip.Bind(transform.Find("Animator/View_Left/EquipItem").gameObject);
            leftEquip.Layout.btnItem.onClick.AddListener(OnClickEquipment);
            textLeftName = transform.Find("Animator/View_Left/Text_Name").GetComponent<Text>();

            rightEquip = new EquipItem();
            rightEquip.Bind(transform.Find("Animator/View_Right/EquipItem").gameObject);
            rightEquip.Layout.btnItem.onClick.AddListener(OnClickEquipment);
            textRightName = transform.Find("Animator/View_Right/Text_Name").GetComponent<Text>();

            propGo = new UI_Equipment_Make_Right_Property();
            propGo.Init(transform.Find("Animator/Attr_Grid"));
        }

        protected override void OnOpen(object arg)
        {            
            result = null;
            if (arg != null)
            {
                result = (Sys_Equip.MakeEquipmentResult)(arg);
            }
        }

        protected override void OnShow()
        {         
            UpdateInfo();
        }

        private void OnClickEquipment()
        {
            if (result != null)
            {
                EquipTipsData tipData = new EquipTipsData();
                tipData.equip = result.preEquip;
                tipData.isCompare = false;

                UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
            }
        }

        private void UpdateInfo()
        {
            if (result != null)
            {
                uint itemId = result.preEquip.cSVItemData.id;
                CSVItem.Data itemInfo = CSVItem.Instance.GetConfData(itemId);

                CSVSuit.Data suitInfo = CSVSuit.Instance.GetConfData(result.paperId);

                //leftEquip
                leftEquip.SetData(result.preEquip);
                textLeftName.text = Sys_Equip.Instance.GetEquipmentName(result.preEquip);

                //rightEquip
                ItemData newItem = Sys_Equip.Instance.GetItemData(result.preEquip.Uuid);
                rightEquip.SetData(newItem);
                textRightName.text = Sys_Equip.Instance.GetEquipmentName(newItem);


                //property
                propGo.UpdateInfo(Sys_Equip.Instance.GetItemData(result.preEquip.Uuid), result.paperId);
            }
        }

    }
}

