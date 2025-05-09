using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] Camera cam;
    [SerializeField] InventoryManager inventoryManager;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 3))
            {
                itemPickable item = hitInfo.collider.gameObject.GetComponent<itemPickable>();
                if (item != null)
                {
                    inventoryManager.ItemPicked(hitInfo.collider.gameObject);
                }

            }

        }
    }
}
