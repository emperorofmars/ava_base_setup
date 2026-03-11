#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	public static class SetupFTLayerWeightControl
	{
		public static void Apply(AVABaseSetupVRC Setup, AVASetupStateVRC State, AnimatorController Controller, int Layer)
		{
			var animatorLayer1 = new AnimatorControllerLayer
			{
				name = "Face Tracking Settings",
				stateMachine = new AnimatorStateMachine { name = "Face Tracking Settings" },
				defaultWeight = 1,
			};
			State.UnityResourcesToSave.Add(animatorLayer1.stateMachine);
			var stateFTOff = new AnimatorState { name = "FT Off", writeDefaultValues = true };
			var stateOn = new AnimatorState { name = "On", writeDefaultValues = true };
			var stateExpressionsOff = new AnimatorState { name = "Expressions Off", writeDefaultValues = true };
			var stateOff = new AnimatorState { name = "Off", writeDefaultValues = true };

			State.UnityResourcesToSave.Add(stateFTOff);
			State.UnityResourcesToSave.Add(stateOn);
			State.UnityResourcesToSave.Add(stateExpressionsOff);
			State.UnityResourcesToSave.Add(stateOff);

			animatorLayer1.stateMachine.AddState(stateFTOff, new Vector3(200, 220));
			animatorLayer1.stateMachine.AddState(stateOn, new Vector3(400, 100));
			animatorLayer1.stateMachine.AddState(stateExpressionsOff, new Vector3(600, 220));
			animatorLayer1.stateMachine.AddState(stateOff, new Vector3(400, 340));

			void addTransition(AnimatorState SourceState, AnimatorStateTransition t)
			{
				SourceState.AddTransition(t);
				State.UnityResourcesToSave.Add(t);
			}

			addTransition(stateFTOff, new AnimatorStateTransition {
				name = "FT off to All on",
				destinationState = stateOn,
				conditions = new AnimatorCondition[] {
					new() { mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
					new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled", threshold = 0 },
				},
			});
			addTransition(stateFTOff, new AnimatorStateTransition {
				name = "FT off to All on",
				destinationState = stateOn,
				conditions = new AnimatorCondition[] {
					new() { mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
					new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled", threshold = 0 },
				},
			});
			addTransition(stateFTOff, new AnimatorStateTransition {
				name = "FT off to Expressions off",
				destinationState = stateExpressionsOff,
				conditions = new AnimatorCondition[] {
					new() { mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
					new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
				},
			});
			addTransition(stateFTOff, new AnimatorStateTransition {
				name = "FT off to Expressions off",
				destinationState = stateExpressionsOff,
				conditions = new AnimatorCondition[] {
					new() { mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
					new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
				},
			});
			addTransition(stateFTOff, new AnimatorStateTransition {
				name = "FT off to All off",
				destinationState = stateOff,
				conditions = new AnimatorCondition[] {
					new() { mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
					new() { mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
					new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
				},
			});
			addTransition(stateOn, new AnimatorStateTransition {
				name = "All on to FT off",
				destinationState = stateFTOff,
				conditions = new AnimatorCondition[] {
					new() { mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
					new() { mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
					new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
				},
			});
			addTransition(stateOn, new AnimatorStateTransition {
				name = "All on to All off",
				destinationState = stateOff,
				conditions = new AnimatorCondition[] {
					new() { mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
					new() { mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
					new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
				},
			});
			addTransition(stateOn, new AnimatorStateTransition {
				name = "All on to Expressions off",
				destinationState = stateExpressionsOff,
				conditions = new AnimatorCondition[] {
					new() { mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
					new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
				},
			});
			addTransition(stateOn, new AnimatorStateTransition {
				name = "All on to Expressions off",
				destinationState = stateExpressionsOff,
				conditions = new AnimatorCondition[] {
					new() { mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
					new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
				},
			});
			addTransition(stateOff, new AnimatorStateTransition {
				name = "All off to FT off",
				destinationState = stateFTOff,
				conditions = new AnimatorCondition[] {
					new() { mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
					new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
				},
			});
			addTransition(stateOff, new AnimatorStateTransition {
				name = "All off to FT off",
				destinationState = stateFTOff,
				conditions = new AnimatorCondition[] {
					new() { mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
					new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
				},
			});
			addTransition(stateOff, new AnimatorStateTransition {
				name = "All off to Expressions off",
				destinationState = stateExpressionsOff,
				conditions = new AnimatorCondition[] {
					new() { mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
					new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
				},
			});
			addTransition(stateOff, new AnimatorStateTransition {
				name = "All off to Expressions off",
				destinationState = stateExpressionsOff,
				conditions = new AnimatorCondition[] {
					new() { mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
					new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled",  },
				},
			});
			addTransition(stateOff, new AnimatorStateTransition {
				name = "All off to All on",
				destinationState = stateOn,
				conditions = new AnimatorCondition[] {
					new() { mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
					new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
				},
			});
			addTransition(stateOff, new AnimatorStateTransition {
				name = "All off to All on",
				destinationState = stateOn,
				conditions = new AnimatorCondition[] {
					new() { mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
					new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
				},
			});
			addTransition(stateExpressionsOff, new AnimatorStateTransition {
				name = "Expressions off to All on",
				destinationState = stateOn,
				conditions = new AnimatorCondition[] {
					new() { mode = AnimatorConditionMode.Greater, parameter = "LipTrackingActive", threshold = 0.9f },
					new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
				},
			});
			addTransition(stateExpressionsOff, new AnimatorStateTransition {
				name = "Expressions off to All on",
				destinationState = stateOn,
				conditions = new AnimatorCondition[] {
					new() { mode = AnimatorConditionMode.Greater, parameter = "EyeTrackingActive", threshold = 0.9f },
					new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
				},
			});
			addTransition(stateExpressionsOff, new AnimatorStateTransition {
				name = "Expressions off to All off",
				destinationState = stateOff,
				conditions = new AnimatorCondition[] {
					new() { mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
					new() { mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
					new() { mode = AnimatorConditionMode.If, parameter = "FacialExpressionsDisabled" },
				},
			});
			addTransition(stateExpressionsOff, new AnimatorStateTransition {
				name = "Expressions off to FT off",
				destinationState = stateFTOff,
				conditions = new AnimatorCondition[] {
					new() { mode = AnimatorConditionMode.Less, parameter = "LipTrackingActive", threshold = 0.1f },
					new() { mode = AnimatorConditionMode.Less, parameter = "EyeTrackingActive", threshold = 0.1f },
					new() { mode = AnimatorConditionMode.IfNot, parameter = "FacialExpressionsDisabled" },
				},
			});

			var startLayer = Controller.layers.Length + 2;
			int ftLayerCount = -1;
			foreach(var controller in State.Layers[Layer].FT)
				ftLayerCount += controller.layers.Length;
			foreach(var controller in State.Layers[Layer].FT_React)
				ftLayerCount += controller.layers.Length;
			var mutLayerCount = 0;
			foreach(var controller in State.Layers[Layer].Mut)
				mutLayerCount += controller.layers.Length;

			SetupBehaviour(State, stateOn, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer, ftLayerCount + mutLayerCount, 1f);
			SetupBehaviour(State, stateExpressionsOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer, ftLayerCount, 1f);
			SetupBehaviour(State, stateExpressionsOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer + ftLayerCount, mutLayerCount, 0f);
			SetupBehaviour(State, stateFTOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer, ftLayerCount, 0f);
			SetupBehaviour(State, stateFTOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer + ftLayerCount, mutLayerCount, 1f);
			SetupBehaviour(State, stateOff, VRC_AnimatorLayerControl.BlendableLayer.FX, startLayer, ftLayerCount + mutLayerCount, 0f);

			Controller.AddLayer(animatorLayer1);
		}

		private static void SetupBehaviour(AVASetupStateVRC State, AnimatorState AnimState, VRC_AnimatorLayerControl.BlendableLayer Layer, int LayerStart, int LayerCount, float GoalWeight)
		{
			for(int i = LayerStart; i < LayerStart + LayerCount; i++)
			{
				var behaviour = AnimState.AddStateMachineBehaviour<VRCAnimatorLayerControl>();
				behaviour.playable = Layer;
				behaviour.layer = i;
				behaviour.goalWeight = GoalWeight;
				State.UnityResourcesToSave.Add(behaviour);
			}
		}
	}
}

#endif
#endif
