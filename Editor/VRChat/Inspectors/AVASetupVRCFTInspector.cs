#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

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
			FTMatch = AVAVRCUtil.MatchFTType(c.gameObject);
		}

		public override VisualElement CreateInspectorGUI()
		{
			var c = (AVASetupVRCFTProducer)target;
			VisualElement ui = new();

			Toolkit.AddElement(ui, FTMatch >= 0 ? new HelpBox("Detected Face Fracking Setup: " + ((FT_Type)FTMatch).ToString(), HelpBoxMessageType.Info) : new HelpBox("Avatar doesn't support known face tracking method!", HelpBoxMessageType.Warning));

			Toolkit.AddElement(ui, new PropertyField(serializedObject.FindProperty("FTMesh"), "Face Tracking MeshRenderer"));
			Toolkit.AddElement(ui, new PropertyField(serializedObject.FindProperty("FTType"), "Face Tracking Setup"));
			Toolkit.AddElement(ui, new PropertyField(serializedObject.FindProperty("RemoveEyetrackingDrivers"), "Remove Eyetracking Drivers"));

			return ui;
		}
	}
}

#endif
#endif
