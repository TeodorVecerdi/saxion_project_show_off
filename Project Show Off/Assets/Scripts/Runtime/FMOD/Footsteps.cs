/* ======================================================================================== */
/* FMOD Studio - Unity Integration Demo.                                                    */
/* Firelight Technologies Pty, Ltd. 2012-2016.                                              */
/* Liam de Koster-Kjaer                                                                     */
/*                                                                                          */
/* Use this script in conjunction with the Viking Village scene tutorial and Unity 5.4.     */
/* http://www.fmod.org/training/                                                            */
/*                                                                                          */
/* 1. Import Viking Village asset package                                                   */
/* 2. Import FMOD Studio Unity Integration package                                          */
/* 3. Replace Audio listener with FMOD Studio listener on player controller                 */
/*   (FlyingRigidBodyFPSController_HighQuality)                                             */
/* 4. Add footsteps script to the player controller                                         */
/* 5. Set footsteps script variable ‘Step Distance’ to a reasonable value (2.0f)            */
/* 6. Change terrain texture import settings so we can sample pixel values                  */
/*     - terrain_01_m                                                                       */
/*     - terrain_wetmud_01_sg                                                               */
/*         - Texture Type: Advanced                                                         */
/*         - Non Power of 2: N/A                                                            */
/*         - Mapping: None                                                                  */
/*         - Convolution Type: N/A                                                          */
/*         - Fixup Edge Seams: N/A                                                          */
/*         - Read/Write Enabled: Yes                                                        */
/*         - Import Type: Default                                                           */
/*         - Alpha from Greyscale: No                                                       */
/*         - Alpha is Transparency: No                                                      */
/*         - Bypass sRGB sampling: No                                                       */
/*         - Encode as RGBM: Off                                                            */
/*         - Sprite Mode: None                                                              */
/*         - Generate Mip Maps: No                                                          */
/*         - Wrap Mode: Repeat                                                              */
/*         - Filter Mode: Bilinear                                                          */
/*         - Aniso Level: 3                                                                 */
/* ======================================================================================== */

using UnityEngine;
using UnityEngine.Serialization;

//This script plays footstep sounds.
//It will play a footstep sound after a set amount of distance travelled.
//When playing a footstep sound, this script will cast a ray downwards.
//If that ray hits the ground terrain mesh, it will retreive material values to determine the surface at the current position.
//If that ray does not hit the ground terrain mesh, we assume it has hit a wooden prop and set the surface values for wood.
public sealed class Footsteps : MonoBehaviour {
    //FMOD Studio variables
    //The FMOD Studio Event path.
    //This script is designed for use with an event that has a game parameter for each of the surface variables, but it will still compile and run if they are not present.
    [FormerlySerializedAs("m_EventPath")] [FMODUnity.EventRef]
    public string eventPath;

    //Surface variables
    //Range: 0.0f - 1.0f
    //These values represent the amount of each type of surface found when raycasting to the ground.
    //They are exposed to the UI (public) only to make it easy to see the values as the player moves through the scene.
    private float grassAmount;
    private float gravelAmount;

    //Step variables
    //These variables are used to control when the player executes a footstep.
    //This is very basic, and simply executes a footstep based on distance travelled.
    //Ideally, in this case, footsteps would be triggered based on the headbob script. Or if there was an animated player model it could be triggered from the animation system.
    //You could also add variation based on speed travelled, and whether the player is running or walking.
    [SerializeField] private float stepDistance = 2.0f;
    [SerializeField] private float raycastDistance = 1.0f;
    private float stepRand;
    private Vector3 prevPos;
    private float distanceTravelled;

    //Debug variables
    //If Debug is true, this script will:
    // - Draw a debug line to represent the ray that was cast into the ground.
    // - Draw the triangle of the mesh that was hit by the ray that was cast into the ground.
    // - Log the surface values to the console.
    // - Log to the console when an expected game parameter is not found in the FMOD Studio event.
    [SerializeField] private bool debug;
    private Vector3 linePos;
    private Vector3 trianglePoint0;
    private Vector3 trianglePoint1;
    private Vector3 trianglePoint2;

    void Start() {
        //Initialise random, set seed
        Random.InitState(System.DateTime.Now.Second);

        //Initialise member variables
        stepRand = Random.Range(0.0f, 0.5f);
        linePos = prevPos = transform.position;
    }

    private void Update() {
        distanceTravelled += (transform.position - prevPos).magnitude;
        if (distanceTravelled >= stepDistance + stepRand) //TODO: Play footstep sound based on position from headbob script
        {
            PlayFootstepSound();
            stepRand = Random.Range(0.0f, 0.5f); //Adding subtle random variation to the distance required before a step is taken - Re-randomise after each step.
            distanceTravelled = 0.0f;
        }

        prevPos = transform.position;

        if (debug) {
            Debug.DrawLine(linePos, linePos + Vector3.down * 1000.0f);
            Debug.DrawLine(trianglePoint0, trianglePoint1);
            Debug.DrawLine(trianglePoint1, trianglePoint2);
            Debug.DrawLine(trianglePoint2, trianglePoint0);
        }
    }

    private void PlayFootstepSound() {
        grassAmount = 1.0f;
        gravelAmount = 0.0f;

        var ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        if (Physics.Raycast(ray, out var hit, raycastDistance + 0.5f, LayerMask.GetMask("Ground"))) {
            if (debug)
                linePos = transform.position;

            var meshFilter = hit.collider.gameObject.GetComponent<MeshFilter>();
            var mesh = meshFilter.mesh;

            if (debug) { //Calculate the points for the triangle in the mesh that we have hit with our raycast.
                if (mesh) {
                    trianglePoint0 = hit.collider.transform.TransformPoint(mesh.vertices[mesh.triangles[hit.triangleIndex * 3 + 0]]);
                    trianglePoint1 = hit.collider.transform.TransformPoint(mesh.vertices[mesh.triangles[hit.triangleIndex * 3 + 1]]);
                    trianglePoint2 = hit.collider.transform.TransformPoint(mesh.vertices[mesh.triangles[hit.triangleIndex * 3 + 2]]);
                }
            }

            //The mask texture determines how the material's main two textures are blended.
            //Colour values from each texture are blended based on the mask texture's alpha channel value.
            //0.0f is full dirt texture, 1.0f is full sand texture, 0.5f is half of each.
            var vertexColor = GetVertexColor(hit, mesh);

            // Grass is none
            // Gravel is alpha
            grassAmount = 1.0f - vertexColor.a;
            gravelAmount = vertexColor.a;
        } else {
            //If the ray hits somethign other than the ground, we assume it hit a wooden prop (This is specific to the Viking Village scene) - and set the parameter values for wood.
            grassAmount = 0.0f;
            gravelAmount = 0.0f;
        }

        if (debug)
            Debug.Log("Grass: " + grassAmount + " Gravel: " + gravelAmount); // + " Sand: " + Sand + " Water: " + Water);

        if (eventPath != null) {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(eventPath);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));

            e.setParameterByName("Grass", grassAmount);
            e.setParameterByName("Gravel", gravelAmount);
            // e.setParameterByName("Sand", Sand);
            // e.setParameterByName("Water", Water);

            e.start();
            e.release(); //Release each event instance immediately, there are fire and forget, one-shot instances.
        }
    }

    private Color GetVertexColor(RaycastHit hit, Mesh mesh) {
        var vertIndex1 = mesh.triangles[hit.triangleIndex * 3 + 0];
        var vertIndex2 = mesh.triangles[hit.triangleIndex * 3 + 1];
        var vertIndex3 = mesh.triangles[hit.triangleIndex * 3 + 2];
        var avgColor = (mesh.colors[vertIndex1] + mesh.colors[vertIndex2] + mesh.colors[vertIndex3]) / 3.0f;
        return avgColor;
    }
}