using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Course.PrototypeScripting
{
    public class SequenceFinderWindow : EditorWindow
    {
        private List<Sequence> _sequences = new List<Sequence>();
        private Vector2 _scrollPosition;
        private string _filterText = "";
        private GUIStyle _headerStyle;
        private GUIStyle _rowStyle;
        private GUIStyle _rowAltStyle;
        private bool _stylesInitialized;

        [MenuItem("Tools/Course/PTS/Sequence Overview")]
        public static void ShowWindow()
        {
            var window = GetWindow<SequenceFinderWindow>("Sequence Overview");
            window.minSize = new Vector2(320, 200);
            window.RefreshSequences();
        }

        private void OnEnable()
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneLoaded += OnSceneLoaded;
            RefreshSequences();
        }

        private void OnDisable()
        {
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            RefreshSequences();
            Repaint();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RefreshSequences();
            Repaint();
        }

        private void RefreshSequences()
        {
            _sequences.Clear();

            // Find all Sequence components in all loaded scenes (including inactive GameObjects)
            var allObjects = Resources.FindObjectsOfTypeAll<Sequence>();
            foreach (var seq in allObjects)
            {
                // Filter: only include objects that are part of a loaded scene (not prefabs/assets)
                if (seq.gameObject.scene.IsValid() && seq.gameObject.scene.isLoaded)
                {
                    _sequences.Add(seq);
                }
            }

            // Sort by priority ascending (1 = highest, at top)
            _sequences = _sequences.OrderBy(s => s.priority).ToList();
        }

        private void InitStyles()
        {
            if (_stylesInitialized) return;

            _headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 13,
                padding = new RectOffset(4, 4, 6, 6)
            };

            _rowStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(6, 6, 4, 4),
                margin = new RectOffset(0, 0, 1, 1)
            };
            _rowStyle.normal.background = MakeTex(1, 1, new Color(0.22f, 0.22f, 0.22f, 1f));

            _rowAltStyle = new GUIStyle(_rowStyle);
            _rowAltStyle.normal.background = MakeTex(1, 1, new Color(0.19f, 0.19f, 0.19f, 1f));

            _stylesInitialized = true;
        }

        private static Texture2D MakeTex(int width, int height, Color col)
        {
            var pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++) pix[i] = col;
            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private void OnGUI()
        {
            InitStyles();

            // ── Toolbar ──────────────────────────────────────────────
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Sequence Finder", _headerStyle, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("↺  Update", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                RefreshSequences();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);

            // ── Filter bar ───────────────────────────────────────────
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Filter:", GUILayout.Width(38));
            var newFilter = EditorGUILayout.TextField(_filterText, EditorStyles.toolbarSearchField);
            if (newFilter != _filterText)
            {
                _filterText = newFilter;
                Repaint();
            }
            if (GUILayout.Button("✕", EditorStyles.toolbarButton, GUILayout.Width(22)))
            {
                _filterText = "";
                GUI.FocusControl(null);
                Repaint();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(2);

            // ── Build filtered list (already sorted by priority) ─────
            var filtered = string.IsNullOrEmpty(_filterText)
                ? _sequences
                : _sequences.Where(s => s != null &&
                    s.gameObject.name.IndexOf(_filterText, System.StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            // ── Status line ──────────────────────────────────────────
            var countLabel = filtered.Count == 0
                ? (_sequences.Count == 0
                    ? "No Sequence components found in the current scene."
                    : "No matches for the current filter.")
                : $"{filtered.Count} of {_sequences.Count} Sequence{(_sequences.Count != 1 ? "s" : "")} shown  •  sorted by priority";
            EditorGUILayout.LabelField(countLabel, EditorStyles.miniLabel);

            EditorGUILayout.Space(2);

            // ── Scroll list ──────────────────────────────────────────
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            for (int i = 0; i < filtered.Count; i++)
            {
                var seq = filtered[i];

                // Skip destroyed objects (can happen when playing in editor)
                if (seq == null) continue;

                var rowStyle = (i % 2 == 0) ? _rowStyle : _rowAltStyle;

                EditorGUILayout.BeginHorizontal(rowStyle);

                // Object name (click to ping/select in hierarchy)
                var displayName = string.IsNullOrEmpty(seq.gameObject.name)
                    ? $"[Unnamed #{i}]"
                    : seq.gameObject.name;

                // Show type name as secondary info if subclass
                var typeName = seq.GetType().Name;
                var priorityTag = $"<color=#aaaaaa><size=10>[{seq.priority}]</size></color>  ";
                var label = typeName != nameof(Sequence)
                    ? $"{priorityTag}{displayName}  <color=#888888><size=10>({typeName})</size></color>"
                    : $"{priorityTag}{displayName}";

                GUILayout.Label(label, new GUIStyle(EditorStyles.label) { richText = true }, GUILayout.ExpandWidth(true));

                // Select button
                if (GUILayout.Button(new GUIContent("☲ Select", $"Select '{displayName}' in hierarchy"), GUILayout.Width(70)))
                {
                    Selection.activeGameObject = seq.gameObject;
                    EditorGUIUtility.PingObject(seq.gameObject);
                }

                // Execute button
                GUI.enabled = Application.isPlaying || true; // always show; warn if not playing
                if (GUILayout.Button("▶ Execute", GUILayout.Width(80)))
                {
                    if (!Application.isPlaying)
                    {
                        bool confirmed = EditorUtility.DisplayDialog(
                            "Not in Play Mode",
                            $"Execute '{displayName}.ExecuteCompleteSequence()' outside of Play Mode?\n\nThis may have unintended side effects.",
                            "Execute anyway",
                            "Cancel");

                        if (!confirmed) goto EndRow;
                    }

                    Debug.Log($"[SequenceFinder] Executing '{displayName}.ExecuteCompleteSequence()'");
                    seq.ExecuteCompleteSequence();
                }

                EndRow:
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(4);

            // ── Footer hint ──────────────────────────────────────────
            EditorGUILayout.LabelField(
                "Auto-refreshes on scene load  •  Sorted by priority",
                EditorStyles.centeredGreyMiniLabel);

            EditorGUILayout.Space(2);
        }
    }
}
