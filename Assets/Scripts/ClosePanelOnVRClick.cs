using UnityEngine;

public class ClosePanelOnVRClick : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Assign the Panel or Canvas to Hide")]
    public GameObject panelToHide;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            if (panelToHide != null)
            {
                panelToHide.SetActive(false);
            }
        }
    }
}
