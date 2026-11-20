/*
Juego: BMX del Desierto
Funcionalidad: Bandera de llegada. Cuando la bicicleta la cruza, se gana el nivel.
Collider: BoxCollider2D con "Is Trigger" activado.
*/
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Meta : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D otro)
    {
        BicicletaController bici = otro.GetComponent<BicicletaController>();
        if (bici != null && bici.EstaViva) GameManager.Instancia.GanarJuego();
    }
}
