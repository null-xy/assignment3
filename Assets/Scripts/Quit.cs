using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Quit : MonoBehaviour
{
    public InputActionReference action;

    // Start is called before the first frame update
    void Start()
    {
        action.action.Enable();
        action.action.performed += (ctx) =>
        {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        };

    }

    // Update is called once per frame
    void Update()
    {

    }
}
