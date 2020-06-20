using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorUtils
{
  public static T CreateNewAssetAtPath<T>(string path) where T : Object
  {
    path = AssetDatabase.GenerateUniqueAssetPath(path);
    ScriptableObject so = ScriptableObject.CreateInstance(typeof(T));
    AssetDatabase.CreateAsset(so, path);

    EditorUtility.SetDirty(so);
    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();

    return AssetDatabase.LoadAssetAtPath<T>(path);
  }

  public static T AddDisabledMonoBehaviour<T>(Component component) where T : MonoBehaviour
  {
    CleanComponent<T>(component);

    T newBehaviour = component.gameObject.AddComponent<T>();
    newBehaviour.enabled = false;

    return newBehaviour;
  }

  public static void CleanComponent<T>(Component component) where T : Component
  {
    T t = component.GetComponent<T>();
    if (t)
      Object.DestroyImmediate(t);
  }

  public static void DrawAddItem<T>(List<T> list, string name) where T : Component
  {
    T item = null;
    item = EditorGUILayout.ObjectField("Add " + name, item, typeof(T), true) as T;
    if (item) {
      List<T> newItems = new List<T>();
      foreach (GameObject go in Selection.gameObjects) {
        item = go.GetComponent<T>();
        if (item)
          list.Add(item);
      }
    }
  }

  public static void DrawList<T>(List<T> list, string name, ref bool foldout) where T : Component
  {
    foldout = EditorGUILayout.Foldout(foldout, name);
    if (foldout) {
      ++EditorGUI.indentLevel;

      T itemToRemove = null;
      foreach (T item in list) {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("X"))
          itemToRemove = item;
        EditorGUILayout.ObjectField(item, typeof(T), true);
        EditorGUILayout.EndHorizontal();
      }

      --EditorGUI.indentLevel;

      if (itemToRemove)
        list.Remove(itemToRemove);
    }
  }

  public static void DrawLine(Color color, int thickness = 2, int padding = 10)
  {
    Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
    r.height = thickness;
    r.y += padding / 2;
    r.width -= 2;
    EditorGUI.DrawRect(r, color);
  }
}
