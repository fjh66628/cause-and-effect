using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DialogueSO", menuName = "GameData/DialogueData")]
public class DialogueSO : ScriptableObject
{
    [SerializeField] private List<Dialogue> dialogues;
    public List<Dialogue> getDialogues => dialogues;
}
