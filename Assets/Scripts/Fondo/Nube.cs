/*
Juego: BMX del Desierto
Funcionalidad: Nube que avanza sola por el cielo (rutina propia en loop).
               Cuando queda muy atrás de la cámara, reaparece adelante.
*/
using UnityEngine;

public class Nube : MonoBehaviour
{
    [SerializeField] private float velocidad = 0.6f;
    [SerializeField] private float distanciaReaparicion = 16f;

    private Transform camara;

    private void Start()
    {
        camara = Camera.main.transform;
    }

    private void Update()
    {
        transform.Translate(Vector3.left * velocidad * Time.deltaTime, Space.World);

        float diferencia = transform.position.x - camara.position.x;
        if (diferencia < -distanciaReaparicion)
            transform.position += Vector3.right * distanciaReaparicion * 2f;
        else if (diferencia > distanciaReaparicion)
            transform.position += Vector3.left * distanciaReaparicion * 2f;
    }
}
