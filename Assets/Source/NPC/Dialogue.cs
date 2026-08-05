using System.Collections.Generic;
using UnityEngine;

namespace Source.NPC
{
    public class Dialogue : MonoBehaviour
    {
        [SerializeField] private string _characterName;
        [SerializeField] private Color _characterNameColor;
        [SerializeField] private List<string> _dialogueText;
    }
}
