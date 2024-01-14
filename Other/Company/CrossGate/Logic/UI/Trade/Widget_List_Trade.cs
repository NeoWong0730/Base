using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class Widget_List_Trade
    {
        public struct ItemList
        {
            public uint id;
            public uint Group;
            public uint GroupName;
            public uint SubName;
            public uint Sub;
        }

        private uint type, subType;
        //private uint defaultSubType;
        private uint defaultSubKey;
        private Transform transform;
        private GameObject selectTypeGo;
        private GameObject selectItemGo;
        private Action<uint, uint> ClickEvt;

        private List<ItemList> itemList = new List<ItemList>();
        private Dictionary<uint, GameObject> typeGoDic = new Dictionary<uint, GameObject>();
        private Dictionary<uint, Dictionary<uint, GameObject>> itemGoDic = new Dictionary<uint, Dictionary<uint, GameObject>>();

        public Widget_List_Trade(Transform transform, Action<uint, uint> ClickEvt, List<ItemList> itemList)
        {
            this.ClickEvt = ClickEvt;
            this.itemList = itemList;
            this.transform = transform;

            var Widget_Menu_Big = transform.Find("Scroll View/Grid_Btn/WIdget_Menu_Big");
            var Widget_Menu_Small = transform.Find("Scroll View/Grid_Btn/WIdget_Menu_Small");
            Widget_Menu_Big.gameObject.SetActive(false);
            Widget_Menu_Small.gameObject.SetActive(false);
            Lib.Core.FrameworkTool.DestroyChildren(transform.Find("Scroll View/Grid_Btn").gameObject, Widget_Menu_Big.name, Widget_Menu_Small.name);

            var typeDic = new Dictionary<uint, List<ItemList>>();
            foreach (var item in itemList)
            {
                List<ItemList> list = null;
                if (!typeDic.TryGetValue(item.Group, out list))
                {
                    list = new List<ItemList>();
                    typeDic.Add(item.Group, list);
                }
                list.Add(item);
            }


            foreach (var item in typeDic)
            {
                var typeGo = Lib.Core.FrameworkTool.CreateGameObject(Widget_Menu_Big.gameObject, Widget_Menu_Big.parent.gameObject);

                typeGo.name = string.Format("{0}{1}", typeGo.name, "(Clone)");
                typeGo.SetActive(true);
                ResetToggle(typeGo.gameObject.GetComponent<Toggle>());
                typeGo.gameObject.GetComponent<Toggle>().onValueChanged.AddListener((bool value) => OnClick_LeftType(item.Key, typeGo, value));
                typeGoDic.Add(item.Key, typeGo);

                var count = item.Value.Count;
                if (count > 0)
                {
                    var itemConf = item.Value[0];
                    typeGo.transform.Find("Text_Menu_Dark").GetComponent<Text>().text = LanguageHelper.GetTextContent(itemConf.GroupName);
                    typeGo.transform.Find("Text_Menu_Light").GetComponent<Text>().text = LanguageHelper.GetTextContent(itemConf.GroupName);
                }

                bool isHasChild = false;
                foreach(var data in item.Value)
                {
                    if (data.Sub != 0u)
                    {
                        isHasChild = true;
                        break;
                    }
                }

                if (!isHasChild) //仅有一项，不创建子节点
                {
                    Transform transArrow = typeGo.transform.Find("Image");
                    if (transArrow != null)
                        transArrow.gameObject.SetActive(false);

                    Transform transArrow2 = typeGo.transform.Find("Image2");
                    if (transArrow2 != null)
                        transArrow2.gameObject.SetActive(false);

                    continue;
                }

                for (int i = 0; i < item.Value.Count; i++)
                {
                    var itemConf = item.Value[i];

                    var itemGo = Lib.Core.FrameworkTool.CreateGameObject(Widget_Menu_Small.gameObject, Widget_Menu_Small.parent.gameObject);
                    itemGo.name = string.Format("{0}{1}", itemGo.name, "(Clone)");
                    itemGo.transform.Find("Text_Menu_Dark").GetComponent<Text>().text = LanguageHelper.GetTextContent(itemConf.SubName);
                    itemGo.transform.Find("Text_Menu_Light").GetComponent<Text>().text = LanguageHelper.GetTextContent(itemConf.SubName);

                    ResetToggle(itemGo.gameObject.GetComponent<Toggle>());
                    itemGo.gameObject.GetComponent<Toggle>().onValueChanged.AddListener((bool value) => OnClick_LeftItem(itemConf.id, itemGo, value));
                    itemGo.gameObject.SetActive(false);

                    if (!itemGoDic.ContainsKey(itemConf.Group))
                    {
                        itemGoDic.Add(itemConf.Group, new Dictionary<uint, GameObject>());
                    }
                    itemGoDic[itemConf.Group].Add(itemConf.id, itemGo);
                }
            }
        }

        public void SetActive(bool v)
        {
            if (v)
            {
                foreach (var item in typeGoDic)
                {
                    SetToggleState(item.Value.transform, false, Sys_Trade.Instance.HasChild(item.Key));
                }
                foreach (var item in itemGoDic)
                {
                    var values = item.Value;
                    foreach (var sub in values)
                    {
                        sub.Value.gameObject.SetActive(false);
                        SetToggleState(sub.Value.transform, false, false);
                    }
                }

                if (typeGoDic.Count > 0)
                {
                    foreach (var item in typeGoDic)
                    {
                        OnClick_LeftType(item.Key, item.Value, true);
                        break;
                    }
                }
                else
                {
                    OnClick_LeftType(0, null, false);
                }
            }
        }

        private void SetToggleState(Transform trans, bool state, bool hasChild = false)
        {
            trans.Find("Btn_Menu").gameObject.SetActive(state);
            trans.Find("Text_Menu_Dark").gameObject.SetActive(!state);
            trans.Find("Text_Menu_Light").gameObject.SetActive(state);

            Transform transArrow = trans.Find("Image");
            if (transArrow != null)
            {
                transArrow.gameObject.SetActive(hasChild && state);
            }

            Transform transArrow2 = trans.Find("Image2");
            if (transArrow2 != null)
            {
                transArrow2.gameObject.SetActive(hasChild && !state);
            }
        }

        private void ResetToggle(Toggle go)
        {
            go.group = null;
            go.graphic = null;
            go.transform.Find("Btn_Menu").gameObject.SetActive(false);
            go.transform.Find("Text_Menu_Light").gameObject.SetActive(false);
        }

        private void OnClick_LeftType(uint key, GameObject go, bool value)
        {
            if (selectTypeGo != null)
            {
                if (selectTypeGo != go)
                {
                    SetToggleState(selectTypeGo.transform, false, Sys_Trade.Instance.HasChild(type));
                    foreach (var item in itemGoDic)
                    {
                        if (item.Key == type)
                        {
                            var values = item.Value;
                            foreach (var sub in values)
                            {
                                sub.Value.gameObject.SetActive(false);
                            }
                        }
                    }
                }
                else
                {
                    //Debug.LogError("======");
                    if (Sys_Trade.Instance.HasChild(type))
                    {
                        //Debug.LogError("======1");
                        bool isExpand = false;
                        foreach (var item in itemGoDic)
                        {
                            if (item.Key == type)
                            {
                                var values = item.Value;
                                foreach (var sub in values)
                                {
                                    isExpand = sub.Value.gameObject.activeSelf;
                                    sub.Value.gameObject.SetActive(!isExpand);
                                }
                            }
                        }

                        Transform transArrow = go.transform.Find("Image");
                        if (transArrow != null)
                        {
                            transArrow.gameObject.SetActive(!isExpand);
                        }

                        Transform transArrow2 = go.transform.Find("Image2");
                        if (transArrow2 != null)
                        {
                            transArrow2.gameObject.SetActive(isExpand);
                        }
                        //ClickEvt?.Invoke(type, 0u);
                        if (Sys_Trade.Instance.SearchParam.isSearch)
                        {
                            if (defaultSubKey != 0)
                            {
                                foreach (var item in itemGoDic)
                                {
                                    if (item.Key == type)
                                    {
                                        var values = item.Value;
                                        foreach (var sub in values)
                                        {
                                            if (sub.Key == defaultSubKey)
                                            {
                                                defaultSubKey = 0u;
                                                OnClick_LeftItem(sub.Key, sub.Value, true);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return;
                    }
                    else
                    {
                        if (Sys_Trade.Instance.SearchParam.isSearch)
                            ClickEvt?.Invoke(type, 0u);
                    }
                }
            }
            if (go == null)
            {
                OnClick_LeftItem(0, null, false);
                return;
            }
            if (selectTypeGo != go)
            {
                type = key;
                selectTypeGo = go;
                SetToggleState(selectTypeGo.transform, true, Sys_Trade.Instance.HasChild(type));

                bool hasChild = false;
                foreach (var item in itemGoDic)
                {
                    if (item.Key == type)
                    {
                        var values = item.Value;
                        bool isFirst = false;
                        foreach (var sub in values)
                        {
                            sub.Value.gameObject.SetActive(true);

                            if (defaultSubKey != 0u && defaultSubKey != sub.Key)
                                continue;

                            if (!isFirst)
                            {
                                isFirst = true;
                                hasChild = true;
                                defaultSubKey = 0u;
                                //defaultSubType = 0u;
                                OnClick_LeftItem(sub.Key, sub.Value, true);
                            }
                        }

                    }
                }

                if (!hasChild)
                {
                    ClickEvt?.Invoke(type, 0u);
                }
            }
        }

        private void OnClick_LeftItem(uint key, GameObject go, bool value)
        {
            if (selectItemGo != null)
            {
                if (selectItemGo != go)
                {
                    SetToggleState(selectItemGo.transform, false, Sys_Trade.Instance.HasChild(key));
                }
            }
            if (go == null)
            {
                return;
            }

            if (selectItemGo != go)
            {
                subType = itemList.Find(a => { return a.id == key; }).Sub;
                selectItemGo = go;
                SetToggleState(selectItemGo.transform, true, false);
            }

            ClickEvt?.Invoke(type, subType);
        }

        public void OnSelect(uint typeId)
        {
            GameObject goTemp;
            //foreach (var data in typeGoDic)
            //{
            //    Toggle toggle = data.Value.GetComponent<Toggle>();
            //    toggle.isOn = data.Key == typeId;
            //}
            if (typeGoDic.TryGetValue(typeId, out goTemp))
            {
                OnClick_LeftType(typeId, goTemp, true);
                //goTemp.GetComponent<Toggle>().isOn = true;
            }
            else
            {
                //defaultSubType = typeId;
                for (int i = 0; i < itemList.Count; ++i)
                {
                    if (itemList[i].Sub == typeId)
                    {
                        defaultSubKey = (uint)i + 1;
                        break;
                    }
                }

                //defaultSubKey = typeId;
                uint parentTypeId = Sys_Trade.Instance.GetCateId(typeId);
                if (typeGoDic.TryGetValue(parentTypeId, out goTemp))
                    OnClick_LeftType(parentTypeId, goTemp, true);
                //goTemp.GetComponent<Toggle>().isOn = true;
            }
        }

        public void UpdateRedPointState()
        {
            foreach(var data in typeGoDic)
            {
                bool isRed = Sys_Knowledge.Instance.IsGlealingRed(data.Key);
                Transform transRed = data.Value.transform.Find("Image_Red");
                if (transRed != null)
                    transRed.gameObject.SetActive(isRed);
            }

            foreach(var data in itemGoDic)
            {
                foreach(var subData in data.Value)
                {
                    uint subType = itemList.Find(a => { return a.id == subData.Key; }).Sub;
                    //Debug.LogErrorFormat("type {0}, subType {1}", data.Key, subType);
                    //Debug.LogError(subType);
                    bool isRed = Sys_Knowledge.Instance.IsSubGlealingRed(data.Key, subType);
                    Transform transRed = subData.Value.transform.Find("Image_Red");
                    if (transRed != null)
                        transRed.gameObject.SetActive(isRed);
                }
            }
        }
    }
}


