using UnityEngine;
using UnityEditor;

public class ReplaceWithPrefab : EditorWindow
{
    public GameObject prefab;

    [MenuItem("Tools/Replace With Prefab")]
    static void Open()
    {
        GetWindow<ReplaceWithPrefab>("Replace With Prefab");
    }

    void OnGUI()
    {
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        if (GUILayout.Button("Replace Selected"))
        {
            Replace();
        }
    }

    void Replace()
    {
        GameObject[] selected = Selection.gameObjects;

        for (int i = 0; i < selected.Length; i++)
        {
            GameObject old = selected[i];

            GameObject copy = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            Undo.RegisterCreatedObjectUndo(copy, "Replace With Prefab");

            copy.transform.SetParent(old.transform.parent);
            copy.transform.localPosition = old.transform.localPosition;
            copy.transform.localRotation = old.transform.localRotation;
            copy.transform.localScale = old.transform.localScale;
            copy.transform.SetSiblingIndex(old.transform.GetSiblingIndex());
            copy.name = old.name;

            Undo.DestroyObjectImmediate(old);
        }
    }
}