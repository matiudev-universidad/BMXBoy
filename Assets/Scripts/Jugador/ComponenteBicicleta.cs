/*
Juego: BMX del Desierto
Funcionalidad: CLASE BASE ABSTRACTA de todas las piezas que controlan la bicicleta.
POO:
  - Herencia: Movimiento, Salto, Voltear, Inclinacion y RuedaGiratoria heredan de esta clase.
  - Polimorfismo: BicicletaController guarda una lista de ComponenteBicicleta y llama
    Actualizar() en cada una sin saber qué tipo concreto es; cada hija hace lo suyo.
  - "protected": los hijos pueden usar rb y bici, pero otras clases no.
*/
using UnityEngine;

public abstract class ComponenteBicicleta : MonoBehaviour
{
    protected Rigidbody2D rb;            // cuerpo físico de la bicicleta
    protected BicicletaController bici;  // el controlador que manda

    // Lo llama BicicletaController en su Awake. "virtual" = los hijos pueden ampliarlo.
    public virtual void Inicializar(BicicletaController controlador)
    {
        bici = controlador;
        rb = controlador.GetComponent<Rigidbody2D>();
    }

    // "abstract" = cada clase hija ESTÁ OBLIGADA a escribir su propia versión.
    public abstract void Actualizar(EntradaJugador entrada);
}
