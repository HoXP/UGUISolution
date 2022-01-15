using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/RollingTextMaxWidth")]
[RequireComponent(typeof(Text))]
public class RollingTextMaxWidth : BaseMeshEffect
{
    public int Speed = 40;
    public int Space = 40;

    private Text text;
    private float offset = 0;

    private TextAnchor oldAlignment;
    private HorizontalWrapMode oldHorWrap;

    private HorizontalLayoutGroup hlg = null;
    private ContentSizeFitter _csf = null;

    private bool rolling = false;

    protected override void Awake()
    {
        base.Start();
        text = GetComponent<Text>();
        _csf = GetComponent<ContentSizeFitter>();
        hlg = transform.parent.GetComponent<HorizontalLayoutGroup>();
        _csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        oldAlignment = text.alignment;
        oldHorWrap = text.horizontalOverflow;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        RefreshText(null);
    }

    public void RefreshText(string content)
    {
        if (content != null)
        {
            if (text is Text)
            {
                var t = text as Text;
                t.text = content;
            }
            else
            {
                text.text = content;
            }
        }
        
        rolling = text.preferredWidth > text.rectTransform.rect.width;
        if (rolling)
        {
            _csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            hlg.childControlWidth = false;
            offset = 0;
            text.alignment = TextAnchor.MiddleLeft;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
        }
        else
        {
            _csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            hlg.childControlWidth = true;
            text.alignment = oldAlignment;
            text.horizontalOverflow = oldHorWrap;
        }
        text.SetVerticesDirty();
    }

    private void Update()
    {
        if (!rolling)
            return;

        offset += Speed * Time.deltaTime;
        if (offset > GetPreferredWidth() + Space)
            offset = 0;
        text.SetVerticesDirty();
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!rolling)
            return;

        var rect = text.rectTransform.rect;

        for (int i = 0; i < vh.currentVertCount; i += 4)
        {
            UIVertex ver = new UIVertex();
            vh.PopulateUIVertex(ref ver, i + 1);
            bool move = ver.position.x - offset < rect.xMin;

            for (int j = 0; j < 4; ++j)
            {
                int idx = i + j;
                UIVertex v = new UIVertex();
                vh.PopulateUIVertex(ref v, idx);
                if (move)
                {
                    v.position.x += GetPreferredWidth() - offset + Space;
                }
                else
                {
                    v.position.x -= offset;
                }

                vh.SetUIVertex(v, idx);
            }
        }
    }

    private float GetPreferredWidth()
    {
        return text.preferredWidth;
    }
}