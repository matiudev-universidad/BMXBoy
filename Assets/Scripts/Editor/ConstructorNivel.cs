/*
Juego: BMX del Desierto
Funcionalidad: HERRAMIENTA DE EDITOR (no va dentro del juego).
               Menú "BMX Desierto > Construir nivel": configura los sprites y arma la escena
               Assets/Scenes/Nivel1.unity con todos los GameObjects ordenados en carpetas.
               Después puedes mover, agregar o borrar objetos a mano en el editor.
*/
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ConstructorNivel
{
    private const string RutaSprites = "Assets/Sprites/";
    private const string RutaAudio = "Assets/Audio/";
    private const string RutaEscenaBase = "Assets/Scenes/SampleScene.unity";
    private const string RutaEscena = "Assets/Scenes/Nivel1.unity";
    private const string RutaMaterial = "Assets/Fisica/BiciSinFriccion.physicsMaterial2D";

    // Orden de dibujo (más alto = más adelante)
    private const int OrdenValla = -1, OrdenSuelo = 0, OrdenObstaculo = 1, OrdenMeta = 2,
                      OrdenEstrella = 3, OrdenEnemigo = 4, OrdenRueda = 5, OrdenBici = 6;

    [MenuItem("BMX Desierto/Construir nivel")]
    public static void Construir()
    {
        if (!Application.isBatchMode && !EditorUtility.DisplayDialog("Construir nivel",
                "Esto crea (o reemplaza) la escena Assets/Scenes/Nivel1.unity. ¿Continuar?", "Sí", "Cancelar"))
            return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        ConfigurarSprites();
        PhysicsMaterial2D sinFriccion = CrearMaterialSinFriccion();

        // Parte de la escena del template (trae Main Camera y Global Light 2D)
        AssetDatabase.DeleteAsset(RutaEscena);
        AssetDatabase.CopyAsset(RutaEscenaBase, RutaEscena);
        EditorSceneManager.OpenScene(RutaEscena, OpenSceneMode.Single);

        // ---------- SISTEMA ----------
        Transform sistema = Carpeta("--- SISTEMA ---");
        GameObject gm = Crear("GameManager", sistema, Vector2.zero);
        gm.AddComponent<GameManager>();
        gm.AddComponent<InterfazJuego>();
        CrearAudioManager(sistema);

        // ---------- JUGADOR ----------
        GameObject bici = CrearBicicleta(new Vector2(2f, 0.72f), sinFriccion);

        // ---------- CÁMARA Y FONDO DINÁMICO ----------
        Camera cam = Camera.main;
        cam.transform.position = new Vector3(5f, 2.5f, -10f);
        cam.orthographicSize = 5f;
        cam.backgroundColor = new Color(0.55f, 0.75f, 0.95f);
        CamaraSeguimiento seguir = cam.gameObject.AddComponent<CamaraSeguimiento>();
        Asignar(seguir, "objetivo", bici.transform);
        CrearFondo(cam);

        // ---------- ESCENARIO ----------
        Transform escenario = Carpeta("--- ESCENARIO ---");
        Transform suelos = Carpeta("Suelo", escenario);
        // Los peligros están separados ~9 unidades: un salto corriendo recorre unas 8.
        Suelo(suelos, -8f, 44f);
        Suelo(suelos, 47f, 63f);     // barranco 1 (se salta)
        Suelo(suelos, 68f, 172f);    // barranco 2 (se cruza con la plataforma móvil)
        Pared(escenario, -0.5f);
        Pared(escenario, 168f);

        Transform obstaculos = Carpeta("Obstaculos", escenario);
        Caja(obstaculos, 7f, 0);
        Piedra(obstaculos, 10.5f);
        Cactus(obstaculos, 19f);
        Caja(obstaculos, 36f, 0); Caja(obstaculos, 37f, 0); Caja(obstaculos, 37f, 1);   // escalera
        Cactus(obstaculos, 75f);
        Cactus(obstaculos, 77.5f);
        Piedra(obstaculos, 94f);
        Caja(obstaculos, 103f, 0); Caja(obstaculos, 103f, 1);
        Cactus(obstaculos, 128f);
        Caja(obstaculos, 145f, 0);
        Piedra(obstaculos, 148.5f);

        Transform plataformas = Carpeta("Plataformas", escenario);
        Plataforma(plataformas, new Vector2(64f, -0.25f), Vector2.right, 3f, 1f);    // cruza el barranco
        Plataforma(plataformas, new Vector2(120f, -0.25f), Vector2.up, 3.2f, 0.9f);  // ascensor a estrellas

        CrearMeta(escenario, 157f);

        // ---------- ENEMIGOS ----------
        Transform enemigos = Carpeta("--- ENEMIGOS ---");
        Pajaro(enemigos, new Vector2(55f, 1.4f), 6f, 0.6f, 2.5f);
        Pajaro(enemigos, new Vector2(112f, 2.1f), 5f, 0.8f, 3f);
        Valla(enemigos, 28f, 0f);
        Valla(enemigos, 86f, 1.5f);
        Valla(enemigos, 137f, 3f);

        // ---------- ESTRELLAS ----------
        Transform estrellas = Carpeta("--- ESTRELLAS ---");
        Vector2[] posEstrellas =
        {
            new(7f, 2.2f), new(10.5f, 2.0f), new(19f, 3.2f), new(28f, 3.0f), new(37f, 3.0f),
            new(45.5f, 2.6f), new(55f, 3.6f), new(65.5f, 1.5f), new(76f, 3.0f), new(86f, 3.0f),
            new(103f, 3.2f), new(120f, 4.8f), new(121.5f, 5.2f), new(137f, 3.0f), new(148.5f, 2.2f)
        };
        foreach (Vector2 p in posEstrellas) Estrella(estrellas, p);

        // Guardar y dejarla como escena del juego
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), RutaEscena);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(RutaEscena, true) };
        Debug.Log("BMX Desierto: nivel construido en " + RutaEscena);
    }

    // ===================================================================
    //  PIEZAS
    // ===================================================================

    private static GameObject CrearBicicleta(Vector2 pos, PhysicsMaterial2D material)
    {
        GameObject bici = Crear("Bicicleta", null, pos);
        Rigidbody2D rb = bici.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
        rb.sharedMaterial = material;

        BoxCollider2D col = bici.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1.72f, 1.07f);
        col.offset = new Vector2(0f, -0.07f);
        col.edgeRadius = 0.04f;

        bici.AddComponent<Movimiento>();
        bici.AddComponent<Salto>();
        bici.AddComponent<Voltear>();
        bici.AddComponent<BicicletaController>();

        // Hijo "Visual": el dibujo (se inclina en el aire sin afectar la física)
        GameObject visual = Crear("Visual", bici.transform, Vector2.zero);
        PonerSprite(visual, "bicicleta", OrdenBici);
        visual.AddComponent<Inclinacion>();

        GameObject trasera = Crear("RuedaTrasera", visual.transform, new Vector2(-0.59f, -0.25f));
        PonerSprite(trasera, "rueda", OrdenRueda);
        trasera.AddComponent<RuedaGiratoria>();
        GameObject delantera = Crear("RuedaDelantera", visual.transform, new Vector2(0.59f, -0.25f));
        PonerSprite(delantera, "rueda", OrdenRueda);
        delantera.AddComponent<RuedaGiratoria>();
        return bici;
    }

    private static void CrearAudioManager(Transform padre)
    {
        GameObject go = Crear("AudioManager", padre, Vector2.zero);
        AudioSource musica = go.AddComponent<AudioSource>();
        musica.volume = 0.45f;
        musica.playOnAwake = false;
        AudioSource efectos = go.AddComponent<AudioSource>();
        efectos.playOnAwake = false;
        AudioSource rodado = go.AddComponent<AudioSource>();
        rodado.playOnAwake = false;

        AudioManager am = go.AddComponent<AudioManager>();
        Asignar(am, "fuenteMusica", musica);
        Asignar(am, "fuenteEfectos", efectos);
        Asignar(am, "fuenteRodado", rodado);
        am.musica = Audio("musica");
        am.sonidoSalto = Audio("salto");
        am.sonidoAterrizaje = Audio("aterrizaje");
        am.sonidoEstrella = Audio("estrella");
        am.sonidoChoque = Audio("choque");
        am.sonidoVictoria = Audio("victoria");
        am.sonidoArrastre = Audio("arrastre");
        EditorUtility.SetDirty(am);
    }

    private static void CrearFondo(Camera cam)
    {
        Transform fondo = Carpeta("--- FONDO ---");

        // Cielo y sol van pegados a la cámara (están "infinitamente lejos")
        GameObject cielo = Crear("Cielo", cam.transform, Vector2.zero);
        cielo.transform.localPosition = new Vector3(0f, 0f, 20f);
        cielo.transform.localScale = new Vector3(200f, 2.8f, 1f);
        PonerSprite(cielo, "cielo", -100);
        GameObject sol = Crear("Sol", cam.transform, Vector2.zero);
        sol.transform.localPosition = new Vector3(5.5f, 2.6f, 19f);
        PonerSprite(sol, "sol", -95);

        Transform nubes = Carpeta("Nubes", fondo);
        Vector3[] posNubes = { new(0f, 6f, 0.5f), new(8f, 5f, 0.8f), new(15f, 6.5f, 0.4f), new(22f, 5.4f, 0.7f) };
        foreach (Vector3 n in posNubes)
        {
            GameObject nube = Crear("Nube", nubes, new Vector2(n.x, n.y));
            PonerSprite(nube, "nube", -85);
            Asignar(nube.AddComponent<Nube>(), "velocidad", n.z);
        }

        CapaParallax(fondo, "MesetasLejanas", "mesetas_lejanas", 1.0f, 0.85f, -80, 6f);
        CapaParallax(fondo, "RocasCercanas", "rocas_cercanas", 0.6f, 0.6f, -70, 4f);
    }

    private static void CapaParallax(Transform padre, string nombre, string sprite, float y, float factor, int orden, float alto)
    {
        GameObject capa = Crear(nombre, padre, new Vector2(0f, y));
        SpriteRenderer sr = PonerSprite(capa, sprite, orden);
        sr.drawMode = SpriteDrawMode.Tiled;   // repite el dibujo a lo ancho
        sr.size = new Vector2(60f, alto);
        Parallax p = capa.AddComponent<Parallax>();
        Asignar(p, "factor", factor);
        Asignar(p, "anchoImagen", 20f);
    }

    private static void Suelo(Transform padre, float x0, float x1)
    {
        float ancho = x1 - x0;
        GameObject suelo = Crear("Suelo", padre, new Vector2((x0 + x1) * 0.5f, -1.5f));
        SpriteRenderer sr = PonerSprite(suelo, "suelo", OrdenSuelo);
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(ancho, 3f);
        BoxCollider2D col = suelo.AddComponent<BoxCollider2D>();
        col.size = new Vector2(ancho, 3f);
    }

    private static void Pared(Transform padre, float x)
    {
        GameObject pared = Crear("ParedInvisible", padre, new Vector2(x, 5f));
        pared.AddComponent<BoxCollider2D>().size = new Vector2(1f, 20f);
    }

    private static void Caja(Transform padre, float x, int piso)
    {
        GameObject caja = Crear("Caja", padre, new Vector2(x, 0.5f + piso));
        PonerSprite(caja, "caja", OrdenObstaculo);
        caja.AddComponent<BoxCollider2D>();
    }

    private static void Piedra(Transform padre, float x)
    {
        GameObject piedra = Crear("Piedra", padre, new Vector2(x, 0.43f));
        PonerSprite(piedra, "piedra", OrdenObstaculo);
        BoxCollider2D col = piedra.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1.35f, 0.8f);
        col.offset = new Vector2(0f, -0.03f);
    }

    private static void Cactus(Transform padre, float x)
    {
        GameObject cactus = Crear("Cactus", padre, new Vector2(x, 0.72f));
        PonerSprite(cactus, "cactus", OrdenObstaculo);
        BoxCollider2D col = cactus.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.6f, 1.35f);
        col.offset = new Vector2(0f, -0.04f);
        cactus.AddComponent<Cactus>();
    }

    private static void Plataforma(Transform padre, Vector2 pos, Vector2 direccion, float distancia, float velocidad)
    {
        GameObject plat = Crear("PlataformaMovil", padre, pos);
        PonerSprite(plat, "plataforma", OrdenObstaculo);
        plat.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        plat.AddComponent<BoxCollider2D>();
        PlataformaMovil pm = plat.AddComponent<PlataformaMovil>();
        Asignar(pm, "direccion", direccion);
        Asignar(pm, "distancia", distancia);
        Asignar(pm, "velocidad", velocidad);
    }

    private static void CrearMeta(Transform padre, float x)
    {
        GameObject meta = Crear("Meta", padre, new Vector2(x, 1.25f));
        PonerSprite(meta, "meta", OrdenMeta);
        BoxCollider2D col = meta.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1f, 6f);
        col.offset = new Vector2(0f, 1.5f);
        meta.AddComponent<Meta>();
    }

    private static void Pajaro(Transform padre, Vector2 pos, float distancia, float onda, float velocidad)
    {
        GameObject pajaro = Crear("Pajaro", padre, pos);
        PonerSprite(pajaro, "pajaro_1", OrdenEnemigo);
        pajaro.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        BoxCollider2D col = pajaro.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.9f, 0.5f);
        Pajaro p = pajaro.AddComponent<Pajaro>();
        Asignar(p, "distancia", distancia);
        Asignar(p, "alturaOnda", onda);
        Asignar(p, "velocidad", velocidad);
        Asignar(p, "cuadrosAleteo", new Object[] { Sprite("pajaro_1"), Sprite("pajaro_2") });
    }

    private static void Valla(Transform padre, float x, float desfase)
    {
        // Empieza escondida bajo el suelo (orden de dibujo detrás del suelo)
        GameObject valla = Crear("Valla", padre, new Vector2(x, -0.85f));
        PonerSprite(valla, "valla", OrdenValla);
        valla.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        BoxCollider2D col = valla.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1.05f, 1.35f);
        col.offset = new Vector2(0f, -0.05f);
        Valla v = valla.AddComponent<Valla>();
        Asignar(v, "altura", 1.6f);
        Asignar(v, "desfase", desfase);
    }

    private static void Estrella(Transform padre, Vector2 pos)
    {
        GameObject estrella = Crear("EstrellaAzul", padre, pos);
        PonerSprite(estrella, "estrella", OrdenEstrella);
        estrella.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        estrella.AddComponent<BoxCollider2D>().isTrigger = true;
        estrella.AddComponent<Estrella>();
    }

    // ===================================================================
    //  AYUDANTES
    // ===================================================================

    private static GameObject Crear(string nombre, Transform padre, Vector2 pos)
    {
        GameObject go = new GameObject(nombre);
        if (padre != null) go.transform.SetParent(padre, false);
        go.transform.localPosition = new Vector3(pos.x, pos.y, 0f);
        return go;
    }

    private static Transform Carpeta(string nombre, Transform padre = null)
    {
        return Crear(nombre, padre, Vector2.zero).transform;
    }

    private static SpriteRenderer PonerSprite(GameObject go, string nombre, int orden)
    {
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite(nombre);
        sr.sortingOrder = orden;
        return sr;
    }

    private static Sprite Sprite(string nombre)
    {
        Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(RutaSprites + nombre + ".png");
        if (s == null) Debug.LogError("No se encontró el sprite " + nombre);
        return s;
    }

    private static AudioClip Audio(string nombre)
    {
        AudioClip c = AssetDatabase.LoadAssetAtPath<AudioClip>(RutaAudio + nombre + ".wav");
        if (c == null) Debug.LogError("No se encontró el audio " + nombre);
        return c;
    }

    // Asigna un campo privado [SerializeField] como si lo hicieras en el Inspector
    private static void Asignar(Object componente, string campo, object valor)
    {
        SerializedObject so = new SerializedObject(componente);
        SerializedProperty p = so.FindProperty(campo);
        if (p == null) { Debug.LogError($"Campo {campo} no existe en {componente.GetType().Name}"); return; }

        switch (valor)
        {
            case float f: p.floatValue = f; break;
            case int i: p.intValue = i; break;
            case bool b: p.boolValue = b; break;
            case Vector2 v: p.vector2Value = v; break;
            case Object o: p.objectReferenceValue = o; break;
            case Object[] arr:
                p.arraySize = arr.Length;
                for (int k = 0; k < arr.Length; k++) p.GetArrayElementAtIndex(k).objectReferenceValue = arr[k];
                break;
        }
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void ConfigurarSprites()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Sprites" });
        foreach (string guid in guids)
        {
            string ruta = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter imp = AssetImporter.GetAtPath(ruta) as TextureImporter;
            if (imp == null) continue;

            imp.textureType = TextureImporterType.Sprite;
            imp.spriteImportMode = SpriteImportMode.Single;
            imp.spritePixelsPerUnit = 64f;
            imp.filterMode = FilterMode.Bilinear;
            imp.mipmapEnabled = false;
            imp.alphaIsTransparency = true;
            imp.textureCompression = TextureImporterCompression.Uncompressed;
            imp.wrapMode = TextureWrapMode.Clamp;

            TextureImporterSettings ajustes = new TextureImporterSettings();
            imp.ReadTextureSettings(ajustes);
            ajustes.spriteMeshType = SpriteMeshType.FullRect;   // necesario para el modo Tiled
            imp.SetTextureSettings(ajustes);
            imp.SaveAndReimport();
        }
    }

    private static PhysicsMaterial2D CrearMaterialSinFriccion()
    {
        PhysicsMaterial2D mat = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(RutaMaterial);
        if (mat != null) return mat;
        if (!AssetDatabase.IsValidFolder("Assets/Fisica")) AssetDatabase.CreateFolder("Assets", "Fisica");
        mat = new PhysicsMaterial2D("BiciSinFriccion") { friction = 0f, bounciness = 0f };
        AssetDatabase.CreateAsset(mat, RutaMaterial);
        return mat;
    }
}
