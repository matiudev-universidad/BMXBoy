/*
Juego: BMX del Desierto
Funcionalidad: Dibuja la interfaz con OnGUI: contador de estrellas y tiempo,
               pantalla de inicio, pantalla de fin del juego y de victoria.
*/
using UnityEngine;

public class InterfazJuego : MonoBehaviour
{
    private const float AltoBase = 720f;   // todo se dibuja pensando en 1280x720 y se escala
    private GUIStyle estiloTitulo, estiloTexto, estiloHud;
    private Texture2D fondoOscuro;

    private void CrearEstilos()
    {
        if (estiloTitulo != null) return;

        fondoOscuro = new Texture2D(1, 1);
        fondoOscuro.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.55f));
        fondoOscuro.Apply();

        estiloTitulo = new GUIStyle(GUI.skin.label)
        { fontSize = 64, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        estiloTexto = new GUIStyle(GUI.skin.label)
        { fontSize = 28, alignment = TextAnchor.MiddleCenter, wordWrap = true };
        estiloHud = new GUIStyle(GUI.skin.label)
        { fontSize = 30, fontStyle = FontStyle.Bold };
    }

    private void OnGUI()
    {
        CrearEstilos();
        float escala = Screen.height / AltoBase;
        GUI.matrix = Matrix4x4.Scale(new Vector3(escala, escala, 1f));
        float ancho = Screen.width / escala;

        GameManager gm = GameManager.Instancia;

        // HUD: estrellas y tiempo
        if (gm.Estado == EstadoJuego.Jugando)
        {
            Sombra(new Rect(24, 16, 400, 44), $"Estrellas: {gm.Estrellas} / {gm.TotalEstrellas}", estiloHud, new Color(0.45f, 0.75f, 1f));
            Sombra(new Rect(24, 58, 400, 44), $"Tiempo: {gm.TiempoPartida:0.0} s", estiloHud, Color.white);
        }

        switch (gm.Estado)
        {
            case EstadoJuego.Inicio:
                Panel(ancho, "BMX DEL DESIERTO", new Color(1f, 0.75f, 0.3f),
                    "Llega a la bandera y junta las estrellas azules.\n" +
                    "Evita el pájaro, las vallas y los cactus.\n\n" +
                    "A / D o Flechas: moverse     Shift: correr     Espacio: saltar\n\n" +
                    "Presiona ENTER para empezar");
                break;

            case EstadoJuego.GameOver:
                Panel(ancho, "¡CHOCASTE!", new Color(1f, 0.35f, 0.3f),
                    $"Fin del juego\nEstrellas: {gm.Estrellas} / {gm.TotalEstrellas}\n\nPresiona R para reintentar");
                break;

            case EstadoJuego.Victoria:
                Panel(ancho, "¡LLEGASTE A LA META!", new Color(0.4f, 1f, 0.5f),
                    $"Estrellas: {gm.Estrellas} / {gm.TotalEstrellas}     Tiempo: {gm.TiempoPartida:0.0} s\n\nPresiona R para jugar otra vez");
                break;
        }
    }

    private void Panel(float ancho, string titulo, Color colorTitulo, string texto)
    {
        GUI.DrawTexture(new Rect(0, 0, ancho, AltoBase), fondoOscuro);
        Sombra(new Rect(0, 170, ancho, 90), titulo, estiloTitulo, colorTitulo);
        Sombra(new Rect(ancho * 0.1f, 280, ancho * 0.8f, 300), texto, estiloTexto, Color.white);
    }

    // Texto con sombra negra para que se lea sobre cualquier fondo
    private void Sombra(Rect r, string texto, GUIStyle estilo, Color color)
    {
        estilo.normal.textColor = Color.black;
        GUI.Label(new Rect(r.x + 3, r.y + 3, r.width, r.height), texto, estilo);
        estilo.normal.textColor = color;
        GUI.Label(r, texto, estilo);
    }
}
