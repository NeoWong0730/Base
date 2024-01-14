using UnityEngine;
using UnityEngine.Rendering;

[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.WeaponModelShaderDissolve)]
public class WS_UIModelShow_WeaponModelShaderDissolve_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_UIModelShowManagerEntity uiModelShowManager = ((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity as WS_UIModelShowManagerEntity;

        if (uiModelShowManager.m_WeaponGo != null)
        {
            if (uiModelShowManager.SetMeshRendererMatArray(uiModelShowManager.m_WeaponGo))
            {
                uiModelShowManager.GetOtherWeapon(out GameObject weapon02Go, out Transform weapon02ParentTrans);
                if (weapon02Go != null)
                    uiModelShowManager.SetMeshRendererMatArray(weapon02Go, true);
            }
        }
        
        if (uiModelShowManager.m_ShaderControllerMaterialInfoList != null)
        {
            float[] ps = CombatHelp.GetStrParseFloat1Array(str);
            bool isOpenDissolve = ps[0] == 1;
            for (int i = 0, count = uiModelShowManager.m_ShaderControllerMaterialInfoList.Count; i < count; i++)
            {
                ShaderControllerMaterialInfo shaderControllerMaterialInfo = uiModelShowManager.m_ShaderControllerMaterialInfoList[i];
                if (shaderControllerMaterialInfo == null || shaderControllerMaterialInfo.Mat == null)
                    continue;

                RenderQueue renderQueue;
                if (shaderControllerMaterialInfo.OriginRenderQueue > (int)RenderQueue.GeometryLast)
                {
                    renderQueue = 0;
                }
                else
                {
                    renderQueue = isOpenDissolve ? RenderQueue.Transparent : ((RenderQueue)shaderControllerMaterialInfo.OriginRenderQueue);
                }

                ShaderController.Instance.ShowDissolveBloom(shaderControllerMaterialInfo.Mat, isOpenDissolve, ps[1], ps[2], ps[3],
                    renderQueue, shaderControllerMaterialInfo.OriginOutlineState);
            }
        }

        m_CurUseEntity.TranstionMultiStates(this);
	}
}