/*
Juego: BMX del Desierto
Enemigo 2: Valla que sale del suelo, se queda arriba, se esconde y repite.
           Hay que pasar cuando está abajo o saltarla cuando está arriba.
Hereda de: Enemigo (que a su vez hereda de ObjetoMovil)
Ciclo:  abajo (espera) -> sube -> arriba (espera) -> baja -> ...
*/
using UnityEngine;

public class Valla : Enemigo
{
    [SerializeField] private float altura = 1.6f;          // cuánto sube
    [SerializeField] private float tiempoAbajo = 1.2f;
    [SerializeField] private float tiempoMovimiento = 0.4f;
    [SerializeField] private float tiempoArriba = 1.5f;
    [SerializeField] private float desfase = 0f;            // para que no estén sincronizadas

    protected override Vector2 CalcularDesplazamiento(float t)
    {
        float ciclo = tiempoAbajo + tiempoMovimiento + tiempoArriba + tiempoMovimiento;
        float fase = (t + desfase) % ciclo;   // % (módulo) hace que se repita en loop

        float subida;   // 0 = escondida, 1 = arriba
        if (fase < tiempoAbajo)
            subida = 0f;
        else if (fase < tiempoAbajo + tiempoMovimiento)
            subida = (fase - tiempoAbajo) / tiempoMovimiento;
        else if (fase < tiempoAbajo + tiempoMovimiento + tiempoArriba)
            subida = 1f;
        else
            subida = 1f - (fase - tiempoAbajo - tiempoMovimiento - tiempoArriba) / tiempoMovimiento;

        return new Vector2(0f, Mathf.SmoothStep(0f, 1f, subida) * altura);
    }
}
