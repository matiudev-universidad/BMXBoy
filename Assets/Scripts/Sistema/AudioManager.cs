/*
Juego: BMX del Desierto
Funcionalidad: Maneja todo el sonido del juego.
  - Música de fondo en loop.
  - Efectos: salto, aterrizaje, estrella, choque y victoria.
  - Sonido de arrastre de las ruedas sobre la tierra (más fuerte al ir rápido).
Usa 3 AudioSource: música, efectos y rodado.
*/
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instancia;
    public static AudioManager Instancia
    {
        get
        {
            if (instancia == null) instancia = FindAnyObjectByType<AudioManager>();
            return instancia;
        }
    }

    [Header("Fuentes de audio")]
    [SerializeField] private AudioSource fuenteMusica;
    [SerializeField] private AudioSource fuenteEfectos;
    [SerializeField] private AudioSource fuenteRodado;

    [Header("Clips")]
    public AudioClip musica;
    public AudioClip sonidoSalto;
    public AudioClip sonidoAterrizaje;
    public AudioClip sonidoEstrella;
    public AudioClip sonidoChoque;
    public AudioClip sonidoVictoria;
    public AudioClip sonidoArrastre;

    private void Awake()
    {
        instancia = this;
    }

    private void Start()
    {
        fuenteMusica.clip = musica;
        fuenteMusica.loop = true;
        fuenteMusica.Play();

        fuenteRodado.clip = sonidoArrastre;
        fuenteRodado.loop = true;
        fuenteRodado.volume = 0f;
        fuenteRodado.Play();
    }

    public void Reproducir(AudioClip clip)
    {
        if (clip != null) fuenteEfectos.PlayOneShot(clip);
    }

    // intensidad: 0 = quieto o en el aire, 1 = a toda velocidad
    public void ActualizarRodado(float intensidad, bool corriendo)
    {
        float volumenObjetivo = Mathf.Clamp01(intensidad) * 0.5f;
        fuenteRodado.volume = Mathf.MoveTowards(fuenteRodado.volume, volumenObjetivo, 3f * Time.deltaTime);
        fuenteRodado.pitch = corriendo ? 1.3f : 1f;
    }

    public void DetenerMusica()
    {
        fuenteMusica.Stop();
        fuenteRodado.volume = 0f;
    }
}
