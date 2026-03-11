#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	[CustomPropertyDrawer(typeof(ControllerMapping))]
	public class ControllerMappingVRCDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var ui = new VisualElement();
			ui.style.flexDirection = FlexDirection.Row;

			var dropdown = new DropdownField(AVAConstantsVRC.ControllerTypeToIndex.Keys.ToList(), 4);
			dropdown.style.minWidth = new StyleLength(100);
			dropdown.BindProperty(property.FindPropertyRelative("Mapping"));
			ui.Add(dropdown);

			var p_Controller = new PropertyField(property.FindPropertyRelative("Controller"), "");
			p_Controller.style.minWidth = new StyleLength(150);
			p_Controller.style.flexGrow = new StyleFloat(1);
			ui.Add(p_Controller);

			return ui;
		}
	}
}

#endif
#endif
