#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;
using VRC.SDKBase.Editor.BuildPipeline;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	[System.Serializable]
	public class VRCExpressionsMenuTarget
	{
		public string Target;
		public VRCExpressionsMenu Menu;
	}

	[AddComponentMenu("AVA/VRChat/AVA Base Setup")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public class AVABaseSetupVRC : IAVABaseSetup, IEditorOnly
	{
		public FT_Setup FaceTrackingSetupType = FT_Setup.Automatic;
		public List<VRCExpressionsMenu> AvatarMenus = new();
		public List<VRCExpressionsMenuTarget> AvatarSubMenus = new();
		public List<VRCExpressionsMenu> AvatarMenusFaceTracking = new();
		public List<VRCExpressionParameters> AvatarParameters = new();
		public List<VRCExpressionParameters> AvatarParametersFaceTracking = new();
		public bool UseLayerWeightDrivers = true;
	}

	[InitializeOnLoad]
	public class AVABaseSetupVRCCallback : IVRCSDKPreprocessAvatarCallback
	{
		// Run before VRCFury (-10000) and ModularAvatar (-11000)
		public int callbackOrder => -20000;

		public bool OnPreprocessAvatar(GameObject Root)
		{
			var candidates = Root.GetComponentsInChildren<AVABaseSetupVRC>();
			if(candidates != null && candidates.Length == 1 && candidates[0] is AVABaseSetupVRC setup)
			{
				AVASetupStateVRC setupState = null;
				try
				{
					setupState = Root.AddComponent<AVASetupStateVRC>();
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
					foreach(var removeMe in Root.GetComponentsInChildren<IAVAControllerProducer>())
						Object.DestroyImmediate(removeMe);
					Object.DestroyImmediate(setup);
					if(setupState)
						Object.DestroyImmediate(setupState);
				}
			}
			else if(candidates != null && candidates.Length > 1)
			{
				Debug.LogError("Error: Multiple 'AVABaseSetupVRC' components found!");
				return false;
			}
			return true; // Nothing to do
		}
	}
}

#endif
#endif
