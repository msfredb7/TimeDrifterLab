﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace CCC.InspectorDisplay
{
    [CustomPropertyDrawer(typeof(AlwaysExpandAttribute))]
    public class AlwaysExpandDrawer : PropertyDrawer
    {
        GUIContent _cachedLabel = new GUIContent();
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = 0;
            _cachedLabel.text = label.text;

            foreach (SerializedProperty child in new DirectChildEnumerator(property))
            {
                if (ShouldUseRealPropertyName)
                    _cachedLabel.text = property.displayName;

                totalHeight += EditorGUI.GetPropertyHeight(property, _cachedLabel, child.hasVisibleChildren) + 2 /*padding*/;
            }
            return totalHeight - 2;  // we have to subtract 2px of extra padding that unity added because of the last child
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.isExpanded = false; // needed to prevent flickering in unity's drawing

            _cachedLabel.text = label.text;

            foreach (SerializedProperty child in new DirectChildEnumerator(property))
            {
                if (ShouldUseRealPropertyName)
                    _cachedLabel.text = property.displayName;

                position.height = EditorGUI.GetPropertyHeight(property, _cachedLabel, child.hasVisibleChildren);
                EditorGUI.PropertyField(position, property, _cachedLabel, child.isExpanded);
                position.y += position.height + 2 /*padding*/;
            }
        }

        bool ShouldUseRealPropertyName => ((AlwaysExpandAttribute)attribute).UseRootPropertyName == false;

        struct DirectChildEnumerator
        {
            SerializedProperty _property;
            bool _enterChildren;
            string _parentPath;
            public DirectChildEnumerator(SerializedProperty property)
            {
                _property = property;
                _enterChildren = property.hasChildren;
                _parentPath = property.propertyPath;
            }

            // Enumerator interface
            public DirectChildEnumerator GetEnumerator() => this;
            public SerializedProperty Current => _property;
            public bool MoveNext()
            {
                bool result = _property.Next(_enterChildren);

                _enterChildren = false;

                return result && _property.propertyPath.Contains(_parentPath);
            }
        }
    }
}