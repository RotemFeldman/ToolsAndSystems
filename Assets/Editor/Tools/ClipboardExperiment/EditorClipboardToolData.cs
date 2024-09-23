using System;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands.Merge.IncomingChanges;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EditorClipboardToolData : ScriptableObject
{
   [SerializeField] public List<CopyData> Data = new();
}