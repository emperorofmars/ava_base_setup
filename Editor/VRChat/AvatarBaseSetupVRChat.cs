#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using UnityEditor;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDKBase.Editor.BuildPipeline;
using VRC.SDK3.Avatars.Components;
using System.Linq;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	[AddComponentMenu("AVA/VRChat/Avatar Setup")]
	[DisallowMultipleComponent]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public class AvatarBaseSetupVRChat : MonoBehaviour, IEditorOnly
	{
	}

	public static class InitAvatarBaseSetupVRChat
	{
		public static AvatarBaseSetupVRChat Init(VRCAvatarDescriptor Avatar)
		{
			var baseSetup = Avatar.gameObject.GetComponentInChildren<AvatarBaseSetupVRChat>();
			if(!baseSetup)
			{
				var baseSetupGo = new GameObject("Avatar Base Setup");
				baseSetupGo.transform.SetParent(Avatar.gameObject.transform);
				baseSetup = baseSetupGo.AddComponent<AvatarBaseSetupVRChat>();
				while(UnityEditorInternal.ComponentUtility.MoveComponentUp(baseSetup));
			}
			return baseSetup;
		}
	}

	[InitializeOnLoad]
	public class AvatarSetupVRChatCallback : IVRCSDKPreprocessAvatarCallback
	{
		// Run before VRCFury (-10000) and ModularAvatar (-11000)
		public int callbackOrder => -20000;

		public bool OnPreprocessAvatar(GameObject Root)
		{
			var candidates = Root.GetComponentsInChildren<AvatarBaseSetupVRChat>().Where(c => c.enabled).ToList();
			if(candidates.Count == 1 && candidates[0] is AvatarBaseSetupVRChat setup)
			{
				var avatar = Root.GetComponent<VRCAvatarDescriptor>();

				try
				{
					AvatarSetupVRChatApplier.Apply(avatar, setup);
					return true;
				}
				catch (System.Exception exception)
				{
					Debug.LogError(exception);
					return false;
				}
				finally
				{
					foreach(var removeMe in Root.GetComponentsInChildren<IAvatarBehaviour>())
						Object.DestroyImmediate(removeMe);
					if(setup.gameObject != Root)
						Object.DestroyImmediate(setup.gameObject);
					else
						Object.DestroyImmediate(setup);
				}
			}
			else if(candidates.Count > 1)
			{
				Debug.LogError("Error: Multiple 'AvatarBaseSetupVRChat' components found!");
				return false;
			}
			return true; // Nothing to do
		}
	}
}

#endif
#endif

