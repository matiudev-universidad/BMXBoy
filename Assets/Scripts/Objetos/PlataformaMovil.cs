/*
Juego: BMX del Desierto
Funcionalidad: Plataforma de roca que va y viene (horizontal o vertical)
               para cruzar barrancos.
Hereda de: ObjetoMovil
*/
using UnityEngine;

public class PlataformaMovil : ObjetoMovil
{
    [SerializeField] private Vector2 direccion = Vector2.right;  // (1,0) horizontal, (0,1) vertical
    [SerializeField] private float distancia = 4f;

    protected override Vector2 CalcularDesplazamiento(float t)
    {
        // Coseno: sale de 0, llega a "distancia" y vuelve, frenando suave en los extremos
        float avance = (1f - Mathf.Cos(t)) * 0.5f * distancia;
        return direccion.normalized * avance;
    }
}
