using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] public DialogueObject starter;
    [SerializeField] public DialogueObject second;
    public DialogueTrigger connect;
    public int narrator,total;

    public Canvas can; // For enabling and disabling the Canvas

    bool doneOnce = false;

    void Start()
    {
        can.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "NPC")
        {
            ConvoStart();
        }
    }
    
    public void ConvoStart()
    {
        if (!connect.hasStarted)
        {
            can.enabled = true; 
            total = starter.dialogueLines.Count + second.dialogueLines.Count;
            narrator = 0;
            connect.DisplaySentence();
        }
    }
}
