using System;
using System.IO;
using UnityEngine;

namespace CodeExtensions
{
    public static class ScriptableObjectExtension
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Asset from ScriptableObject", true)]
        private static bool CreateScriptableObjAsAssetValidator()
        {
            var activeObject = UnityEditor.Selection.activeObject;

            // make sure it is a text asset
            if ((activeObject == null) || !(activeObject is TextAsset))
            {
                return false;
            }

            // make sure it is a persistant asset
            var assetPath = UnityEditor.AssetDatabase.GetAssetPath(activeObject);
            if (assetPath == null)
            {
                return false;
            }

            // load the asset as a monoScript
            var monoScript =
                (UnityEditor.MonoScript) UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath,
                    typeof(UnityEditor.MonoScript));
            if (monoScript == null)
            {
                return false;
            }

            // get the type and make sure it is a scriptable object
            var scriptType = monoScript.GetClass();
            if (scriptType == null || !scriptType.IsSubclassOf(typeof(UnityEngine.ScriptableObject)))
            {
                return false;
            }

            return true;
        }

        [UnityEditor.MenuItem("Assets/Create/Asset from ScriptableObject")]
        private static void CreateScriptableObjectAssetMenuCommand(UnityEditor.MenuCommand command)
        {
            // we already validated this path, and know these calls are safe
            var activeObject = UnityEditor.Selection.activeObject;
            var assetPath = UnityEditor.AssetDatabase.GetAssetPath(activeObject);
            var script = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.MonoScript>(assetPath);
            CreateNewInstance(assetPath, script);
        }

        private static void CreateNewInstance(string assetPath, UnityEditor.MonoScript script)
        {
            var scriptType = script.GetClass();
            var path = Path.Combine(Path.GetDirectoryName(assetPath) ?? string.Empty, scriptType.Name + ".asset");
            try
            {
                var inst = UnityEngine.ScriptableObject.CreateInstance(scriptType);
                if (UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path))
                    path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path);
                UnityEditor.AssetDatabase.CreateAsset(inst, path);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
#endif
    }
}