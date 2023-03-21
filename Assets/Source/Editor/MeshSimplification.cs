using UnityEditor;
using UnityEngine;
using UnityMeshSimplifier;

namespace Source.Editor
{
    public class MeshSimplification : MonoBehaviour
    {
        [MenuItem("GameObject/Simplify Mesh Lossless", true, -11)]
        public static bool SimplifyMeshLosslessValidateFunction()
        {
            GameObject selectedGameObject = Selection.activeGameObject;
            return selectedGameObject != null &&
                   selectedGameObject.TryGetComponent<MeshFilter>(out MeshFilter meshFilter);
        }

        [MenuItem("GameObject/Simplify Mesh Lossless", false, -11)]
        public static void SimplifyMeshLossless()
        {
            GameObject selectedGameObject = Selection.activeGameObject;
            if (selectedGameObject != null &&
                selectedGameObject.TryGetComponent<MeshFilter>(out MeshFilter meshFilter) &&
                meshFilter != null)
            {
                Mesh mesh = meshFilter.sharedMesh;

                MeshSimplifier meshSimplifier = new MeshSimplifier(mesh);
                meshSimplifier.Verbose = true;
                
                SimplificationOptions simplificationOptions = SimplificationOptions.Default;
                simplificationOptions.PreserveBorderEdges = true;
                simplificationOptions.PreserveUVSeamEdges = true;
                simplificationOptions.PreserveUVFoldoverEdges = true;
                simplificationOptions.PreserveSurfaceCurvature = true;

                meshSimplifier.SimplificationOptions = simplificationOptions;
                
                meshSimplifier.SimplifyMeshLossless();

                meshFilter.sharedMesh = meshSimplifier.ToMesh();
            }
        }
        
        [MenuItem("GameObject/Simplify Mesh Quality 60", true, -11)]
        public static bool SimplifyMeshQuality60ValidateFunction()
        {
            GameObject selectedGameObject = Selection.activeGameObject;
            return selectedGameObject != null &&
                   selectedGameObject.TryGetComponent<MeshFilter>(out MeshFilter meshFilter);
        }

        [MenuItem("GameObject/Simplify Mesh Quality 60", false, -11)]
        public static void SimplifyMeshQuality60()
        {
            GameObject selectedGameObject = Selection.activeGameObject;
            if (selectedGameObject != null &&
                selectedGameObject.TryGetComponent<MeshFilter>(out MeshFilter meshFilter) &&
                meshFilter != null)
            {
                Mesh mesh = meshFilter.sharedMesh;

                MeshSimplifier meshSimplifier = new MeshSimplifier(mesh);
                meshSimplifier.Verbose = true;
                
                SimplificationOptions simplificationOptions = SimplificationOptions.Default;
                simplificationOptions.PreserveBorderEdges = true;
                simplificationOptions.PreserveUVSeamEdges = true;
                simplificationOptions.PreserveUVFoldoverEdges = true;
                simplificationOptions.PreserveSurfaceCurvature = true;

                meshSimplifier.SimplificationOptions = simplificationOptions;
                
                meshSimplifier.SimplifyMesh(0.6f);

                meshFilter.sharedMesh = meshSimplifier.ToMesh();
            }
        }
    }
}