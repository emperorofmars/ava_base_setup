#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public class AvatarHandlerContextVRChat
	{
		public readonly VRCAvatarDescriptor Avatar;
		public readonly AvatarBaseSetupVRChat Setup;
		private readonly SetupStateVRC State;

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

		public void RegisterParameters(VRCExpressionParameters Parameters)
		{
			State.Parameters.AddRange(Parameters.parameters.ToList());
		}

		public void RegisterMenuControl(string TargetPath, int Order, VRCExpressionsMenu.Control MenuControl)
		{
			var target = TargetPath.ToLower().Trim();
			if(!State.Menus.ContainsKey(target)) State.Menus.Add(target, new ());
			if(!State.Menus[target].MenuControls.ContainsKey(Order)) State.Menus[target].MenuControls.Add(Order, new ());
			State.Menus[target].MenuControls[Order].Add(MenuControl);
		}

		public void RegisterDirectBlendTree(VRCAvatarDescriptor.AnimLayerType Layer, BlendTree Blendtree, bool Overridable, string Parameter = "_weight", float Default = 1)
		{
			(Overridable ? State.GetLayer(Layer).DirectBlendPre : State.GetLayer(Layer).DirectBlendAfter).Add((Blendtree, Parameter));
			if(State.GetLayer(Layer).ControllerParameters.Find(p => p.Name == Parameter) == null)
				State.GetLayer(Layer).ControllerParameters.Add(new () { Name = Parameter, Type = AnimatorControllerParameterType.Float, DefaultValue = Default });
		}

		public void RegisterDirectBlendParameter(VRCAvatarDescriptor.AnimLayerType Layer, string Parameter, VRCExpressionParameters.ValueType ValueType, float Default, bool Saved)
		{
			if(State.Parameters.Find(p => p.name == Parameter) == null)
				State.Parameters.Add(new VRCExpressionParameters.Parameter {
					name = Parameter,
					defaultValue = Default,
					saved = Saved,
					valueType = ValueType
				});
			if(State.GetLayer(Layer).ControllerParameters.Find(p => p.Name == Parameter) == null)
				State.GetLayer(Layer).ControllerParameters.Add(new () { Name = Parameter, Type = AnimatorControllerParameterType.Float, DefaultValue = Default });
		}

		public void SaveResource(UnityEngine.Object Resource) { State.UnityResourcesToSave.Add(Resource); }
		public void SaveResource(List<UnityEngine.Object> Resource) { State.UnityResourcesToSave.AddRange(Resource); }

		public List<UnityEngine.Object> UnityResourcesToSave => State.UnityResourcesToSave;
		public List<UnityEngine.Object> UnityResourcesToSaveInDir => State.UnityResourcesToSaveInDir;

		public void SetFaceTrackingEnabled(bool Enabled) { State.FaceTrackingEnabled = Enabled; }
		public void SetUseLayerWeightDrivers(bool Enabled) { State.UseLayerWeightDrivers = Enabled; }
	}
}

#endif
#endif
