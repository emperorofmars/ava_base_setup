#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using static com.squirrelbite.ava_base_setup.AvatarExpressionBindings;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public class AvatarExpressionBindingsHandlerVRC : IAvatarBehaviourHandlerVRChat
	{
		public override Type HandlesBehaviour => typeof(AvatarExpressionBindingsVRC);
		public override uint Priority => 1;
		public override uint Order => 100;
		public override string Label => "Expression Bindings";

		public override void Handle(AvatarHandlerContextVRChat Context, IAvatarBehaviour Behaviour)
		{
			var bindingsBehaviour = Behaviour as AvatarExpressionBindingsVRC;
			Apply(Context, bindingsBehaviour);
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


		public static readonly ReadOnlyDictionary<HandGesture, int> HandGestureToParameterIndex = new(new Dictionary<HandGesture, int>()
		{
			{ HandGesture.None, 0 },
			{ HandGesture.Fist, 1 },
			{ HandGesture.Open, 2 },
			{ HandGesture.Point, 3 },
			{ HandGesture.Peace, 4 },
			{ HandGesture.RockNRoll, 5 },
			{ HandGesture.Gun, 6 },
			{ HandGesture.ThumbsUp, 7 },
		});

		public static void Apply(AvatarHandlerContextVRChat Context, AvatarExpressionBindingsVRC Behaviour)
		{
			var animatorFX = SetupBaseFX();
			SetupEmotes(Context, Behaviour, animatorFX);
			Context.RegisterController(VRCAvatarDescriptor.AnimLayerType.FX, SetupStateVRC.LogicLayer.Expressions, animatorFX);
			return;
		}

		private static AnimatorController SetupBaseFX()
		{
			var animatorFX = new AnimatorController { name = "FX" };
			animatorFX.AddParameter("GestureLeft", AnimatorControllerParameterType.Int);
			animatorFX.AddParameter("GestureLeftWeight", AnimatorControllerParameterType.Float);
			animatorFX.AddParameter("GestureRight", AnimatorControllerParameterType.Int);
			animatorFX.AddParameter("GestureRightWeight", AnimatorControllerParameterType.Float);
			return animatorFX;
		}

		private static AnimatorState AddSingleSideHandGestureState(AvatarHandlerContextVRChat Context, AvatarExpressionBindingsVRC Behaviour, AnimatorControllerLayer Layer, HandGesture Gesture, bool IsLeft, string OverrideGestureName = null)
		{
			var state = new AnimatorState { name = string.IsNullOrWhiteSpace(OverrideGestureName) ? Gesture.ToString() : OverrideGestureName, writeDefaultValues = true, timeParameterActive = true };
			Layer.stateMachine.AddState(state, new Vector3(300, 60 * HandGestureToParameterIndex[Gesture], 0));
			var transitionIdle = Layer.stateMachine.AddAnyStateTransition(state);
			transitionIdle.AddCondition(AnimatorConditionMode.Equals, HandGestureToParameterIndex[Gesture], IsLeft ? "GestureLeft" : "GestureRight");
			transitionIdle.hasExitTime = false;

			var expressions = Context.Setup.GetComponentInChildren<AvatarExpressions>();

			if (expressions && Behaviour.ExpressionBindings.Find(b => IsLeft ? b.GuestureLeftHand == Gesture && b.GuestureRightHand == HandGesture.None : b.GuestureRightHand == Gesture && b.GuestureLeftHand == HandGesture.None) is var emoteBinding && emoteBinding != null && !string.IsNullOrWhiteSpace(emoteBinding.Expression))
			{
				if (expressions.Expressions.Find(e => e.Mapping == emoteBinding.Expression) is var emote && emote != null)
				{
					state.motion = emote.Animation;
					state.timeParameter = emoteBinding.UseTriggerIntensity > 0 ? emoteBinding.UseTriggerIntensity == TriggerIntensity.Left ? "GestureLeftWeight" : "GestureRightWeight" : null;
				}
			}
			return state;
		}

		private static AnimatorState AddHandGestureState(AvatarHandlerContextVRChat Context, AvatarExpressionBindingsVRC Behaviour, AnimatorControllerLayer Layer, AnimatorState Idle, HandGesture GestureLeft, HandGesture GestureRight, string Name, int Index)
		{
			var state = new AnimatorState { name = Name, writeDefaultValues = true, timeParameterActive = true };
			Layer.stateMachine.AddState(state, new Vector3(550, 60 * Index, 0));
			var transitionIdle = Idle.AddTransition(state);
			transitionIdle.AddCondition(AnimatorConditionMode.Equals, HandGestureToParameterIndex[GestureLeft], "GestureLeft");
			transitionIdle.AddCondition(AnimatorConditionMode.Equals, HandGestureToParameterIndex[GestureRight], "GestureRight");
			transitionIdle.hasExitTime = false;

			var transitionExitLeft = state.AddExitTransition(false);
			transitionExitLeft.AddCondition(AnimatorConditionMode.NotEqual, HandGestureToParameterIndex[GestureLeft], "GestureLeft");
			var transitionExitRight = state.AddExitTransition(false);
			transitionExitRight.AddCondition(AnimatorConditionMode.NotEqual, HandGestureToParameterIndex[GestureRight], "GestureRight");

			var expressions = Context.Setup.GetComponentInChildren<AvatarExpressions>();

			if (Behaviour.ExpressionBindings.Find(b => b.GuestureLeftHand == GestureLeft && b.GuestureRightHand == GestureRight) is var emoteBinding && emoteBinding != null && !string.IsNullOrWhiteSpace(emoteBinding.Expression))
			{
				if (expressions.Expressions.Find(e => e.Mapping == emoteBinding.Expression) is var emote && emote != null)
				{
					state.motion = emote.Animation;
					state.timeParameter = emoteBinding.UseTriggerIntensity > 0 ? emoteBinding.UseTriggerIntensity == TriggerIntensity.Left ? "GestureLeftWeight" : "GestureRightWeight" : null;
				}
			}
			return state;
		}

		private static void SetupEmotes(AvatarHandlerContextVRChat Context, AvatarExpressionBindingsVRC Behaviour, AnimatorController animatorFX)
		{
			var layerHandLeft = new AnimatorControllerLayer { name = "Left Hand", stateMachine = new AnimatorStateMachine(), defaultWeight = 1 };
			{
				AddSingleSideHandGestureState(Context, Behaviour, layerHandLeft, HandGesture.None, true, "Idle");
				AddSingleSideHandGestureState(Context, Behaviour, layerHandLeft, HandGesture.Fist, true);
				AddSingleSideHandGestureState(Context, Behaviour, layerHandLeft, HandGesture.Open, true);
				AddSingleSideHandGestureState(Context, Behaviour, layerHandLeft, HandGesture.Point, true);
				AddSingleSideHandGestureState(Context, Behaviour, layerHandLeft, HandGesture.Peace, true);
				AddSingleSideHandGestureState(Context, Behaviour, layerHandLeft, HandGesture.RockNRoll, true);
				AddSingleSideHandGestureState(Context, Behaviour, layerHandLeft, HandGesture.Gun, true);
				AddSingleSideHandGestureState(Context, Behaviour, layerHandLeft, HandGesture.ThumbsUp, true);
			}

			var layerHandRight = new AnimatorControllerLayer { name = "Right Hand", stateMachine = new AnimatorStateMachine(), defaultWeight = 1 };
			{
				AddSingleSideHandGestureState(Context, Behaviour, layerHandRight, HandGesture.None, false, "Idle");
				AddSingleSideHandGestureState(Context, Behaviour, layerHandRight, HandGesture.Fist, false);
				AddSingleSideHandGestureState(Context, Behaviour, layerHandRight, HandGesture.Open, false);
				AddSingleSideHandGestureState(Context, Behaviour, layerHandRight, HandGesture.Point, false);
				AddSingleSideHandGestureState(Context, Behaviour, layerHandRight, HandGesture.Peace, false);
				AddSingleSideHandGestureState(Context, Behaviour, layerHandRight, HandGesture.RockNRoll, false);
				AddSingleSideHandGestureState(Context, Behaviour, layerHandRight, HandGesture.Gun, false);
				AddSingleSideHandGestureState(Context, Behaviour, layerHandRight, HandGesture.ThumbsUp, false);
			}

			if (Behaviour.UseHandDominance == HandDominance.Right)
			{
				animatorFX.AddLayer(layerHandLeft);
				animatorFX.AddLayer(layerHandRight);
			}
			else if (Behaviour.UseHandDominance == HandDominance.Left)
			{
				animatorFX.AddLayer(layerHandRight);
				animatorFX.AddLayer(layerHandLeft);
			}

			if (Behaviour.UseHandDominance == HandDominance.Explicit)
			{
				var layerHands = new AnimatorControllerLayer { name = "Hands", stateMachine = new AnimatorStateMachine(), defaultWeight = 1 };
				var stateIdle = new AnimatorState { name = "Idle", writeDefaultValues = true, timeParameterActive = true };
				layerHands.stateMachine.AddState(stateIdle, new Vector3(300, 0, 0));
				animatorFX.AddLayer(layerHands);
				var index = 0;
				foreach (var binding in Behaviour.ExpressionBindings)
				{
					if (!string.IsNullOrWhiteSpace(binding.Expression))
					{
						AddHandGestureState(Context, Behaviour, layerHands, stateIdle, binding.GuestureLeftHand, binding.GuestureRightHand, binding.Expression, index);
					}
					index++;
				}
			}
			else
			{
				var layerHands = new AnimatorControllerLayer { name = "Hands", stateMachine = new AnimatorStateMachine(), defaultWeight = 1 };
				var stateIdle = new AnimatorState { name = "Idle", writeDefaultValues = true, timeParameterActive = true };
				layerHands.stateMachine.AddState(stateIdle, new Vector3(300, 0, 0));
				animatorFX.AddLayer(layerHands);
				var index = 0;
				foreach (var binding in Behaviour.ExpressionBindings)
				{
					if (binding.GuestureLeftHand > 0 && binding.GuestureRightHand > 0 && !string.IsNullOrWhiteSpace(binding.Expression))
					{
						AddHandGestureState(Context, Behaviour, layerHands, stateIdle, binding.GuestureLeftHand, binding.GuestureRightHand, binding.Expression, index);
					}
					index++;
				}
			}
		}
	}
}

#endif
#endif
