#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class StreetLifeSceneSetup
{
    [MenuItem("StreetLife/Setup Playable Prototype Scene")]
    public static void BuildScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Lighting
        var lightGo = new GameObject("Directional Light");
        var light = lightGo.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.15f;
        lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // World
        var city = new GameObject("CityBuilder");
        city.AddComponent<ProceduralCityBuilder>();

        // Player
        var player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 1f, -3f);
        var cc = player.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.radius = 0.35f;
        cc.center = new Vector3(0f, 0.9f, 0f);

        var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        visual.name = "PlayerVisual";
        visual.transform.SetParent(player.transform, false);
        visual.transform.localPosition = new Vector3(0f, 0.9f, 0f);
        visual.transform.localScale = new Vector3(0.7f, 0.9f, 0.7f);
        Object.DestroyImmediate(visual.GetComponent<Collider>());

        var target = new GameObject("CameraTarget").transform;
        target.SetParent(player.transform, false);
        target.localPosition = new Vector3(0f, 1.45f, 0f);

        // Camera
        var cameraGo = new GameObject("Main Camera");
        cameraGo.tag = "MainCamera";
        var camera = cameraGo.AddComponent<Camera>();
        camera.fieldOfView = 65f;
        cameraGo.AddComponent<AudioListener>();
        cameraGo.transform.position = new Vector3(0f, 3f, -8f);

        var playerScript = player.AddComponent<ThirdPersonPlayer>();
        playerScript.cameraTransform = cameraGo.transform;
        playerScript.cameraTarget = target;

        // Event system + canvas
        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        var canvasGo = new GameObject("HUD Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // Left joystick
        var joyBg = CreateUI("Move Joystick Background", canvasGo.transform, new Vector2(0, 0),
            new Vector2(0, 0), new Vector2(0.12f, 0.18f), new Vector2(190, 190),
            new Color(0.05f, 0.05f, 0.05f, 0.42f));
        var joyHandle = CreateUI("Joystick Handle", joyBg.transform, new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(75, 75),
            new Color(0.85f, 0.9f, 1f, 0.75f));
        joyBg.GetComponent<RectTransform>().anchoredPosition = new Vector2(160f, 150f);
        var joystick = joyBg.AddComponent<VirtualJoystick>();
        SetPrivateField(joystick, "background", joyBg.GetComponent<RectTransform>());
        SetPrivateField(joystick, "handle", joyHandle.GetComponent<RectTransform>());
        playerScript.moveJoystick = joystick;

        // Right half camera look area
        var look = CreateUI("Camera Look Area", canvasGo.transform, new Vector2(0.5f, 0f),
            new Vector2(1f, 1f), new Vector2(1f, 0f), Vector2.zero,
            new Color(1f, 1f, 1f, 0.005f));
        var lookScript = look.AddComponent<TouchLookArea>();
        lookScript.player = player;
        lookScript.lookSensitivity = 0.12f;

        // Keep joystick and buttons above look panel in raycast order
        joyBg.transform.SetAsLastSibling();

        // Interact button
        var interactButton = CreateButton("Interact Button", canvasGo.transform, "ENTER / EXIT",
            new Vector2(1, 0), new Vector2(1, 0), new Vector2(0.88f, 0.15f), new Vector2(230, 80));
        interactButton.GetComponentInChildren<Text>().fontSize = 26;

        // Car
        var car = GameObject.CreatePrimitive(PrimitiveType.Cube);
        car.name = "Starter Car";
        car.transform.position = new Vector3(5f, 0.65f, -3f);
        car.transform.localScale = new Vector3(1.9f, 0.8f, 3.7f);
        var carRb = car.AddComponent<Rigidbody>();
        carRb.mass = 1200f;
        carRb.drag = 0.1f;
        carRb.angularDrag = 1f;
        var carController = car.AddComponent<SimpleCarController>();
        var enterExit = car.AddComponent<VehicleEnterExit>();
        enterExit.player = playerScript;
        enterExit.playerCamera = cameraGo.transform;
        enterExit.carController = carController;
        enterExit.readKeyboardInput = false;

        var seat = new GameObject("DriverSeat").transform;
        seat.SetParent(car.transform, false);
        seat.localPosition = new Vector3(0f, 0.5f, 0f);
        enterExit.driverSeat = seat;

        var exit = new GameObject("ExitPoint").transform;
        exit.SetParent(car.transform, false);
        exit.localPosition = new Vector3(1.6f, 0f, 0f);
        enterExit.exitPoint = exit;

        interactButton.GetComponent<Button>().onClick.AddListener(enterExit.Interact);

        // Driving touch buttons, shown around bottom-right
        CreateDriveButton(canvasGo.transform, "Accelerate", "GAS", new Vector2(1, 0), new Vector2(1, 0),
            new Vector2(0.89f, 0.31f), new Vector2(120, 100), VehicleTouchButton.ControlType.Accelerate, carController);
        CreateDriveButton(canvasGo.transform, "Reverse", "REV", new Vector2(1, 0), new Vector2(1, 0),
            new Vector2(0.76f, 0.31f), new Vector2(110, 90), VehicleTouchButton.ControlType.Reverse, carController);
        CreateDriveButton(canvasGo.transform, "Left", "◀", new Vector2(1, 0), new Vector2(1, 0),
            new Vector2(0.73f, 0.17f), new Vector2(95, 80), VehicleTouchButton.ControlType.SteerLeft, carController);
        CreateDriveButton(canvasGo.transform, "Right", "▶", new Vector2(1, 0), new Vector2(1, 0),
            new Vector2(0.84f, 0.17f), new Vector2(95, 80), VehicleTouchButton.ControlType.SteerRight, carController);
        CreateDriveButton(canvasGo.transform, "Brake", "BRAKE", new Vector2(1, 0), new Vector2(1, 0),
            new Vector2(0.96f, 0.17f), new Vector2(95, 80), VehicleTouchButton.ControlType.Brake, carController);

        // Mission marker
        var mission = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        mission.name = "Mission Marker - First Delivery";
        mission.transform.position = new Vector3(0f, 0.75f, 42f);
        mission.transform.localScale = new Vector3(2f, 1.5f, 2f);
        var missionCollider = mission.GetComponent<Collider>();
        missionCollider.isTrigger = true;
        var trigger = mission.AddComponent<MissionTrigger>();
        trigger.missionTitle = "First Delivery";
        trigger.description = "Reach the glowing marker to complete your first mission.";
        trigger.rewardCash = 250;

        // Save manager
        var saveGo = new GameObject("SaveManager");
        var save = saveGo.AddComponent<SaveManager>();
        save.player = player.transform;
        save.loadOnStart = true;

        var saveButton = CreateButton("Save Button", canvasGo.transform, "SAVE", new Vector2(1, 1),
            new Vector2(1, 1), new Vector2(0.93f, 0.93f), new Vector2(150, 65));
        saveButton.GetComponent<Button>().onClick.AddListener(save.SaveGame);

        // Minimap camera + render texture
        var rt = new RenderTexture(512, 512, 16);
        rt.name = "StreetLife_Minimap_RT";
        AssetDatabase.CreateAsset(rt, "Assets/StreetLife_Minimap_RT.renderTexture");

        var mapGo = new GameObject("MinimapCamera");
        var mapCam = mapGo.AddComponent<Camera>();
        mapCam.orthographic = true;
        mapCam.orthographicSize = 40f;
        mapCam.clearFlags = CameraClearFlags.SolidColor;
        mapCam.backgroundColor = new Color(0.12f, 0.16f, 0.18f);
        mapCam.targetTexture = rt;
        mapCam.depth = -2;
        var mapFollow = mapGo.AddComponent<MinimapFollow>();
        mapFollow.target = player.transform;
        mapFollow.height = 70f;

        var map = CreateUI("Minimap", canvasGo.transform, new Vector2(1, 1),
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(230, 230),
            Color.white);
        Object.DestroyImmediate(map.GetComponent<Image>());
        var raw = map.AddComponent<RawImage>();
        raw.texture = rt;
        var mapRect = map.GetComponent<RectTransform>();
        mapRect.anchoredPosition = new Vector2(-145, -160);

        // Put look area behind other controls
        look.transform.SetAsFirstSibling();

        EditorSceneManager.MarkSceneDirty(scene);
        string scenePath = "Assets/StreetLifePrototype.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = player;
        EditorUtility.DisplayDialog("StreetLife Prototype", 
            "Scene created at " + scenePath + ". Press Play to test. Configure Android Build Support before building an APK.", "OK");
    }

    private static GameObject CreateUI(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 pivot, Vector2 size, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        if (size != Vector2.zero) rect.sizeDelta = size;
        var image = go.GetComponent<Image>();
        image.color = color;
        return go;
    }

    private static GameObject CreateButton(string name, Transform parent, string label, Vector2 anchorMin,
        Vector2 anchorMax, Vector2 pivot, Vector2 size)
    {
        var go = CreateUI(name, parent, anchorMin, anchorMax, pivot, size, new Color(0.08f, 0.1f, 0.12f, 0.78f));
        var button = go.AddComponent<Button>();
        var textGo = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textGo.transform.SetParent(go.transform, false);
        var rect = textGo.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
        var text = textGo.GetComponent<Text>();
        text.text = label;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 20;
        text.resizeTextForBestFit = true;
        return go;
    }

    private static void CreateDriveButton(Transform parent, string name, string label, Vector2 anchorMin,
        Vector2 anchorMax, Vector2 pivot, Vector2 size, VehicleTouchButton.ControlType type, SimpleCarController car)
    {
        var go = CreateButton(name, parent, label, anchorMin, anchorMax, pivot, size);
        var control = go.AddComponent<VehicleTouchButton>();
        control.controlType = type;
        control.targetCar = car;
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic);
        if (field != null) field.SetValue(target, value);
    }
}
#endif
