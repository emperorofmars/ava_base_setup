#if UNITY_EDITOR

using com.squirrelbite.ava_base_setup.util;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace com.squirrelbite.ava_base_setup
{
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
}

#endif
