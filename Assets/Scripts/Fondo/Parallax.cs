/*
Juego: BMX del Desierto
Funcionalidad: FONDO DINÁMICO con efecto parallax. Cada capa se mueve a distinta
               velocidad que la cámara: lo lejano (mesetas) se mueve lento y lo
               cercano (rocas) más rápido, dando sensación de profundidad.
               La capa se repite sola para que el fondo nunca se acabe.
factor: 0 = se mueve igual que el escenario (cerca), 1 = va pegado a la cámara (infinitamente lejos).
*/
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float factor = 0.5f;
    [SerializeField] private float anchoImagen = 20f;   // ancho de UNA copia del dibujo, en unidades

    private Transform camara;
    private float inicioX;

    private void Start()
    {
        camara = Camera.main.transform;
        inicioX = transform.position.x;
    }

    private void LateUpdate()
    {
        float recorrido = camara.position.x * factor;          // cuánto acompaña a la cámara
        float relativo = camara.position.x * (1f - factor);     // cuánto se "queda atrás"

        Vector3 pos = transform.position;
        pos.x = inicioX + recorrido;
        transform.position = pos;

        // Si la cámara ya pasó una copia completa, adelantamos la capa (loop infinito)
        if (relativo > inicioX + anchoImagen) inicioX += anchoImagen;
        else if (relativo < inicioX - anchoImagen) inicioX -= anchoImagen;
    }
}
