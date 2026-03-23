#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using com.squirrelbite.ava_base_setup.util;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	[CustomEditor(typeof(PuppetVRC))]
	public class PuppetVRCInspector : Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			var c = (PuppetVRC)target;
			VisualElement ui = new();

			Toolkit.AddElement(ui, new PropertyField(serializedObject.FindProperty("Name")));
			Toolkit.AddElement(ui, new PropertyField(serializedObject.FindProperty("Icon")));
			var typeToggle = Toolkit.AddElement(ui, new PropertyField(serializedObject.FindProperty("Type")));
			var persistentToggle = Toolkit.AddElement(ui, new PropertyField(serializedObject.FindProperty("IsPersistent")));
			Toolkit.AddElement(ui, new PropertyField(serializedObject.FindProperty("IsOverridable")));
			Toolkit.AddSpacer(ui);

			VisualElement createPropertyGui(VisualElement Parent, string ForceParameter, System.Func<string> GetProperty, string Label)
			{
				var box = Toolkit.AddElement(Parent, new Box());
				box.style.marginTop = box.style.marginBottom = 2;
				box.style.paddingLeft = box.style.paddingTop = box.style.paddingBottom = 2;

				var labelRow = Toolkit.AddElement(box, new VisualElement());
				labelRow.style.flexDirection = FlexDirection.Row;
				labelRow.style.justifyContent = Justify.SpaceBetween;

				Toolkit.AddElement(labelRow, new Label(Label));
				var paramRow = Toolkit.AddElement(labelRow, new VisualElement());
				paramRow.style.flexDirection = FlexDirection.Row;
				var parameterLabel = Toolkit.AddElement(paramRow, new Label(GetProperty()));
				var button = Toolkit.AddElement(paramRow, new Button { tooltip = "Copy", text = "copy" });
				button.RegisterCallback<ClickEvent>(e => { GUIUtility.systemCopyBuffer = GetProperty(); });
				button.style.scale = new StyleScale(new Scale(new Vector2(0.7f, 0.7f)));
				button.style.marginTop = button.style.marginBottom = 0;
				parameterLabel.selection.isSelectable = true;

				var forceProperty = Toolkit.AddElement(box, new PropertyField(serializedObject.FindProperty(ForceParameter)));
				forceProperty.RegisterValueChangeCallback(e => { parameterLabel.text = GetProperty(); });
				return box;
			}

			createPropertyGui(ui, "ForceParameterEnabledName", () => c.ParameterEnabledName, "Enabled Parameter");
			var propertyUI = Toolkit.AddElement(ui, new VisualElement());
			createPropertyGui(propertyUI, "ForceParameterXName", () => c.ParameterXName, "X Axis Parameter");
			var propertyYAxisUI = createPropertyGui(propertyUI, "ForceParameterYName", () => c.ParameterYName, "Y Axis Parameter");

			void handlePersistentToggle() {
				if(c.IsPersistent)
					propertyUI.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
				else
					propertyUI.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
			};
			persistentToggle.RegisterValueChangeCallback(e => { handlePersistentToggle(); });
			handlePersistentToggle();

			void handleTypeToggle() {
				if(c.Type == Puppet.PuppetType.D2)
					propertyYAxisUI.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
				else
					propertyYAxisUI.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
			}
			typeToggle.RegisterValueChangeCallback(e => { handleTypeToggle(); });
			handleTypeToggle();

			Toolkit.AddSpacer(ui);
			Toolkit.AddElement(ui, new PropertyField(serializedObject.FindProperty("Blendtree")));

			return ui;
		}
	}
}

#endif
#endif
