using UnityEditor;
using SC;
using UnityEngine;
using UnityEngine.UI;
namespace SC
{
    [CustomEditor(typeof(InfinityGrid))]
    public class ScrollRectGridInsprctor : Editor
    {
        InfinityGrid _grid = null;
        InfinityGrid grid
        {
            get
            {
                if (_grid == null)
                    _grid = target as InfinityGrid;
                return _grid;
            }
        }

        bool _foldout = false;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(Application.isPlaying)
            {
                EditorGUILayout.LabelField(string.Format("Cell Count {0}", grid.CellCount));
            }
            else
            {
                grid.CellCount = EditorGUILayout.IntField("Cell Count", grid.CellCount);
            }

            EditorGUI.BeginChangeCheck();
            _foldout = EditorGUILayout.Foldout(_foldout, "Padding");
            if (_foldout)
            {
                grid.Left = EditorGUILayout.IntField("Left", grid.Left);
                grid.Right = EditorGUILayout.IntField("Right", grid.Right);
                grid.Top = EditorGUILayout.IntField("Top", grid.Top);
                grid.Bottom = EditorGUILayout.IntField("Bottom", grid.Bottom);
                EditorGUILayout.Space();
            }            

            grid.CellSize = EditorGUILayout.Vector2Field("Cell Size", grid.CellSize);
            grid.Spacing = EditorGUILayout.Vector2Field("Spacing", grid.Spacing);
            grid.StartAxis = (GridLayoutGroup.Axis)EditorGUILayout.EnumPopup("StartAxis", grid.StartAxis);
            if (grid.StartAxis == GridLayoutGroup.Axis.Horizontal)
            {
                grid.AxisLimit = EditorGUILayout.IntField("Horizontal Count", grid.AxisLimit);
            }
            else
            {
                grid.AxisLimit = EditorGUILayout.IntField("Vertical Count", grid.AxisLimit);
            }

            if(EditorGUI.EndChangeCheck())
            {
                grid.Update();
            }

            if (GUILayout.Button("clear"))
            {
                grid.Clear();
            }
            if (GUILayout.Button("test"))
            {
                grid.Update();
            }
        }
    }
}