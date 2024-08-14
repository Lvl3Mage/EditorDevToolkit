using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Lvl3Mage.EditorDevToolkit.Editor
{
    // /// <author>
    // ///   HiddenMonk
    // ///   http://answers.unity3d.com/users/496850/hiddenmonk.html
    // /// </author>
    // /// <remarks>
    // /// See also http://answers.unity3d.com/questions/627090/convert-serializedproperty-to-custom-class.html
    // /// </remarks>
    
    
    /// <summary>
    /// A class that provides helper functions for working with the Unity Editor
    /// </summary>
	public static class EditorUtils
    {
	    //Todo Fix -This breaks when the property is in an array since it treats the array index in the path as a field (ie "myArray.0.myField" 0 is treated as a field name)
        // public static T SerializedPropertyToObject<T>(SerializedProperty property)
        // {
        //     return GetNestedObject<T>(property.propertyPath, property.serializedObject.targetObject, true); //The "true" means we will also check all base classes
        // }
        //
        // public static Component GetSerializedPropertyRootComponent(SerializedProperty property)
        // {
        //     return (Component)property.serializedObject.targetObject;
        // }

        // public static T GetNestedObject<T>(string path, object obj, bool includeAllBases = false)
        // {
        //     foreach(string part in path.Split('.'))
        //     {
        //         obj = GetFieldOrPropertyValue<object>(part, obj, includeAllBases);
        //     }
        //     return (T)obj;
        // }
        
        /// <summary>
        /// Gets a sibling property of the given property
        /// </summary>
        /// <param name="property">
        /// The property to get the sibling of
        /// </param>
        /// <param name="siblingName">
        /// The name of the sibling property
        /// </param>
        /// <returns>
        /// The sibling property, or null if not found
        /// </returns>
		public static SerializedProperty GetSiblingProperty(SerializedProperty property, string siblingName){
			string propPath = property.propertyPath;
			string[] path = propPath.Split(".");
			StringBuilder pathBuilder = new StringBuilder("");
			for(int i = 0; i < path.Length-1; i++){
				pathBuilder.Append(path[i]);
				if(i != path.Length-1){
					pathBuilder.Append(".");
				}
			}
			pathBuilder.Append(siblingName);
			return property.serializedObject.FindProperty(pathBuilder.ToString());
		}
        /// <summary>
        /// Computes the rect for a property and increments the drawHeight
        /// </summary>
        /// <param name="prop">
        /// The property to get the rect for
        /// </param>
        /// <param name="x">
        /// The x position of the rect
        /// </param>
        /// <param name="width">
        /// The width of the rect
        /// </param>
        /// <param name="drawHeight">
        /// The current height of the draw. This will be incremented by the height of the property
        /// </param>
        /// <returns>
        /// The rect for the property
        /// </returns>
        public static Rect PropertyRect(SerializedProperty prop, float x, float width, ref float drawHeight){
			float propHeight = EditorGUI.GetPropertyHeight(prop, true);
			var propRect = new Rect(x, drawHeight, width, propHeight);
			
			if(propHeight > 0) drawHeight += EditorGUIUtility.standardVerticalSpacing;
			
			drawHeight += propRect.height;
			return propRect;
		}
        /// <summary>
        /// Computes a rect for a line and increments the drawHeight
        /// </summary>
        /// <param name="x">
        /// The x position of the rect
        /// </param>
        /// <param name="width">
        /// The width of the rect
        /// </param>
        /// <param name="drawHeight">
        /// The current height of the draw. This will be incremented by the height of the line
        /// </param>
        /// <returns>
        /// The rect for the line
        /// </returns>
		public static Rect LineRect(float x, float width, ref float drawHeight){
			var propRect = new Rect(x, drawHeight, width, EditorGUIUtility.singleLineHeight);
			drawHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			return propRect;
		}
		/// <summary>
		/// Gets the parent object of a serialized property. Useful for reflection
		/// </summary>
		/// <param name="parentProp">
		/// The property to get the parent object of
		/// </param>
		/// <returns>
		/// The parent object of the property
		/// </returns>
		public static object GetParentObject(SerializedProperty parentProp)
		{
			var path = parentProp.propertyPath.Split(".");
			object obj = parentProp.serializedObject.targetObject;
			int i = 0;
			while(true){
				if(i >= path.Length-1){
					return obj;
				}
				//Get field 
				FieldInfo field = obj.GetType().GetField(path[i], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				if(field == null){
					Debug.LogError($"Cannot find field {path[i]} in object {obj}");
					return null;
				}
				i++;
				//IF following path is an array
				if (path[i] == "Array"){//reads from the array and skips it
					
					i++;// Skip ARRAY keyword
					
					string[] split = path[i].Split('[', ']');
					int enumerableIndex = Convert.ToInt32(split[1]);
					
					object value = field.GetValue(obj);
					if(value == null){
						Debug.LogError("Array is null");
						return null;
					}
					
					IEnumerable<object> enumerable = value as IEnumerable<object>;
					if(enumerable == null){
						Debug.LogError("Value is not an enumerable");
						return null;
					}
					obj = enumerable.ElementAt(enumerableIndex);
					i++;
					continue;
				}
				
				obj = field.GetValue(obj);
			}
		}

/*
        public static T GetFieldOrPropertyValue<T>(string fieldName, object obj, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if(field != null) return (T)field.GetValue(obj);

            PropertyInfo property = obj.GetType().GetProperty(fieldName, bindings);
            if(property != null) return (T)property.GetValue(obj, null);

            if(includeAllBases)
            {
                foreach(Type type in GetBaseClassesAndInterfaces(obj.GetType()))
                {
                    field = type.GetField(fieldName, bindings);
                    if(field != null) return (T)field.GetValue(obj);
                    

                    property = type.GetProperty(fieldName, bindings);
                    if(property != null) return (T)property.GetValue(obj, null);
                }
            }

            return default(T);
        }
*/
        

/*
        public static void SetFieldOrPropertyValue<T>(string fieldName, object obj, object value, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if(field != null)
            {
                field.SetValue(obj, value);
                return;
            }

            PropertyInfo property = obj.GetType().GetProperty(fieldName, bindings);
            if(property != null)
            {
                property.SetValue(obj, value, null);
                return;
            }

            if(includeAllBases)
            {
                foreach(Type type in GetBaseClassesAndInterfaces(obj.GetType()))
                {
                    field = type.GetField(fieldName, bindings);
                    if(field != null)
                    {
                        field.SetValue(obj, value);
                        return;
                    }

                    property = type.GetProperty(fieldName, bindings);
                    if(property != null)
                    {
                        property.SetValue(obj, value, null);
                        return;
                    }
                }
            }
        }
*/

/*
        public static IEnumerable<Type> GetBaseClassesAndInterfaces(this Type type, bool includeSelf = false)
        {
            List<Type> allTypes = new List<Type>();

            if(includeSelf) allTypes.Add(type);

            if(type.BaseType == typeof(object))
            {
                allTypes.AddRange(type.GetInterfaces());
            }else{
                allTypes.AddRange(
                        Enumerable
                        .Repeat(type.BaseType, 1)
                        .Concat(type.GetInterfaces())
                        .Concat(type.BaseType.GetBaseClassesAndInterfaces())
                        .Distinct());
                     //I found this on stackoverflow
            }

            return allTypes;
        }
*/
            
    }
}