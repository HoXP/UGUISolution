using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public Text _txt = null;
    public Image _img = null;
    public Canvas canvas;

    private void Awake()
    {
        StartCoroutine(Co());
    }
    private void OnEnable()
    {
        StartCoroutine(Co());
    }

    private IEnumerator Co()
    {
        TextGenerationSettings settings = _txt.GetGenerationSettings(_txt.rectTransform.rect.size);
        _txt.cachedTextGenerator.Invalidate();
        _txt.cachedTextGenerator.Populate(_txt.text, settings);
        Canvas.ForceUpdateCanvases();

        UILineInfo line = _txt.cachedTextGenerator.lines[_txt.cachedTextGenerator.lineCount - 1];
        string s = _txt.text;
        string sLastLine = s.Substring(line.startCharIdx);
        sLastLine = GetStringWithoutMark(sLastLine);
        float width = _txt.cachedTextGenerator.GetPreferredWidth(sLastLine, settings);
        width = width / settings.scaleFactor;
        _img.rectTransform.anchoredPosition = new Vector2(width, line.topY);
        Debug.LogError($"### sLastLine={sLastLine}; width={width};");

        yield break;
    }

    private string GetStringWithoutMark(string s)
    {
        string ret = s.Replace("</color>", string.Empty);
        int indexL = ret.IndexOf("<color=");
        int indexR = ret.IndexOf(">");
        if(indexL >= 0 && indexR >= 0)
        {
            ret = ret.Remove(indexL, indexR - indexL + 1);
        }
        return ret;
    }
    
    /// <summary>
    /// 得到Text中字符的位置；canvas:所在的Canvas，text:需要定位的Text,charIndex:Text中的字符位置
    /// </summary>
    public Vector3 GetPosAtText(Canvas canvas, string textStr, Text text, int charIndex)
    {
        Vector3 charPos = Vector3.zero;
        if (charIndex <= textStr.Length && charIndex > 0)
        {
            TextGenerator textGen = new TextGenerator(textStr.Length);
            Vector2 extents = text.gameObject.GetComponent<RectTransform>().rect.size;
            textGen.Populate(textStr, text.GetGenerationSettings(extents));

            int newLine = textStr.Substring(0, charIndex).Split('\n').Length - 1;
            int whiteSpace = textStr.Substring(0, charIndex).Split(' ').Length - 1;
            int indexOfTextQuad = (charIndex * 4) + (newLine * 4) - 4;
            if (indexOfTextQuad < textGen.vertexCount)
            {
                charPos = (textGen.verts[indexOfTextQuad].position +
                    textGen.verts[indexOfTextQuad + 1].position +
                    textGen.verts[indexOfTextQuad + 2].position +
                    textGen.verts[indexOfTextQuad + 3].position) / 4f;
            }
        }
        //charPos /= canvas.scaleFactor;//适应不同分辨率的屏幕
        charPos = text.transform.TransformPoint(charPos);//转换为世界坐标
        return charPos;
    }
}