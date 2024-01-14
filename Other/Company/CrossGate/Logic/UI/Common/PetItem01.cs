using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using UnityEngine.EventSystems;

namespace Logic
{
    public class PetItem01
    {
        public Transform transform;

        public Button BtnBg;
        public Image ImgQuality;

        public Transform TransPet;
        public Image ImgIcon;
        public Image ImgFight;
        public Image ImgLevel;
        public Text TextLevel;
        public Text TextBound;
        public Image ImgLock;

        public Transform TransNone;
        public Image ImgNoneLock;

        public Image ImgChange;
        //public Image ImgChange1;
        //public Image ImgChange2;

        public Image ImgAdd;

        public void Bind(Transform trans)
        {
            transform = trans;

            BtnBg = transform.Find("ItemBg").GetComponent<Button>();
            //ImgQuality = transform.Find("Item_Quality").GetComponent<Image>();

            TransPet = transform.Find("Pet01");
            ImgQuality = TransPet.Find("Item_Quality").GetComponent<Image>();
            ImgIcon = TransPet.Find("Image_Icon").GetComponent<Image>();
            ImgFight = TransPet.Find("Imag_Fight").GetComponent<Image>();
            ImgLevel = TransPet.Find("Image_Level").GetComponent<Image>();
            TextLevel = TransPet.Find("Image_Level/Text_Level/Text_Num").GetComponent<Text>();
            TextBound = TransPet.Find("Text_Bound").GetComponent<Text>();
            ImgLock = TransPet.Find("Image_Lock").GetComponent<Image>();

            TransNone = transform.Find("PetNone");
            ImgNoneLock = TransNone.Find("Image_Lock").GetComponent<Image>();

            ImgChange = transform.Find("Image_Change").GetComponent<Image>();

            ImgAdd = transform.Find("Image_Add").GetComponent<Image>();
        }

        public void SetActive(bool active)
        {
            transform.gameObject.SetActive(active);
        }

        public void SetData(CSVPetNew.Data data)
        {
            ImageHelper.SetIcon(ImgIcon, data.icon_id);
        }

        public void SetData(ClientPet client)
        {
            if(null != client)
            {
                SetData(client.petData);
                TextLevel.text = client.petUnit.SimpleInfo.Level.ToString();
            }
        }
    }
}

