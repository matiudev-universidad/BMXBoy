/*
Juego: BMX del Desierto
Funcionalidad: Hace girar cada rueda según la velocidad de la bicicleta.
               Va en los objetos hijos "RuedaTrasera" y "RuedaDelantera".
Hereda de: ComponenteBicicleta
*/
using UnityEngine;

public class RuedaGiratoria : ComponenteBicicleta
{
    [SerializeField] private float radio = 0.41f;   // en unidades de Unity

    public override void Actualizar(EntradaJugador entrada)
    {
        // Velocidad "hacia adelante" (positiva si avanza hacia donde mira)
        float mirada = Mathf.Sign(bici.transform.localScale.x);
        float avance = rb.linearVelocity.x * mirada;

        // Ángulo girado = distancia / radio (convertido a grados). Negativo = sentido horario.
        float grados = -(avance / radio) * Mathf.Rad2Deg * Time.deltaTime;
        transform.Rotate(0f, 0f, grados);
    }
}
