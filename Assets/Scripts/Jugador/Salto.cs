/*
Juego: BMX del Desierto
Funcionalidad: Hace saltar la bicicleta y detecta si está tocando el suelo.
Hereda de: ComponenteBicicleta
Detección de suelo: revisa los puntos de contacto del Rigidbody2D; si alguno
empuja hacia arriba (normal.y > 0.6) estamos parados sobre algo (suelo, caja,
piedra o plataforma). Así no necesitamos tags.
*/
using UnityEngine;

public class Salto : ComponenteBicicleta
{
    [SerializeField] private float fuerzaSalto = 13.5f;

    public bool EnSuelo { get; private set; }

    private readonly ContactPoint2D[] contactos = new ContactPoint2D[8];
    private float velocidadCaida;   // para saber si el aterrizaje fue fuerte

    public override void Actualizar(EntradaJugador entrada)
    {
        bool estabaEnSuelo = EnSuelo;
        EnSuelo = DetectarSuelo();

        // Sonido al aterrizar después de una caída
        if (EnSuelo && !estabaEnSuelo && velocidadCaida < -4f)
            AudioManager.Instancia.Reproducir(AudioManager.Instancia.sonidoAterrizaje);
        velocidadCaida = rb.linearVelocity.y;

        if (entrada.Saltar && EnSuelo)
        {
            // Anula la velocidad vertical y aplica un impulso hacia arriba
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
            EnSuelo = false;
            AudioManager.Instancia.Reproducir(AudioManager.Instancia.sonidoSalto);
        }
    }

    private bool DetectarSuelo()
    {
        int cantidad = rb.GetContacts(contactos);
        for (int i = 0; i < cantidad; i++)
        {
            if (contactos[i].normal.y > 0.6f) return true;
        }
        return false;
    }
}
