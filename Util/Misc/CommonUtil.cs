#region

//文件创建者：Egg
//创建时间：09-14 08:16

#endregion

using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace EggFramework.Util
{
    public static class CommonUtil
    {
        public static TMP_Text ShowTextInWorldPosition(this MonoBehaviour self, string text, Vector3 worldPosition,
            Color fontColor, float fontSize = 5f)
        {
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(self.transform);
            var sortGroup = textObj.AddComponent<SortingGroup>();
            sortGroup.sortingLayerName = "Overlay";
            var textComponent = textObj.AddComponent<TextMeshPro>();
            textComponent.text = text;
            textComponent.transform.position = worldPosition;
            textComponent.fontSize = fontSize;
            textComponent.color = fontColor;
            textComponent.horizontalAlignment = HorizontalAlignmentOptions.Center;
            textComponent.verticalAlignment = VerticalAlignmentOptions.Middle;
            return textComponent;
        }

        public static TMP_Text SetTextInWorldPosition(this MonoBehaviour self, TMP_Text tmpText, string text, Vector3 worldPosition,
            Color fontColor, float fontSize = 5f)
        {
            tmpText.text = text;
            tmpText.transform.position = worldPosition;
            tmpText.fontSize = fontSize;
            tmpText.color = fontColor;
            tmpText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            tmpText.verticalAlignment = VerticalAlignmentOptions.Middle;
            return tmpText;
        }
    }
}