/*
Juego: BMX del Desierto
Funcionalidad: CLASE BASE ABSTRACTA de los enemigos.
POO - cadena de herencia:
      MonoBehaviour -> ObjetoMovil -> Enemigo -> Pajaro
                                             -> Valla
  - De ObjetoMovil hereda la rutina de movimiento automático en loop.
  - Aquí agrega lo que TODOS los enemigos comparten: si chocan con la
    bicicleta, la partida termina.
  - Los enemigos NO persiguen al jugador: solo repiten su propio movimiento.
*/
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class Enemigo : ObjetoMovil
{
    private void OnCollisionEnter2D(Collision2D choque)
    {
        BicicletaController bici = choque.gameObject.GetComponent<BicicletaController>();
        if (bici != null) AlTocarJugador(bici);
    }

    // "virtual": un enemigo hijo podría reaccionar distinto si quisiera.
    protected virtual void AlTocarJugador(BicicletaController bici)
    {
        bici.Morir();
    }
}
