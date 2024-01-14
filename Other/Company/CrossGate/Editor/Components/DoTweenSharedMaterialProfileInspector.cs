using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.DOTweenEditor.Core;
using DG.DOTweenEditor.UI;
using DG.Tweening;
using UnityEditor;
using UnityEngine.Rendering;

namespace DG.DOTweenEditor
{
    [CustomEditor(typeof(DoTweenSharedMaterialProfile))]
    public class DoTweenSharedMaterialProfileInspector : Editor
    {
        private DoTweenSharedMaterialProfile _profile;

        private GUIContent colorGUIContent = new GUIContent("Color");
        private GUIContent vectorGUIContent = new GUIContent("Vector");
        private GUIContent floatGUIContent = new GUIContent("Number");
        private GUIContent gradientGUIContent = new GUIContent("Gradient", "Gradient Color");

        private GUIContent notSupportGUIContent = new GUIContent("Not Supported");
        private GUIContent materialGUIContent = new GUIContent("material");
        private GUIContent attributeGUIContent = new GUIContent("attribute");
        private GUIContent useGradientColorGUIContent = new GUIContent("useGradientColor");

        private int currentIndex = -1;
        private int propertyCount = 0;
        private string[] mAttributes;
        private Vector2 rangeLimits = new Vector2(0, 1);
        private ShaderPropertyFlags shaderPropertyFlags;

        void OnEnable()
        {
            _profile = target as DoTweenSharedMaterialProfile;
            RefreshMaterial();
        }

        private void OnDisable()
        {
            currentIndex = -1;
            propertyCount = 0;
            mAttributes = null;
            rangeLimits = new Vector2(0, 1);

            _profile = null;            
        }

        public override void OnInspectorGUI()
        {            
            if (!_profile)
                return;

            EditorGUI.BeginChangeCheck();
            _profile.mMaterial = (Material)EditorGUILayout.ObjectField(materialGUIContent, _profile.mMaterial, typeof(Material), true);
            if (EditorGUI.EndChangeCheck())
            {
                RefreshMaterial();
                EditorUtility.SetDirty(target);
            }

            if (!_profile.mMaterial)
                return;

            EditorGUI.BeginChangeCheck();

            int newIndex = EditorGUILayout.Popup(attributeGUIContent, currentIndex, mAttributes);
            if (currentIndex != newIndex)
            {
                currentIndex = newIndex;
                RefreshProperty();
            }

            switch (_profile.eShaderPropertyType)
            {
                case UnityEngine.Rendering.ShaderPropertyType.Color:
                    OnInspectorGUI_Color();
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Vector:
                    OnInspectorGUI_Vector();
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Float:
                    OnInspectorGUI_Float();
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Range:
                    OnInspectorGUI_Range();
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Texture:
                    OnInspectorGUI_Texture();
                    break;
                default:
                    break;
            }

            OnProfileInspectorGUI(_profile);

            if (EditorGUI.EndChangeCheck())
            {                
                EditorUtility.SetDirty(target);
            }
        }

        private void OnInspectorGUI_Color()
        {
            bool hdr = shaderPropertyFlags.HasFlag(ShaderPropertyFlags.HDR);
            _profile.useGradientColor = EditorGUILayout.Toggle(useGradientColorGUIContent, _profile.useGradientColor);
            if (_profile.useGradientColor)
            {
                _profile.gradientColor = EditorGUILayout.GradientField(gradientGUIContent, _profile.gradientColor, hdr);
            }
            else
            {
                Color color = new Color(_profile.endValueVector.x, _profile.endValueVector.y, _profile.endValueVector.z, _profile.endValueVector.w);
                color = EditorGUILayout.ColorField(colorGUIContent, color, true, true, hdr);
                _profile.endValueVector = new Vector4(color.r, color.g, color.b, color.a);
            }
        }

        private void OnInspectorGUI_Vector()
        {
            _profile.endValueVector = EditorGUILayout.Vector4Field(vectorGUIContent, _profile.endValueVector);
        }

        private void OnInspectorGUI_Float()
        {
            _profile.endValueVector.x = EditorGUILayout.FloatField(floatGUIContent, _profile.endValueVector.x);
        }

        private void OnInspectorGUI_Range()
        {
            _profile.endValueVector.x = EditorGUILayout.Slider(floatGUIContent, _profile.endValueVector.x, rangeLimits.x, rangeLimits.y);
        }

        private void OnInspectorGUI_Texture()
        {
            EditorGUILayout.LabelField(notSupportGUIContent);
        }

        public static void OnProfileInspectorGUI(DoTweenSharedMaterialProfile profile)
        {
            GUILayout.BeginHorizontal();
            profile.duration = EditorGUILayout.FloatField("Duration", profile.duration);
            if (profile.duration < 0) profile.duration = 0;
            GUILayout.EndHorizontal();
            profile.delay = EditorGUILayout.FloatField("Delay", profile.delay);
            if (profile.delay < 0) profile.delay = 0;
            profile.isIndependentUpdate = EditorGUILayout.Toggle("Ignore TimeScale", profile.isIndependentUpdate);
            profile.easeType = EditorGUIUtils.FilteredEasePopup("Ease", profile.easeType);
            if (profile.easeType == Ease.INTERNAL_Custom)
            {
                profile.easeCurve = EditorGUILayout.CurveField("   Ease Curve", profile.easeCurve);
            }
            profile.loops = EditorGUILayout.IntField(new GUIContent("Loops", "Set to -1 for infinite loops"), profile.loops);
            if (profile.loops < -1) profile.loops = -1;
            if (profile.loops > 1 || profile.loops == -1)
                profile.loopType = (LoopType)EditorGUILayout.EnumPopup("   Loop Type", profile.loopType);
        }

        private void RefreshProperty()
        {
            if (currentIndex >= 0 && currentIndex < mAttributes.Length)
            {
                _profile.sPropertyName = mAttributes[currentIndex];
                _profile.eShaderPropertyType = _profile.mMaterial.shader.GetPropertyType(currentIndex);

                shaderPropertyFlags = _profile.mMaterial.shader.GetPropertyFlags(currentIndex);
                if (_profile.eShaderPropertyType == UnityEngine.Rendering.ShaderPropertyType.Range)
                {
                    rangeLimits = _profile.mMaterial.shader.GetPropertyRangeLimits(currentIndex);
                }
            }
            else
            {
                _profile.sPropertyName = string.Empty;
                shaderPropertyFlags = ShaderPropertyFlags.None;
            }
        }

        private void RefreshMaterial()
        {
            if (_profile && _profile.mMaterial)
            {
                currentIndex = -1;
                Shader shader = _profile.mMaterial.shader;
                propertyCount = shader.GetPropertyCount();

                mAttributes = new string[propertyCount];

                for (int i = 0; i < propertyCount; ++i)
                {
                    mAttributes[i] = shader.GetPropertyName(i);
                    if (string.Equals(_profile.sPropertyName, mAttributes[i]))
                    {
                        currentIndex = i;
                    }
                }

                RefreshProperty();
            }
        }
    }
}