using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] GameObject[] slots = new GameObject[9];
    [SerializeField] GameObject inventoryParent;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Camera cam;

    GameObject draggedObject;
    GameObject lastItemSlot;
    public PlayerController playerController;
    public GameObject inventoryPanel;






    private void Awake()
    {
        
    }
    void Start()
    {
    }
    void Update()
    {
        // Ejemplo: Abrir/cerrar inventario con la tecla 'I'
        if (Input.GetKeyDown(KeyCode.I))
        {
            bool isInventoryOpen = !inventoryPanel.activeSelf; // Suponiendo que tienes un panel de inventario
            inventoryPanel.SetActive(isInventoryOpen);

            if (playerController != null)
            {
                playerController.SetUIMode(isInventoryOpen); // Llama a la función del PlayerController
            }
        }

        if (draggedObject != null)
        {
            draggedObject.transform.position = Input.mousePosition;

        }
    }
    

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlot slot = clickedObject.GetComponent<InventorySlot>();

            //There is item in the slot - pick it up
            if (slot != null && slot.heldItem != null)
            {
                draggedObject = slot.heldItem;
                slot.heldItem = null;
                lastItemSlot = clickedObject;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (draggedObject != null && eventData.pointerCurrentRaycast.gameObject != null && eventData.button == PointerEventData.InputButton.Left)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlot slot = clickedObject.GetComponent<InventorySlot>();

            //There isn't item in the slot - place item
            if (slot != null && slot.heldItem == null)
            {
                slot.SetHeldItem(draggedObject);
            }
            //There is item in the slot - switch items
            else if (slot != null && slot.heldItem != null)
            {
                lastItemSlot.GetComponent<InventorySlot>().SetHeldItem(slot.heldItem);
                slot.SetHeldItem(draggedObject);
            }
            //Return item to last slot
            else if (clickedObject.name != "DropItem")
            {
                lastItemSlot.GetComponent<InventorySlot>().SetHeldItem(draggedObject);
            }
            //Drop item
            else
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                Vector3 position = ray.GetPoint(3);

                GameObject newItem = Instantiate(draggedObject.GetComponent<InventoryItem>().itemScriptableObject.prefab, position, new Quaternion());
                newItem.GetComponent<itemPickable>().itemScriptableObject = draggedObject.GetComponent<InventoryItem>().itemScriptableObject;

                lastItemSlot.GetComponent<InventorySlot>().heldItem = null;
            
               
                Destroy(draggedObject);

                
            }

            draggedObject = null;

        }
    }

    public void ItemPicked(GameObject pickedItem)
    {
        GameObject emptySlot = null;

        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i].GetComponent<InventorySlot>();

            if (slot.heldItem == null)
            {
                emptySlot = slots[i];
                break;
            }
        }

        if (emptySlot != null)
        {
            

            GameObject newItem = Instantiate(itemPrefab);
            newItem.GetComponent<InventoryItem>().itemScriptableObject = pickedItem.GetComponent<itemPickable>().itemScriptableObject;
            newItem.transform.SetParent(emptySlot.transform.parent.parent.GetChild(2));
            newItem.GetComponent<InventoryItem>().stackCurrent = 1;

            emptySlot.GetComponent<InventorySlot>().SetHeldItem(newItem);
            newItem.transform.localScale = new Vector3(1, 1, 1);

            Destroy(pickedItem);
        }
    }





}
