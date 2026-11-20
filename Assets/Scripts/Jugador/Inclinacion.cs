/*
Juego: BMX del Desierto
Funcionalidad: Inclina el dibujo de la bicicleta en el aire (nariz arriba al
               subir, nariz abajo al caer), como un BMX. Solo es visual:
               va en el objeto hijo "Visual", no en el cuerpo físico.
Hereda de: ComponenteBicicleta
*/
using UnityEngine;

public class Inclinacion : ComponenteBicicleta
{
    [SerializeField] private float anguloMaximo = 25f;
    [SerializeField] private float suavizado = 10f;

    public override void Actualizar(EntradaJugador entrada)
    {
        float objetivo = 0f;
        if (!bici.EnSuelo)
            objetivo = Mathf.Clamp(rb.linearVelocity.y * 2.5f, -anguloMaximo, anguloMaximo);

        float actual = Mathf.DeltaAngle(0f, transform.localEulerAngles.z);
        float nuevo = Mathf.Lerp(actual, objetivo, suavizado * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(0f, 0f, nuevo);
    }
}
