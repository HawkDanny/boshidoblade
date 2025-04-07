using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHawk;

[ExecuteInEditMode]
public class BoshidoManager : MonoBehaviour
{
    [SerializeField] Emulator emulator;

    // Start is called before the first frame update
    void OnEnable()
    {
        emulator.RegisterLuaCallback("IncrementBoshiScore", IncrementBoshiScore);
    }

    static string IncrementBoshiScore(string arg)
    {
        print(arg);
        return "";
    }
}
