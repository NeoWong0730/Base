using Lib.Core;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using System;

namespace Logic
{
    public class UI_Compete_RoleInfo : UIComponent
    {
        private Image imgRole;
        private Text textName;
        private Text textNum;        

        protected override void Loaded()
        {
            imgRole = transform.Find("Image_Blank/Character").GetComponent<Image>();
            textName = transform.Find("Text_Name").GetComponent<Text>();
            textNum = transform.Find("Image/Text_Number").GetComponent<Text>();
        }

        public void UpdateRole(ulong roleId, uint teamNum)
        {
            uint fashionIconId = 0;
            string roleName = "";
            if (roleId == Sys_Role.Instance.Role.RoleId)
            {
                roleName = Sys_Role.Instance.Role.Name.ToStringUtf8();
                fashionIconId = Sys_Fashion.Instance.GetClothId(GameCenter.mainHero?.heroBaseComponent?.fashData);
                fashionIconId = fashionIconId * 10000 + Sys_Role.Instance.Role.HeroId;
            }
            else
            {
                //Hero hero =  GameCenter.mainWorld.GetActor(Hero.Type, roleId) as Hero;
                Hero hero = GameCenter.GetSceneHero(roleId);
                if (hero != null)
                {
                    roleName = hero.heroBaseComponent.Name;
                    fashionIconId = Sys_Fashion.Instance.GetClothId(hero.heroBaseComponent?.fashData);
                    fashionIconId = fashionIconId * 10000 + hero.heroBaseComponent.HeroID;
                }
            }

            CSVFashionIcon.Data cSVFashionIconData = CSVFashionIcon.Instance.GetConfData(fashionIconId);
            if (cSVFashionIconData != null)
            {
                ImageHelper.SetIcon(imgRole, null, cSVFashionIconData.Icon_Path, true);
                imgRole.transform.localScale = new Vector3(cSVFashionIconData.CompeteIcon_scale, cSVFashionIconData.CompeteIcon_scale, 1);
                (imgRole.transform as RectTransform).anchoredPosition = new Vector2(cSVFashionIconData.CompeteIcon_pos[0], cSVFashionIconData.CompeteIcon_pos[1]);
            }

            if (teamNum == 0)
            {
                textNum.text = "";
                textName.text = roleName;
            }
            else
            {
                textNum.text = string.Format("{0}/5", teamNum.ToString());
                textName.text = LanguageHelper.GetTextContent(4000007, roleName);
            }
            
        }
    }
}
