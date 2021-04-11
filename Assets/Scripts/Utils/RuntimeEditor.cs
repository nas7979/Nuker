using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RuntimeEditor : MonoBehaviour
{
#if UNITY_EDITOR
    private void Start()
    {
        GameEditor.Sword_Field = (int)SwordManager.Instance.SwordID;
        GameEditor.Score_Field = BackEndRTRank.Instance.MyRTRank.rankData.score;
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(RuntimeEditor))]
public class GameEditor : Editor
{
    public static int Sword_Field = 0;
    public static int Score_Field = 0;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("인게임 에디터");
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Sword Option");
        Sword_Field = EditorGUILayout.IntField("Target Sword ID : ", Sword_Field);
        if (GUILayout.Button("Change Sword"))
            SwordManager.Instance.ChangeSword((uint)Sword_Field);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Score Option");
        Score_Field = EditorGUILayout.IntField("Target Score : ", Score_Field);
        if (GUILayout.Button("Set Score"))
            BackEndRTRank.Instance.UpdateRTRank(Score_Field);


    }
}
#endif