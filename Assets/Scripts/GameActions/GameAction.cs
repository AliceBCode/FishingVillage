using System;
using UnityEngine;

[Serializable]
public abstract class GameAction
{
    public abstract string ActionName { get; }
    public abstract void Execute();
}