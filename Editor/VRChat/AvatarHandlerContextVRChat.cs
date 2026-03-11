#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public class AvatarHandlerContextVRChat
	{
		public VRCAvatarDescriptor Avatar { get; private set; }
		public AvatarBaseSetupVRChat Setup { get; private set; }
		private SetupStateVRC State;

		public AvatarHandlerContextVRChat(VRCAvatarDescriptor Avatar, AvatarBaseSetupVRChat Setup, SetupStateVRC State)
		{
			this.Avatar = Avatar;
			this.Setup = Setup;
			this.State = State;
		}

		public void RegisterDirectBlendTree(VRCAvatarDescriptor.AnimLayerType Layer, BlendTree Blendtree, bool Overridable)
		{
			(Overridable ? State.GetLayer(Layer).DirectBlendPre : State.GetLayer(Layer).DirectBlendAfter).Add(Blendtree);
		}

		public void RegisterDirectBlendParameter(VRCAvatarDescriptor.AnimLayerType Layer, string Parameter, float Default, bool Saved)
		{
			State.Parameters.Add(new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter {
				name = Parameter,
				defaultValue = Default,
				saved = Saved,
				valueType = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.ValueType.Bool
			});
			State.GetLayer(Layer).ControllerParameters.Add((Parameter, AnimatorControllerParameterType.Float));
		}

		public void RegisterMenu(string TargetPath, int Order, VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control MenuControl)
		{
			State.MenuControls.Add((TargetPath, Order, MenuControl));
		}

	}
}

#endif
#endif
