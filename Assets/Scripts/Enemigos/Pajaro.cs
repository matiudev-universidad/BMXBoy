/*
Juego: BMX del Desierto
Enemigo 1: Pájaro que vuela de lado a lado, subiendo y bajando un poco,
           y aleteando (cambia entre 2 sprites).
Hereda de: Enemigo (que a su vez hereda de ObjetoMovil)
*/
using UnityEngine;

public class Pajaro : Enemigo
{
    [SerializeField] private float distancia = 6f;      // ancho del recorrido
    [SerializeField] private float alturaOnda = 0.6f;   // cuánto sube y baja
    [SerializeField] private Sprite[] cuadrosAleteo;    // sprites del aleteo
    [SerializeField] private float tiempoPorCuadro = 0.15f;

    private SpriteRenderer dibujo;
    private float xAnterior;

    protected override void Awake()
    {
        base.Awake();   // primero hace lo de ObjetoMovil
        dibujo = GetComponent<SpriteRenderer>();
    }

    protected override Vector2 CalcularDesplazamiento(float t)
    {
        // PingPong va de 0 a distancia y vuelve, para siempre (loop)
        float x = Mathf.PingPong(t, distancia) - distancia * 0.5f;
        float y = Mathf.Sin(t * 2f) * alturaOnda;
        return new Vector2(x, y);
    }

    private void Update()
    {
        // Mira hacia donde vuela
        float x = transform.position.x;
        if (x > xAnterior + 0.001f) dibujo.flipX = false;
        else if (x < xAnterior - 0.001f) dibujo.flipX = true;
        xAnterior = x;

        // Aleteo: elige el sprite según el tiempo
        if (cuadrosAleteo != null && cuadrosAleteo.Length > 0)
        {
            int cuadro = (int)(Time.time / tiempoPorCuadro) % cuadrosAleteo.Length;
            dibujo.sprite = cuadrosAleteo[cuadro];
        }
    }
}
