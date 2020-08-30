using UnityEngine;

public class UIConroller : MonoBehaviour
{

    [SerializeField] GameObject inventory;
    [SerializeField] KeyCode inventoryToggle = KeyCode.I;

    [SerializeField] GameObject minimap;
    [SerializeField] KeyCode minimapToggle = KeyCode.M;

    private void Start()
    {
        inventory.SetActive(false); 
    }


    void Update()
    {
     
        if(Input.GetKeyDown(inventoryToggle))
        {
            inventory.SetActive(!inventory.activeSelf);
        }
        
        if (Input.GetKeyDown(minimapToggle))
        {
            minimap.SetActive(!minimap.activeSelf);
        }  
    }
}
