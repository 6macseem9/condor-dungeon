using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class NameGenerator
{
    private static List<string> _names = new List<string>();

    public static string GetRandomName()
    {
        if (_names.Count == 0) _names = (UnityEngine.Resources.Load<TextAsset>("names").text).Split('\n').ToList();

        return _names[Random.Range(0, _names.Count)];
    }
}
