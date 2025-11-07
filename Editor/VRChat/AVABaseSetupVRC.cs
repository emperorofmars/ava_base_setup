#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;
using VRC.SDKBase.Editor.BuildPipeline;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public enum AVA_FT_Setup_Type { Automatic, Manual };

	[AddComponentMenu("AVA/VRChat/AVA Base Setup")]
	public class AVABaseSetupVRC : AVABaseSetup, IEditorOnly
	{
		public AVA_FT_Setup_Type FaceTrackingSetupType = AVA_FT_Setup_Type.Automatic;
		public List<VRCExpressionsMenu> AvatarMenus = new();
		public List<VRCExpressionsMenu> AvatarMenusFaceTracking = new();
		public List<VRCExpressionParameters> AvatarParameters = new();
		public List<VRCExpressionParameters> AvatarParametersFaceTracking = new();
	}


	[CustomPropertyDrawer(typeof(ControllerMapping))]
	public class ControllerMappingDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var ui = new VisualElement();
			ui.style.flexDirection = FlexDirection.Row;

			var dropdown = new DropdownField(AVAConstants.ControllerTypeToIndex.Keys.ToList(), 4);
			dropdown.style.minWidth = new StyleLength(100);
			dropdown.BindProperty(property.FindPropertyRelative("Mapping"));
			ui.Add(dropdown);

			var p_Controller = new PropertyField(property.FindPropertyRelative("Controller"), "");
			p_Controller.style.minWidth = new StyleLength(150);
			p_Controller.style.flexGrow = new StyleFloat(1);
			ui.Add(p_Controller);

			return ui;
		}
	}

	[InitializeOnLoad]
	public class AVABaseSetupVRCCallback : IVRCSDKPreprocessAvatarCallback
	{
		// Run before VRCFury (-10000) and ModularAvatar (-11000)
		public int callbackOrder => -20000;

		public bool OnPreprocessAvatar(GameObject Root)
		{
			if (Root.GetComponent<AVABaseSetupVRC>() is var setup && setup != null)
			{
				AVASetupStateVRC setupState = null;
				try
				{
					setupState = setup.gameObject.AddComponent<AVASetupStateVRC>();
					AVASetupVRCApplier.Apply(setup, setupState);
					return true;
				}
				catch (System.Exception exception)
				{
					Debug.LogError(exception);
					return false;
				}
				finally
				{
					foreach(var removeMe in setup.GetComponentsInChildren<IAVAControllerProducer>())
						Object.DestroyImmediate(removeMe);
					Object.DestroyImmediate(setup);
					if(setupState)
						Object.DestroyImmediate(setupState);
				}
			}
			return true; // Nothing to do
		}
	}
}

#endif
#endif
