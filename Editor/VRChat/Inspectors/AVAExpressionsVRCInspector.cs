#if UNITY_EDITOR
#if AVA_BASE_SETUP_VRCHAT

using System.Collections.Generic;
using System.Linq;
using com.squirrelbite.ava_base_setup.util;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.squirrelbite.ava_base_setup.vrchat
{

	[CustomEditor(typeof(AVAExpressionsVRC))]
	public class AVAExpressionsVRCInspector : Editor
	{
		static readonly Color SpacerColor = new(0.17f, 0.17f, 0.17f);

		private int FTMatch = -1;

		void OnEnable()
		{
		}

		public override VisualElement CreateInspectorGUI()
		{
			var c = (AVAExpressions)target;
			VisualElement ui = new();

			var l = Toolkit.AddElement(ui, new Label("<size=+1><font-weight=700>Avatar Expressions</font-weight></size>"));
			l.style.marginTop = l.style.marginBottom = 5;

			Toolkit.AddList(ui, serializedObject.FindProperty("Expressions"));

			// Unity plz
			/*var v = Toolkit.AddElement(ui, new MultiColumnListView());
			v.showAddRemoveFooter = true;
			v.showBoundCollectionSize = false;
			v.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
			v.reorderable = true;
			v.reorderMode = ListViewReorderMode.Animated;
			v.showBorder = true;
			v.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
			
			v.itemsSource = c.Expressions;

			v.itemsAdded += (e) => {
				Debug.Log(e);
				Debug.Log(e.Count());
				Debug.Log(c.Expressions.Count());
				Debug.Log((e as List<int>)[0]);
				//c.Expressions[(e as List<int>)[0]] = new AvatarExpression();
				//v.RefreshItems();
				v.Rebuild();
			};
			//v.BindProperty(serializedObject.FindProperty("Expressions"));

			v.columns.Add(new Column { title = "Expression Meaning", minWidth = 100, stretchable = true, optional = false });
			v.columns.Add(new Column { title = "Animation", minWidth = 100, stretchable = true, optional = false });

			v.columns[0].makeCell = () => new TextField();
			v.columns[1].makeCell = () => new ObjectField { objectType = typeof(AnimationClip) };

			v.columns[0].bindCell = (element, index) => (element as TextField).BindProperty(serializedObject.FindProperty("Expressions").GetArrayElementAtIndex(index).FindPropertyRelative("Expression"));
			v.columns[1].bindCell = (element, index) => (element as ObjectField).BindProperty(serializedObject.FindProperty("Expressions").GetArrayElementAtIndex(index).FindPropertyRelative("Animation"));
			*/

			return ui;
		}
	}
}

#endif
#endif
