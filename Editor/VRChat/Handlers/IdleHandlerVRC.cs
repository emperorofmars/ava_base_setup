#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public class IdleHandlerVRC : IAvatarBehaviourHandlerVRChat
	{
		public override Type HandlesBehaviour => typeof(IdleVRC);
		public override uint Priority => 1;
		public override uint Order => 100;
		public override string Label => "Idle Animation";

		public override void Handle(AvatarHandlerContextVRChat Context, IAvatarBehaviour Behaviour)
		{
			var idleBehaviour = Behaviour as IdleVRC;

			var ret = new AnimatorController { name = "Idle " + idleBehaviour.Name };
			var animatorLayer = new AnimatorControllerLayer
			{
				name = "Idle " + idleBehaviour.Name,
				stateMachine = new AnimatorStateMachine() { name = "Idle " + idleBehaviour.Name },
				blendingMode = idleBehaviour.IsAdditive ? AnimatorLayerBlendingMode.Additive : AnimatorLayerBlendingMode.Override,
				defaultWeight = 1,
			};
			Context.SaveResource(animatorLayer.stateMachine);
			var idleState = animatorLayer.stateMachine.AddState("Idle Animation");
			idleState.motion = idleBehaviour.IdleAnimation;
			ret.AddLayer(animatorLayer);
			Context.RegisterController(VRCAvatarDescriptor.AnimLayerType.FX, idleBehaviour.IsAdditive ? SetupStateVRC.LogicLayer.Additive : SetupStateVRC.LogicLayer.After, ret);
		}

		public override List<(string Parameter, VRCExpressionParameters.ValueType ValueType)> GetParameters(IAvatarBehaviour Behaviour)
		{
			return null;
		}
	}
}

#endif
#endif
