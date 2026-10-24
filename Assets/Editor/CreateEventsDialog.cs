using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Tools > UI > Create Events Dialog: builds PopupEvents (EventLogPopup) in the open scene once, from a duplicate of
// PopupTips (Unity's own duplicate, so its X button - a nested prefab - stays linked): same frame and X, the title
// "Events" top-left like the other dialogs (placed like PopupMusic's title), and a framed scroll view for the entries.
// Afterwards it's an ordinary scene object: save the scene and edit it like any other dialog. Undoable.
public static class CreateEventsDialog
{
    const float ExtraWidth = 60f;                               // wider than the tips: event lines are long
    const float SidePadding = 30f, TopPadding = 42f, BottomPadding = 14f;
    const int FramePaddingX = 12, FramePaddingY = 8;            // text inside the frame
    const string FrameSpriteGuid = "30fea15b87e3ec842bba846fdd660fdf"; // the rebirth cards' panel frame
    static readonly Color FrameColor = new(0.368f, 0.3345f, 0.301f, 1f);

    [MenuItem("Tools/UI/Create Events Dialog")]
    static void Create()
    {
        if (Object.FindObjectsByType<EventLogPopup>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 0)
        {
            var existing = Object.FindObjectsByType<EventLogPopup>(FindObjectsInactive.Include, FindObjectsSortMode.None)[0];
            Selection.activeGameObject = existing.gameObject;
            EditorUtility.DisplayDialog("Create Events Dialog", $"'{existing.name}' already exists - nothing done.", "OK");
            return;
        }

        var tips = Object.FindObjectsByType<TipsPopupScript>(FindObjectsInactive.Include, FindObjectsSortMode.None).FirstOrDefault();
        if (tips == null)
        {
            EditorUtility.DisplayDialog("Create Events Dialog", "No TipsPopupScript (PopupTips) in the open scene.", "OK");
            return;
        }

        // Duplicate like Ctrl+D (keeps the nested prefab instance of the X button).
        Selection.activeGameObject = tips.gameObject;
        Unsupported.DuplicateGameObjectsUsingPasteboard();
        var go = Selection.activeGameObject;
        if (go == null || go == tips.gameObject)
        {
            Debug.LogError("Create Events Dialog: duplicating PopupTips failed");
            return;
        }
        Undo.RecordObject(go, "Create Events Dialog");
        go.name = "PopupEvents";
        var rootRt = (RectTransform)go.transform;
        Undo.RecordObject(rootRt, "Create Events Dialog");
        rootRt.sizeDelta += new Vector2(ExtraWidth, 0f);

        var popup = Undo.AddComponent<EventLogPopup>(go);
        Undo.DestroyObjectImmediate(go.GetComponent<TipsPopupScript>());

        // Sort the copied children: keep the header, the X button and TextTips (-> entry template); drop the rest.
        TextMeshProUGUI header = null, entryTemplate = null;
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
            if (child.name == "TextTips" && child.TryGetComponent(out entryTemplate))
                continue;
            drop.Add(child.gameObject);
        }
        foreach (var d in drop)
            Undo.DestroyObjectImmediate(d);

        if (header != null)
            SetupHeader(header);
        if (close != null)
        {
            Undo.RecordObject(close, "Create Events Dialog");
            for (int i = close.onClick.GetPersistentEventCount() - 1; i >= 0; --i)
                UnityEventTools.RemovePersistentListener(close.onClick, i);
            UnityEventTools.AddPersistentListener(close.onClick, popup.Close);
        }

        // Scroll view: the frame (also the viewport, clipping), content with a vertical layout.
        var scrollGo = new GameObject("EventsScroll", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(scrollGo, "Create Events Dialog");
        scrollGo.layer = go.layer;
        scrollGo.transform.SetParent(go.transform, false);
        var scrollRt = (RectTransform)scrollGo.transform;
        scrollRt.anchorMin = Vector2.zero;
        scrollRt.anchorMax = Vector2.one;
        scrollRt.offsetMin = new Vector2(SidePadding, BottomPadding);
        scrollRt.offsetMax = new Vector2(-SidePadding, -TopPadding);
        var frame = scrollGo.AddComponent<Image>();
        frame.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(FrameSpriteGuid));
        frame.type = Image.Type.Sliced;
        frame.color = frame.sprite != null ? FrameColor : new Color(0, 0, 0, 0);
        scrollGo.AddComponent<RectMask2D>();

        var contentGo = new GameObject("Content", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(contentGo, "Create Events Dialog");
        contentGo.layer = go.layer;
        contentGo.transform.SetParent(scrollGo.transform, false);
        var content = (RectTransform)contentGo.transform;
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);
        content.offsetMin = content.offsetMax = Vector2.zero;
        var layout = contentGo.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(FramePaddingX, FramePaddingX, FramePaddingY, FramePaddingY);
        layout.childControlWidth = layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        contentGo.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var scroll = scrollGo.AddComponent<ScrollRect>();
        scroll.content = content;
        scroll.viewport = scrollRt;
        scroll.horizontal = false;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 20f;

        if (entryTemplate != null)
        {
            Undo.SetTransformParent(entryTemplate.transform, content, "Create Events Dialog");
            Undo.RecordObject(entryTemplate, "Create Events Dialog");
            Undo.RecordObject(entryTemplate.gameObject, "Create Events Dialog");
            entryTemplate.name = "EntryTemplate";
            entryTemplate.text = "24-10-2026 09:33  <color=#AADC64>•</color> Example event  <color=#9A9A9A>(1h ago)</color>";
            entryTemplate.margin = Vector4.zero;
            entryTemplate.alignment = TextAlignmentOptions.TopLeft;
            entryTemplate.raycastTarget = false;
            var rt = entryTemplate.rectTransform;
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            entryTemplate.gameObject.SetActive(false);
        }

        Undo.RecordObject(popup, "Create Events Dialog");
        popup.Scroll = scroll;
        popup.Content = content;
        popup.EntryTemplate = entryTemplate;

        // Tips: the Events button opens it.
        Undo.RecordObject(tips, "Create Events Dialog");
        tips.EventsPopup = popup;
        var eventsButton = tips.transform.Find("ButtonEvents");
        if (eventsButton != null && eventsButton.TryGetComponent<Button>(out var button))
        {
            Undo.RecordObject(button, "Create Events Dialog");
            for (int i = button.onClick.GetPersistentEventCount() - 1; i >= 0; --i)
                UnityEventTools.RemovePersistentListener(button.onClick, i);
            UnityEventTools.AddPersistentListener(button.onClick, tips.OnEventsClick);
        }
        else
        {
            Debug.LogWarning("Create Events Dialog: no ButtonEvents under PopupTips - wire it to TipsPopupScript.OnEventsClick by hand");
        }

        EditorSceneManager.MarkSceneDirty(go.scene);
        Selection.activeGameObject = go;
        Debug.Log("Create Events Dialog: PopupEvents created - save the scene");
    }

    // The title top-left, like PopupMusic's (TextMusic) when it's there, else the same values by hand.
    static void SetupHeader(TextMeshProUGUI header)
    {
        Undo.RecordObject(header, "Create Events Dialog");
        Undo.RecordObject(header.rectTransform, "Create Events Dialog");
        Undo.RecordObject(header.gameObject, "Create Events Dialog");
        header.name = "TextEventsHeader";
        header.text = "Events";

        var music = Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .FirstOrDefault(t => t.name == "TextMusic");
        var rt = header.rectTransform;
        if (music != null)
        {
            var m = music.rectTransform;
            rt.anchorMin = m.anchorMin;
            rt.anchorMax = m.anchorMax;
            rt.pivot = m.pivot;
            rt.anchoredPosition = m.anchoredPosition;
            rt.sizeDelta = m.sizeDelta;
            header.fontSize = music.fontSize;
            header.alignment = music.alignment;
        }
        else
        {
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(20f, -11.8f);
            rt.sizeDelta = new Vector2(200f, 27.52f);
            header.fontSize = 16f;
            header.alignment = TextAlignmentOptions.TopLeft;
        }
    }
}
