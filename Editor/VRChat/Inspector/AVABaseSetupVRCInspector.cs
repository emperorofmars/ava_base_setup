#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

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

			{
				var outer = new VisualElement();
				ui.Add(outer);
				var box = new VisualElement();
				outer.Add(box);
				box.Add(new Label("<size=+3><font-weight=700>Menus & Parameters Toggled with Face Tracking</font-weight></size>"));
				var p_AvatarMenusFaceTracking = new PropertyField(serializedObject.FindProperty("AvatarMenusFaceTracking"));
				box.Add(p_AvatarMenusFaceTracking);
				var p_AvatarParametersFaceTracking = new PropertyField(serializedObject.FindProperty("AvatarParametersFaceTracking"));
				box.Add(p_AvatarParametersFaceTracking);

				void handle_ft_menu_params()
				{
					if(c.UseFaceTracking && c.FaceTrackingSetupType == AVA_FT_Setup_Type.Manual)
					{
						if(!outer.Contains(box)) outer.Add(box);
					}
					else
					{
						if(outer.Contains(box)) outer.Remove(box);
					}
				}
				handle_ft_menu_params();
				p_UseFaceTracking.RegisterValueChangeCallback(e => {
					handle_ft_menu_params();
				});
				p_FaceTrackingSetupType.RegisterValueChangeCallback(e => {
					handle_ft_menu_params();
				});
			}

			return ui;
		}
	}
}

#endif
#endif
