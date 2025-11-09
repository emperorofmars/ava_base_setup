#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace com.squirrelbite.ava_base_setup
{
	[System.Serializable]
	public class ControllerMapping
	{
		// For applications which have just *the* one AnimatorController, leave the Mapping as null. Applications like VRChat have 5 separate layers of controllers, use the Mapping to refer to each.
		public string Mapping = null;
		public AnimatorController Controller = null;
	}

	[System.Serializable]
	public class ControllerSource
	{
		// Only one field will be used, preferentially the ProducerComponent.

		// When a simple set of AnimatorControllers is enough, just map that.
		//public AnimatorController Controller = null;
		public List<ControllerMapping> Controllers = new();

		// When more complex setup logic is needed, map a ProducerComponent.
		public IAVAControllerProducer ProducerComponent = null;
	}

	[DisallowMultipleComponent]
	[AddComponentMenu("AVA/Generic/AVA Base Setup")]
	[HelpURL("https://codeberg.org/emperorofmars/ava_base_setup")]
	public abstract class AVABaseSetup : MonoBehaviour
	{
		public bool UseFaceTracking = true;

		// Controller to put before the face tracking layers
		[InspectorName("Top Layers")]
		[Tooltip("Controller to put before the face tracking layers.")]
		public List<ControllerSource> LayerPreFT = new();

		// The face tracking controller source
		[InspectorName("Face Tracking Layers")]
		[Tooltip("The face tracking controller source.")]
		public List<ControllerSource> LayerFT = new();

		// Animations that react to facial tracking
		[InspectorName("Layers Reacting to Face Tracking")]
		[Tooltip("Animations that react to face tracking.")]
		public List<ControllerSource> LayerFTReact = new();

		// E.g. hand expressions that are usually mutually exclusive to face tracking
		[InspectorName("Manual Expression Layers")]
		[Tooltip("E.g. hand expressions that are usually mutually exclusive to face tracking.")]
		public List<ControllerSource> LayerManualExpressions = new();

		// Functionality that is not affected by face tracking
		[InspectorName("Bottom Layers")]
		[Tooltip("Functionality that is not affected by face tracking.")]
		public List<ControllerSource> LayerPost = new();
	}
}

#endif
