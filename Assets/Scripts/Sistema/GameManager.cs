/*
Juego: BMX del Desierto
Funcionalidad: Controla el estado de la partida: pantalla de inicio, jugando,
               fin del juego (choque) y victoria (llegar a la meta).
               También cuenta las estrellas azules.
Patrón Singleton: existe UNA sola instancia y se accede con GameManager.Instancia.
*/
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum EstadoJuego { Inicio, Jugando, GameOver, Victoria }

public class GameManager : MonoBehaviour
{
    // Si por algún motivo aún no está asignada (por ej. Unity recompiló), la busca en la escena.
    private static GameManager instancia;
    public static GameManager Instancia
    {
        get
        {
            if (instancia == null) instancia = FindAnyObjectByType<GameManager>();
            return instancia;
        }
    }

    public EstadoJuego Estado { get; private set; } = EstadoJuego.Inicio;
    public int Estrellas { get; private set; }
    public int TotalEstrellas { get; private set; }
    public float TiempoPartida { get; private set; }

    private void Awake()
    {
        instancia = this;
    }

    private void Update()
    {
        Keyboard teclado = Keyboard.current;
        if (teclado == null) return;

        switch (Estado)
        {
            case EstadoJuego.Inicio:
                if (teclado.enterKey.wasPressedThisFrame || teclado.spaceKey.wasPressedThisFrame)
                    Estado = EstadoJuego.Jugando;   // ¡empieza la partida!
                break;

            case EstadoJuego.Jugando:
                TiempoPartida += Time.deltaTime;
                break;

            case EstadoJuego.GameOver:
            case EstadoJuego.Victoria:
                if (teclado.rKey.wasPressedThisFrame || teclado.enterKey.wasPressedThisFrame)
                    Reiniciar();
                break;
        }
    }

    public void RegistrarEstrella() => TotalEstrellas++;
    public void SumarEstrella() => Estrellas++;

    public void TerminarJuego()
    {
        if (Estado != EstadoJuego.Jugando) return;
        Estado = EstadoJuego.GameOver;
        AudioManager.Instancia.DetenerMusica();
    }

    public void GanarJuego()
    {
        if (Estado != EstadoJuego.Jugando) return;
        Estado = EstadoJuego.Victoria;
        AudioManager.Instancia.DetenerMusica();
        AudioManager.Instancia.Reproducir(AudioManager.Instancia.sonidoVictoria);
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
