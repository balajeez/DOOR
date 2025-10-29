ClickDoorDemo (auto-generated)
==============================

This Unity project is prepared for WebGL builds on Unity Cloud Build.
It contains an Editor helper script (Assets/Editor/AutoSetupOnImport.cs) that will
create the scene 'Assets/Scenes/InteractiveDoorScene.unity' on the first import in Unity.

How to use:
1. Extract this folder and push the contents to a new GitHub repository (root should contain Assets/, Packages/, ProjectSettings/).
2. In Unity Dashboard -> Cloud Build, connect your GitHub repository using a Personal Access Token (PAT).
3. Create a new build target using Unity version 2022.3.13f1 and choose WebGL as the build platform.
4. Start Build. The Editor script should run during import and create the scene, door, animations, player, and crosshair UI.
5. After a successful build, open the WebGL link from Cloud Build to play in the browser.

Notes:
- If the Cloud Build environment blocks editor-time scripts, the scene may not be auto-generated. In that case, running Unity Editor locally once will create the scene.
- The included SimpleFirstPerson is minimal and uses legacy Input axes (Mouse X, Mouse Y, Horizontal, Vertical).
- If the DoorInteraction or SimpleFirstPerson scripts fail to attach at import because scripts haven't compiled yet, they can be attached manually in the Editor later.
