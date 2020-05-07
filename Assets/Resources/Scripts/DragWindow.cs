using UnityEngine;
using UnityEngine.EventSystems;

public class DragWindow: MonoBehaviour, IDragHandler, IPointerDownHandler
{
    // Properties //
    public RectTransform mainWindow;
    public bool limitByMainWindow = false;

    Vector2 resolution;
    Vector3 mainWindowScale;

    // Functions //
    void Start()
    {
        if (mainWindow == null)
        {
            mainWindow = GetComponent<RectTransform>();
        }

        resolution = new Vector2(Screen.width, Screen.height);
        mainWindowScale = mainWindow.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mainWindow.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdatePosition(eventData.delta);
    }

    void UpdatePosition(Vector2 MouseShift)
    {
        Vector2 ScreenSize = new Vector2(Screen.width, Screen.height);
        Vector3[] corners = new Vector3[4];
        RectTransform limitBox;

        if (limitByMainWindow)
        {
            limitBox = mainWindow;
        }
        else
        {
            limitBox = GetComponent<RectTransform>();
        }

        Vector2 boxSize = limitBox.rect.size;
        limitBox.GetWorldCorners(corners);

        // alignment by the bottom left corner with index 0
        float left = Mathf.Clamp(corners[0].x + MouseShift.x, 0, ScreenSize.x - boxSize.x);
        float down = Mathf.Clamp(corners[0].y + MouseShift.y, 0, ScreenSize.y - boxSize.y);

        MouseShift.x = left - corners[0].x;
        MouseShift.y = down - corners[0].y;

        mainWindow.anchoredPosition += MouseShift;
    }

    void Update()
    {
        // if resolutions changed update our item position
        if (resolution.x != Screen.width || resolution.y != Screen.height)
        {
            UpdatePosition(Vector2.zero);

            resolution.x = Screen.width;
            resolution.y = Screen.height;
        }

        // if main window scale changed update our item position
        if (mainWindowScale != mainWindow.localScale)
        {
            UpdatePosition(Vector2.zero);

            mainWindowScale = mainWindow.localScale;
        }
    }
}
