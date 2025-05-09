using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para gestionar escenas

public class ControladorEscenas : MonoBehaviour
{
        public void CambiarEscena(string nombreDeEscena)

    {
        if (string.IsNullOrEmpty(nombreDeEscena))
        {
            Debug.LogError("El nombre de la escena no puede estar vac�o.");
            return;
        }
        Debug.Log("Cargando escena: " + nombreDeEscena);
        SceneManager.LoadScene(nombreDeEscena);
    }

    // M�todo p�blico para cambiar a una escena espec�fica por su �ndice en Build Settings
    // �til si prefieres usar �ndices en lugar de nombres.
    public void CambiarEscenaPorIndice(int indiceDeEscena)
    {
        // Opcional: verificar si el �ndice es v�lido
        if (indiceDeEscena < 0 || indiceDeEscena >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("�ndice de escena fuera de rango: " + indiceDeEscena);
            return;
        }
        Debug.Log("Cargando escena con �ndice: " + indiceDeEscena);
        SceneManager.LoadScene(indiceDeEscena);
    }

    // --- SALIDA DEL JUEGO ---

    // M�todo p�blico para salir del juego
    // Puedes llamar a este m�todo desde un bot�n de UI.
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");

        // Si est�s en el Editor de Unity, esto detendr� la ejecuci�n del juego.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Si est�s en una build compilada, esto cerrar� la aplicaci�n.
        Application.Quit();
#endif
    }
}