using Prototype.Gameplay.Player;
using UnityEditor;

namespace Prototype.Editor
{
    [CustomEditor(typeof(ItemPicker))]
    public class EditorItemPicker : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var picker = target as ItemPicker;

            EditorGUI.BeginChangeCheck();
            var layerMask =  EditorGUILayout.LayerField("Item Layer", picker.LayerMask);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Edit Item Layer");
                picker.LayerMask = layerMask;
            }
            

        }
    }
}