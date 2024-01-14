//Auto Generate Code
using Logic.Core;
using System.Collections.Generic;
using Table;
using Lib.AssetLoader;
using System.IO;
using UnityEngine;
using Lib.Core;

namespace Logic
{
	public static class CSVRegister
	{		
        //public static bool isFinished { get { return _csvCurrentLoading >= _csvCount; } }
        //public static float Progress { get { return _csvCurrentLoading / (float)_csvCount; } }
		//private static List<ITable> _loaders;
		//private static int _csvCount = 0;
		//private static int _csvCurrentLoading = -1;

        public static bool isFinished { get; private set; }
        public static float Progress { get; private set; }
		public static void Load()
		{
			Progress = 0f;

			CSVWordStyle.Load();
			Progress = 0.04f;
			CSVLanguage.Load();
			Progress = 0.08f;
			CSVAtlas.Load();
			Progress = 0.12f;
			CSVTaskType.Load();
			Progress = 0.16f;
			CSVTaskTarget.Load();
			Progress = 0.2f;
			CSVTaskMainType.Load();
			Progress = 0.24f;
			CSVTaskMainTarget.Load();
			Progress = 0.28f;
			CSVTaskMain.Load();
			Progress = 0.32f;
			CSVTask.Load();
			Progress = 0.36f;
			CSVSysTips.Load();
			Progress = 0.4f;
			CSVSkillAction.Load();
			Progress = 0.44f;
			CSVLanguageTask.Load();
			Progress = 0.48f;
			CSVLanguageMainTask.Load();
			Progress = 0.52f;
			CSVItemFun.Load();
			Progress = 0.56f;
			CSVItem.Load();
			Progress = 0.6f;
			CSVIcon.Load();	//depends: CSVAtlas;
			Progress = 0.64f;
			CSVCharacter.Load();
			Progress = 0.68f;
			CSVCareerModel.Load();
			Progress = 0.72f;
			CSVCareerAttr.Load();
			Progress = 0.76f;
			CSVCareer.Load();
			Progress = 0.8f;
			CSVBaseAction.Load();
			Progress = 0.84f;
			CSVBagType.Load();
			Progress = 0.88f;
			CSVBag.Load();
			Progress = 0.92f;
			CSVAudio.Load();
			Progress = 0.96f;
			CSVActionState.Load();
			Progress = 1f;

			isFinished = true;
		}
		public static void Unload()
		{
			Progress = 0f;
			isFinished = false;
			CSVWordStyle.Unload();
			CSVLanguage.Unload();
			CSVAtlas.Unload();
			CSVTaskType.Unload();
			CSVTaskTarget.Unload();
			CSVTaskMainType.Unload();
			CSVTaskMainTarget.Unload();
			CSVTaskMain.Unload();
			CSVTask.Unload();
			CSVSysTips.Unload();
			CSVSkillAction.Unload();
			CSVLanguageTask.Unload();
			CSVLanguageMainTask.Unload();
			CSVItemFun.Unload();
			CSVItem.Unload();
			CSVIcon.Unload();
			CSVCharacter.Unload();
			CSVCareerModel.Unload();
			CSVCareerAttr.Unload();
			CSVCareer.Unload();
			CSVBaseAction.Unload();
			CSVBagType.Unload();
			CSVBag.Unload();
			CSVAudio.Unload();
			CSVActionState.Unload();

		}
		public static List<string> GetAllFiles()
		{
			List<string> paths = new List<string>(25);
			paths.Add("Config/Table/CSVWordStyle.bytes");
			paths.Add("Config/Table/CSVLanguage.bytes");
			paths.Add("Config/Table/CSVAtlas.bytes");
			paths.Add("Config/Table/CSVTaskType.bytes");
			paths.Add("Config/Table/CSVTaskTarget.bytes");
			paths.Add("Config/Table/CSVTaskMainType.bytes");
			paths.Add("Config/Table/CSVTaskMainTarget.bytes");
			paths.Add("Config/Table/CSVTaskMain.bytes");
			paths.Add("Config/Table/CSVTask.bytes");
			paths.Add("Config/Table/CSVSysTips.bytes");
			paths.Add("Config/Table/CSVSkillAction.bytes");
			paths.Add("Config/Table/CSVLanguageTask.bytes");
			paths.Add("Config/Table/CSVLanguageMainTask.bytes");
			paths.Add("Config/Table/CSVItemFun.bytes");
			paths.Add("Config/Table/CSVItem.bytes");
			paths.Add("Config/Table/CSVIcon.bytes");
			paths.Add("Config/Table/CSVCharacter.bytes");
			paths.Add("Config/Table/CSVCareerModel.bytes");
			paths.Add("Config/Table/CSVCareerAttr.bytes");
			paths.Add("Config/Table/CSVCareer.bytes");
			paths.Add("Config/Table/CSVBaseAction.bytes");
			paths.Add("Config/Table/CSVBagType.bytes");
			paths.Add("Config/Table/CSVBag.bytes");
			paths.Add("Config/Table/CSVAudio.bytes");
			paths.Add("Config/Table/CSVActionState.bytes");
			return paths;
		}
	}
}
