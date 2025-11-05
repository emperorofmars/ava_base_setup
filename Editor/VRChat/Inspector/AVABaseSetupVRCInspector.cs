#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VRC.SDK3.Editor;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	[CustomEditor(typeof(AVABaseSetupVRC))]
	public class AVABaseSetupVRCInspector : Editor
	{
		static readonly Color SpacerColor = new(0.17f, 0.17f, 0.17f);

		private int FTMatch = -1;

		void OnEnable()
		{
			var c = (AVABaseSetupVRC)target;
			SkinnedMeshRenderer ftMesh = null;
			if(c.gameObject.TryGetComponent<AVASetupVRCFTProducer>(out var ftProducer))
				ftMesh = ftProducer.FTMesh != null ? ftProducer.FTMesh : FTTypeMatcher.DetectFaceMesh(AnimationPathUtil.GetRoot(c.transform).gameObject);
			if(ftMesh)
				FTMatch = FTTypeMatcher.Match(ftMesh);
		}

		public override VisualElement CreateInspectorGUI()
		{
			var c = (AVABaseSetupVRC)target;
			VisualElement ui = new();
			{
				var b = new HelpBox("Base Setup for Avatar Animator Controllers with Facie Tracking!\nLayers get toggled based on what functionality is enabled at runtime.\nCurrently mapped FX & Gesture Controllers, Parameters and Menus will be replaced!", HelpBoxMessageType.Info);
				b.style.marginBottom = 10;
				ui.Add(b);
			}

#if AVA_BASE_SETUP_VRCFTTEMPLATES
			bool isFTAvailable = true;
#else
			bool isFTAvailable = false;
#endif

			var p_UseFaceTracking = new PropertyField(serializedObject.FindProperty("UseFaceTracking"));
			var p_FaceTrackingSetupType = new PropertyField(serializedObject.FindProperty("FaceTrackingSetupType"));

			{
				var box = new VisualElement();
				ui.Add(box);
				box.Add(p_UseFaceTracking);

				var box_inner = new VisualElement();
				box.Add(box_inner);

				HelpBox p_helpBox = FTMatch >= 0 ? new HelpBox("Detected Face Fracking Setup: " + ((FT_Type)FTMatch).ToString(), HelpBoxMessageType.Info) : new HelpBox("Avatar doesn't support known face tracking method!", HelpBoxMessageType.Warning);

				box_inner.Add(p_FaceTrackingSetupType);
				box_inner.Add(p_helpBox);
				void handleInner()
				{
					if(c.UseFaceTracking)
					{
						if(!box.Contains(box_inner)) box.Add(box_inner);
					}
					else
					{
						if(box.Contains(box_inner)) box.Remove(box_inner);
					}
				}
				handleInner();
				p_UseFaceTracking.RegisterValueChangeCallback(e => {
					handleInner();
				});

				void handleHelp()
				{
					if(c.UseFaceTracking && c.FaceTrackingSetupType == AVA_FT_Setup_Type.Automatic)
					{
						if(!box_inner.Contains(p_helpBox)) box_inner.Add(p_helpBox);
					}
					else
					{
						if(box_inner.Contains(p_helpBox)) box_inner.Remove(p_helpBox);
					}
				}
				handleHelp();
				p_FaceTrackingSetupType.RegisterValueChangeCallback(e => {
					handleHelp();
				});
			}
			if(!isFTAvailable && FTMatch >= 0)
			{
				var box = new VisualElement();
				box.Add(new HelpBox("Warning, Face Tracking Templates are not installed!\nInstall them from here:", HelpBoxMessageType.Warning));
				var b = new Button() { text = "https://adjerry91.github.io/VRCFaceTracking-Templates" };
				b.clicked += () => {
					Application.OpenURL("https://adjerry91.github.io/VRCFaceTracking-Templates");
				};
				box.Add(b);
				ui.Add(box);
			}
			{
				var l = new Label("<size=+3><font-weight=700>Animator Controller Setup</font-weight></size>");
				l.style.marginTop = 15;
				l.style.marginBottom = 10;
				l.style.borderBottomWidth = 1;
				l.style.borderBottomColor = new StyleColor(new Color(0.17f, 0.17f, 0.17f));
				ui.Add(l);
			}

			var p_LayerPreFT = new PropertyField(serializedObject.FindProperty("LayerPreFT"));
			ui.Add(p_LayerPreFT);

			{
				var outer = new VisualElement();
				ui.Add(outer);
				var box = new Box();
				box.style.marginTop = 5;
				box.style.marginBottom = 5;
				outer.Add(box);

				void handle_box()
				{
					if(c.UseFaceTracking)
					{
						if(!outer.Contains(box)) outer.Add(box);
					}
					else
					{
						if(outer.Contains(box)) outer.Remove(box);
					}
				}
				handle_box();
				p_UseFaceTracking.RegisterValueChangeCallback(e => {
					handle_box();
				});

				box.Add(new Label("<font-weight=700>Face Tracking Layers (Toggled)</font-weight>"));

				var ftLayerHolder = new VisualElement();
				ftLayerHolder.style.marginLeft = 10;
				box.Add(ftLayerHolder);

				var l_ftInfo = new HelpBox("Face Tracking Setup will be handled automatically", HelpBoxMessageType.Info);
				ftLayerHolder.Add(l_ftInfo);

				var p_LayerFT = new PropertyField(serializedObject.FindProperty("LayerFT"));
				ftLayerHolder.Add(p_LayerFT);

				void handle_ft()
				{
					if (c.UseFaceTracking && c.FaceTrackingSetupType == AVA_FT_Setup_Type.Manual)
					{
						if (ftLayerHolder.Contains(l_ftInfo)) ftLayerHolder.Remove(l_ftInfo);
						if (!ftLayerHolder.Contains(p_LayerFT)) ftLayerHolder.Add(p_LayerFT);
					}
					else
					{
						if (ftLayerHolder.Contains(p_LayerFT)) ftLayerHolder.Remove(p_LayerFT);
						if (!ftLayerHolder.Contains(l_ftInfo)) ftLayerHolder.Add(l_ftInfo);
					}
				}
				handle_ft();
				p_FaceTrackingSetupType.RegisterValueChangeCallback(e => {
					handle_ft();
				});
				
				var p_LayerFTReact = new PropertyField(serializedObject.FindProperty("LayerFTReact"));
				p_LayerFTReact.style.marginLeft = 10;
				box.Add(p_LayerFTReact);
			}
			
			var p_LayerManualExpressions = new PropertyField(serializedObject.FindProperty("LayerManualExpressions"));
			ui.Add(p_LayerManualExpressions);
			
			var p_LayerPost = new PropertyField(serializedObject.FindProperty("LayerPost"));
			ui.Add(p_LayerPost);

			{
				var l = new Label("<size=+3><font-weight=700>Menus & Parameters</font-weight></size>");
				l.style.marginTop = 15;
				l.style.marginBottom = 10;
				l.style.borderBottomWidth = 1;
				l.style.borderBottomColor = new StyleColor(new Color(0.17f, 0.17f, 0.17f));
				ui.Add(l);
			}

			var p_AvatarMenus = new PropertyField(serializedObject.FindProperty("AvatarMenus"));
			ui.Add(p_AvatarMenus);
			var p_AvatarParameters = new PropertyField(serializedObject.FindProperty("AvatarParameters"));
			ui.Add(p_AvatarParameters);

			//if(c.UseFaceTracking && c.FaceTrackingSetupType == AVA_FT_Setup_Type.Manual)
			var p_AvatarMenusFaceTracking = new PropertyField(serializedObject.FindProperty("AvatarMenusFaceTracking"));
			ui.Add(p_AvatarMenusFaceTracking);
			var p_AvatarParametersFaceTracking = new PropertyField(serializedObject.FindProperty("AvatarParametersFaceTracking"));
			ui.Add(p_AvatarParametersFaceTracking);

			return ui;
		}

		/*public override void OnInspectorGUI()
		{
			var c = (AVABaseSetupVRC)target;

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.HelpBox("Base Setup for Avatar Animator Controllers with Facie Tracking!\nLayers get toggled based on what functionality is enabled at runtime.\nCurrently mapped FX & Gesture Controllers, Parameters and Menus will be replaced!", MessageType.Info, true);

#if AVA_BASE_SETUP_VRCFTTEMPLATES
			bool isFTAvailable = true;
#else
			bool isFTAvailable = false;
#endif

			EditorGUILayout.PropertyField(serializedObject.FindProperty("LayerFT"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("foo"));

			if(!isFTAvailable)
			{
				EditorGUILayout.HelpBox("Warning, Face Tracking Templates are not installed!\nInstall them from here:", MessageType.Warning, true);
				using(new EditorGUI.IndentLevelScope()) using(new EditorGUILayout.HorizontalScope(GUI.skin.box))
				{
					EditorGUILayout.PrefixLabel("");
					if(EditorGUILayout.LinkButton("https://adjerry91.github.io/VRCFaceTracking-Templates"))
						Application.OpenURL("https://adjerry91.github.io/VRCFaceTracking-Templates");
				}
			}
			else
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("UseFaceTracking"));
				if(c.UseFaceTracking)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("FaceTrackingSetupType"));
					if(c.FaceTrackingSetupType == AVA_FT_Setup_Type.Automatic)
					{
						if(FTMatch >= 0)
							EditorGUILayout.HelpBox("Detected Face Fracking Setup: " + ((FT_Type)FTMatch).ToString(), MessageType.Info, true);
						else
							EditorGUILayout.HelpBox("Avatar doesn't support face tracking!", MessageType.Warning, true);
					}
				}
			}
			GUILayout.Space(10f);

			DrawHLine();
			DrawLayer(c.LayerPreFT, "LayerPreFT", c.UseFaceTracking && isFTAvailable ? "Top Layers (Always On)" : "Top Layers");

			if(c.UseFaceTracking && (c.FaceTrackingSetupType == AVA_FT_Setup_Type.Manual || isFTAvailable && FTMatch >= 0))
			{
				DrawHLine();
				EditorGUILayout.LabelField("Face Tracking (Toggled)", EditorStyles.boldLabel);
				EditorGUI.indentLevel++;
				if(c.FaceTrackingSetupType == AVA_FT_Setup_Type.Manual)
					DrawLayer(c.LayerFT, "LayerFT", "Face Tracking Setup");
				else
					EditorGUILayout.LabelField("Face Tracking Setup will be automatically detected.", EditorStyles.helpBox);
				DrawLayer(c.LayerFTReact, "LayerFTReact", "Face Tracking Reactions");
				EditorGUI.indentLevel--;
			}
			DrawHLine();
			DrawLayer(c.LayerManualExpressions, "LayerManualExpressions", c.UseFaceTracking && isFTAvailable ? "Manual Expression Layers (Toggled)" : "Manual Expression Layers");
			DrawHLine();
			DrawLayer(c.LayerPost, "LayerPost", c.UseFaceTracking && isFTAvailable ? "Bottom Layers (Always On)" : "Bottom Layers");
			DrawHLine();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("AvatarMenus"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("AvatarParameters"));
			if(c.UseFaceTracking && c.FaceTrackingSetupType == AVA_FT_Setup_Type.Manual)
			{
				EditorGUILayout.LabelField("Face Tracking Menus & Parameters");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("AvatarMenusFaceTracking"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("AvatarParametersFaceTracking"));
			}

			if(EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(c);
			}
			GUILayout.Space(10f);
			DrawHLine();
			if(GUILayout.Button("Create Controllers", GUILayout.ExpandWidth(false)))
			{
				AVASetupStateVRC state = null;
				try
				{
					state = c.gameObject.AddComponent<AVASetupStateVRC>();
					AVASetupVRCApplier.Apply(c, state);
				}
				finally
				{
					if(state)
						DestroyImmediate(state);
				}
			}
		}

		private void DrawLayer(List<ControllerSource> Layer, string LayerName, string Label)
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
		}*/
	}
}

#endif
#endif
