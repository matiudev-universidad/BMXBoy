/*
Juego: BMX del Desierto
Funcionalidad: Da vuelta la bicicleta para que mire hacia donde se mueve
               (invierte la escala X, igual que el Flip del profe).
Hereda de: ComponenteBicicleta
*/
using UnityEngine;

public class Voltear : ComponenteBicicleta
{
    public bool MiraDerecha { get; private set; } = true;

    public override void Actualizar(EntradaJugador entrada)
    {
        if (entrada.Horizontal > 0f && !MiraDerecha) Girar();
        else if (entrada.Horizontal < 0f && MiraDerecha) Girar();
    }

    private void Girar()
    {
        MiraDerecha = !MiraDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1f;
        transform.localScale = escala;
    }
}
