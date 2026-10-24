using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Tools > UI > Create More Tips Dialog: builds PopupMoreTips (MoreTipsPopup) in the open scene once, the same way as
// the Events dialog (a duplicate of PopupTips: same frame and X, the title top-left) but with just a text instead of
// the framed list. Afterwards it's an ordinary scene object: save the scene, write the tips in TextMoreTips. Undoable.
public static class CreateMoreTipsDialog
{
    const float SidePadding = 30f, TopPadding = 42f, BottomPadding = 14f;

    [MenuItem("Tools/UI/Create More Tips Dialog")]
    static void Create()
    {
        var existing = Object.FindObjectsByType<MoreTipsPopup>(FindObjectsInactive.Include, FindObjectsSortMode.None).FirstOrDefault();
        if (existing != null)
        {
            Selection.activeGameObject = existing.gameObject;
            EditorUtility.DisplayDialog("Create More Tips Dialog", $"'{existing.name}' already exists - nothing done.", "OK");
            return;
        }

        var tips = Object.FindObjectsByType<TipsPopupScript>(FindObjectsInactive.Include, FindObjectsSortMode.None).FirstOrDefault();
        if (tips == null)
        {
            EditorUtility.DisplayDialog("Create More Tips Dialog", "No TipsPopupScript (PopupTips) in the open scene.", "OK");
            return;
        }

        // Duplicate like Ctrl+D (keeps the nested prefab instance of the X button).
        Selection.activeGameObject = tips.gameObject;
        Unsupported.DuplicateGameObjectsUsingPasteboard();
        var go = Selection.activeGameObject;
        if (go == null || go == tips.gameObject)
        {
            Debug.LogError("Create More Tips Dialog: duplicating PopupTips failed");
            return;
        }
        Undo.RecordObject(go, "Create More Tips Dialog");
        go.name = "PopupMoreTips";

        var popup = Undo.AddComponent<MoreTipsPopup>(go);
        Undo.DestroyObjectImmediate(go.GetComponent<TipsPopupScript>());

        // Keep the header, the X button and TextTips (-> the text); drop the rest.
        TextMeshProUGUI header = null, text = null;
        Button close = null;
        var drop = new List<GameObject>();
        foreach (Transform child in go.transform)
        {
            if (child.name == "TextTipsHeader" && child.TryGetComponent(out header))
                continue;
            if (child.GetComponentInChildren<ButtonBackScript>(true) != null)
            {
                close = child.GetComponentInChildren<Button>(true);
                continue;
            }
            if (child.name == "TextTips" && child.TryGetComponent(out text))
                continue;
            drop.Add(child.gameObject);
        }
        foreach (var d in drop)
            Undo.DestroyObjectImmediate(d);

        if (header != null)
            CreateEventsDialog.SetupHeader(header, "TextMoreTipsHeader", "More tips", "Create More Tips Dialog");
        if (close != null)
        {
            Undo.RecordObject(close, "Create More Tips Dialog");
            for (int i = close.onClick.GetPersistentEventCount() - 1; i >= 0; --i)
                UnityEventTools.RemovePersistentListener(close.onClick, i);
            UnityEventTools.AddPersistentListener(close.onClick, popup.Close);
        }

        if (text != null)
        {
            Undo.RecordObject(text, "Create More Tips Dialog");
            Undo.RecordObject(text.rectTransform, "Create More Tips Dialog");
            Undo.RecordObject(text.gameObject, "Create More Tips Dialog");
            text.name = "TextMoreTips";
            text.text = "Write the extra tips here (PopupMoreTips > TextMoreTips).";
            text.margin = Vector4.zero;
            text.alignment = TextAlignmentOptions.TopLeft;
            var rt = text.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = new Vector2(SidePadding, BottomPadding);
            rt.offsetMax = new Vector2(-SidePadding, -TopPadding);
            text.gameObject.SetActive(true);
        }

        Undo.RecordObject(popup, "Create More Tips Dialog");
        popup.Text = text;

        // Tips: the More tips button opens it.
        Undo.RecordObject(tips, "Create More Tips Dialog");
        tips.MoreTipsPopup = popup;
        var moreButton = tips.transform.Find("ButtonMoreTips");
        if (moreButton != null && moreButton.TryGetComponent<Button>(out var button))
        {
            Undo.RecordObject(button, "Create More Tips Dialog");
            for (int i = button.onClick.GetPersistentEventCount() - 1; i >= 0; --i)
                UnityEventTools.RemovePersistentListener(button.onClick, i);
            UnityEventTools.AddPersistentListener(button.onClick, tips.OnMoreTipsClick);
        }
        else
        {
            Debug.LogWarning("Create More Tips Dialog: no ButtonMoreTips under PopupTips - wire it to TipsPopupScript.OnMoreTipsClick by hand");
        }

        EditorSceneManager.MarkSceneDirty(go.scene);
        Selection.activeGameObject = go;
        Debug.Log("Create More Tips Dialog: PopupMoreTips created - save the scene");
    }
}
