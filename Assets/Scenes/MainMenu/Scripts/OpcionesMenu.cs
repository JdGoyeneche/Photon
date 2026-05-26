using UnityEngine;
using UnityEngine.UI;

public class OpcionesMenu : MonoBehaviour
{
    public static OpcionesMenu instancia;

    [Header("UI")]
    public Slider sliderVolumen;
    public Toggle togglePantallaCompleta;

    private void Awake()
    {
        // Evita duplicados
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Cargar configuraciones guardadas
        float volumen = PlayerPrefs.GetFloat("Volumen", 1f);
        int pantallaCompleta = PlayerPrefs.GetInt("PantallaCompleta", 1);

        // Aplicar configuraciones
        AudioListener.volume = volumen;
        Screen.fullScreen = pantallaCompleta == 1;

        // Actualizar UI
        sliderVolumen.value = volumen;
        togglePantallaCompleta.isOn = Screen.fullScreen;

        // Eventos
        sliderVolumen.onValueChanged.AddListener(CambiarVolumen);
        togglePantallaCompleta.onValueChanged.AddListener(CambiarPantallaCompleta);
    }

    // =========================
    // VOLUMEN
    // =========================
    public void CambiarVolumen(float volumen)
    {
        AudioListener.volume = volumen;

        PlayerPrefs.SetFloat("Volumen", volumen);
        PlayerPrefs.Save();
    }

    // =========================
    // PANTALLA COMPLETA
    // =========================
    public void CambiarPantallaCompleta(bool completa)
    {
        Screen.fullScreen = completa;

        PlayerPrefs.SetInt("PantallaCompleta", completa ? 1 : 0);
        PlayerPrefs.Save();
    }
}