using System;
using System.Collections.Generic;

using Logic.Core;

using UnityEngine;
using UnityEngine.UI;

using Table;
namespace Logic
{
    public class ShowSceneAndRoleModleLoad
    {
        private ShowSceneControl mShowSceneControl;
        private DisplayControl<EPetModelParts> m_DisplayControl;

        private HeroLoader m_heroLoader;

       // private AdPrefabLoader m_ModelLoader = new AdPrefabLoader();

        public RawImage BindImage { get; set; }
        public uint ModelID { get; set; }

        private CSVPartner.Data mPartnerData;

        public Vector3 ShowScenePos { get; set; } = Vector3.zero;
        private ShowSceneControl LoadShowScene(GameObject orginObj,Vector3 pos = new Vector3())
        {
            ShowSceneControl secneControl = CreateShowScene(orginObj, pos);

            secneControl.mRoot.gameObject.transform.position = pos;

            return secneControl;
        }

        private ShowSceneControl CreateShowScene(GameObject origin, Vector3 pos)
        {
            GameObject scene = GameObject.Instantiate<GameObject>(origin);

            ShowSceneControl showSceneControl = new ShowSceneControl();

            scene.transform.position = pos;

            scene.transform.SetParent(GameCenter.sceneShowRoot.transform);

            showSceneControl.Parse(scene);

            return showSceneControl;
        }
        public void SetShowScene(GameObject orginObj)
        {
            ShowSceneControl secneControl = LoadShowScene(orginObj, ShowScenePos);

            DisposeMemberActor();

            Vector2 size = BindImage.rectTransform.sizeDelta;

            mShowSceneControl = secneControl;

            BindImage.texture = secneControl.GetTemporary((int)size.x * 2, (int)size.y * 2, 16, RenderTextureFormat.ARGB32, 1, false);

        }

        public void LoadPawnModel(uint heroID, uint occupation, Dictionary<uint, List<dressData>> DressValue)
        {
            DisposeActor();            

            ModelID = heroID;

            CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(occupation);
            CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(cSVCareerData.weapon);

            m_heroLoader = HeroLoader.Create(true);
            m_heroLoader.LoadHero(heroID, cSVCareerData.weapon, ELayerMask.ModelShow, DressValue, o =>
            {

                m_heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).SetParent(mShowSceneControl.mModelPos, null);
            });


            m_heroLoader.heroDisplay.onLoaded += DisplayControlLoaded;
        }

        public void DisplayControlLoaded(int intValue)
        {
            if (m_heroLoader == null || m_heroLoader.heroDisplay.bMainPartFinished == false)
                return;

            CSVCharacter.Data cSVCharacterData = CSVCharacter.Instance.GetConfData(ModelID);

            if (cSVCharacterData == null)
                return;

            uint id = Sys_Fashion.Instance.GetDressedId(EHeroModelParts.Main);
            id = (uint)(id * 10000 + ModelID);
            CSVFashionModel.Data cSVFashionModelData = CSVFashionModel.Instance.GetConfData(id);

            var activeid = cSVFashionModelData.action_show_id;


            m_heroLoader.heroDisplay?.mAnimation.UpdateHoldingAnimations(activeid, cSVCharacterData.show_weapon_id,
         Constants.IdleAndRunAnimationClipHashSet);//


        }

        public void LoadPawnModel(CSVPartner.Data data)
        {
            DisposeActor();

            mPartnerData = data;
            m_DisplayControl = DisplayControl<EPetModelParts>.Create((int)EHeroModelParts.Count);
            m_DisplayControl.eLayerMask = ELayerMask.ModelShow;
            m_DisplayControl.onLoaded = OnLoadend;

            m_DisplayControl.LoadMainModel(EPetModelParts.Main, data.model_show, EPetModelParts.None, "Pos");
        }


        private void OnLoadend(int id)
        {
            if (m_DisplayControl == null || m_DisplayControl.bMainPartFinished == false)
                return;


            var value = m_DisplayControl.GetPart(id);

            value.gameObject.transform.SetParent(mShowSceneControl.mModelPos.transform, false);


            m_DisplayControl.mAnimation.UpdateHoldingAnimations(mPartnerData.id + 100, mPartnerData.show_weapon_id);

        }

        public void DisposeMemberActor()
        {
            mShowSceneControl?.Dispose();

            m_heroLoader?.Dispose();

            //m_DisplayControl?.Dispose();
            //m_DisplayControl = null;
            DisplayControl<EPetModelParts>.Destory(ref m_DisplayControl);

            mShowSceneControl = null;

            m_heroLoader = null;

            //m_ModelLoader.OnDestory();

        }

        private void DisposeActor()
        {
            m_heroLoader?.Dispose();
            m_heroLoader = null;

            //m_DisplayControl?.Dispose();
            //m_DisplayControl = null;
            DisplayControl<EPetModelParts>.Destory(ref m_DisplayControl);            
        }
    }
}
