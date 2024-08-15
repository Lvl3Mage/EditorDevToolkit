using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Lvl3Mage.EditorDevToolkit.Editor
{
	/// <summary>
	/// A utility class that can be used to draw multiple elements in a vertical layout.
	/// </summary>
	public class VerticalLayout
	{
		public struct Element
		{
			public Action<Rect> draw;
			public Func<Rect,float> getHeight;
			public bool indent;
			public int order;
			public Element(Action<Rect> draw, Func<Rect,float> getHeight, int order, bool indent)
			{
				this.draw = draw;
				this.getHeight = getHeight;
				this.indent = indent;
				this.order = order;
			}
		}

		readonly List<Element> elements = new();
		/// <summary>
		/// Adds an element to the layout.
		/// </summary>
		/// <param name="draw">
		/// The action that will be called to draw the element. The action should draw the element at the given rect.
		/// </param>
		/// <param name="getHeight">
		/// A function that returns the height of the element given the rect it will be drawn at.
		/// </param>
		/// <param name="order">
		/// The order in which the element will be drawn. Elements with lower order will be drawn first.
		/// </param>
		/// <param name="indent">
		/// Whether the element should be indented.
		/// </param>
		public void Add(Action<Rect> draw, Func<Rect,float> getHeight, int order = 0, bool indent = true)
		{
			for (int i = 0; i < elements.Count; i++)
			{
				int currentOrder = elements[i].order;
				if (order < currentOrder)
				{
					elements.Insert(i, new Element(draw, getHeight, order, indent));
					return;
				}
			}
			elements.Add(new Element(draw, getHeight, order, indent));
		}
		public void Add(Action<Rect> draw, float height, int order = 0, bool indent = true)
		{
			Add(draw, _ => height, order, indent);
		}
		public void Draw(Rect drawArea)
		{
			float y = drawArea.y;
			foreach (var element in elements)
			{
				float height = element.getHeight(drawArea);
				Rect elementRect = new Rect(drawArea.x, y, drawArea.width, height);
				if (element.indent)
				{
					elementRect = EditorGUI.IndentedRect(elementRect);
				}
				element.draw(elementRect);
				y += height + EditorGUIUtility.standardVerticalSpacing;
			}
		}
		public float GetHeight()
		{
			float height = 0;
			foreach (var element in elements)
			{
				
				//Todo this breaks a little when the label wraps
				Rect area = new Rect(0,0,EditorGUIUtility.currentViewWidth,0);
				if (element.indent){
					
					area = EditorGUI.IndentedRect(area);
				}
				
				height += element.getHeight(area);
			}
			return height + (elements.Count - 1) * EditorGUIUtility.standardVerticalSpacing;
		}
	}
}