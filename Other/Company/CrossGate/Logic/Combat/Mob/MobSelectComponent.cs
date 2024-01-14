using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lib.AssetLoader;
using Logic;
using Lib.Core;

public class MobSelectComponent : BaseComponent<MobEntity>
{
    private GameObject m_CanSelectGo;
    private GameObject m_SelectedGo;

    private GameObject m_Mob;

    private ulong _modelIdCanSelect;
    private ulong _modelIdSelected;

    public void Init(GameObject mob)
    {
        m_Mob = mob;

        _modelIdCanSelect = CombatModelManager.Instance.CreateModel(GlobalAssets.sPrefab_Fx_suoding_1, mob.transform, delegate (GameObject selectGo, ulong modelId)
        {
            m_CanSelectGo = selectGo;
            m_CanSelectGo.transform.localPosition = new Vector3(0, 0.033f, 0);
            m_CanSelectGo.SetActive(false);
            foreach (Transform tran in m_CanSelectGo.GetComponentInChildren<Transform>())
            {
                tran.gameObject.layer = LayerMaskUtil.MaskToLayer(ELayerMask.TransparentFX); //LayerMask.NameToLayer(ELayerMask.TransparentFX.ToString());
            }
        });

        _modelIdSelected= CombatModelManager.Instance.CreateModel(GlobalAssets.sPrefab_Fx_suoding_2, mob.transform, delegate (GameObject selectGo, ulong modelId)
        {
            m_SelectedGo = selectGo;
            m_SelectedGo.SetActive(false);
            foreach (Transform tran in m_SelectedGo.GetComponentInChildren<Transform>())
            {
                tran.gameObject.layer = LayerMaskUtil.MaskToLayer(ELayerMask.TransparentFX); //LayerMask.NameToLayer(ELayerMask.TransparentFX.ToString());
            }
        });
    }

    public void ShowCanSelect(bool show)
    {
        if (m_CanSelectGo != null)
        {
            m_CanSelectGo.SetActive(show);
        }
        else
        {
            DebugUtil.LogFormat(ELogType.eBattleCommand,"光圈null");
        }
    }

    public void ShowSelected()
    {
        if (m_SelectedGo != null)
        {
            if (m_SelectedGo.activeInHierarchy)
            {
                m_SelectedGo.SetActive(false);
            }
            m_SelectedGo.SetActive(true);
        }
        else
        {
            Debug.LogError("选中 null");
        }
    }

    public override void Dispose()
    {
        CombatModelManager.Instance.FreeModel(_modelIdCanSelect, CombatManager.Instance.m_WorkStreamTrans);
        CombatModelManager.Instance.FreeModel(_modelIdSelected, CombatManager.Instance.m_WorkStreamTrans);

        m_CanSelectGo = null;
        m_SelectedGo = null;
        m_Mob = null;

        _modelIdCanSelect = 0ul;
        _modelIdSelected = 0ul;

        base.Dispose();
    }
}
