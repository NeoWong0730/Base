using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Logic
{
    public class UI_JewelCompound_Left_TypeSwitch : UIParseCommon
    {
        private InfinityGridLayoutGroup gridGroup;
        private Dictionary<GameObject, UI_JewelTypeSwitchNode> jewelDict = new Dictionary<GameObject, UI_JewelTypeSwitchNode>();
        private int visualGridCount;

        private Text textSelectType;
        private Image imgArrowUp;
        private Image imgArrwoDown;

        private GameObject switchParent;

        private IListener listener;

        protected override void Parse()
        {
            transform.GetComponent<Button>().onClick.AddListener(OnClickSwitch);
            textSelectType = transform.Find("Text").GetComponent<Text>();
            imgArrowUp = transform.Find("Image_ArrowUp").GetComponent<Image>();
            imgArrwoDown = transform.Find("Image_ArrowDown").GetComponent<Image>();

            switchParent = transform.Find("Scroll_Switch").gameObject;

            gridGroup = transform.Find("Scroll_Switch/ToggleGroup_Switch").GetComponent<InfinityGridLayoutGroup>();
            gridGroup.minAmount = 9;
            gridGroup.updateChildrenCallback = UpdateChildrenCallback;
            gridGroup.GetComponent<ToggleGroup>().allowSwitchOff = true;

            for (int i = 0; i < gridGroup.transform.childCount; ++i)
            {
                Transform trans = gridGroup.transform.GetChild(i);

                UI_JewelTypeSwitchNode node = new UI_JewelTypeSwitchNode();
                node.Init(trans);
                node.AddListener(OnSelectJewelType);
                jewelDict.Add(trans.gameObject, node);
            }

            switchParent.SetActive(false);
            imgArrowUp.gameObject.SetActive(false);
            imgArrwoDown.gameObject.SetActive(true);

            visualGridCount = Enum.GetNames(typeof(EJewelType)).Length;
        }

        private void OnClickSwitch()
        {
            switchParent.SetActive(!switchParent.activeSelf);
            gridGroup.SetAmount(visualGridCount);

            imgArrowUp.gameObject.SetActive(switchParent.activeSelf);
            imgArrwoDown.gameObject.SetActive(!switchParent.activeSelf);
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= visualGridCount)
                return;

            if (jewelDict.ContainsKey(trans.gameObject))
            {
                UI_JewelTypeSwitchNode node = jewelDict[trans.gameObject];
                node.UpdateJewelInfo((EJewelType)index);
            }
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void UpdateType(EJewelType type = EJewelType.All)
        {
            Sys_Equip.Instance.CurJewelType = type;
            OnSelectJewelType(type);
        }

        public void ReisterListener(IListener _listener)
        {
            listener = _listener;
        }

        private void OnSelectJewelType(EJewelType _jewelType)
        {
            listener?.SwitchJewelType(_jewelType);

            TextHelper.SetText(textSelectType, 4161 + (uint)_jewelType);
            switchParent.SetActive(false);
            imgArrowUp.gameObject.SetActive(false);
            imgArrwoDown.gameObject.SetActive(true);
        }

        public interface IListener
        {
            void SwitchJewelType(EJewelType _type);
        }
    }
}


