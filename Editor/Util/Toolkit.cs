#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.squirrelbite.ava_base_setup.util
{
	public static class Toolkit
	{
		public static T AddElement<T>(VisualElement Container, T Element) where T : VisualElement
		{
			Container.Add(Element);
			return Element;
		}

		public static VisualElement AddList(VisualElement Container, SerializedProperty Property, VisualElement Header = null)
		{
			var outer = new VisualElement();

			if(Header != null) outer.Add(Header);

			var v = new ListView();
			outer.Add(v);
			v.showAddRemoveFooter = true;
			v.showBoundCollectionSize = false;
			v.reorderable = true;
			v.reorderMode = ListViewReorderMode.Animated;
			v.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
			v.showBorder = true;
			v.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
			v.BindProperty(Property);

			Container.Add(outer);
			return outer;
		}

		public static VisualElement AddSpacer(VisualElement Container)
		{
			var spacer = AddElement(Container, new VisualElement());
			spacer.style.marginTop = 5;
			spacer.style.borderBottomWidth = 5;
			spacer.style.marginBottom = 5;
			spacer.style.borderBottomColor = new StyleColor(new Color(0.17f, 0.17f, 0.17f));
			return spacer;
		}
	}
}

#endif
