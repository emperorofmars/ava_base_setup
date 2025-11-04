#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	[CustomEditor(typeof(AVABaseSetupVRC))]
	public class AVABaseSetupVRCInspector : Editor
	{
		static readonly Color SpacerColor = new(0.17f, 0.17f, 0.17f);

		void OnEnable()
		{
		}

		public override void OnInspectorGUI()
		{
			var c = (AVABaseSetupVRC)target;

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.LabelField("Base Setup for Avatar Controllers!\nLayers get toggled based on what functionality is enabled at runtime.\nCurrently mapped FX and Additive Controllers will be replaced!", EditorStyles.helpBox);
			GUILayout.Space(10f);

#if AVA_BASE_SETUP_VRCFTTEMPLATES
			bool isFTAvailable = true;
#else
			bool isFTAvailable = false;
#endif

			if(!isFTAvailable)
			{
				EditorGUILayout.HelpBox("Warning, Facial Tracking Templates are not installed!\nInstall them from here:", MessageType.Warning, true);
				EditorGUI.indentLevel++;
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("");
				if(EditorGUILayout.LinkButton("https://adjerry91.github.io/VRCFaceTracking-Templates"))
					Application.OpenURL("https://adjerry91.github.io/VRCFaceTracking-Templates");
				EditorGUILayout.EndHorizontal();
				EditorGUI.indentLevel--;
			}
			else
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("UseFacialTracking"));
				if(c.UseFacialTracking)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("FacialTrackingSetupType"));
					if(c.FacialTrackingSetupType == AVA_FT_Setup_Type.Manual)
					{
						EditorGUILayout.PropertyField(serializedObject.FindProperty("FacialTrackingMenu"));
						if(c.FacialTrackingMenu == FacialTrackingMenu.Manual)
							EditorGUILayout.PropertyField(serializedObject.FindProperty("FacialTrackingMenuManual"));
					}
				}
			}
			GUILayout.Space(10f);
			DrawHLine();
			DrawLayer(c.LayerPreFT, "LayerPreFT", c.UseFacialTracking && isFTAvailable ? "Top Layers (Always On)" : "Top Layers");

			if(c.UseFacialTracking && isFTAvailable)
			{
				DrawHLine();
				EditorGUILayout.LabelField("Face Tracking (Toggled)", EditorStyles.boldLabel);
				EditorGUI.indentLevel++;
				if(c.FacialTrackingSetupType == AVA_FT_Setup_Type.Manual)
					DrawLayer(c.LayerFT, "LayerFT", "Face Tracking Setup");
				else
					EditorGUILayout.LabelField("Face Tracking Setup will be automatically detected.", EditorStyles.helpBox);
				DrawLayer(c.LayerFTReact, "LayerFTReact", "Face Tracking Reactions");
				EditorGUI.indentLevel--;
			}
			DrawHLine();
			DrawLayer(c.LayerManualExpressions, "LayerManualExpressions", c.UseFacialTracking && isFTAvailable ? "Manual Expression Layers (Toggled)" : "Manual Expression Layers");
			DrawHLine();
			DrawLayer(c.LayerPost, "LayerPost", c.UseFacialTracking && isFTAvailable ? "Bottom Layers (Always On)" : "Bottom Layers");
			DrawHLine();
			EditorGUILayout.ObjectField(serializedObject.FindProperty("LayerPostAdditiveController"), typeof(AnimatorController), new GUIContent("Extra Additive Animator Controller"));

			if(EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(c);
			}
		}

		private void DrawLayer(List<IAVABaseSetup.ControllerSource> Layer, string LayerName, string Label)
		{
			EditorGUILayout.LabelField(Label, EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			for(int i = 0; i < Layer.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.BeginVertical();
				if(Layer[i].ProducerComponent || !Layer[i].Controller)
					EditorGUILayout.ObjectField(serializedObject.FindProperty(LayerName).GetArrayElementAtIndex(i).FindPropertyRelative("ProducerComponent"), typeof(IAVAController), new GUIContent());
				if(!Layer[i].ProducerComponent)
					EditorGUILayout.ObjectField(serializedObject.FindProperty(LayerName).GetArrayElementAtIndex(i).FindPropertyRelative("Controller"), typeof(AnimatorController), new GUIContent());
				EditorGUILayout.EndVertical();
				if(GUILayout.Button("X", GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true))) Layer.Remove(Layer[i]);
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(5f);
			}
			EditorGUILayout.BeginHorizontal();
			if(Layer.Count == 0)
				EditorGUILayout.LabelField("-");
			GUILayout.FlexibleSpace();
			if(GUILayout.Button(new GUIContent("Add +", "Add a new Animator Controller or Controller Component"), GUILayout.ExpandWidth(false))) Layer.Add(new());
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel--;
		}

		private void DrawHLine()
		{
			GUILayout.Space(5f);
			EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), SpacerColor);
			GUILayout.Space(5f);
		}
	}
}

#endif
#endif
