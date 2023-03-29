using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableWindow
    : MonoBehaviour,
        IDragHandler,
        IBeginDragHandler,
        IEndDragHandler,
        IPointerDownHandler
{
    [SerializeField]
    private RectTransform _dragRectTransform;

    [SerializeField]
    private Canvas _canvas;

    [SerializeField]
    private Image _backgroundImage;
    private Color _backgroundColor;

    private void Awake()
    {
        if (_backgroundImage == null)
        {
            _backgroundImage = GetComponent<Image>();
        }
        _backgroundColor = _backgroundImage.color;

        if (_dragRectTransform == null)
        {
            _dragRectTransform = GetComponent<RectTransform>();
        }

        if (_canvas == null)
        {
            Transform testCanvasTransform = transform.parent;
            while (testCanvasTransform != null)
            {
                _canvas = testCanvasTransform.GetComponent<Canvas>();
                if (_canvas != null)
                {
                    break;
                }
                testCanvasTransform = testCanvasTransform.parent;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        _dragRectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _backgroundColor.a = .4f;
        _backgroundImage.color = _backgroundColor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _backgroundColor.a = 1f;
        _backgroundImage.color = _backgroundColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _dragRectTransform.SetAsLastSibling();
    }
}
