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

//This script plays footstep sounds.
//It will play a footstep sound after a set amount of distance travelled.
//When playing a footstep sound, this script will cast a ray downwards.
//If that ray hits the ground terrain mesh, it will retreive material values to determine the surface at the current position.
//If that ray does not hit the ground terrain mesh, we assume it has hit a wooden prop and set the surface values for wood.
public class Footsteps : MonoBehaviour {
    //FMOD Studio variables
    //The FMOD Studio Event path.
    //This script is designed for use with an event that has a game parameter for each of the surface variables, but it will still compile and run if they are not present.
    [FMODUnity.EventRef]
    public string m_EventPath;

    //Surface variables
    //Range: 0.0f - 1.0f
    //These values represent the amount of each type of surface found when raycasting to the ground.
    //They are exposed to the UI (public) only to make it easy to see the values as the player moves through the scene.
    public float m_Grass;
    public float m_Gravel;
    //public float m_Dirt;
    //public float m_Sand;

    //Step variables
    //These variables are used to control when the player executes a footstep.
    //This is very basic, and simply executes a footstep based on distance travelled.
    //Ideally, in this case, footsteps would be triggered based on the headbob script. Or if there was an animated player model it could be triggered from the animation system.
    //You could also add variation based on speed travelled, and whether the player is running or walking.
    public float m_StepDistance = 2.0f;
    float m_StepRand;
    Vector3 m_PrevPos;
    float m_DistanceTravelled;

    //Debug variables
    //If m_Debug is true, this script will:
    // - Draw a debug line to represent the ray that was cast into the ground.
    // - Draw the triangle of the mesh that was hit by the ray that was cast into the ground.
    // - Log the surface values to the console.
    // - Log to the console when an expected game parameter is not found in the FMOD Studio event.
    public bool m_Debug;
    Vector3 m_LinePos;
    Vector3 m_TrianglePoint0;
    Vector3 m_TrianglePoint1;
    Vector3 m_TrianglePoint2;

    void Start() {
        //Initialise random, set seed
        Random.InitState(System.DateTime.Now.Second);

        //Initialise member variables
        m_StepRand = Random.Range(0.0f, 0.5f);
        m_LinePos = m_PrevPos = transform.position;
    }

    private void Update() {
        m_DistanceTravelled += (transform.position - m_PrevPos).magnitude;
        if (m_DistanceTravelled >= m_StepDistance + m_StepRand) //TODO: Play footstep sound based on position from headbob script
        {
            PlayFootstepSound();
            m_StepRand = Random.Range(0.0f, 0.5f); //Adding subtle random variation to the distance required before a step is taken - Re-randomise after each step.
            m_DistanceTravelled = 0.0f;
        }

        m_PrevPos = transform.position;

        if (m_Debug) {
            Debug.DrawLine(m_LinePos, m_LinePos + Vector3.down * 1000.0f);
            Debug.DrawLine(m_TrianglePoint0, m_TrianglePoint1);
            Debug.DrawLine(m_TrianglePoint1, m_TrianglePoint2);
            Debug.DrawLine(m_TrianglePoint2, m_TrianglePoint0);
        }
    }

    private void PlayFootstepSound() {
        m_Grass = 1.0f;
        m_Gravel = 0.0f;

        var ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        if (Physics.Raycast(ray, out var hit, 1000.0f, LayerMask.GetMask("Ground"))) {
            if (m_Debug)
                m_LinePos = transform.position;

            var meshFilter = hit.collider.gameObject.GetComponent<MeshFilter>();
            var mesh = meshFilter.mesh;

            if (m_Debug) { //Calculate the points for the triangle in the mesh that we have hit with our raycast.
                if (mesh) {
                    m_TrianglePoint0 = hit.collider.transform.TransformPoint(mesh.vertices[mesh.triangles[hit.triangleIndex * 3 + 0]]);
                    m_TrianglePoint1 = hit.collider.transform.TransformPoint(mesh.vertices[mesh.triangles[hit.triangleIndex * 3 + 1]]);
                    m_TrianglePoint2 = hit.collider.transform.TransformPoint(mesh.vertices[mesh.triangles[hit.triangleIndex * 3 + 2]]);
                }
            }

            //The mask texture determines how the material's main two textures are blended.
            //Colour values from each texture are blended based on the mask texture's alpha channel value.
            //0.0f is full dirt texture, 1.0f is full sand texture, 0.5f is half of each.
            var vertexColor = GetVertexColor(hit, mesh);

            // Grass is none
            // Gravel is alpha
            m_Grass = 1.0f - vertexColor.a;
            m_Gravel = vertexColor.a;
        } else {
            //If the ray hits somethign other than the ground, we assume it hit a wooden prop (This is specific to the Viking Village scene) - and set the parameter values for wood.
            m_Grass = 0.0f;
            m_Gravel = 0.0f;
        }

        if (m_Debug)
            Debug.Log("Grass: " + m_Grass + " Gravel: " + m_Gravel); // + " Sand: " + m_Sand + " Water: " + m_Water);

        if (m_EventPath != null) {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(m_EventPath);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));

            e.setParameterByName("Grass", m_Grass);
            e.setParameterByName("Gravel", m_Gravel);
            // e.setParameterByName("Sand", m_Sand);
            // e.setParameterByName("Water", m_Water);

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