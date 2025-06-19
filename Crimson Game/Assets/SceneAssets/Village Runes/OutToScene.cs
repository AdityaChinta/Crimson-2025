using UnityEngine;

public class OutToScene : MonoBehaviour
{
    [SerializeField] CanvasGroup blurGroup; // Assign this in Inspector
    [SerializeField] float fadeDuration = 2f; // Duration of fade
    [SerializeField] float fadeDelay = 1f;    // Delay before fade starts (serialized)

    private float timer = 0f;
    private bool startFade = false;

    void Start()
    {
        Invoke(nameof(StartFade), fadeDelay);  // Use serialized delay
    }

    void StartFade()
    {
        startFade = true;
    }

    void Update()
    {
        if (startFade && blurGroup.alpha < 1f)
        {
            timer += Time.deltaTime;
            blurGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
        }
    }
}