#if UNITY_EDITOR

using System.Collections.Generic;
using com.squirrelbite.ava_base_setup.util;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

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

	[CustomPropertyDrawer(typeof(ControllerSource))]
	public class ControllerSourceDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			return new UnityPlz(property);
		}
	}

	sealed class UnityPlz : VisualElement
	{
		private readonly SerializedProperty _property;

		public UnityPlz(SerializedProperty property)
		{
			_property = property;

			var ui = this;
			ui.style.paddingTop = ui.style.paddingBottom = 5;

			var warn = Toolkit.AddElement(ui, new Label("<color=yellow>Set the Producer Component or add AnimatorControllers</color>"));
			var producer = Toolkit.AddElement(ui, new PropertyField(property.FindPropertyRelative("ProducerComponent")));
			var controllers = Toolkit.AddElement(ui, new PropertyField(property.FindPropertyRelative("Controllers")));

			void handle()
			{
				if(property.FindPropertyRelative("ProducerComponent").boxedValue != null)
				{
					warn.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
					producer.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
					controllers.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
				}
				else if(property.FindPropertyRelative("Controllers").arraySize > 0)
				{
					warn.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
					producer.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
					controllers.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
				}
				else
				{
					warn.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
					producer.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
					controllers.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
				}
			}
			handle();
			controllers.RegisterValueChangeCallback(e => {
				handle();
			});
			producer.RegisterValueChangeCallback(e => {
				handle();
			});
		}
	}

	[DisallowMultipleComponent]
	[AddComponentMenu("AVA/Generic/AVA Base Setup")]
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
