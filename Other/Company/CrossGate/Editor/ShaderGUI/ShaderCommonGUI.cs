using System;
using UnityEditor;
using UnityEngine;

public class ShaderCommonGUI : BaseShaderGUI
{
    protected MaterialProperty stencilProp { get; set; }
    protected MaterialProperty stencilCompProp { get; set; }
    protected MaterialProperty passStencilOpProp { get; set; }
    protected MaterialProperty failStencilCompProp { get; set; }
    protected MaterialProperty zFailStencilOpProp { get; set; }
    protected MaterialProperty stencilWriteMaskProp { get; set; }
    protected MaterialProperty stencilReadMaskProp { get; set; }
    protected MaterialProperty colorMaskProp { get; set; }
    protected MaterialProperty zWriteProp { get; set; }
    protected MaterialProperty zTestProp { get; set; }

    private MaterialProperty[] allProperties;

    public override void MaterialChanged(Material material)
    {
        if (material == null)
            throw new ArgumentNullException("material");

        SetMaterialKeywords(material);
    }

    public override void FindProperties(MaterialProperty[] properties)
    {
        base.FindProperties(properties);

        stencilProp = FindProperty("_Stencil", properties);
        stencilCompProp = FindProperty("_StencilComp", properties);
        passStencilOpProp = FindProperty("_PassStencilOp", properties, false);
        if(passStencilOpProp == null)
        {
            passStencilOpProp = FindProperty("_StencilOp", properties);
        }
        failStencilCompProp = FindProperty("_FailStencilComp", properties);
        zFailStencilOpProp = FindProperty("_ZFailStencilOp", properties);
        stencilWriteMaskProp = FindProperty("_StencilWriteMask", properties);
        stencilReadMaskProp = FindProperty("_StencilReadMask", properties);
        colorMaskProp = FindProperty("_ColorMask", properties, false);

        zWriteProp = FindProperty("_ZWrite", properties, false);
        zTestProp = FindProperty("_ZTest", properties, false);

        queueOffsetProp = FindProperty("_QueueOffset", properties, false);
    }

    public override void OnGUI(MaterialEditor materialEditorIn, MaterialProperty[] properties)
    {
        allProperties = properties;
        base.OnGUI(materialEditorIn, properties);
        allProperties = null;
        //materialEditorIn.PropertiesDefaultGUI(properties);
    }

    public override void DrawSurfaceInputs(Material material)
    {
        materialEditor.PropertiesDefaultGUI(allProperties);
    }

    public override void DrawAdvancedOptions(Material material)
    {
        base.DrawAdvancedOptions(material);
        EditorGUILayout.Space();
        materialEditor.ShaderProperty(stencilProp, stencilProp.displayName);
        materialEditor.ShaderProperty(stencilCompProp, stencilCompProp.displayName);
        materialEditor.ShaderProperty(passStencilOpProp, passStencilOpProp.displayName);
        materialEditor.ShaderProperty(failStencilCompProp, failStencilCompProp.displayName);
        materialEditor.ShaderProperty(zFailStencilOpProp, zFailStencilOpProp.displayName);
        materialEditor.ShaderProperty(stencilWriteMaskProp, stencilWriteMaskProp.displayName);
        materialEditor.ShaderProperty(stencilReadMaskProp, stencilReadMaskProp.displayName);
        //materialEditor.ShaderProperty(colorMaskProp, colorMaskProp.displayName);

        UnityEngine.Rendering.ColorWriteMask colorWriteMask = (UnityEngine.Rendering.ColorWriteMask)colorMaskProp.floatValue;
        colorWriteMask = (UnityEngine.Rendering.ColorWriteMask)EditorGUILayout.EnumFlagsField(colorMaskProp.displayName, colorWriteMask);
        colorMaskProp.floatValue = (float)colorWriteMask;

        if (zWriteProp != null)
        {
            materialEditor.ShaderProperty(zWriteProp, zWriteProp.displayName);
        }

        if (zTestProp != null)
        {
            materialEditor.ShaderProperty(zTestProp, zTestProp.displayName);
        }
    }
}