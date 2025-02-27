using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class InputTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public InputActionReference action;
    private void Start()
    {
        action.action.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (action.action.WasPressedThisFrame())
        {
            GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
        }
    }
}
