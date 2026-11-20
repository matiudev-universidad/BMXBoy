/*
Juego: BMX del Desierto
Funcionalidad: Objeto (clase C# normal, NO MonoBehaviour) que lee el teclado
               una vez por frame y guarda lo que el jugador quiere hacer.
Controles:  A / D o Flechas  -> moverse
            Shift            -> correr (pedalear más rápido)
            Espacio / W / ↑  -> saltar
*/
using UnityEngine.InputSystem;

public class EntradaJugador
{
    // Propiedades de solo lectura desde afuera (encapsulamiento)
    public float Horizontal { get; private set; }   // -1 izquierda, 0 nada, 1 derecha
    public bool Correr { get; private set; }
    public bool Saltar { get; private set; }         // true solo en el frame en que se presiona

    public void Leer()
    {
        Keyboard teclado = Keyboard.current;
        if (teclado == null) return;   // no hay teclado conectado

        float h = 0f;
        if (teclado.aKey.isPressed || teclado.leftArrowKey.isPressed) h -= 1f;
        if (teclado.dKey.isPressed || teclado.rightArrowKey.isPressed) h += 1f;
        Horizontal = h;

        Correr = teclado.leftShiftKey.isPressed || teclado.rightShiftKey.isPressed;

        Saltar = teclado.spaceKey.wasPressedThisFrame
              || teclado.wKey.wasPressedThisFrame
              || teclado.upArrowKey.wasPressedThisFrame;
    }

    public void Limpiar()
    {
        Horizontal = 0f;
        Correr = false;
        Saltar = false;
    }
}
