#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System;
using System.Collections.Generic;
using com.squirrelbite.ava_base_setup.util;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public class PuppetHandlerVRC : IAvatarBehaviourHandlerVRChat
	{
		public const string ParameterXNonPersistent = "override_puppet_x";
		public const string ParameterYNonPersistent = "override_puppet_y";

		public override Type HandlesBehaviour => typeof(PuppetVRC);
		public override uint Priority => 1;
		public override uint Order => 100;
		public override string Label => "Puppet";

		public override void Handle(AvatarHandlerContextVRChat Context, IAvatarBehaviour Behaviour)
		{
			var puppetBehaviour = Behaviour as PuppetVRC;
			var paramX = puppetBehaviour.IsPersistent ? puppetBehaviour.ParameterXName : ParameterXNonPersistent;
			var paramY = puppetBehaviour.IsPersistent ? puppetBehaviour.ParameterYName : ParameterYNonPersistent;

			var blendtree = new BlendTree {
				name = puppetBehaviour.Name,
				blendType = puppetBehaviour.Type == Puppet.PuppetType.D1 ? BlendTreeType.Simple1D : BlendTreeType.FreeformDirectional2D,
				blendParameter = paramX,
			};
			if(puppetBehaviour.Type == Puppet.PuppetType.D2)
				blendtree.blendParameterY = paramY;

			foreach(var mapping in puppetBehaviour.Blendtree)
			{
				blendtree.AddChild(mapping.Animation ? mapping.Animation : AssetDatabase.LoadAssetAtPath<AnimationClip>(Constants.ASSET_PATH + "_Empty.anim"), mapping.Position);
			}
			Context.RegisterDirectBlendTree(VRCAvatarDescriptor.AnimLayerType.FX, blendtree, puppetBehaviour.IsOverridable, puppetBehaviour.ParameterEnabledName, 0);

			Context.RegisterDirectBlendParameter(VRCAvatarDescriptor.AnimLayerType.FX, puppetBehaviour.ParameterEnabledName, VRCExpressionParameters.ValueType.Bool, 0, puppetBehaviour.IsPersistent);
			Context.RegisterDirectBlendParameter(VRCAvatarDescriptor.AnimLayerType.FX, paramX, VRCExpressionParameters.ValueType.Float, 0, puppetBehaviour.IsPersistent);
			if(puppetBehaviour.Type == Puppet.PuppetType.D2)
				Context.RegisterDirectBlendParameter(VRCAvatarDescriptor.AnimLayerType.FX, paramY, VRCExpressionParameters.ValueType.Float, 0, puppetBehaviour.IsPersistent);

			var subParameters = new List<VRCExpressionsMenu.Control.Parameter>() { new() { name = paramX } };
			if(puppetBehaviour.Type == Puppet.PuppetType.D2) subParameters.Add(new() { name = paramY });
			Context.RegisterMenuControl("Puppets", 0, new VRCExpressionsMenu.Control {
				name = puppetBehaviour.Name,
				icon = puppetBehaviour.Icon,
				parameter = puppetBehaviour.IsPersistent ? null : new VRCExpressionsMenu.Control.Parameter { name = puppetBehaviour.ParameterEnabledName },
				type = puppetBehaviour.Type == Puppet.PuppetType.D2 ? VRCExpressionsMenu.Control.ControlType.TwoAxisPuppet : VRCExpressionsMenu.Control.ControlType.RadialPuppet,
				value = 0,
				subParameters = subParameters.ToArray(),
			});
			if(puppetBehaviour.IsPersistent)
			{
				Context.RegisterMenuControl("Puppets", 0, new VRCExpressionsMenu.Control {
					name = "Enable " + puppetBehaviour.Name,
					icon = puppetBehaviour.Icon,
					parameter = new VRCExpressionsMenu.Control.Parameter { name = puppetBehaviour.ParameterEnabledName },
					type = VRCExpressionsMenu.Control.ControlType.Toggle,
					value = 0,
					subParameters = new VRCExpressionsMenu.Control.Parameter[] {},
				});
			}
		}

		public override List<(string Parameter, VRCExpressionParameters.ValueType ValueType)> GetParameters(IAvatarBehaviour Behaviour)
		{
			var puppetBehaviour = Behaviour as PuppetVRC;
			var ret = new List<(string Parameter, VRCExpressionParameters.ValueType ValueType)> {
				(puppetBehaviour.ParameterEnabledName, VRCExpressionParameters.ValueType.Bool)
			};
			if(puppetBehaviour.IsPersistent)
			{
				ret.Add((puppetBehaviour.ParameterXName, VRCExpressionParameters.ValueType.Float));
				if(puppetBehaviour.Type == Puppet.PuppetType.D2)
					ret.Add((puppetBehaviour.ParameterYName, VRCExpressionParameters.ValueType.Float));
			}
			else
			{
				ret.Add((ParameterXNonPersistent, VRCExpressionParameters.ValueType.Float));
				if(puppetBehaviour.Type == Puppet.PuppetType.D2)
					ret.Add((ParameterYNonPersistent, VRCExpressionParameters.ValueType.Float));
			}
			return ret;
		}

		public override VisualElement CreateGUI(IAvatarBehaviour Behaviour)
		{
			var puppetBehaviour = Behaviour as PuppetVRC;
			var ret = new VisualElement();
			Toolkit.AddElement(ret, new Label($"{(puppetBehaviour.IsPersistent ? "Persistent " : "")}{(puppetBehaviour.IsOverridable ? "Overridable " : "")}{(puppetBehaviour.Type == Puppet.PuppetType.D1 ? "1D " : "2D")} Puppet"));
			return ret;
		}
	}
}

#endif
#endif
