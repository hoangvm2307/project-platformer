using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private Transform _body;
    [SerializeField] private Transform _head;
    [SerializeField] private SpriteRenderer _tailRenderer;

    /// <summary>
    /// Updates the arrow's length. Assumes the arrow prefab is oriented pointing UP (along the positive Y-axis).
    /// The body sprite should be 1 unit long by default to scale correctly.
    /// </summary>
    /// <param name="distance">The desired length of the arrow.</param>
    /// <param name="maxLength">The maximum allowed length for the arrow.</param>
    /// <param name="tailOpacity">The desired opacity for the tail renderer.</param>
    public void UpdateArrow(float distance, float maxLength, float tailOpacity)
    {
        float clampedDistance = Mathf.Min(distance, maxLength);

        if (_body != null)
        {
            // Scale the body along its local Y-axis (upwards)
            _body.transform.localScale = new Vector3(_body.transform.localScale.x, clampedDistance, 1);
        }

        if (_head != null)
        {
            // Reset local rotation to ensure it aligns with the arrow's body
            _head.transform.localRotation = Quaternion.identity;
            
            // Move the head to the tip of the scaled body
            _head.transform.localPosition = new Vector3(0, clampedDistance, 0);
        }

        if (_tailRenderer != null)
        {
            var tailTransform = _tailRenderer.transform;
            // Rotate the tail to point backwards
            tailTransform.localRotation = Quaternion.Euler(0, 0, 180);
            // Scale the tail to match the body's length
            tailTransform.localScale = new Vector3(tailTransform.localScale.x, clampedDistance, 1);
            
            // Set tail opacity
            Color tailColor = _tailRenderer.color;
            _tailRenderer.color = new Color(tailColor.r, tailColor.g, tailColor.b, tailOpacity);
        }
    }
} 