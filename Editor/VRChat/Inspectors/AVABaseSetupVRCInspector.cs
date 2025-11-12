#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using com.squirrelbite.ava_base_setup.util;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

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
			FTMatch = AVAVRCUtil.MatchFTType(c.gameObject);
		}

		public override VisualElement CreateInspectorGUI()
		{
			var c = (AVABaseSetupVRC)target;
			VisualElement ui = new();

			Toolkit.AddElement(ui, new HelpBox("Base Setup for Avatar Animator Controllers with Face Tracking!\nLayers get toggled based on what functionality is enabled at runtime.\nCurrently mapped FX & Gesture Controllers, Parameters and Menus will be replaced!", HelpBoxMessageType.Info)).style.marginBottom = 10;

			bool isFTAvailable = AVAVRCUtil.CheckVRCFTTemplates();

			var p_UseFaceTracking = Toolkit.AddElement(ui, new PropertyField(serializedObject.FindProperty("UseFaceTracking")));
			var p_FaceTrackingSetupType = new PropertyField(serializedObject.FindProperty("FaceTrackingSetupType"));
			{
				var box = Toolkit.AddElement(ui, new VisualElement());
				box.Add(p_FaceTrackingSetupType);
				HelpBox p_helpBox = Toolkit.AddElement(ui, FTMatch >= 0 ? new HelpBox("Detected Face Fracking Setup: " + ((FT_Type)FTMatch).ToString(), HelpBoxMessageType.Info) : new HelpBox("Avatar doesn't support known face tracking method!", HelpBoxMessageType.Warning));

				void handleBox()
				{
					if(c.UseFaceTracking) box.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
					else box.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
				}
				handleBox();
				p_UseFaceTracking.RegisterValueChangeCallback(e => {
					handleBox();
				});

				void handleHelp()
				{
					if(c.UseFaceTracking && c.FaceTrackingSetupType == AVA_FT_Setup_Type.Automatic) p_helpBox.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
					else p_helpBox.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
				}
				handleHelp();
				p_FaceTrackingSetupType.RegisterValueChangeCallback(e => {
					handleHelp();
				});
			}
			if(!isFTAvailable && FTMatch >= 0)
			{
				Toolkit.AddElement(ui, new HelpBox("Warning, Face Tracking Templates are not installed!\nInstall them from here: <a href=\"https://adjerry91.github.io/VRCFaceTracking-Templates\">https://adjerry91.github.io/VRCFaceTracking-Templates</a>", HelpBoxMessageType.Warning));
			}

			{
				var foldoutOuter = Toolkit.AddElement(ui, new Box());
				foldoutOuter.style.marginTop = 15;
				var foldout = Toolkit.AddElement(foldoutOuter, new Foldout {text = "<size=+3><font-weight=700>Animator Controller Setup</font-weight></size>", viewDataKey = "animator_controller_foldout"});
				foldout.value = false;

				Toolkit.AddList(foldout, serializedObject.FindProperty("LayerPreFT"), new Label("<size=+1><font-weight=700>Layers before Face Tracking (Always On)</font-weight></size>"));

				{
					var outer = Toolkit.AddElement(foldout, new VisualElement());
					var box = Toolkit.AddElement(outer, new Box());
					box.style.marginTop = 5;
					box.style.marginBottom = 5;

					void handle_box()
					{
						if(c.UseFaceTracking) box.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
						else box.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
					}
					handle_box();
					p_UseFaceTracking.RegisterValueChangeCallback(e => {
						handle_box();
					});

					box.Add(new Label("<size=+1><font-weight=700>Face Tracking Layers (Toggled)</font-weight></size>"));

					var ftLayerHolder = Toolkit.AddElement(box, new VisualElement());
					ftLayerHolder.style.marginLeft = 10;

					var l_ftInfo = Toolkit.AddElement(ftLayerHolder, new HelpBox("Face Tracking Setup will be handled automatically", HelpBoxMessageType.Info));
					l_ftInfo.style.marginBottom = 5;

					var p_LayerFT = Toolkit.AddList(ftLayerHolder, serializedObject.FindProperty("LayerFT"));

					void handle_ft()
					{
						if (c.UseFaceTracking && c.FaceTrackingSetupType == AVA_FT_Setup_Type.Manual)
						{
							l_ftInfo.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
							p_LayerFT.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
						}
						else
						{
							l_ftInfo.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
							p_LayerFT.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
						}
					}
					handle_ft();
					p_FaceTrackingSetupType.RegisterValueChangeCallback(e => {
						handle_ft();
					});

					var p_LayerFTReact = Toolkit.AddList(box, serializedObject.FindProperty("LayerFTReact"), new Label("<size=+1><font-weight=700>Layers that React to Face Tracking (Toggled)</font-weight></size>"));
					p_LayerFTReact.style.marginLeft = 10;
				}

				var p_LayerManualExpressions = Toolkit.AddList(foldout, serializedObject.FindProperty("LayerManualExpressions"), new Label("<size=+1><font-weight=700>Manual Expression Layers (Toggled)</font-weight></size>"));

				var p_LayerPost = Toolkit.AddList(foldout, serializedObject.FindProperty("LayerPost"), new Label("<size=+1><font-weight=700>Bottom Layers (Always On)</font-weight></size>"));
			}
			{
				var foldoutOuter = Toolkit.AddElement(ui, new Box());
				foldoutOuter.style.marginTop = 15;
				var foldout = Toolkit.AddElement(foldoutOuter, new Foldout { text = "<size=+3><font-weight=700>Menus & Parameters</font-weight></size>", viewDataKey = "menus_parameters_foldout" });
				foldout.value = false;

				var p_AvatarMenus = Toolkit.AddList(foldout, serializedObject.FindProperty("AvatarMenus"), new Label("<size=+1><font-weight=700>Top Level Menus</font-weight></size>"));
				var p_AvatarSubMenus = Toolkit.AddList(foldout, serializedObject.FindProperty("AvatarSubMenus"), new Label("<size=+1><font-weight=700>Sub Menus</font-weight></size>"));
				var p_AvatarParameters = Toolkit.AddList(foldout, serializedObject.FindProperty("AvatarParameters"), new Label("<size=+1><font-weight=700>Parameters</font-weight></size>"));

				var box = Toolkit.AddElement(foldout, new VisualElement());
				box.style.marginLeft = 10;
				Toolkit.AddElement(box, new Label("<size=+1><font-weight=700>Menus & Parameters Toggled with Face Tracking</font-weight></size>"));
				var p_AvatarMenusFaceTracking = Toolkit.AddList(box, serializedObject.FindProperty("AvatarMenusFaceTracking"), new Label("<size=+1><font-weight=700>Face Tracking Menus</font-weight></size>"));
				var p_AvatarParametersFaceTracking = Toolkit.AddList(box, serializedObject.FindProperty("AvatarParametersFaceTracking"), new Label("<size=+1><font-weight=700>Face Tracking Parameters</font-weight></size>"));

				void handle_ft_menu_params()
				{
					if(c.UseFaceTracking && c.FaceTrackingSetupType == AVA_FT_Setup_Type.Manual) box.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
					else box.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
				}
				handle_ft_menu_params();
				p_UseFaceTracking.RegisterValueChangeCallback(e => {
					handle_ft_menu_params();
				});
				p_FaceTrackingSetupType.RegisterValueChangeCallback(e => {
					handle_ft_menu_params();
				});
			}

			{
				var spacer = Toolkit.AddElement(ui, new VisualElement());
				spacer.style.marginTop = 10;
				spacer.style.borderBottomWidth = 5;
				spacer.style.marginBottom = 5;
				spacer.style.borderBottomColor = new StyleColor(new Color(0.17f, 0.17f, 0.17f));

				var h_box = Toolkit.AddElement(ui, new VisualElement());
				h_box.style.flexDirection = FlexDirection.RowReverse;
				var h_spacer = Toolkit.AddElement(h_box, new VisualElement());

				var b = Toolkit.AddElement(h_box, new Button());
				b.Add(new Label("<size=-1><font-weight=700>Apply the Setup Now!</font-weight></size>"));
				b.RegisterCallback<ClickEvent>((e) =>
				{
					AVASetupStateVRC state = null;
					try
					{
						state = c.gameObject.AddComponent<AVASetupStateVRC>();
						AVASetupVRCApplier.Apply(c, state);
						Debug.Log("AVA setup created successfully! Find it under: Packages/com.squirrelbite.ava_base_setup/Output/");
					}
					finally
					{
						if(state)
							DestroyImmediate(state);
					}
				});
			}

			return ui;
		}
	}
}

#endif
#endif
