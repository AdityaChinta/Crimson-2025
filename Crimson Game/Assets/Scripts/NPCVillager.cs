using UnityEngine;

public class NPCVillagerIdle : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Get reference to the Animator component attached to the NPC
        animator = GetComponent<Animator>();

        // Start playing idle animation (optional if animator is set up to default to idle)
        animator.Play("Idle");
    }

    void Update()
    {
        // Keep the idle animation playing to prevent interruption
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            animator.Play("Idle");
        }
    }
}
