using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Framework
{
	public class FaceModifyData
	{
		public bool hasBoneChange;
		public bool hasTextureChange;
		public bool hasEyeChange;

		public float[] mBoneModifyValues;

		public DetailModifyValue[] mTextureModifyValues;
		public string[] mTextureAssetPaths;

		public string sEyeMaterialAssetPath;

		public FaceModifyData(int count)
        {
			mBoneModifyValues = new float[count];
		}
	}
}