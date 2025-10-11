using System;
using System.Collections;
using RobbieWagnerGames.FirstPerson.Interaction;
using UnityEngine;

public class ConsoleInteractable : Interactable
{
    [SerializeField] private GameConsole console;

    public override IEnumerator Interact()
    {
        yield return StartCoroutine(console.BeginUse());
    }
}