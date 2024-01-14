using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Rendering.Universal.ShaderGUI.LitGUI;

namespace UnityEditor.Rendering.ToneBasedShading.ShaderGUI
{
    public static class StencilGUI
    {
        public static class Styles
        {
            public static readonly GUIContent Title = EditorGUIUtility.TrTextContent("Stencil Options");

            public static readonly GUIContent StencilIDLabel = EditorGUIUtility.TrTextContent("Stencil ID");

            public static readonly GUIContent CompareFunctionLabel = EditorGUIUtility.TrTextContent("Compare Function");

            public static readonly GUIContent PassStencilOpLabel = EditorGUIUtility.TrTextContent("Pass Operation");

            public static readonly GUIContent FailStencilOpLabel = EditorGUIUtility.TrTextContent("Fail Operation");

            public static readonly GUIContent ZFailStencilOpLabel = EditorGUIUtility.TrTextContent("ZFail Operation");

            public static readonly GUIContent StencilWriteMaskLabel = EditorGUIUtility.TrTextContent("Stencil Write Mask");

            public static readonly GUIContent StencilReadMaskLabel = EditorGUIUtility.TrTextContent("Stencil Read Mask");
        }

        public struct StencilProperties
        {
            public MaterialProperty stencil;
            public MaterialProperty stencilComp;
            public MaterialProperty passStencilOp;
            public MaterialProperty failStencilOp;
            public MaterialProperty zFailStencilOp;
            public MaterialProperty stencilWriteMask;
            public MaterialProperty stencilReadMask;

            public StencilProperties(MaterialProperty[] properties)
            {
                stencil = BaseShaderGUI.FindProperty("_Stencil", properties, false);
                stencilComp = BaseShaderGUI.FindProperty("_StencilComp", properties, false);
                passStencilOp = BaseShaderGUI.FindProperty("_PassStencilOp", properties, false);
                failStencilOp = BaseShaderGUI.FindProperty("_FailStencilOp", properties, false);
                zFailStencilOp = BaseShaderGUI.FindProperty("_ZFailStencilOp", properties, false);
                stencilWriteMask = BaseShaderGUI.FindProperty("_StencilWriteMask", properties, false);
                stencilReadMask = BaseShaderGUI.FindProperty("_StencilReadMask", properties, false);
            }
        }

        public static void Inputs(StencilProperties properties, MaterialEditor materialEditor, Material material)
        {
            materialEditor.ShaderProperty(properties.stencil, Styles.StencilIDLabel);
            materialEditor.ShaderProperty(properties.stencilComp, Styles.CompareFunctionLabel);
            materialEditor.ShaderProperty(properties.passStencilOp, Styles.PassStencilOpLabel);
            materialEditor.ShaderProperty(properties.failStencilOp, Styles.FailStencilOpLabel);
            materialEditor.ShaderProperty(properties.zFailStencilOp, Styles.ZFailStencilOpLabel);
            materialEditor.ShaderProperty(properties.stencilWriteMask, Styles.StencilWriteMaskLabel);
            materialEditor.ShaderProperty(properties.stencilReadMask, Styles.StencilReadMaskLabel);
        }
    }
}