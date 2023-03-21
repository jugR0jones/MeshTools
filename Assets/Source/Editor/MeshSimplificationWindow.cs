using System;
using UnityEditor;
using UnityEngine;

namespace Source.Editor
{
    public class MeshSimplificationWindow : EditorWindow
    {
        #region Constants
        
        private const float WindowWidth = 1000.0f;
        private const float WindowHeight = 605.0f;

        private const float ObjectSelectionHeight = 20.0f;

        private const float Margin = 5.0f;

        private const float PreviewWidth = 700.0f - Margin - Margin / 2.0f;
        private const float PreviewHeight = WindowHeight - Margin - Margin - ObjectSelectionHeight;
        private const float PreviewX = Margin;
        private const float PreviewY = ObjectSelectionHeight + Margin;

        private const float OptionsToolbarWidth = 300.0f - Margin - Margin / 2.0f;
        private const float OptionsToolbarX = WindowWidth - OptionsToolbarWidth - Margin;

        #endregion

        [MenuItem("Meshes/Mesh Simplification")]
        public static void ShowWindow()
        {
            MeshSimplificationWindow meshSimplificationWindow = GetWindow<MeshSimplificationWindow>("Mesh Simplification");
            meshSimplificationWindow.minSize = new Vector2(WindowWidth, WindowHeight);
            meshSimplificationWindow.maxSize = new Vector2(WindowWidth, WindowHeight);
            meshSimplificationWindow.Show();
        }

        private GameObject gameObject;

        private GameObject gameObjectToEdit;

        // private Editor gameObjectEditor;
        private MeshSimplificationEditor gameObjectEditor;
        private Texture2D previewBackgroundTexture;

        private float zoomLevel = 0;

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            gameObject = (GameObject) EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

            if (EditorGUI.EndChangeCheck())
            {
                if (gameObjectEditor != null)
                {
                    DestroyImmediate(gameObjectEditor);
                    DestroyImmediate(gameObjectToEdit);
                }

                if (gameObjectEditor == null)
                {
                    gameObjectToEdit = Instantiate(gameObject);
                    gameObjectEditor = (MeshSimplificationEditor) UnityEditor.Editor.CreateEditor(gameObjectToEdit, typeof(MeshSimplificationEditor));
                    gameObjectEditor.Initialise();
                }
            }

            GUIStyle bgColor = new GUIStyle();

            bgColor.normal.background = previewBackgroundTexture;

            if (gameObjectToEdit != null)
            {
                if (gameObjectEditor == null)
                {
                    //  gameObjectEditor = Editor.CreateEditor(gameObjectToEdit);
                    //      gameObjectEditor = AssetViewEditor.CreateEditor(gameObjectToEdit);

                    // gameObjectEditor = (AssetViewEditor) Editor.CreateEditor(gameObjectToEdit, typeof(AssetViewEditor));
                    zoomLevel = 1.0f;
                }

                gameObjectEditor.OnInteractivePreviewGUI(new Rect(PreviewX, PreviewY, PreviewWidth, PreviewHeight), bgColor);
            }

            EditorGUI.BeginDisabledGroup(gameObjectEditor != null);
            EditorGUI.LabelField(new Rect(OptionsToolbarX, 30, OptionsToolbarWidth, 25), "Zoom");
            zoomLevel = EditorGUI.Slider(new Rect(OptionsToolbarX + 60.0f, 30.0f, OptionsToolbarWidth - 60.0f, 25), zoomLevel, 0.01f, 4.0f);

            if (gameObjectEditor != null)
            {
                gameObjectEditor.SetZoomLevel(in zoomLevel);
            }

            EditorGUI.EndDisabledGroup();
        }

        // private void OnInspectorUpdate()
        // {
        //TODO: Use this event to determine if the window size changed.
        //     Debug.Log("Window resized");
        // }

//     private void OnInspectorUpdate()
//     {
//         if (gameObjectToEdit != null)
//         {
//             gameObjectToEdit.transform.localScale = new Vector3(zoomLevel, zoomLevel, zoomLevel);
// gameObjectEditor.ResetTarget();
//             gameObjectEditor.Repaint();
//         }
//     }

        //[CustomEditor(typeof(MeshFilter))]
        //[CanEditMultipleObjects]
        public class MeshSimplificationEditor : UnityEditor.Editor
        {
            #region Private Fields

            private PreviewRenderUtility previewRenderUtility;
            private GameObject targetGameObject;
            private MeshFilter targetMeshFilter;
            private MeshRenderer targetMeshRenderer;

            private Vector2 drag;

            #endregion

            #region Public Methods

            public void SetZoomLevel(in float zoomLevel)
            {

            }

            #endregion

            #region Editor Methods

            public override bool HasPreviewGUI()
            {
                //        ValidateData();

                return false;
            }

            #endregion

            public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
            {
                drag = Drag2D(drag, r);

                if (Event.current.type == EventType.Repaint)
                {
                    if (targetGameObject == null)
                    {
                        EditorGUI.DropShadowLabel(r, "GameObject Required");
                    }
                    else
                    {
                        previewRenderUtility.BeginPreview(r, background);
//
//
//                     //   previewRenderUtility.DrawMesh(targetMeshFilter.sharedMesh, Matrix4x4.identity, targetMeshRenderer.sharedMaterial, 0);
//                     //
//                     // previewRenderUtility.camera.transform.position = Vector3.zero;
//                     // previewRenderUtility.camera.transform.rotation = Quaternion.Euler(new Vector3(-drag.y, -drag.x, 0));
//                     // previewRenderUtility.camera.transform.position = previewRenderUtility.camera.transform.forward * -1f;
//                     // previewRenderUtility.camera.Render();
//

                        Bounds bounds = targetMeshFilter.sharedMesh.bounds;

                        // we are technically rendering everything in the scene, so scene fog might affect it...
                        bool fog = RenderSettings.fog;
                        Unsupported.SetRenderSettingsUseFogNoDirty(false);

//     //TODO: Need to set the pivot or local rotation instead. So it rotates around the centre of the mesh

                        Vector3 position = -bounds.center;
                        Quaternion rotation = Quaternion.Euler(new Vector3(-drag.y, -drag.x, 0.0f));

                        // sub-mesh support, in case the mesh is made of multiple parts
                        int subMeshCount = targetMeshFilter.sharedMesh.subMeshCount;
                        if (subMeshCount > 1)
                        {
                            for (int i = 0; i < subMeshCount; i++)
                            {
                                previewRenderUtility.DrawMesh(targetMeshFilter.sharedMesh, position, rotation,
                                    targetMeshRenderer.sharedMaterial, i);
                            }
                        }
                        else
                        {
                            previewRenderUtility.DrawMesh(targetMeshFilter.sharedMesh, position, rotation,
                                targetMeshRenderer.sharedMaterial, 0);
                        }

                        float magnitude = bounds.extents.magnitude;
                        float distance = 8.0f * magnitude;

                        // setup the ObjectPreview's camera
                        previewRenderUtility.camera.backgroundColor = Color.gray;
                        previewRenderUtility.camera.clearFlags = CameraClearFlags.Color;
                        previewRenderUtility.camera.transform.position = new Vector3(position.x, position.y, -distance);
                        previewRenderUtility.camera.transform.rotation = Quaternion.identity;
                        previewRenderUtility.camera.nearClipPlane = 0.3f;
                        previewRenderUtility.camera.farClipPlane = distance + magnitude * 1.1f;

                        // VERY IMPORTANT: this manually tells the camera to render and produce the render texture
                        previewRenderUtility.camera.Render();

                        // reset the scene's fog from before
                        Unsupported.SetRenderSettingsUseFogNoDirty(fog);

                        Texture resultRender = previewRenderUtility.EndPreview();
                        GUI.DrawTexture(r, resultRender, ScaleMode.StretchToFill, false);
                    }
                }
            }

            public override void OnPreviewSettings()
            {
                if (GUILayout.Button("Reset Camera", EditorStyles.whiteMiniLabel))
                {
                    drag = Vector2.zero;
                }
            }

            #region Unity Events

            private void OnDestroy()
            {
                if (previewRenderUtility == null)
                {
                    return;
                }
                
                previewRenderUtility.Cleanup();
                previewRenderUtility = null;
            }

            private void OnDisable()
            {
                if (previewRenderUtility == null)
                {
                    return;
                }
                
                previewRenderUtility.Cleanup();
                previewRenderUtility = null;
            }

            #endregion

            #region Private Methods

            public void Initialise()
                // private void ValidateData()
            {
                if (previewRenderUtility == null)
                {
                    previewRenderUtility = new PreviewRenderUtility();
                    previewRenderUtility.camera.transform.position = new Vector3(0.0f, 0.0f, -0.0f);
                    previewRenderUtility.camera.transform.rotation = Quaternion.identity;
                }

                targetGameObject = target as GameObject;
                if (targetGameObject != null)
                {
                    // previewRenderUtility.AddSingleGO(targetGameObject);
                    targetGameObject = target as GameObject;
                    targetMeshFilter = targetGameObject.GetComponent<MeshFilter>();
                    targetMeshRenderer = targetGameObject.GetComponent<MeshRenderer>();

                    // Don't save the preview
                    targetGameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
                }
            }

            private static Vector2 Drag2D(in Vector2 scrollPosition, in Rect position)
            {
                Vector2 result = scrollPosition;

                int controlID = GUIUtility.GetControlID("Slider".GetHashCode(), FocusType.Passive);
                Event current = Event.current;
                switch (current.GetTypeForControl(controlID))
                {
                    case EventType.MouseDown:
                    {
                        if (position.Contains(current.mousePosition) && position.width > 50f)
                        {
                            GUIUtility.hotControl = controlID;
                            current.Use();
                            EditorGUIUtility.SetWantsMouseJumping(1);
                        }
                    }
                        break;

                    case EventType.MouseUp:
                    {
                        if (GUIUtility.hotControl == controlID)
                        {
                            GUIUtility.hotControl = 0;
                        }

                        EditorGUIUtility.SetWantsMouseJumping(0);
                    }
                        break;

                    case EventType.MouseDrag:
                    {
                        if (GUIUtility.hotControl == controlID)
                        {
                            result -= current.delta * (float) ((!current.shift) ? 1 : 3) / Mathf.Min(position.width, position.height) * 140.0f;
                            // result.y = Mathf.Clamp(result.y, -90f, 90f);

                            if (result.y > 360.0f)
                            {
                                result.y -= 360.0f;
                            }
                            else if (result.y < -360.0f)
                            {
                                result.y += 360.0f;
                            }

                            current.Use();
                            GUI.changed = true;
                        }
                    }

                        break;
                }

                return result;
            }

            #endregion
        }
    }
}