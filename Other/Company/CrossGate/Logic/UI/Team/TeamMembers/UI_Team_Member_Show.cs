using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Logic;
using System.Collections.Generic;
using System;
using Table;
using Framework;
using Packet;



public partial class UI_Team_Member_Layout
{
    public enum EMemeModelShow
    {
        Role,
        Partnet
    }
    public enum EMemeModelShowPos
    {
        Pos0 = 0,
        Pos1,
        Pos2,
        Pos3,
        Pos4
    }

    class ShowModelPositionItem
    {
        public VirtualGameObject VGO { get; set; } = null;

        public MemberModelShow ModelShow { get; private set; } = null;

        public void SetModelShow(MemberModelShow modelshow)
        {
            if (ModelShow == modelshow)
            {
                ModelShow.SetActive(true);
                return;
            }

            if (ModelShow != null)
                ModelShow.IsUsed = false;

            ModelShow = modelshow;

            if (ModelShow != null)
            {
                ModelShow.IsUsed = true;
                ModelShow.SetActive(true);
                ModelShow.SetParent(VGO);
            }
               

        }
    }
    private ShowSceneControl m_ShowSceneControl = null;

    private List<ShowModelPositionItem> m_ShowPosList = new List<ShowModelPositionItem>(5);

    private List<MemberModelShow> m_ModelShow = new List<MemberModelShow>(5);
    public void LoadShowScene()
    {
        GameObject scene = GameObject.Instantiate<GameObject>(mInfoAssetDependencies.mCustomDependencies[0] as GameObject);

        m_ShowSceneControl = new ShowSceneControl();

        scene.transform.SetParent(GameCenter.sceneShowRoot.transform);

        m_ShowSceneControl.Parse(scene);


        for (int i = 1; i < 6; i++)
        {
           Transform objTrans = m_ShowSceneControl.mRoot.transform.Find("Pos_" + i.ToString());

            VirtualGameObject vobj = new VirtualGameObject();
            vobj.SetGameObject(objTrans.gameObject,true);

            m_ShowPosList.Add(new ShowModelPositionItem() { VGO = vobj});

        }
       

        
    }
    public void UpdataShowScenePosPosition()
    {
      
        int count = m_ShowPosList.Count;
        for (int i = 0; i < count; i++)
        {
            Transform objTrans = m_ShowPosList[i].VGO.transform;

            Vector3 posshow = m_ShowSceneControl.mCamera.WorldToViewportPoint(objTrans.position);
  

            var itemposition = GetMemberItemPosition(i);
            Vector3 uiitem = UIManager.mUICamera.WorldToViewportPoint(itemposition);

            posshow.x = uiitem.x;

            Vector3 position = m_ShowSceneControl.mCamera.ViewportToWorldPoint(posshow);

            objTrans.position = position;
        }
    }

    public void UnLoadShowScene()
    {
        if (m_ShowSceneControl == null)
            return;

        m_ShowSceneControl.Dispose();

        m_ShowSceneControl = null;

        m_ShowPosList.Clear();

        int count = m_ModelShow.Count;
        for (int i = 0; i < count; i++)
        {
            m_ModelShow[i].Dispose();
            m_ModelShow[i] = null;
        }
        m_ModelShow.Clear();
    }

    public void RestShowScene()
    {
        int count = m_ShowPosList.Count;

        for (int i = 0; i < count; i++)
        {
            var value = m_ShowPosList[i];

            if (value.ModelShow != null)
            {
                value.ModelShow.IsUsed = false;

                value.SetModelShow (null);
            }
                

        }
    }

    public void UpdateShowScene()
    {
        int count = m_ModelShow.Count;

        for (int i = count - 1; i >= 0; i--)
        {
            var value = m_ModelShow[i];

            if (value != null && value.IsUsed == false)
            {
                value.Dispose();

                m_ModelShow.RemoveAt(i);
            }
        }
    }

    public void SetMemberModel(EMemeModelShow type, EMemeModelShowPos ePos, ulong uid, uint id,  uint caree = 0,uint WeaponID = 0, uint DressID = 0, Dictionary<uint, List<dressData>> DressValue = null)
    {
        MemberModelShow modelShow = null;

        modelShow = m_ModelShow.Find(o => o.UID == uid);

        if (modelShow != null)
        {
            //m_ModelShow.Remove(modelShow);
            //modelShow.Dispose();
            //modelShow = null;
            modelShow.ChangeWeapon(WeaponID);
        }
        
        if (modelShow == null)
        {
            modelShow = CreateModeShow(type, ePos, id, caree,WeaponID, DressID, DressValue);

            modelShow.UID = uid;

            m_ModelShow.Add(modelShow);

        }

        var value = GetModelPosition(ePos);

        if (value.ModelShow != null)
        {
            value.ModelShow.SetActive(false);
        }
        value.SetModelShow(modelShow);

    }

    private ShowModelPositionItem GetModelPosition(EMemeModelShowPos ePos)
    {
        ShowModelPositionItem posObj = null;

        posObj = m_ShowPosList[(int)ePos];

        return posObj;
    }

    private MemberModelShow CreateModeShow(EMemeModelShow type, EMemeModelShowPos ePos,uint id, uint caree = 0,uint WeaponID = 0, uint DressID = 0, Dictionary<uint, List<dressData>> DressValue = null)
    {
        MemberModelShow modeShow = null;

        switch (type)
        {
            case EMemeModelShow.Role:
                var mrms = new MemberRoleModelShow();
                mrms.WeaponID = WeaponID;
                mrms.DressID = DressID;
                modeShow = mrms;
                break;
            case EMemeModelShow.Partnet:
                modeShow = new MemberPartnerModelShow();
                break;
        }

        var parent = GetModelPosition(ePos);

        modeShow.Parent = parent.VGO;

        modeShow.LoadModel(id, caree,WeaponID, DressValue);

        return modeShow;
    }
}

public partial class UI_Team_Member_Layout
{
    public class MemberModelShow
    {
        public VirtualGameObject Parent { get; set; }

        public ulong UID { get; set; }

        public bool IsUsed { get; set; } = false;
        public virtual void LoadModel(uint id, uint caree = 0,uint weaponId = 0, Dictionary<uint, List<dressData>> DressValue = null)
        {
        }

        public virtual void SetActive(bool active)
        {

        }

        public virtual void ChangeWeapon(uint weaponId)
        {

        }
        public virtual void SetParent(VirtualGameObject parent)
        {
            Parent = parent;
        }
        public virtual void Dispose()
        {       
            Parent = null;    
        }
    }
    public class MemberRoleModelShow : MemberModelShow
    {
        public uint ModelID { get; set; }

        public ulong RoleID { get; set; }

        public uint DressID { get; set; } = 0;

        public uint WeaponID { get; set; }

        protected HeroLoader m_heroLoader;

        private void DisplayControlLoaded(int intValue)
        {
            if (m_heroLoader == null || m_heroLoader.heroDisplay.bMainPartFinished == false)
                return;

            CSVCharacter.Data cSVCharacterData = CSVCharacter.Instance.GetConfData(ModelID);

            if (cSVCharacterData == null)
                return;

            if (DressID == 0)
                return;

            uint id = (uint)(DressID * 10000 + ModelID);
            CSVFashionModel.Data cSVFashionModelData = CSVFashionModel.Instance.GetConfData(id);

            var activeid = /*cSVFashionModelData == null ? ():*/ cSVFashionModelData.action_show_id;


            m_heroLoader.heroDisplay?.mAnimation.UpdateHoldingAnimations(activeid, WeaponID == 0?cSVCharacterData.show_weapon_id : WeaponID,
         Constants.IdleAndRunAnimationClipHashSet);//


        }

        public override void LoadModel(uint heroID, uint occupation,uint weaponId, Dictionary<uint, List<dressData>> DressValue)
        {
            ModelID = heroID;

            m_heroLoader = HeroLoader.Create(true);

            // CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(occupation);
            //CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(cSVCareerData.weapon);

            m_heroLoader.LoadHero(heroID, weaponId, ELayerMask.ModelShow, DressValue, o =>
            {

                m_heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).SetParent(Parent, null);
            });


            m_heroLoader.heroDisplay.onLoaded += DisplayControlLoaded;
        }

        public override void ChangeWeapon(uint weaponId)
        {
            m_heroLoader.LoadWeaponPart(m_heroLoader.showParts[(int)EHeroModelParts.Weapon], weaponId);
        }

        public override void SetParent(VirtualGameObject parent)
        {
            if (Parent == parent)
                return;

            Parent = parent;

            if (m_heroLoader == null || m_heroLoader.heroDisplay == null)
                return;

            var value = m_heroLoader.heroDisplay.GetPart(EHeroModelParts.Main);

            if (value == null)
                return;

               value.SetParent(Parent, null);
        }

        public override void SetActive(bool active)
        {
            if (m_heroLoader == null || m_heroLoader.heroDisplay == null)
                return;

            var value = m_heroLoader.heroDisplay.GetPart(EHeroModelParts.Main);

            if (value == null)
                return;

            if (value.gameObject == null)
                return;

            if (value.gameObject.activeSelf != active)
            {
                value.gameObject.SetActive(active);

                m_heroLoader.heroDisplay?.mAnimation.Play((uint)EStateType.Idle);
            }


        }
        public override void Dispose()
        {
            base.Dispose();

            m_heroLoader?.Dispose();

            m_heroLoader = null;
   
        }
    }

    class MemberPartnerModelShow : MemberModelShow
    {
        private CSVPartner.Data mPartnerData;
        protected DisplayControl<EPetModelParts> m_DisplayControl;

        public override void LoadModel(uint id, uint caree = 0,uint weaponid =0, Dictionary<uint, List<dressData>> DressValue = null)
        {

            mPartnerData = CSVPartner.Instance.GetConfData(id);
            m_DisplayControl = DisplayControl<EPetModelParts>.Create((int)EHeroModelParts.Count);
            m_DisplayControl.eLayerMask = ELayerMask.ModelShow;
            m_DisplayControl.onLoaded = OnLoadend;

            m_DisplayControl.LoadMainModel(EPetModelParts.Main, mPartnerData.model_show, EPetModelParts.None, "Pos");

        }


        private void OnLoadend(int id)
        {
            if (m_DisplayControl == null || m_DisplayControl.bMainPartFinished == false)
                return;

            var value = m_DisplayControl.GetPart(id);

            value.gameObject.transform.SetParent(Parent.transform, false);

            m_DisplayControl.mAnimation.UpdateHoldingAnimations(mPartnerData.id + 100, mPartnerData.show_weapon_id);

        }

        public override void SetParent(VirtualGameObject parent)
        {
            if (Parent == parent)
                return;

            Parent = parent;

            if (m_DisplayControl == null || m_DisplayControl.bMainPartFinished == false)
                return;

            var value = m_DisplayControl.GetPart(EPetModelParts.Main);

            if (value != null)
                value.gameObject.transform.SetParent(Parent.transform, false);
        }

        public override void SetActive(bool active)
        {
            if (m_DisplayControl == null || m_DisplayControl.bMainPartFinished == false)
                return;

            var value = m_DisplayControl.GetPart(EPetModelParts.Main);

            if (value != null && value.gameObject.activeSelf != active)
            {
                value.gameObject.SetActive(active);
                m_DisplayControl?.mAnimation.Play((uint)EStateType.Idle);
            }

        }
        public override void Dispose()
        {
            base.Dispose();

            //m_DisplayControl?.Dispose();
            //m_DisplayControl = null;
            DisplayControl<EPetModelParts>.Destory(ref m_DisplayControl);
        }
    }
}