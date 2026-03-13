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
	public class AnimationToggleHandlerVRC : IAvatarBehaviourHandlerVRChat
	{
		public override Type HandlesBehaviour => typeof(AnimationToggleVRC);
		public override uint Priority => 1;
		public override uint Order => 100;
		public override string Label => "VRChat Animation Toggle";

		private string ToParam(AnimationToggleVRC Behaviour)
		{
			return "toggle_" + Behaviour.Name.Replace(" ", "_");
		}

		public override void Handle(AvatarHandlerContextVRChat Context, IAvatarBehaviour Behaviour)
		{
			var toggleBehaviour = Behaviour as AnimationToggleVRC;
			var parameter = ToParam(toggleBehaviour);
			var blendtree = new BlendTree {
				name = toggleBehaviour.Name,
				blendType = BlendTreeType.Simple1D,
				blendParameter = ToParam(toggleBehaviour)
			};
			blendtree.AddChild(toggleBehaviour.Off ? toggleBehaviour.Off : AssetDatabase.LoadAssetAtPath<AnimationClip>(Constants.ASSET_PATH + "_Empty.anim"), new Vector2(0, 0));
			blendtree.AddChild(toggleBehaviour.On ? toggleBehaviour.On : AssetDatabase.LoadAssetAtPath<AnimationClip>(Constants.ASSET_PATH + "_Empty.anim"), new Vector2(1, 0));

			Context.RegisterDirectBlendTree(VRCAvatarDescriptor.AnimLayerType.FX, blendtree, toggleBehaviour.IsOverridable);
			Context.RegisterDirectBlendParameter(VRCAvatarDescriptor.AnimLayerType.FX, parameter, 0, true);
			Context.RegisterMenu("Toggles", 0, new VRCExpressionsMenu.Control {
				name = toggleBehaviour.Name,
				icon = toggleBehaviour.Icon,
				parameter = new VRCExpressionsMenu.Control.Parameter {
					name = parameter,
				},
				type = VRCExpressionsMenu.Control.ControlType.Toggle,
				value = 0,
			});
		}

		public override List<(string Parameter, VRCExpressionParameters.ValueType ValueType)> GetParameters(IAvatarBehaviour Behaviour)
		{
			var toggleBehaviour = Behaviour as AnimationToggleVRC;
			return new() {
				(ToParam(toggleBehaviour), VRCExpressionParameters.ValueType.Bool)
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
