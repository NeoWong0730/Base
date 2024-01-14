using Framework;
using Lib.Core;
using System.IO;
using UnityEngine;
public static class AnimationClipProload
{
	public static void Preload(AssetsPreload assetsPreload)
	{
		Stream stream = Lib.AssetLoader.AssetMananger.Instance.LoadStream("Config/AniClipProload.txt");
		StreamReader streamReader = new StreamReader(stream);
		
		string line;
        while (!string.IsNullOrWhiteSpace(line = streamReader.ReadLine()))
        {
			assetsPreload.Preload<AnimationClip>(line);
            DebugUtil.Log(ELogType.eAssets, line);
        }

        stream.Close();
	}
}
