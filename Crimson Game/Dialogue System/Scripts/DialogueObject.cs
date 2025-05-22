using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "New Dialogue")]

public class DialogueObject : ScriptableObject
{
    
    [Header("Narrator")]
    [SerializeField] public string narrator;

    [Header("Dialogue")] 
    [TextArea(2,3)]
    [SerializeField] public List<string> dialogueLines = new List<string>();

}
