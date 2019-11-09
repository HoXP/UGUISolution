using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Text))]
[AddComponentMenu("UI/Effects/SpacingText")]
public class SpacingText : BaseMeshEffect
{//让字符Mesh自适应其RectTransform的宽度
    private const int CharPointCount = 6;   //每个字符点数

    protected override void OnEnable()
    {
        base.OnEnable();
        Text t = GetComponent<Text>();
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Truncate;
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive() || vh.currentIndexCount == 0)
        {
            return;
        }
        List<UIVertex> vertexs = new List<UIVertex>();
        vh.GetUIVertexStream(vertexs);
        //计算该条文本宽度
        float textWidth = vertexs[vertexs.Count - 3].position.x - vertexs[0].position.x;
        //计算每个空白宽度
        RectTransform rt = GetComponent<RectTransform>();
        Bounds b = new Bounds(rt.rect.center, rt.rect.size);
        float width = b.max.x - b.min.x;
        float spaceWidth = width - textWidth;
        int charCount = vh.currentIndexCount / CharPointCount;  //该条文本字符数
        float perSpaceWidth = spaceWidth * 1.0f / (charCount - 1);
        //给每个字符之间塞进去一个空白
        float offset = b.min.x - vertexs[0].position.x;
        UIVertex vt;
        for (int i = 0; i < vh.currentIndexCount; i++)
        {
            vt = vertexs[i];
            int div = i / CharPointCount;
            vt.position += new Vector3(perSpaceWidth * div + offset, 0, 0);
            vertexs[i] = vt;
            //以下注意点与索引的对应关系
            int remainder = i % CharPointCount;
            if (remainder < 3)
            {
                vh.SetUIVertex(vt, div * 4 + remainder);
            }
            if (remainder == 4)
            {
                vh.SetUIVertex(vt, div * 4 + remainder - 1);
            }
        }
    }
}