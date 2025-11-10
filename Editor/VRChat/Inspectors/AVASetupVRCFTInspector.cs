#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using System.Linq;
using com.squirrelbite.ava_base_setup.util;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.squirrelbite.ava_base_setup.vrchat
{

	[CustomEditor(typeof(AVASetupVRCFTProducer))]
	public class AVASetupVRCFTInspector : Editor
	{
		static readonly Color SpacerColor = new(0.17f, 0.17f, 0.17f);

		private int FTMatch = -1;

		void OnEnable()
		{
			var c = (AVASetupVRCFTProducer)target;
			SkinnedMeshRenderer ftMesh = c.FTMesh;
			if(!ftMesh)
				ftMesh = FTTypeMatcher.DetectFaceMesh(AnimationPathUtil.GetRoot(c.transform).gameObject);
			if(ftMesh)
				FTMatch = FTTypeMatcher.Match(ftMesh);
		}

		public override VisualElement CreateInspectorGUI()
		{
			var c = (AVASetupVRCFTProducer)target;
			VisualElement ui = new();

			Toolkit.AddElement(ui, FTMatch >= 0 ? new HelpBox("Detected Face Fracking Setup: " + ((FT_Type)FTMatch).ToString(), HelpBoxMessageType.Info) : new HelpBox("Avatar doesn't support known face tracking method!", HelpBoxMessageType.Warning));

			Toolkit.AddElement(ui, new PropertyField(serializedObject.FindProperty("FTMesh"), "Face Tracking MeshRenderer"));
			Toolkit.AddElement(ui, new PropertyField(serializedObject.FindProperty("FTType"), "Face Tracking Setup"));

			return ui;
		}
	}
}

#endif
#endif
