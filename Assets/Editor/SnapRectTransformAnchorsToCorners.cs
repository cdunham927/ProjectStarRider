using UnityEditor;
using UnityEngine;

public class SnapRectTransformAnchorsToCorners : MonoBehaviour
{
    [MenuItem("CONTEXT/RectTransform/Snap Anchors To Corners", priority = 50)]
    private static void Execute(MenuCommand command)
    {
        RectTransform rectTransform = command.context as RectTransform;
        RectTransform parent = rectTransform.parent as RectTransform;
        Vector2 parentSize = parent.rect.size;
        Vector2 parentMin = parent.rect.min;

        // Find corner point coordinates in parent's local space
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        corners[0] = parent.InverseTransformPoint(corners[0]); // Bottom left
        corners[2] = parent.InverseTransformPoint(corners[2]); // Top right

        Undo.RecordObject(rectTransform, "Anchor Modification");

        rectTransform.anchorMin = new Vector2((corners[0].x - parentMin.x) / parentSize.x, (corners[0].y - parentMin.y) / parentSize.y);
        rectTransform.anchorMax = new Vector2((corners[2].x - parentMin.x) / parentSize.x, (corners[2].y - parentMin.y) / parentSize.y);
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
    }

    [MenuItem("CONTEXT/RectTransform/Snap Anchors To Corners", validate = true)]
    private static bool Validate(MenuCommand command)
    {
        RectTransform rectTransform = command.context as RectTransform;
        return rectTransform.parent != null && !PrefabUtility.IsPartOfImmutablePrefab(rectTransform);
    }
}