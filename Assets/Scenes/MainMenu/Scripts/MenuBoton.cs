using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuBoton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Animación")]
    public Vector3 escalaNormal = Vector3.one;
    public Vector3 escalaGrande = new Vector3(1.1f, 1.1f, 1.1f);
    public float velocidad = 8f;

    [Header("Canvas")]
    public GameObject canvasEncender;
    public GameObject canvasApagar;

    private bool mouseEncima = false;

    void Update()
    {
        if (mouseEncima)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                escalaGrande,
                velocidad * Time.deltaTime
            );
        }
        else
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                escalaNormal,
                velocidad * Time.deltaTime
            );
        }
    }

    // Cuando el mouse entra
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseEncima = true;
    }

    // Cuando el mouse sale
    public void OnPointerExit(PointerEventData eventData)
    {
        mouseEncima = false;
    }

    // BOTÓN JUGAR
    public void CargarEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    // BOTÓN OPCIONES
    public void CambiarCanvas()
    {
        if (canvasEncender != null)
            canvasEncender.SetActive(true);

        if (canvasApagar != null)
            canvasApagar.SetActive(false);
    }

    // BOTÓN SALIR
    public void SalirJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}