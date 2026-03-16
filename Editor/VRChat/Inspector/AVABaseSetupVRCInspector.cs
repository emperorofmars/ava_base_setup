#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using System.Linq;
using com.squirrelbite.ava_base_setup.util;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace com.squirrelbite.ava_base_setup.vrchat
{
	[CustomEditor(typeof(AvatarBaseSetupVRChat))]
	public class AvatarBaseSetupVRChatInspector : Editor
	{
		static readonly Color SpacerColor = new(0.17f, 0.17f, 0.17f);

		private int FTMatch = -1;

		void OnEnable()
		{
			var c = (AvatarBaseSetupVRChat)target;
			FTMatch = AVAVRCUtil.MatchFTType(c.gameObject);
		}

		public override VisualElement CreateInspectorGUI()
		{
			var c = (AvatarBaseSetupVRChat)target;
			VisualElement ui = new();

			Toolkit.AddElement(ui, new Label("<size=+2><font-weight=700>VRChat Avatar Base Setup</font-weight></size>"));

			Toolkit.AddSpacer(ui);

			Toolkit.AddElement(ui, new PropertyField(serializedObject.FindProperty("BaseMenu")));

			Toolkit.AddElement(ui, new PropertyField(serializedObject.FindProperty("BaseParameters")));

			Toolkit.AddSpacer(ui);

			var behaviourListParentParent = Toolkit.AddElement(ui, new VisualElement());
			var behaviourListParent = new VisualElement();

			void drawBehaviours()
			{
				var behaviourHeader = new VisualElement();
				var allParameters = new List<(string Parameter, VRCExpressionParameters.ValueType ValueType)>();
				if(c.BaseParameters && c.BaseParameters.parameters != null && c.BaseParameters.parameters.Count() > 0)
				{
					Toolkit.AddElement(behaviourHeader, new Label($"{c.BaseParameters.parameters.Count()} Base Parameters ({c.BaseParameters.CalcTotalCost()} Bits)"));
					foreach(var param in c.BaseParameters.parameters)
						allParameters.Add((param.name, param.valueType));
				}

				var behaviourList = new VisualElement();
				foreach(var handler in AvatarHandlerRegistryVRChat.Handlers)
				{
					foreach(IAvatarBehaviour behaviour in c.GetComponentsInChildren(handler.HandlesBehaviour).Cast<IAvatarBehaviour>())
					{
						var box = Toolkit.AddElement(behaviourList, new Box());
						box.style.marginTop = box.style.marginBottom = 5;
						box.style.paddingTop = box.style.paddingBottom = box.style.paddingLeft = 5;
						var behaviourLabel = Toolkit.AddElement(box, new Label($"<font-weight=700>{handler.Label}</font-weight>{(!string.IsNullOrWhiteSpace(behaviour.Name) ? ": " + behaviour.Name : "")}"));
						behaviourLabel.style.marginBottom = 5;

						var parameters = handler.GetParameters(behaviour);
						if(parameters != null && parameters.Count > 0)
						{
							allParameters.AddRange(parameters);

							var foldout = Toolkit.AddElement(box, new Foldout{ text = $"{parameters.Count()} Parameter{(parameters.Count() > 1 ? "s" : "")}", value = false, viewDataKey = $"ava_base_behaviour_parameters_{behaviour.name}_{behaviour.Name}" });
							var paramBox = Toolkit.AddElement(foldout, new VisualElement());
							paramBox.style.marginLeft = 10;
							foreach(var (Parameter, ValueType) in parameters)
							{
								Toolkit.AddElement(paramBox, new Label($"<size=-2>{Parameter}: {ValueType} ({(ValueType == VRCExpressionParameters.ValueType.Bool ? 1 : 8)} Bit)</size>"));
							}
						}

						if(handler.CreateGUI(behaviour) is VisualElement handlerGui)
						{
							handlerGui.style.paddingTop = handlerGui.style.paddingBottom = 5;
							handlerGui.style.marginLeft = 5;
							box.Add(handlerGui);
						}
					}
				}

				var parametersDeduped = new Dictionary<string, VRCExpressionParameters.ValueType>();
				var totalSize = 0;
				foreach(var parameter in allParameters)
				{
					if(!parametersDeduped.ContainsKey(parameter.Parameter))
					{
						parametersDeduped.Add(parameter.Parameter, parameter.ValueType);
						totalSize += parameter.ValueType == VRCExpressionParameters.ValueType.Bool ? 1 : 8;
					}
					else if(parametersDeduped[parameter.Parameter] != parameter.ValueType)
					{
						Toolkit.AddElement(behaviourList, new Label($"ERROR: Duplicate Parameter with incompatible Type: {parameter.Parameter}"));
					}
				}

				var paramSizeLabel = Toolkit.AddElement(behaviourHeader, new Label($"Total Parameters Size: {totalSize} / 256 Bits"));
				paramSizeLabel.style.marginTop = behaviourHeader.style.marginBottom = 5;

				behaviourListParent.Clear();
				behaviourListParent.Add(behaviourHeader);
				behaviourListParent.Add(behaviourList);
			}
			drawBehaviours();

			var refreshButton = Toolkit.AddElement(behaviourListParentParent, new Button());
			refreshButton.text = "Refresh";
			refreshButton.RegisterCallback<ClickEvent>(c => {
				drawBehaviours();
			});
			behaviourListParentParent.Add(behaviourListParent);

			Toolkit.AddSpacer(ui);

			var h_box = Toolkit.AddElement(ui, new VisualElement());
			h_box.style.flexDirection = FlexDirection.RowReverse;
			var h_spacer = Toolkit.AddElement(h_box, new VisualElement());

			var b = Toolkit.AddElement(h_box, new Button());
			b.Add(new Label("<size=+1><font-weight=700>Apply the Setup Now!</font-weight></size>"));
			b.RegisterCallback<ClickEvent>((e) =>
			{
				try
				{
					AvatarSetupVRChatApplier.Apply(AnimationPathUtil.GetRoot(c.transform).GetComponent<VRCAvatarDescriptor>(), c);
					Debug.Log("AVA setup created successfully! Find it under: Packages/com.squirrelbite.ava_base_setup/Output/");
				}
				finally
				{
				}
			});

			return ui;
		}
	}
}

#endif
#endif
