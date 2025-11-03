#if UNITY_EDITOR

using UnityEditor.Animations;
using UnityEngine;

namespace com.squirrelbite.ava_base_setup
{
	public abstract class IAVABaseSetup : MonoBehaviour
	{
		[System.Serializable]
		public class ControllerSource
		{
			public AnimatorController Controller;
			public IAVAController ProducerComponent;
		}

		public bool UseFacialTracking = true;

		// Controller to put before the facial tracking layers
		public ControllerSource PreFacialTracking = new();

		// The facial tracking controller source
		public ControllerSource FacialTracking = new();

		// Animations that react to facial tracking
		public ControllerSource FacialTrackingReact = new();

		// E.g. hand expressions that are usually mutually exclusive to facial tracking
		public ControllerSource ManualExpressions = new();

		// Functionality that is not affected by facial tracking
		public ControllerSource OtherFunctionality = new();
	}
}

#endif
