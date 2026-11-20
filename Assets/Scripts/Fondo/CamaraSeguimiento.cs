/*
Juego: BMX del Desierto
Funcionalidad: La cámara sigue a la bicicleta en forma suave, mostrando un
               poco más de lo que viene adelante. No baja de cierta altura.
*/
using UnityEngine;

public class CamaraSeguimiento : MonoBehaviour
{
    [SerializeField] private Transform objetivo;
    [SerializeField] private float adelanto = 3f;      // muestra más camino hacia la derecha
    [SerializeField] private float alturaMinima = 2.5f;
    [SerializeField] private float suavizado = 5f;
    [SerializeField] private float limiteIzquierdo = 5f;

    private void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 destino = new Vector3(
            Mathf.Max(objetivo.position.x + adelanto, limiteIzquierdo),
            Mathf.Max(objetivo.position.y - 1.5f, alturaMinima),   // solo sube si la bici está muy alta
            transform.position.z);

        // Si la bici murió y cae, la cámara se queda quieta
        BicicletaController bici = objetivo.GetComponent<BicicletaController>();
        if (bici != null && !bici.EstaViva) return;

        transform.position = Vector3.Lerp(transform.position, destino, suavizado * Time.deltaTime);
    }
}
