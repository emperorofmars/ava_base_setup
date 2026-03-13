#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System;
using System.Collections.Generic;
using System.Linq;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public class AvatarAnimatorControllerHandlerVRC : IAvatarBehaviourHandlerVRChat
	{
		public override Type HandlesBehaviour => typeof(AvatarAnimatorControllerVRC);
		public override uint Priority => 1;
		public override uint Order => 100;
		public override string Label => "VRChat Animator Controller";

		public override void Handle(AvatarHandlerContextVRChat Context, IAvatarBehaviour Behaviour)
		{
			var controllerBehaviour = Behaviour as AvatarAnimatorControllerVRC;

			if(controllerBehaviour.VRChatLayer != VRCAvatarDescriptor.AnimLayerType.FX && controllerBehaviour.VRChatLayer != VRCAvatarDescriptor.AnimLayerType.Additive)
				throw new Exception("Unsupported VRChat controller layer");

			if(controllerBehaviour.Controller)
				Context.RegisterController(controllerBehaviour.VRChatLayer, controllerBehaviour.Order, controllerBehaviour.Controller);
			if(controllerBehaviour.Menu)
				foreach(var control in controllerBehaviour.Menu.controls)
					Context.RegisterMenu("", 0, control); // todo vastly improve this
			if(controllerBehaviour.Parameters)
				Context.RegisterParameters(controllerBehaviour.Parameters.parameters.ToList());
		}

		public override List<(string Parameter, VRCExpressionParameters.ValueType ValueType)> GetParameters(IAvatarBehaviour Behaviour)
		{
			var controllerBehaviour = Behaviour as AvatarAnimatorControllerVRC;
			var ret = new List<(string Parameter, VRCExpressionParameters.ValueType ValueType)>();
			if(controllerBehaviour.Parameters && controllerBehaviour.Parameters.parameters.Count() > 0)
				foreach(var parameter in controllerBehaviour.Parameters.parameters)
					if(parameter.networkSynced)
						ret.Add((parameter.name, parameter.valueType));
			return ret;
		}
	}
}

#endif
#endif
