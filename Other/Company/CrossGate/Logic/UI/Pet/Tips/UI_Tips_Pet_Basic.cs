using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;

namespace Logic
{
    public class UI_Tips_Pet_Basic 
    {
        private class PKAttr
        {
            private Transform transform;

            private Slider _slider;
            private Text _textValue;
            public void Init(Transform trans)
            {
                transform = trans;

                _slider = transform.Find("Title/Image").GetComponent<Slider>();
                _textValue = transform.Find("Title/Text").GetComponent<Text>();
            }

            public void SetData(long curValue, long maxValue)
            {
                _slider.value = curValue / (maxValue * 1f);
                _textValue.text = string.Format("{0}/{1}", curValue, maxValue);
            }
        }

        private class BasicAttr
        {
            private Transform transform;

            private Text _textName;
            private Text _textValue;

            public void Init(Transform trans)
            {
                transform = trans;

                _textName = transform.Find("Basis_Property01").GetComponent<Text>();
                _textValue = transform.Find("Number").GetComponent<Text>();
            }

            public void Show()
            {
                transform.gameObject.SetActive(true);
            }

            public void Hide()
            {
                transform.gameObject.SetActive(false);
            }

            public void SetData(AttributeRow row)
            {
                CSVAttr.Data data = CSVAttr.Instance.GetConfData(row.Id);
                if (data != null)
                {
                    _textName.text = LanguageHelper.GetTextContent(data.name);
                    _textValue.text = row.Value.ToString();
                }
            }
        }

        private Transform transform;

        private PKAttr _hpAttr;
        private PKAttr _mpAttr;

        private List<BasicAttr> listAttrs = new List<BasicAttr>(8);

        public void Init(Transform trans)
        {
            transform = trans;

            _hpAttr = new PKAttr();
            _hpAttr.Init(transform.Find("View_Basis_Prop0/Property0"));

            _mpAttr = new PKAttr();
            _mpAttr.Init(transform.Find("View_Basis_Prop0/Property1"));

            Transform tempTrans = transform.Find("View_Basis_Prop1");
            int count = tempTrans.childCount;
            for (int i = 0; i < count; ++i)
            {
                Transform propTrans = tempTrans.GetChild(i);

                BasicAttr left = new BasicAttr();
                left.Init(propTrans.Find("Basis_PropertyLeft"));

                BasicAttr right = new BasicAttr();
                right.Init(propTrans.Find("Basis_PropertyRight"));

                listAttrs.Add(left);
                listAttrs.Add(right);
            }
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void SetData(PetUnit pet)
        {
            _hpAttr.SetData(pet.PkAttr.CurHp, Sys_Pet.Instance.GetPetUnitPkAttrValue(pet, (int)EPkAttr.MaxHp));
            _mpAttr.SetData(pet.PkAttr.CurMp, Sys_Pet.Instance.GetPetUnitPkAttrValue(pet, (int)EPkAttr.MaxMp));

            List<AttributeRow> attrRows = new List<AttributeRow>();

            attrRows.Add(new AttributeRow() { Id = 19, Value = (int)Sys_Pet.Instance.GetPetUnitPkAttrValue(pet, (int)EPkAttr.Atk) });   //攻击
            attrRows.Add(new AttributeRow() { Id = 21, Value = (int)Sys_Pet.Instance.GetPetUnitPkAttrValue(pet, (int)EPkAttr.Def) });   //防御
            attrRows.Add(new AttributeRow() { Id = 23, Value = (int)Sys_Pet.Instance.GetPetUnitPkAttrValue(pet, (int)EPkAttr.Agi) });   //敏捷
            attrRows.Add(new AttributeRow() { Id = 27, Value = (int)Sys_Pet.Instance.GetPetUnitPkAttrValue(pet, (int)EPkAttr.Mnd) });   //精神
            attrRows.Add(new AttributeRow() { Id = 29, Value = (int)Sys_Pet.Instance.GetPetUnitPkAttrValue(pet, (int)EPkAttr.Rehp) });   //回复

            for (int i = 0; i < listAttrs.Count; ++i)
            {
                if (i < attrRows.Count)
                {
                    listAttrs[i].Show();
                    listAttrs[i].SetData(attrRows[i]);
                }
                else
                {
                    listAttrs[i].Hide();
                }
            }

            Lib.Core.FrameworkTool.ForceRebuildLayout(transform.gameObject);
        }
    }
}
