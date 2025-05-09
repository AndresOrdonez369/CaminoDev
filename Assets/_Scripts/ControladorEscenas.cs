using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para gestionar escenas

public class ControladorEscenas : MonoBehaviour
{
        public void CambiarEscena(string nombreDeEscena)

    {
        if (string.IsNullOrEmpty(nombreDeEscena))
        {
            Debug.LogError("El nombre de la escena no puede estar vacío.");
            return;
        }
        Debug.Log("Cargando escena: " + nombreDeEscena);
        SceneManager.LoadScene(nombreDeEscena);
    }

    // Método público para cambiar a una escena específica por su índice en Build Settings
    // Útil si prefieres usar índices en lugar de nombres.
    public void CambiarEscenaPorIndice(int indiceDeEscena)
    {
        // Opcional: verificar si el índice es válido
        if (indiceDeEscena < 0 || indiceDeEscena >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("Índice de escena fuera de rango: " + indiceDeEscena);
            return;
        }
        Debug.Log("Cargando escena con índice: " + indiceDeEscena);
        SceneManager.LoadScene(indiceDeEscena);
    }

    // --- SALIDA DEL JUEGO ---

    // Método público para salir del juego
    // Puedes llamar a este método desde un botón de UI.
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");

        // Si estás en el Editor de Unity, esto detendrá la ejecución del juego.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Si estás en una build compilada, esto cerrará la aplicación.
        Application.Quit();
#endif
    }
}