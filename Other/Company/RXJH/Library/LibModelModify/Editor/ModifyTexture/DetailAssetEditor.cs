using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DetailAsset))]
public class DetailAssetEditor : Editor
{
    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        DetailAsset detailAsset = target as DetailAsset;
        base.OnPreviewGUI(r, background);
        GUI.DrawTexture(r, detailAsset.mMaterial.mainTexture);
    }
}
