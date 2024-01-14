using Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public abstract class ModelShow
    {
        public VirtualGameObject Parent { get; set; }

        public ulong UID { get; set; }

        public bool IsUsed { get; set; } = false;
        public virtual void LoadModel(uint id, uint caree = 0, Dictionary<uint, List<dressData>> DressValue = null)
        {
        }

        public virtual void LoadModel()
        {
        }
        public virtual void SetActive(bool active)
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

    class ShowModelPositionItem
    {
        public VirtualGameObject VGO { get; set; } = null;

        public ModelShow ModelShow { get; private set; } = null;

        public void SetModelShow(ModelShow modelshow)
        {
            if (ModelShow == modelshow)
                return;

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
    public class CutSceneModelShowController
    {
        public AssetDependencies mInfoAssetDependencies { get; set; }

        public ShowSceneControl m_ShowSceneControl { get; private set; }

   

        private List<ShowModelPositionItem> m_ShowPosList = new List<ShowModelPositionItem>(5);

        private List<ModelShow> m_ModelShow = new List<ModelShow>(5);
        public void LoadShowScene()
        {
            GameObject scene = GameObject.Instantiate<GameObject>(mInfoAssetDependencies.mCustomDependencies[0] as GameObject);

            m_ShowSceneControl = new ShowSceneControl();

            scene.transform.SetParent(GameCenter.sceneShowRoot.transform);

            m_ShowSceneControl.Parse(scene);

            m_ShowPosList.Add(new ShowModelPositionItem() { VGO = m_ShowSceneControl.mModelPos });

            for (int i = 1; i < 6; i++)
            {
                Transform objTrans = m_ShowSceneControl.mRoot.transform.Find("Pos_" + i.ToString());

                if (objTrans != null)
                {
                    VirtualGameObject vobj = new VirtualGameObject();
                    vobj.SetGameObject(objTrans.gameObject, true);

                    m_ShowPosList.Add(new ShowModelPositionItem() { VGO = vobj });
                }

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
            }
            m_ModelShow.Clear();
        }

        private ShowModelPositionItem GetShowModerPostion(int posindex = 0)
        {

            return m_ShowPosList[posindex];
        }

        public void AddModelShow(ModelShow modelShow,int posindex = 0)
        {

            var parent = GetShowModerPostion(posindex);

            modelShow.Parent = parent.VGO;

            modelShow.LoadModel();

            m_ModelShow.Add(modelShow);

            parent.SetModelShow(modelShow);

        }

    }
}
