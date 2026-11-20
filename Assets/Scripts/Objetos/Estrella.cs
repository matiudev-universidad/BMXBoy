/*
Juego: BMX del Desierto
Funcionalidad: Estrella azul coleccionable. Flota arriba y abajo y gira.
               Al tocarla la bicicleta, suma un punto y desaparece.
Hereda de: ObjetoMovil
Collider: BoxCollider2D con "Is Trigger" activado (se atraviesa, no choca).
*/
using UnityEngine;

public class Estrella : ObjetoMovil
{
    [SerializeField] private float alturaFlote = 0.2f;
    [SerializeField] private float velocidadGiro = 120f;

    private void Start()
    {
        GameManager.Instancia.RegistrarEstrella();   // avisa que existe una estrella más
        tiempo = posicionInicial.x;                  // desfase: no flotan todas iguales
    }

    protected override Vector2 CalcularDesplazamiento(float t)
    {
        return new Vector2(0f, Mathf.Sin(t * 3f) * alturaFlote);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();   // hace el movimiento de la clase padre...
        rb.MoveRotation(rb.rotation + velocidadGiro * Time.fixedDeltaTime);   // ...y además gira
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.GetComponent<BicicletaController>() == null) return;

        GameManager.Instancia.SumarEstrella();
        AudioManager.Instancia.Reproducir(AudioManager.Instancia.sonidoEstrella);
        Destroy(gameObject);
    }
}
