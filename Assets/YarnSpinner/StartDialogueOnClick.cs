using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Yarn.Unity;

public class StartDialogueOnClick : MonoBehaviour, IPointerClickHandler
{
    private DialogueRunner runner;
    [SerializeField] string startNode;

    private void Start()
    {
        runner = FindAnyObjectByType<DialogueRunner>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        runner.StartDialogue(startNode);
    }
}
