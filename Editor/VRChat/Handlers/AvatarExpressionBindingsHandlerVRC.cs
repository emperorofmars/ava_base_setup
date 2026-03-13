#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public class AvatarExpressionBindingsHandlerVRC : IAvatarBehaviourHandlerVRChat
	{
		public override Type HandlesBehaviour => typeof(AvatarExpressionBindingsVRC);
		public override uint Priority => 1;
		public override uint Order => 100;
		public override string Label => "VRChat Expressions";

		public override void Handle(AvatarHandlerContextVRChat Context, IAvatarBehaviour Behaviour)
		{
			var bindingsBehaviour = Behaviour as AvatarExpressionBindingsVRC;
		}

		public override List<(string Parameter, VRCExpressionParameters.ValueType ValueType)> GetParameters(IAvatarBehaviour Behaviour)
		{
			var toggleBehaviour = Behaviour as AvatarExpressionBindingsVRC;
			return new() {
			};
		}

		public override VisualElement CreateGUI(IAvatarBehaviour Behaviour)
		{
			var ret = new VisualElement();
			var label = new Label("Foo");
			ret.Add(label);
			return ret;
		}
	}
}

#endif
#endif
