using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class SignaturePad : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Signature texture")]
    [SerializeField] private int textureWidth = 700;
    [SerializeField] private int textureHeight = 180;
    [SerializeField] private int brushRadius = 3;

    private RawImage rawImage;
    private Texture2D texture;
    private bool drawing;
    private Vector2Int previousPixel;
    private bool hasPreviousPixel;

    public bool HasSignature { get; private set; }

    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
        CreateTexture();
        Clear();
    }

    private void CreateTexture()
    {
        texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        rawImage.texture = texture;
    }

    public void Clear()
    {
        if (texture == null)
            return;

        Color32[] pixels = new Color32[textureWidth * textureHeight];
        Color32 white = new Color32(255, 255, 255, 255);

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = white;

        texture.SetPixels32(pixels);
        texture.Apply();

        HasSignature = false;
        drawing = false;
        hasPreviousPixel = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        drawing = true;
        hasPreviousPixel = false;
        DrawAtPointer(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (drawing)
            DrawAtPointer(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (drawing)
            DrawAtPointer(eventData);

        drawing = false;
        hasPreviousPixel = false;
    }

    private void DrawAtPointer(PointerEventData eventData)
    {
        RectTransform rectTransform = (RectTransform)transform;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint))
        {
            return;
        }

        Rect rect = rectTransform.rect;
        float normalizedX = Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x);
        float normalizedY = Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y);

        if (normalizedX < 0f || normalizedX > 1f || normalizedY < 0f || normalizedY > 1f)
            return;

        Vector2Int pixel = new Vector2Int(
            Mathf.RoundToInt(normalizedX * (textureWidth - 1)),
            Mathf.RoundToInt(normalizedY * (textureHeight - 1))
        );

        if (hasPreviousPixel)
            DrawLine(previousPixel, pixel);
        else
            DrawBrush(pixel.x, pixel.y);

        previousPixel = pixel;
        hasPreviousPixel = true;
        HasSignature = true;
        texture.Apply();
    }

    private void DrawLine(Vector2Int from, Vector2Int to)
    {
        int distance = Mathf.CeilToInt(Vector2.Distance(from, to));
        distance = Mathf.Max(distance, 1);

        for (int i = 0; i <= distance; i++)
        {
            float t = i / (float)distance;
            int x = Mathf.RoundToInt(Mathf.Lerp(from.x, to.x, t));
            int y = Mathf.RoundToInt(Mathf.Lerp(from.y, to.y, t));
            DrawBrush(x, y);
        }
    }

    private void DrawBrush(int centerX, int centerY)
    {
        Color32 ink = new Color32(15, 15, 15, 255);
        int radiusSquared = brushRadius * brushRadius;

        for (int x = -brushRadius; x <= brushRadius; x++)
        {
            for (int y = -brushRadius; y <= brushRadius; y++)
            {
                if (x * x + y * y > radiusSquared)
                    continue;

                int px = centerX + x;
                int py = centerY + y;

                if (px >= 0 && px < textureWidth && py >= 0 && py < textureHeight)
                    texture.SetPixel(px, py, ink);
            }
        }
    }
}
