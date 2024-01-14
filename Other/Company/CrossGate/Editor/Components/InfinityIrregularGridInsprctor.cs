using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace SC
{
    [CustomEditor(typeof(InfinityIrregularGrid))]
    public class InfinityIrregularGridInsprctor : Editor
    {
        InfinityIrregularGrid _grid = null;
        InfinityIrregularGrid grid
        {
            get
            {
                if (_grid == null)
                    _grid = target as InfinityIrregularGrid;
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

            _foldout = EditorGUILayout.Foldout(_foldout, "Padding");
            if (_foldout)
            {
                grid.Left = EditorGUILayout.IntField("Left", grid.Left);
                grid.Right = EditorGUILayout.IntField("Right", grid.Right);
                grid.Top = EditorGUILayout.IntField("Top", grid.Top);
                grid.Bottom = EditorGUILayout.IntField("Bottom", grid.Bottom);
                EditorGUILayout.Space();
            }
            grid.Spacing = EditorGUILayout.IntField("Spacing", grid.Spacing);

            grid.StartAxis = (GridLayoutGroup.Axis)EditorGUILayout.EnumPopup("StartAxis", grid.StartAxis);
            grid.MinSize = EditorGUILayout.IntField("MinSize", grid.MinSize);
            grid.MaxSize = EditorGUILayout.IntField("MaxSize", grid.MaxSize);

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