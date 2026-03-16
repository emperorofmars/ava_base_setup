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
		public override string Label => "Animation Toggle";

		public override void Handle(AvatarHandlerContextVRChat Context, IAvatarBehaviour Behaviour)
		{
			var toggleBehaviour = Behaviour as AnimationToggleVRC;
			var parameter = toggleBehaviour.ParameterName;
			var blendtree = new BlendTree {
				name = toggleBehaviour.Name,
				blendType = BlendTreeType.Simple1D,
				blendParameter = parameter
			};
			blendtree.AddChild(toggleBehaviour.Off ? toggleBehaviour.Off : AssetDatabase.LoadAssetAtPath<AnimationClip>(Constants.ASSET_PATH + "_Empty.anim"), new Vector2(0, 0));
			blendtree.AddChild(toggleBehaviour.On ? toggleBehaviour.On : AssetDatabase.LoadAssetAtPath<AnimationClip>(Constants.ASSET_PATH + "_Empty.anim"), new Vector2(1, 0));

			Context.RegisterDirectBlendTree(VRCAvatarDescriptor.AnimLayerType.FX, blendtree, toggleBehaviour.IsOverridable);
			Context.RegisterDirectBlendParameter(VRCAvatarDescriptor.AnimLayerType.FX, parameter, VRCExpressionParameters.ValueType.Bool, toggleBehaviour.DefaultOn ? 1 : 0, true);
			Context.RegisterMenuControl(toggleBehaviour.SubMenuPath ?? "Toggles", 0, new VRCExpressionsMenu.Control {
				name = toggleBehaviour.Name,
				icon = toggleBehaviour.Icon,
				parameter = new VRCExpressionsMenu.Control.Parameter { name = parameter },
				type = VRCExpressionsMenu.Control.ControlType.Toggle,
				value = toggleBehaviour.DefaultOn ? 1 : 0,
				subParameters = new VRCExpressionsMenu.Control.Parameter[] {},
			});
		}

		public override List<(string Parameter, VRCExpressionParameters.ValueType ValueType)> GetParameters(IAvatarBehaviour Behaviour)
		{
			var toggleBehaviour = Behaviour as AnimationToggleVRC;
			return new() {
				(toggleBehaviour.ParameterName, VRCExpressionParameters.ValueType.Bool)
			};
		}

		public override VisualElement CreateGUI(IAvatarBehaviour Behaviour)
		{
			var toggleBehaviour = Behaviour as AnimationToggleVRC;
			var ret = new VisualElement();
			var label = new Label("Parameter: " + toggleBehaviour.ParameterName);
			ret.Add(label);
			return ret;
		}
	}
}

#endif
#endif
