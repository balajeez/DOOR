\
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEditor.Animations;
using System.IO;

[InitializeOnLoad]
public static class AutoSetupOnImport
{
    static AutoSetupOnImport()
    {
        EditorApplication.delayCall += RunOnce;
    }

    private static void RunOnce()
    {
        string markerPath = "Assets/Editor/.setup_done";
        if (File.Exists(markerPath))
            return;

        try
        {
            CreateSceneAndAssets();
            File.WriteAllText(markerPath, "done");
            Debug.Log("[AutoSetupOnImport] Project auto-setup completed.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[AutoSetupOnImport] Failed to auto-setup the project: " + ex);
        }
    }

    private static void CreateSceneAndAssets()
    {
        // Create a new scene
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Ground
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(3,1,3);

        // Door cube and hinge
        GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door.name = "Door";
        door.transform.position = new Vector3(0, 1, 2);
        door.transform.localScale = new Vector3(1, 2, 0.1f);

        GameObject hinge = new GameObject("Door_Hinge");
        hinge.transform.position = new Vector3(-0.5f, 0, 2);
        door.transform.SetParent(hinge.transform, true);
        door.transform.localPosition = new Vector3(0.5f, 1f, 0f);

        // Add components
        var animator = hinge.AddComponent<Animator>();
        var audioSource = door.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f;
        var audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/door_click.wav");
        if (audioClip != null) audioSource.clip = audioClip;

        // Add DoorInteraction to hinge
        var doorInteractionType = System.Type.GetType("DoorInteraction, Assembly-CSharp");
        if (doorInteractionType != null)
        {
            hinge.AddComponent(doorInteractionType);
        }
        else
        {
            Debug.LogWarning("DoorInteraction type not found at auto-setup time. It will be attachable after scripts compile.");
        }

        // Animator controller and clips
        string animFolder = "Assets/Animations";
        if (!Directory.Exists(animFolder)) Directory.CreateDirectory(animFolder);

        var controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Animations/DoorController.controller");
        controller.AddParameter("isOpen", AnimatorControllerParameterType.Bool);

        AnimationClip openClip = new AnimationClip();
        openClip.name = "DoorOpen";
        var curveRot = AnimationCurve.Linear(0f, 0f, 1f, 90f);
        openClip.SetCurve("", typeof(Transform), "localEulerAngles.y", curveRot);
        AssetDatabase.CreateAsset(openClip, "Assets/Animations/DoorOpen.anim");

        AnimationClip closeClip = new AnimationClip();
        closeClip.name = "DoorClose";
        var curveRot2 = AnimationCurve.Linear(0f, 90f, 1f, 0f);
        closeClip.SetCurve("", typeof(Transform), "localEulerAngles.y", curveRot2);
        AssetDatabase.CreateAsset(closeClip, "Assets/Animations/DoorClose.anim");

        var rootStateMachine = controller.layers[0].stateMachine;
        var closedState = rootStateMachine.AddState("Closed");
        closedState.motion = closeClip;
        var openState = rootStateMachine.AddState("Open");
        openState.motion = openClip;
        rootStateMachine.defaultState = closedState;

        var tOpen = closedState.AddTransition(openState);
        tOpen.AddCondition(AnimatorConditionMode.If, 0, "isOpen");
        tOpen.hasExitTime = false;

        var tClose = openState.AddTransition(closedState);
        tClose.AddCondition(AnimatorConditionMode.IfNot, 0, "isOpen");
        tClose.hasExitTime = false;

        animator.runtimeAnimatorController = controller;

        // First-person player and camera
        GameObject player = new GameObject("Player");
        player.transform.position = new Vector3(0, 0, -2);
        var cc = player.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.center = new Vector3(0, 0.9f, 0);

        GameObject cameraObj = new GameObject("Main Camera");
        cameraObj.tag = "MainCamera";
        var cam = cameraObj.AddComponent<Camera>();
        cam.transform.SetParent(player.transform, false);
        cam.transform.localPosition = new Vector3(0, 1.6f, 0);
        cameraObj.AddComponent<AudioListener>();

        var simpleFPType = System.Type.GetType("SimpleFirstPerson, Assembly-CSharp");
        if (simpleFPType != null)
        {
            player.AddComponent(simpleFPType);
        }
        else
        {
            player.AddComponent(typeof(SimpleFirstPerson));
            Debug.LogWarning("SimpleFirstPerson type not found by reflection; added by type instead.");
        }

        // Crosshair UI
        GameObject canvasObj = new GameObject("Canvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject cross = new GameObject("Crosshair");
        cross.transform.SetParent(canvasObj.transform, false);
        var img = cross.AddComponent<UnityEngine.UI.Image>();
        // tiny 5x5 white square, will be tinted
        img.rectTransform.sizeDelta = new Vector2(8,8);
        img.color = Color.white;

        // Save the scene
        if (!Directory.Exists("Assets/Scenes")) Directory.CreateDirectory("Assets/Scenes");
        string scenePath = "Assets/Scenes/InteractiveDoorScene.unity";
        EditorSceneManager.SaveScene(scene, scenePath);

        AssetDatabase.Refresh();
        Debug.Log("[AutoSetupOnImport] Scene and assets created: " + scenePath);
    }
}
