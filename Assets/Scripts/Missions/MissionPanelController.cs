using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;   // si usas DOTween

public class MissionPanelController : MonoBehaviour
{
    [Header("UI")]
    public RectTransform panel;          // El panel completo
    public Button closeButton;           // Botón dentro del panel
    public GameObject openButton;        // Botón flotante

    [Header("Positions")]
    public Vector2 visiblePosition;      // Posición dentro de pantalla
    public Vector2 hiddenPosition;       // Posición fuera de pantalla

    [Header("Animation")]
    public float moveTime = 0.5f;
    public Ease moveEase = Ease.OutCubic;

    private bool isOpen = false;

    private void Start()
    {
        // Panel inicia oculto
        panel.anchoredPosition = hiddenPosition;
        openButton.SetActive(true);
        closeButton.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel()
    {
        if (isOpen) return;

        isOpen = true;

        panel.gameObject.SetActive(true);
        openButton.SetActive(false);  // ocultar botón flotante

        panel.DOAnchorPos(visiblePosition, moveTime).SetEase(moveEase);
    }

    public void ClosePanel()
    {
        if (!isOpen) return;

        isOpen = false;

        panel.DOAnchorPos(hiddenPosition, moveTime).SetEase(moveEase)
            .OnComplete(() =>
            {
                panel.gameObject.SetActive(false);   // ocultarlo cuando ya se fue
                openButton.SetActive(true);          // reactivar botón flotante
            });
    }

    // Para el botón flotante
    public void ToggleFromButton()
    {
        if (isOpen)
            ClosePanel();
        else
            OpenPanel();
    }
}
