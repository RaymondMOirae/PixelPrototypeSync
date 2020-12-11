using System;
using Prototype.Gameplay.Player;
using UnityEditor;
using UnityEngine;

namespace Prototype.Editor
{
    [CustomEditor(typeof(PlayerController))]
    public class EditorPlayerController : UnityEditor.Editor
    {
        private bool _showLeftAttackRange = true;
        private bool _showRightAttackRange = true;
        private bool _showMidAttackRange = true;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor Settings: ");
            _showLeftAttackRange = EditorGUILayout.ToggleLeft("Show Left Attack Range", _showLeftAttackRange);
            _showRightAttackRange = EditorGUILayout.ToggleLeft("Show Right Attack Range", _showRightAttackRange);
            _showMidAttackRange = EditorGUILayout.ToggleLeft("Show Mid Attack Range", _showMidAttackRange);
        }

        private void OnSceneGUI()
        {
            var controller = target as PlayerController;
            if (!controller) return;

            if (_showLeftAttackRange)
            {
                Handles.color = Color.red.WithAlpha(0.2f);
                Handles.DrawSolidArc(
                    controller.transform.position,
                    Vector3.forward,
                    MathUtility.Rotate(controller.CurDir, 0),
                    controller.SideOuterAngle,
                    controller.AttackRadius); 
            }

            if (_showRightAttackRange)
            {
                Handles.color = Color.cyan.WithAlpha(.2f);
                Handles.DrawSolidArc(
                    controller.transform.position,
                    Vector3.forward,
                    MathUtility.Rotate(controller.CurDir, 0),
                    -controller.SideOuterAngle,
                    controller.AttackRadius);
            }

            if (_showMidAttackRange)
            {
                Handles.color = Color.yellow.WithAlpha(.2f);
                Handles.DrawSolidArc(
                    controller.transform.position,
                    Vector3.forward, 
                    MathUtility.Rotate(controller.CurDir, -controller.MidOuterAngle * Mathf.Deg2Rad),
                    2 * controller.MidOuterAngle,
                    controller.AttackRadius);
            }
            
            
            
        }
    }
}