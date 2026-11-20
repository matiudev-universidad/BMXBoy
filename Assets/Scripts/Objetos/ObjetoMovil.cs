/*
Juego: BMX del Desierto
Funcionalidad: CLASE BASE ABSTRACTA para todo objeto que se mueve solo,
               con una rutina automática, continua y en loop.
               Ej.: enemigos, plataformas móviles y estrellas.
POO:
  - Esta clase hace lo COMÚN: guarda la posición inicial, cuenta el tiempo y mueve
    el Rigidbody2D cada paso de física.
  - Cada hija solo dice CÓMO se desplaza, sobrescribiendo CalcularDesplazamiento().
Física: usa un Rigidbody2D Kinematic (lo mueve el código y la gravedad no lo afecta).
*/
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class ObjetoMovil : MonoBehaviour
{
    [SerializeField] protected float velocidad = 1f;   // qué tan rápido hace su rutina

    protected Rigidbody2D rb;
    protected Vector2 posicionInicial;
    protected float tiempo;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        posicionInicial = rb.position;
    }

    protected virtual void FixedUpdate()
    {
        tiempo += Time.fixedDeltaTime * velocidad;
        rb.MovePosition(posicionInicial + CalcularDesplazamiento(tiempo));
    }

    // Cada clase hija devuelve cuánto se aleja de su posición inicial en el tiempo t.
    protected abstract Vector2 CalcularDesplazamiento(float t);
}
