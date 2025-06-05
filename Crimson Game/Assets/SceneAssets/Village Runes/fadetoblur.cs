using UnityEngine;

public class FadeToBlur : MonoBehaviour
{
    [SerializeField] CanvasGroup blurGroup; // Assign this in Inspector
    public float fadeDuration = 2f; // Seconds
    private float timer = 0f;
    private bool startFade = false;

    void Start()
    {
        // Optional: Delay fade to give players a moment before it starts
        Invoke("StartFade", 1f);  // 1-second delay
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