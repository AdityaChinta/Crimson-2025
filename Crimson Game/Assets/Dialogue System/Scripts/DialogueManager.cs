using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] public DialogueObject starter;
    [SerializeField] public DialogueObject second;
    [SerializeField] DialogueTrigger connect;
    public int narrator,total;

    public Canvas can; // For enabling and disabling the Canvas

    void Start()
    {
        can.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "NPC")
        {
            PlayerControl.canMove = false;
            ConvoStart();
        }
    }
    
    public void ConvoStart()
    {
        if (!connect.hasStarted)
        {
            can.enabled = true; 
            total = starter.dialogueLines.Count + second.dialogueLines.Count ; // Why add 1? I'll look into it later
            narrator = 0;
            connect.DisplaySentence();
        }
    }
}
