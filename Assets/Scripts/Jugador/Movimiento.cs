/*
Juego: BMX del Desierto
Funcionalidad: Mueve la bicicleta a la izquierda/derecha. Con Shift corre más rápido.
Hereda de: ComponenteBicicleta
*/
using UnityEngine;

public class Movimiento : ComponenteBicicleta
{
    [SerializeField] private float velocidadNormal = 5f;   // pedaleo normal
    [SerializeField] private float velocidadCorrer = 9f;   // con Shift
    [SerializeField] private float aceleracion = 25f;      // qué tan rápido alcanza la velocidad

    public bool EstaCorriendo { get; private set; }

    public override void Actualizar(EntradaJugador entrada)
    {
        EstaCorriendo = entrada.Correr && entrada.Horizontal != 0f;
        float velocidadMax = EstaCorriendo ? velocidadCorrer : velocidadNormal;
        float objetivo = entrada.Horizontal * velocidadMax;

        // Acelera de a poco hacia la velocidad objetivo (se siente como pedalear)
        Vector2 v = rb.linearVelocity;
        v.x = Mathf.MoveTowards(v.x, objetivo, aceleracion * Time.deltaTime);
        rb.linearVelocity = v;
    }
}
