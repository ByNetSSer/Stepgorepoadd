using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class SceneButton : MonoBehaviour
{
    [Header("Referencias")]
    public CloudManager cloudManager;
    public UIScene currentScene;
    public UIScene targetScene;

    [Header("Opciones")]
    public bool useCloudTransition = true;
    public bool useInstantSwap = false;

    public Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnPressed);
    }

    private void OnPressed()
    {
        if (cloudManager != null && cloudManager.state == CloudManager.CloudState.Animating)
            return;

        if (useCloudTransition && cloudManager != null)
        {
            cloudManager.PlayLoadingTransition();

            // Esperar el tiempo total antes de cambiar escenas
            float delay = cloudManager.upDuration + cloudManager.bounceDuration + cloudManager.holdTime;
            DOVirtual.DelayedCall(delay, SwapScenes);
        }
        else
        {
            SwapScenes();
        }
    }

    private void SwapScenes()
    {
        if (targetScene == null || currentScene == null)
            return;

        // 1?? Activa primero la escena objetivo (invisible o desplazada)
        targetScene.Show(useInstantSwap);

        // 2?? Luego oculta la escena actual
        currentScene.Hide(useInstantSwap);
    }
    public void OnClick()
    {
        OnPressed();
    }
}
