#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using com.squirrelbite.ava_base_setup.util;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	[CustomEditor(typeof(AVAFaceTrackingProducerVRC))]
	public class AVAFaceTrackingProducerVRCInspector : Editor
	{
		static readonly Color SpacerColor = new(0.17f, 0.17f, 0.17f);

		private int FTMatch = -1;

		void OnEnable()
		{
			var c = (AVAFaceTrackingProducerVRC)target;
			FTMatch = AVAVRCUtil.MatchFTType(c.gameObject);
		}

		public override VisualElement CreateInspectorGUI()
		{
			var c = (AVAFaceTrackingProducerVRC)target;
			VisualElement ui = new();

			Toolkit.AddElement(ui, FTMatch >= 0 ? new HelpBox("Detected Face Fracking Setup: " + ((FT_Type)FTMatch).ToString(), HelpBoxMessageType.Info) : new HelpBox("Avatar doesn't support known face tracking method!", HelpBoxMessageType.Warning));

			var p_setup = Toolkit.AddElement(ui, new PropertyField(serializedObject.FindProperty("FTSetup"), "Face Tracking Setup"));
			var p_type = Toolkit.AddElement(ui, new PropertyField(serializedObject.FindProperty("FTType"), "Face Tracking Type"));
			var p_mesh = Toolkit.AddElement(ui, new PropertyField(serializedObject.FindProperty("FTMesh"), "Face Tracking MeshRenderer"));

			void handle() {
				if(c.FTSetup == FT_Setup.Automatic)
					p_type.style.display = p_mesh.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
				else
					p_type.style.display = p_mesh.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
			};

			p_setup.RegisterValueChangeCallback(e => {
				handle();
			});
			handle();

			Toolkit.AddElement(ui, new PropertyField(serializedObject.FindProperty("RemoveEyetrackingDrivers"), "Remove Eyetracking Drivers"));

			return ui;
		}
	}
}

#endif
#endif
