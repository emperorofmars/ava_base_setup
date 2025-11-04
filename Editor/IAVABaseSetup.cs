#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace com.squirrelbite.ava_base_setup
{
	public abstract class IAVABaseSetup : MonoBehaviour
	{
		[System.Serializable]
		public class ControllerSource
		{
			public AnimatorController Controller = null;
			public IAVAController ProducerComponent = null;
		}

		public bool UseFacialTracking = true;

		// Controller to put before the facial tracking layers
		public List<ControllerSource> LayerPreFT = new();

		// The facial tracking controller source
		public List<ControllerSource> LayerFT = new();

		// Animations that react to facial tracking
		public List<ControllerSource> LayerFTReact = new();

		// E.g. hand expressions that are usually mutually exclusive to facial tracking
		public List<ControllerSource> LayerManualExpressions = new();

		// Functionality that is not affected by facial tracking
		public List<ControllerSource> LayerPost = new();
	}
}

#endif
