using static UnityEngine.GUILayout;
using static UnityEditor.EditorStyles;
using static UnityEditor.EditorPrefs;

using UnityEngine;
using UnityEditor;

public sealed class MessagerDevtoolsWindow : EditorWindow
{
    static bool DevtoolsEnabled
    {
        get => HasKey(devtoolsMenuName) && GetBool(devtoolsMenuName);
        set => SetBool(devtoolsMenuName, value);
    }

    private const string devtoolsMenuName = "Messager/Enable Devtools";
    static readonly string[] _toolbars = { "Message History", "Subscribers" };

    int _toolbarIndex = 0;
    Vector2 _scrollPosHistory;
    Vector2 _scrollPosSubs;
    string _subSearch;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitMiddlewares()
    {
        if (!DevtoolsEnabled)
            return;

        Messager.DefaultInstance.InitMiddlewares(
            onDispatch: MessagerDevtools.AddHistoryRecord,
            onListen: MessagerDevtools.AddSubscriptionRecord
        );
    }

    [MenuItem("Messager/Open Devtools Window")]
    static void ShowWindow()
    {
        var window = GetWindow<MessagerDevtoolsWindow>();
        window.titleContent = new GUIContent("Messager Devtools");
        window.Show();
    }

    [MenuItem(devtoolsMenuName, false, 150)]
    static void EnableDevtools()
    {
        DevtoolsEnabled = !DevtoolsEnabled;
        Menu.SetChecked(devtoolsMenuName, DevtoolsEnabled);
    }

    [MenuItem(devtoolsMenuName, true)]
    static bool EnableDevtoolsValidate()
    {
        Menu.SetChecked(devtoolsMenuName, DevtoolsEnabled);
        return true;
    }

    void OnGUI()
    {
        if (!DevtoolsEnabled)
        {
            Space(20);
            Label($"Devtools are disabled, please enable them under from the menu bar.", boldLabel);
            return;
        }

        _toolbarIndex = Toolbar(_toolbarIndex, _toolbars, Height(25));

        switch (_toolbarIndex)
        {
            case 0:
                DrawHistoryTab();
                break;
            case 1:
                DrawSubsTab();
                break;
            default: break;
        }
    }

    void DrawHistoryTab()
    {
        var history = MessagerDevtools.GetMessageHistory();
        var width = 300;

        Space(10);

        if (history.Length == 0)
        {
            Label("No history available for now, start firing some events!", boldLabel);
        }
        else
        {
            _scrollPosHistory = EditorGUILayout.BeginScrollView(_scrollPosHistory);
            for (int i = 0; i < history.Length; i++)
            {
                var record = history[i];

                BeginHorizontal();

                record.IsVisible = EditorGUILayout.Foldout(record.IsVisible, record.Time);
                EditorGUILayout.TextField($"<{record.Type}>", Width(width));
                EditorGUILayout.TextField(record.Caller, Width(width));

                EndHorizontal();

                if (record.IsVisible)
                {
                    Space(5);
                    Label("payload", boldLabel);
                    TextArea(record.Payload);
                }

                Space(5);
            }
            EndScrollView();
        }
    }

    void DrawSubsTab()
    {
        var spacing = 12;
        var width = 400;

        Space(10);
        DrawSearchField();
        Space(10);

        if (Application.isPlaying && Messager.DefaultInstance != null)
        {
            _scrollPosSubs = BeginScrollView(_scrollPosSubs);

            foreach (var item in MessagerDevtools.GetSubscriptions())
            {
                if (!item.Key.ToString().Contains(_subSearch))
                    continue;

                Label($">> {item.Key} ({item.Value.Count})", boldLabel);

                Space(2);
                var owners = item.Value.ToArray();

                for (int i = 0; i < owners.Length; i++)
                {
                    string delimiter = i == owners.Length - 1 ? "└" : "├";
                    object owner = owners[i];

                    BeginHorizontal();
                    switch (owner)
                    {
                        case Object go:
                            Label(delimiter, Width(spacing));
                            EditorGUILayout.ObjectField(go, typeof(Object), false, Width(width));
                            break;

                        default:
                            Label(delimiter, Width(spacing));
                            EditorGUILayout.TextField(owner.GetType().ToString(), Width(width));
                            break;
                    }
                    EndHorizontal();
                }
                Space(15);
            }
            EndScrollView();
        }
        else
        {
            Label("This view is only available in play mode.", boldLabel);
        }
    }

    void DrawSearchField()
    {
        var width = 60;

        BeginHorizontal();
        Label("Search", boldLabel, Width(width));
        _subSearch = EditorGUILayout.TextField(_subSearch);
        if (Button("Clear", Width(width)))
        {
            _subSearch = string.Empty;
            GUI.FocusControl("");
        }
        EndHorizontal();
    }
}