using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Lib.Core;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Knowledge_Award_Cell
    {
        private class CellItem
        {
            private Transform transform;

            private PropItem _propItem;

            public void Init(Transform trans)
            {
                transform = trans;

                _propItem = new PropItem();
                _propItem.BindGameObject(transform.gameObject);
            }

            public void OnShow()
            {
                transform.gameObject.SetActive(true);
            }

            public void OnHide()
            {
                transform.gameObject.SetActive(false);
            }

            private void OnClick()
            {

            }

            public void UpdateInfo(ItemIdCount item, bool isGet)
            {
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(item.id, item.count, true, false, false, false, false, true, false, true);
                _propItem.SetData(itemData, EUIID.UI_Knowledge_Award);

                _propItem.gotGo.SetActive(isGet);
            }
        }

        private Transform transform;

        private Text _textNum;
        private GameObject _itemTemplate;

        private CSVStageReward.Data _stageData;

        public void Init(Transform trans)
        {
            transform = trans;

            _textNum = transform.Find("Image_Title/Text_Num").GetComponent<Text>();

            _itemTemplate = transform.Find("Scroll_View/Viewport/Item").gameObject;
            _itemTemplate.SetActive(false);
        }

        public void OnShow()
        {
            transform.gameObject.SetActive(true);
        }

        public void OnHide()
        {
            transform.gameObject.SetActive(false);
        }

        public void OnDestroy()
        {

        }

        public void UpdateInfo(Sys_Knowledge.ETypes type, int stage)
        {
            _stageData = CSVStageReward.Instance.GetConfData((uint)type);

            int totalNum = (int)_stageData.stage[stage];
            int activeNum = Sys_Knowledge.Instance.GetUnlockEventsCount(type);

            _textNum.text = string.Format("{0}/{1}", activeNum, totalNum);

            FrameworkTool.DestroyChildren(_itemTemplate.transform.parent.gameObject, _itemTemplate.name);

            bool isGet = Sys_Knowledge.Instance.GetStageReward(type) > stage;
            ImageHelper.SetImageGray(_textNum, isGet);

            uint dropId = stage < _stageData.Reward.Count ? _stageData.Reward[stage] : 0u;
            List<ItemIdCount>  listItems = CSVDrop.Instance.GetDropItem(dropId);
            for (int i = 0; i < listItems.Count; ++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(_itemTemplate);
                go.transform.SetParent(_itemTemplate.transform.parent);
                go.SetActive(true);

                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = _itemTemplate.transform.localScale;

                CellItem item = new CellItem();
                item.Init(go.transform);
                item.UpdateInfo(listItems[i], isGet);
            }
        }
    }
}


