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
				using(new EditorGUI.IndentLevelScope()) using(new EditorGUILayout.HorizontalScope(GUI.skin.box))
				{
					EditorGUILayout.BeginHorizontal(GUI.skin.box);
					EditorGUILayout.PrefixLabel("");
					if(EditorGUILayout.LinkButton("https://adjerry91.github.io/VRCFaceTracking-Templates"))
						Application.OpenURL("https://adjerry91.github.io/VRCFaceTracking-Templates");
				}
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

			if(EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(c);
			}
		}

		private void DrawLayer(List<IAVABaseSetup.ControllerSource> Layer, string LayerName, string Label)
		{
			EditorGUILayout.LabelField(Label, EditorStyles.boldLabel);
			using (new EditorGUI.IndentLevelScope())
			{
				for(int i = 0; i < Layer.Count; i++)
				{
					using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
					{
						using (new EditorGUILayout.VerticalScope())
						{
							if(Layer[i].ProducerComponent || Layer[i].Controllers.Count == 0)
							{
								using (new EditorGUILayout.HorizontalScope())
								{
									EditorGUILayout.PrefixLabel("Producer Component");
									EditorGUILayout.ObjectField(serializedObject.FindProperty(LayerName).GetArrayElementAtIndex(i).FindPropertyRelative("ProducerComponent"), typeof(IAVAControllerProducer), new GUIContent());
								}
							}
							if(!Layer[i].ProducerComponent)
							{
								for(int j = 0; j < Layer[i].Controllers.Count; j++)
								{
									using (new EditorGUILayout.HorizontalScope())
									{
										var selection = AVAConstants.ControllerTypeToIndex.Keys.ToList();
										var oldSelectionIndex = selection.IndexOf(Layer[i].Controllers[j].Mapping);
										if(oldSelectionIndex <= 0)
										{
											oldSelectionIndex = 4;
											Layer[i].Controllers[j].Mapping = selection[oldSelectionIndex];
										}
										var newSelectionIndex = EditorGUILayout.Popup(oldSelectionIndex, selection.ToArray());
										if(oldSelectionIndex != newSelectionIndex)
											Layer[i].Controllers[j].Mapping = selection[newSelectionIndex];

										EditorGUILayout.ObjectField(serializedObject.FindProperty(LayerName).GetArrayElementAtIndex(i).FindPropertyRelative("Controllers").GetArrayElementAtIndex(j).FindPropertyRelative("Controller"), typeof(AnimatorController), new GUIContent());
										if(GUILayout.Button("X", GUILayout.ExpandWidth(false))) Layer[i].Controllers.Remove(Layer[i].Controllers[j]);
									}
								}
								using (new EditorGUILayout.HorizontalScope())
								{
									if(Layer[i].Controllers.Count == 0)
										EditorGUILayout.LabelField("Add Controller Mapping");
									GUILayout.FlexibleSpace();
									if(GUILayout.Button(new GUIContent("Add +", "Add a new Animator Controller Mapping"), GUILayout.ExpandWidth(false))) Layer[i].Controllers.Add(new());
								}
							}
						}
						GUILayout.Space(5f);
						if(GUILayout.Button("X", GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true))) Layer.Remove(Layer[i]);
					}
					GUILayout.Space(5f);
				}
				using (new EditorGUILayout.HorizontalScope())
				{
					if(Layer.Count == 0)
						EditorGUILayout.LabelField("-");
					GUILayout.FlexibleSpace();
					if(GUILayout.Button(new GUIContent("Add +", "Add a new Animator Controller or Controller Component"), GUILayout.ExpandWidth(false))) Layer.Add(new());
				}
			}
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
