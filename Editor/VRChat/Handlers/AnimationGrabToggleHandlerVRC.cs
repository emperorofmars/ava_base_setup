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
using VRC.SDK3.Dynamics.Contact.Components;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public class AnimationGrabToggleHandlerVRC : IAvatarBehaviourHandlerVRChat
	{
		public override Type HandlesBehaviour => typeof(AnimationGrabToggleVRC);
		public override uint Priority => 1;
		public override uint Order => 100;
		public override string Label => "Animation Toggle";

		public override void Handle(AvatarHandlerContextVRChat Context, IAvatarBehaviour Behaviour)
		{
			var toggleBehaviour = Behaviour as AnimationGrabToggleVRC;

			// Regular manual toggle setup

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
				subParameters = new VRCExpressionsMenu.Control.Parameter[] {},
				value = 1,
			});

			// ensure contact

			var contact = toggleBehaviour.Contact;
			if(!toggleBehaviour.Contact)
			{
				if(!toggleBehaviour.Collider)
				{
					Debug.LogError($"Grab Toggle {toggleBehaviour} has no valid collider defined");
					return;
				}

				contact = toggleBehaviour.Collider.gameObject.AddComponent<VRCContactReceiver>();
				contact.rootTransform = toggleBehaviour.Collider.rootTransform;
				contact.localOnly = true;
				contact.allowOthers = false;
				contact.allowSelf = true;
				switch (toggleBehaviour.Collider.shapeType)
				{
					case VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Capsule: contact.shapeType = VRC.Dynamics.ContactBase.ShapeType.Capsule; break;
					case VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Sphere: contact.shapeType = VRC.Dynamics.ContactBase.ShapeType.Sphere; break;
					default: Debug.LogError($"Grab Toggle {toggleBehaviour} has invalid collider shape"); return;
				}

				contact.shape = toggleBehaviour.Collider.shape;

				switch(toggleBehaviour.Hand)
				{
					case "left": contact.collisionTags.Add("HandL"); break;
					case "right": contact.collisionTags.Add("HandR"); break;
					default: contact.collisionTags.Add("Hand"); break;
				}
				contact.collisionTags.Add("HandR");
				contact.receiverType = VRC.Dynamics.ContactReceiver.ReceiverType.Constant;
				contact.parameter = parameter + "__contact";
			}
			var contactParameter = contact.parameter;

			// Contact parameter driver setup

			var controller = new AnimatorController();
			Context.UnityResourcesToSave.Add(controller);
			var layer = new AnimatorControllerLayer { name = "Grabtoggle - " + (toggleBehaviour.Name ?? toggleBehaviour.ParameterName), stateMachine = new AnimatorStateMachine(), defaultWeight = 1 };
			Context.UnityResourcesToSave.Add(layer.stateMachine);

			controller.AddParameter(parameter, AnimatorControllerParameterType.Float);
			controller.AddParameter(toggleBehaviour.GrabEnabledParameter, AnimatorControllerParameterType.Bool);
			controller.AddParameter(contactParameter, AnimatorControllerParameterType.Bool);

			var stateIdle = new AnimatorState { name = "Idle", writeDefaultValues = true, timeParameterActive = true };
			layer.stateMachine.AddState(stateIdle, new (275, 120, 0));
			layer.stateMachine.defaultState = stateIdle;
			Context.UnityResourcesToSave.Add(stateIdle);

			var stateGrab = new AnimatorState { name = "Grab", writeDefaultValues = true, timeParameterActive = true };
			layer.stateMachine.AddState(stateGrab, new (525, 60, 0));
			var exitGrab = stateGrab.AddExitTransition();
			exitGrab.exitTime = 0;
			exitGrab.hasExitTime = true;
			exitGrab.duration = 0;
			Context.UnityResourcesToSave.Add(stateGrab);

			var stateNoGrab = new AnimatorState { name = "No Grab", writeDefaultValues = true, timeParameterActive = true};
			layer.stateMachine.AddState(stateNoGrab, new (525, 180, 0));
			var exitNoGrab = stateNoGrab.AddExitTransition();
			exitNoGrab.exitTime = 0;
			exitNoGrab.hasExitTime = true;
			exitNoGrab.duration = 0;
			Context.UnityResourcesToSave.Add(stateNoGrab);

			var transitionHandGrab1 = new AnimatorStateTransition {
				name = "Hand Grab",
				destinationState = stateGrab,
				conditions = new AnimatorCondition[] {
					new() { parameter = toggleBehaviour.Hand == "left" ? "GestureLeft" : "GestureRight", mode = AnimatorConditionMode.Equals, threshold = 1},
					new() { parameter = contactParameter, mode = AnimatorConditionMode.If },
					new() { parameter = parameter, mode = AnimatorConditionMode.Less, threshold = 0.5f },
					new() { parameter = toggleBehaviour.GrabEnabledParameter, mode = AnimatorConditionMode.If },
				}
			};
			stateIdle.AddTransition(transitionHandGrab1);
			Context.UnityResourcesToSave.Add(transitionHandGrab1);

			var transitionHandGrab7 = new AnimatorStateTransition {
				name = "Hand Grab",
				destinationState = stateGrab,
				conditions = new AnimatorCondition[] {
					new() { parameter = toggleBehaviour.Hand == "left" ? "GestureLeft" : "GestureRight", mode = AnimatorConditionMode.Equals, threshold = 7},
					new() { parameter = contactParameter, mode = AnimatorConditionMode.If },
					new() { parameter = parameter, mode = AnimatorConditionMode.Less, threshold = 0.5f },
					new() { parameter = toggleBehaviour.GrabEnabledParameter, mode = AnimatorConditionMode.If },
				}
			};
			stateIdle.AddTransition(transitionHandGrab7);
			Context.UnityResourcesToSave.Add(transitionHandGrab7);

			var transitionNoHandGrab = new AnimatorStateTransition {
				name = "Let Loose",
				destinationState = stateNoGrab,
				conditions = new AnimatorCondition[] {
					new() { parameter = toggleBehaviour.Hand == "left" ? "GestureLeft" : "GestureRight", mode = AnimatorConditionMode.NotEqual, threshold = 7},
					new() { parameter = toggleBehaviour.Hand == "left" ? "GestureLeft" : "GestureRight", mode = AnimatorConditionMode.NotEqual, threshold = 1},
					new() { parameter = contactParameter, mode = AnimatorConditionMode.If },
					new() { parameter = parameter, mode = AnimatorConditionMode.Greater, threshold = 0.5f },
					new() { parameter = toggleBehaviour.GrabEnabledParameter, mode = AnimatorConditionMode.If },
				}
			};
			stateIdle.AddTransition(transitionNoHandGrab);

			Debug.Log("PARAM: " + parameter);

			var vrcBehaviourGrab = stateGrab.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
			vrcBehaviourGrab.parameters.Add(new () {
				type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set,
				value = 1,
				name = parameter,
			});

			var vrcBehaviourNoGrab = stateNoGrab.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
			vrcBehaviourNoGrab.parameters.Add(new () {
				type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set,
				value = 0,
				name = parameter,
			});

			controller.AddLayer(layer);
			Context.RegisterController(VRCAvatarDescriptor.AnimLayerType.FX, SetupStateVRC.LogicLayer.After, controller);

			Context.RegisterParameters(new List<VRCExpressionParameters.Parameter>() {
				new() {name = toggleBehaviour.GrabEnabledParameter, defaultValue = toggleBehaviour.DefaultOn ? 1f : 0f, networkSynced = true, saved = true, valueType = VRCExpressionParameters.ValueType.Bool },
				new() {name = contactParameter, networkSynced = false, saved = false, valueType = VRCExpressionParameters.ValueType.Bool },
			});

			Context.RegisterMenuControl("Settings", 0, new VRCExpressionsMenu.Control {
				name = "Enable Grab: " + toggleBehaviour.Name,
				icon = toggleBehaviour.Icon,
				parameter = new VRCExpressionsMenu.Control.Parameter { name = toggleBehaviour.GrabEnabledParameter, },
				type = VRCExpressionsMenu.Control.ControlType.Toggle,
				subParameters = new VRCExpressionsMenu.Control.Parameter[] {},
				value = 1,
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
