using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Prototype.Gameplay.Enemy;
using System;

namespace Prototype.Editor
{
    [CustomEditor(typeof(Enemy),true)]
    public class EditorEnemy : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Enemy script = target as Enemy;

            DrawOptionalField(ref script.UseMeleeSnesor, "近战", ref script._meleeAttackRadius, "近战半径");
            DrawOptionalField(ref script.UseRangedSensor, "远程", ref script._rangedAttackRadius, "远程半径");

            //EditorGUILayout.Toggle("InView", script.SensorResult.InViewField);
            //EditorGUILayout.Toggle("InAttack", script.SensorResult.InAttackField);

        }

        private void DrawOptionalField(ref bool visibility, string vLabel, ref float field, string fLabel)
        {
            visibility = EditorGUILayout.Toggle(vLabel, visibility);
            if(visibility == true)
            {
                EditorGUI.indentLevel++;
                field = EditorGUILayout.FloatField(fLabel, field);
                EditorGUI.indentLevel--;
            }

        }


    }

}

