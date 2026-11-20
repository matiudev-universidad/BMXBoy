/*
Juego: BMX del Desierto
Funcionalidad: Obstáculo con espinas. Es fijo (no se mueve), pero si la
               bicicleta lo toca, se pincha y la partida termina.
               Las cajas y piedras, en cambio, solo son obstáculos sólidos.
*/
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Cactus : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D choque)
    {
        BicicletaController bici = choque.gameObject.GetComponent<BicicletaController>();
        if (bici != null) bici.Morir();
    }
}
