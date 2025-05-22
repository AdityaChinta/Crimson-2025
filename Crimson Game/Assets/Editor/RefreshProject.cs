using UnityEditor;  // Required to access AssetDatabase
using UnityEngine;

public class RefreshProject
{
    [MenuItem("Tools/Refresh Assets")]
    public static void RefreshAssets()
    {
        AssetDatabase.Refresh();
        Debug.Log("Assets refreshed!");
    }
}
