#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

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

		public void RegisterController(VRCAvatarDescriptor.AnimLayerType Layer, SetupStateVRC.LogicLayer Order, AnimatorController Controller)
		{
			var layer = State.GetLayer(Layer);
			switch(Order)
			{
				case SetupStateVRC.LogicLayer.Top: layer.ControllersPre.Add(Controller); break;
				case SetupStateVRC.LogicLayer.FaceTracking: layer.ControllersFaceTracking.Add(Controller); break;
				case SetupStateVRC.LogicLayer.FaceTrackingReact: layer.ControllersFaceTrackingReact.Add(Controller); break;
				case SetupStateVRC.LogicLayer.Expressions: layer.ControllersExpression.Add(Controller); break;
				case SetupStateVRC.LogicLayer.After: layer.ControllersAfter.Add(Controller); break;
				case SetupStateVRC.LogicLayer.Additive: layer.ControllersAdditive.Add(Controller); break;
			}
		}

		public void RegisterParameters(List<VRCExpressionParameters.Parameter> Parameters)
		{
			State.Parameters.AddRange(Parameters);
		}

		public void RegisterMenu(string TargetPath, int Order, VRCExpressionsMenu.Control MenuControl)
		{
			State.MenuControls.Add((TargetPath, Order, MenuControl));
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

		public void SaveResource(UnityEngine.Object Resource) { State.UnityResourcesToSave.Add(Resource); }
		public void SaveResource(List<UnityEngine.Object> Resource) { State.UnityResourcesToSave.AddRange(Resource); }

		public List<UnityEngine.Object> UnityResourcesToSave => State.UnityResourcesToSave;

		public void SetFaceTrackingEnabled(bool Enabled) { State.FaceTrackingEnabled = Enabled; }
		public void SetUseLayerWeightDrivers(bool Enabled) { State.UseLayerWeightDrivers = Enabled; }
	}
}

#endif
#endif
