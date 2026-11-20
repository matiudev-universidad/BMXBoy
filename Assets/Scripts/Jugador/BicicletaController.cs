/*
Juego: BMX del Desierto
Funcionalidad: Clase principal del jugador (la bicicleta). Lee el teclado con un
               objeto EntradaJugador y se lo pasa a cada ComponenteBicicleta.
POO:
  - Composición: la bicicleta "tiene" Movimiento, Salto, Voltear, etc.
  - Polimorfismo: recorre un arreglo de ComponenteBicicleta y llama Actualizar()
    en cada uno; cada clase hija responde a su manera.
Componentes que exige en el GameObject: Rigidbody2D y BoxCollider2D.
*/
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Movimiento))]
[RequireComponent(typeof(Salto))]
[RequireComponent(typeof(Voltear))]
public class BicicletaController : MonoBehaviour
{
    [SerializeField] private float alturaMuerte = -6f;   // si cae más abajo, pierde

    private Rigidbody2D rb;
    private Movimiento movimiento;
    private Salto salto;
    private ComponenteBicicleta[] componentes;
    private readonly EntradaJugador entrada = new EntradaJugador();   // un OBJETO de una clase propia
    private bool viva = true;

    public bool EnSuelo => salto.EnSuelo;
    public bool EstaViva => viva;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;   // la bici no se tumba
        movimiento = GetComponent<Movimiento>();
        salto = GetComponent<Salto>();

        // Busca TODOS los componentes que heredan de ComponenteBicicleta
        // (en este objeto y en sus hijos: Visual y las ruedas).
        componentes = GetComponentsInChildren<ComponenteBicicleta>();
        foreach (ComponenteBicicleta c in componentes)
            c.Inicializar(this);
    }

    private void Update()
    {
        if (!viva) return;

        // Solo se lee el teclado mientras la partida está en curso
        if (GameManager.Instancia.Estado == EstadoJuego.Jugando) entrada.Leer();
        else entrada.Limpiar();

        foreach (ComponenteBicicleta c in componentes)
            c.Actualizar(entrada);   // <- polimorfismo

        // Sonido de ruedas sobre la tierra: más fuerte cuanto más rápido va
        float intensidad = EnSuelo ? Mathf.Abs(rb.linearVelocity.x) / 9f : 0f;
        AudioManager.Instancia.ActualizarRodado(intensidad, movimiento.EstaCorriendo);

        if (transform.position.y < alturaMuerte) Morir();
    }

    // La llaman los enemigos y los cactus al chocar con la bicicleta.
    public void Morir()
    {
        if (!viva) return;
        viva = false;

        AudioManager.Instancia.ActualizarRodado(0f, false);
        AudioManager.Instancia.Reproducir(AudioManager.Instancia.sonidoChoque);

        // Pequeña "voltereta" de choque
        rb.freezeRotation = false;
        rb.linearVelocity = new Vector2(-2f * Mathf.Sign(transform.localScale.x), 7f);
        rb.angularVelocity = 400f;
        GetComponent<Collider2D>().enabled = false;   // atraviesa el suelo y cae fuera

        GameManager.Instancia.TerminarJuego();
    }
}
