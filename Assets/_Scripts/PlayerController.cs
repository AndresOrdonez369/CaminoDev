using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5.0f;
    public float jumpForce = 8.0f;
    public float gravity = 20.0f;

    [Header("Camera Look")]
    public Camera playerCamera;
    public float mouseSensitivity = 2.0f;
    public float verticalLookLimit = 80.0f;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float verticalLookRotation = 0f;

    // Nueva variable para controlar el estado del cursor/UI
    private bool isUIModeActive = false;

    public Camera cam; // Asigna la cámara del jugador en el Inspector
    public float pickupDistance = 3f; // Distancia a la que se puede recoger
    public LayerMask pickupLayerMask;   // Capa(s) en la(s) que están los ítems recogibles


    private int itemsCollectedCount = 0; // Contador para los ítems recogidos
    public int totalItemsToWin = 9;      // Número total de ítems para ganar
    public string winSceneName = "escenaFinal"; // Nombre de la escena a cargar

    [SerializeField] InventoryManager inventoryManager;
    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                Debug.LogError("Player Camera not assigned and Main Camera not found!");
                enabled = false;
                return;
            }
        }

        // Inicialmente, el cursor está bloqueado para el modo juego
        SetCursorState(false); // false para modo juego
    }

    void Update()
    {
        // Tecla para alternar entre modo UI y modo Juego (ej. Tab)
        if (Input.GetKeyDown(KeyCode.Tab)) // Puedes cambiar esta tecla
        {
            ToggleUIMode();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("'E' presionada - Intentando recoger...");
            AttemptPickup();
        }

        // Solo procesar movimiento y vista si NO estamos en modo UI
        if (!isUIModeActive)
        {
            HandleMovement();
            HandleLook();
        }
        // Si isUIModeActive es true, el EventSystem de Unity se encargará de los clics en la UI.
    }

    // Función para cambiar entre modo UI y modo Juego
    public void ToggleUIMode()
    {
        isUIModeActive = !isUIModeActive;
        SetCursorState(isUIModeActive);
    }

    void AttemptPickup()
    {
        // Lanzar un rayo desde el centro de la vista de la cámara
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        // Alternativa más precisa para el centro del viewport:
        // Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); 
        RaycastHit hitInfo;
        Debug.DrawRay(ray.origin, ray.direction * pickupDistance, Color.red, 2.0f);
        // Realizar el raycast, usando la distancia y la LayerMask
        if (Physics.Raycast(ray, out hitInfo, pickupDistance, pickupLayerMask))
        {
            Debug.Log("Raycast golpeó: " + hitInfo.collider.gameObject.name + " en la capa: " + LayerMask.LayerToName(hitInfo.collider.gameObject.layer));

            // Intentar obtener el componente itemPickable del objeto golpeado
            itemPickable item = hitInfo.collider.gameObject.GetComponent<itemPickable>();

            if (item != null)
            {
                Debug.Log("Ítem recogible encontrado: " + item.gameObject.name);
                // Llamar al método del InventoryManager para recoger el ítem
                // Asumo que ItemPicked espera el GameObject del ítem.
                // Si espera el script itemPickable, pasa 'item' en su lugar.
                itemsCollectedCount++;
                Debug.Log("almaceno un gran valor");
                Debug.Log(itemsCollectedCount);


                if (itemsCollectedCount >= totalItemsToWin)
                {
                    SceneManager.LoadScene(winSceneName);
                }
                inventoryManager.ItemPicked(item.gameObject);
            }
            else
            {
                Debug.Log(hitInfo.collider.gameObject.name + " no es un ítem recogible (no tiene el script 'itemPickable').");
            }
        }
        else
        {
            Debug.Log("Raycast no golpeó ningún objeto recogible dentro del rango o en la capa correcta.");
        }
    }


// Función para establecer explícitamente el modo UI (útil desde otros scripts)
public void SetUIMode(bool setActive)
    {
        isUIModeActive = setActive;
        SetCursorState(isUIModeActive);
    }


    // Función para configurar el estado del cursor
    private void SetCursorState(bool uiMode)
    {
        if (uiMode)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        if (characterController.isGrounded)
        {
            moveDirection = (forward * verticalInput + right * horizontalInput).normalized * moveSpeed;
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpForce;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleLook()
    {
        // Si estamos en modo UI, no procesamos la vista de la cámara
        if (isUIModeActive) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -verticalLookLimit, verticalLookLimit);
        playerCamera.transform.localEulerAngles = Vector3.right * verticalLookRotation;
    }

    // OnApplicationFocus puede ser problemático si gestionamos el cursor manualmente.
    // Considera si realmente lo necesitas o ajústalo.
    void OnApplicationFocus(bool hasFocus)
    {
        // Solo volvemos a bloquear el cursor si el juego recupera el foco Y NO estamos en modo UI.
        if (hasFocus && !isUIModeActive)
        {
            SetCursorState(false); // Volver a modo juego
        }
        // Si pierde el foco, podrías querer desbloquear el cursor temporalmente,
        // pero es más simple dejar que el sistema operativo lo maneje, o
        // llamar a SetCursorState(true) si es necesario.
        // if (!hasFocus)
        // {
        //     SetCursorState(true); // Temporalmente liberar cursor
        // }
    }
}