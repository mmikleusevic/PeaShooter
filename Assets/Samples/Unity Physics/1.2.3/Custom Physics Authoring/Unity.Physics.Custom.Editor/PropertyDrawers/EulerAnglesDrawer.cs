using Unity.Physics.Authoring;
using UnityEditor;
using UnityEngine;

namespace Unity.Physics.Editor
{
    [CustomPropertyDrawer(typeof(EulerAngles))]
    internal class EulerAnglesDrawer : BaseDrawer
    {
        protected override bool IsCompatible(SerializedProperty property)
        {
            return true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty value = property.FindPropertyRelative(nameof(EulerAngles.Value));
            return EditorGUI.GetPropertyHeight(value);
        }

        protected override void DoGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty value = property.FindPropertyRelative(nameof(EulerAngles.Value));
            EditorGUI.PropertyField(position, value, label, true);
        }
    }
}