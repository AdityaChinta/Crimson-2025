using System.Collections;
using UnityEngine; 
using TMPro;
using UnityEngine.UI;
using UnityEditor.Rendering;
using System.Linq;
using Unity.Mathematics;
using System.Configuration.Assemblies;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueObject dialogue;

    [Header("Text Area(Narrator and the Lines)")]
    [SerializeField] TextMeshProUGUI lines;
    [SerializeField] TextMeshProUGUI speaker;

    [Header("Text Type Speed")]
    [SerializeField] float textSpeed;
    public bool hasStarted = false;
    int i = 0; int k = 0; int tot = 0;

    public DialogueManager forSwap;
    // The below comment is just for reference and can be ignored
    /*
    public void NextSentence()
    {
        if (forSwap.narrator == 1)
        {
            if (i < dialogue.dialogueLines.Count)
            {
                if (i != 0 && lines.text == dialogue.dialogueLines[i - 1])
                {
                    StartCoroutine(DialogueDisplay());
                }
                else if (lines.text != dialogue.dialogueLines[i])
                {
                    lines.text = dialogue.dialogueLines[i];
                    Debug.Log(dialogue.dialogueLines[i]);
                    i++;
                    Invoke("forSwap.DisplayNextSentence", textSpeed);
                }
                else
                {
                    i++;
                    Invoke("forSwap.DisplayNextSentence", textSpeed);
                }
            }
        }
    }*/

    IEnumerator Type()
    {
        if (dialogue.dialogueLines.Count != 0)
        {
            lines.text = string.Empty;
            foreach (char c in dialogue.dialogueLines[i])
            {
                lines.text += c;
                yield return new WaitForSeconds(textSpeed);
            }
            k++;
            DisplaySentence();
        }
    }

    public void DisplaySentence()
    {
        if (k == 2)
        {
            i++;
            k = 0;
        }
        StopAllCoroutines();

        if (forSwap.total != tot )
        {
            SwapNarrator();
            speaker.text = dialogue.narrator;
        }
        else
        { EndConversation(); }
    }

    public void OnContinue()
    {
        // This method prints the whole sentence and when already a whole sentence has been printed and the button is clicked once more, the next sentence won't be typed
        // The next sentence will be printed as a whole
        if (dialogue.dialogueLines.Count != 0)
        {
            StopAllCoroutines();
            lines.text = dialogue.dialogueLines[i];
            k++;
            Invoke("DisplaySentence", 4*textSpeed-(textSpeed/3));
        }
    }

    public void EndConversation()
    {
        StopAllCoroutines();
        forSwap.can.enabled = false;
        //PlayerControl.canMove = true;
        //PlayerControl.
    }

    public void SwapNarrator()
    {
        tot++;
        if (forSwap.narrator == 1)
        {
            forSwap.narrator = 0;
            dialogue = forSwap.second;
        }

        else
        {
            forSwap.narrator = 1;
            dialogue = forSwap.starter;
        }
        StartCoroutine(Type());
    }
}
