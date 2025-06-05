using UnityEngine;
using TMPro;

public class TextMoveController : MonoBehaviour
{
    public RectTransform textTransform; // Assign the text UI here
    public Vector2 targetPosition = new Vector2(0, 0); // Final position
    public float moveSpeed = 300f;
    public float delayBeforeMove = 1.5f; // Time to wait for blur to finish

    void Start()
    {
        textTransform.gameObject.SetActive(false); // Hide initially
        StartCoroutine(MoveTextAfterDelay());
    }

    System.Collections.IEnumerator MoveTextAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeMove);

        textTransform.gameObject.SetActive(true); // Show text
        Vector2 start = textTransform.anchoredPosition;

        while (Vector2.Distance(textTransform.anchoredPosition, targetPosition) > 1f)
        {
            textTransform.anchoredPosition = Vector2.MoveTowards(
                textTransform.anchoredPosition,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
    }
}
