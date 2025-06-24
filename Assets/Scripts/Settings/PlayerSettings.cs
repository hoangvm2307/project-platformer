using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Settings/Player Settings")]
public class PlayerSettings : ScriptableObject
{
    [Header("Movement")]
    public float LaunchForceMultiplier = 5f;
    public float MinImpactVelocityToSurvive = 5f;

    [Header("Visuals")]
    public Color PlayerColor = Color.white;
    public float ArrowMaxLength = 3f;
    [Range(0, 1)]
    public float ArrowTailOpacity = 0.5f;

    [Header("Juiciness")]
    public float StretchScale = 1.5f;
    public float StretchDuration = 0.1f;
    public float StretchBounceDuration = 0.4f;
    public float GroundCheckDistance = 1f;
    public Vector2 LandSquashScale = new Vector2(1.25f, 0.75f);
    public float LandSquashDuration = 0.1f;
    public float LandElasticDuration = 0.3f;
    public float IdleRotationSpeed = 5f;


    
}