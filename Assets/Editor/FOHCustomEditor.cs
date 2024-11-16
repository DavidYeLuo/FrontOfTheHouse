using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace CustomEditor {
public class FOHCustomEditor : EditorWindow {
  private Label playerCountLabel;
  private Button refreshButton;

  [SerializeField]
  private VisualTreeAsset uxml_tree = default;
  [MenuItem("FOH/CustomEditor")]
  public static void ShowEditor() {
    FOHCustomEditor window = GetWindow<FOHCustomEditor>();
    window.titleContent = new GUIContent("FOH");
  }
  public void CreateGUI() {
    // Each editor contain rootVisualElement
    VisualElement root = rootVisualElement;

    // Import UXML
    var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
        "Assets/Editor/UIToolkit/FOHCustomEditor.uxml");
    var labelFromUXML = visualTree.Instantiate();
    root.Add(labelFromUXML);

    playerCountLabel = root.Query<Label>("PlayerCount");
    playerCountLabel.text = Player.Player.Count.ToString();
    refreshButton = root.Query<Button>().First();
    refreshButton.clickable.clicked += UpdatePlayerCountLabel;
  }
  private void OnDisable() {
    refreshButton.clickable.clicked -= UpdatePlayerCountLabel;
  }
  private void UpdatePlayerCountLabel() {
    playerCountLabel.text = Player.Player.Count.ToString();
  }
}
}
