using System.Collections;
using UnityEngine; 
using TMPro;
using UnityEngine.UI;
using UnityEditor.Rendering;
using System.Linq;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Sciptable Object")]
    [SerializeField] DialogueObject dialogue;

    [Header("Text Area(Narrator and the Lines)")]
    [SerializeField] TextMeshProUGUI lines;
    [SerializeField] TextMeshProUGUI speaker;

    [Header("Text Type Speed")]
    [SerializeField] float textSpeed;
    bool hasStarted = false;
    int i=0; int k=0;
   
    public void StartDialogue()
    {
        if(!hasStarted)
        {
            hasStarted = true;
            speaker.text = dialogue.narrator;
            BridgeFunction();
        }

        else
            NextSentence();
    }

    public void NextSentence()
    {
        StopAllCoroutines();

        
        if(i<dialogue.dialogueLines.Count)
        {
            if(i!=0 && lines.text == dialogue.dialogueLines[i-1])
            {
                StartCoroutine(DialogueDisplay());
            }
            else if(lines.text != dialogue.dialogueLines[i])
            {
                lines.text = dialogue.dialogueLines[i];
                Debug.Log(dialogue.dialogueLines[i]);
                i++;
                Invoke("BridgeFunction",textSpeed);
            }
            else
            {
                i++;
                Invoke("BridgeFunction",textSpeed);
            }
        }
    }

    public void BridgeFunction()
    {
        StartCoroutine(DialogueDisplay());
    }

    IEnumerator DialogueDisplay()
    {
        
        for ( ;i < dialogue.dialogueLines.Count; )
        {
            lines.text = string.Empty;
            foreach (char c in dialogue.dialogueLines[i])
            {
                lines.text += c;
                yield return new WaitForSeconds(textSpeed);
            }
            break;
        }
        k++;
        NextSentence();
        
    }
}
