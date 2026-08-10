using UnityEngine;

// Attach to the ROOT GameObject named "cat" (the object moved by the existing
// cat special-move component), not to the sprite child also named "cat".
public class CatLaneGuard : MonoBehaviour
{
    [SerializeField] private float laneY = -1.75f;
    [SerializeField] private float scaleMultiplier = 1.25f;
    [SerializeField] private bool enforceLane = true;

    private Vector3 startingScale;

    private void Awake()
    {
        startingScale = transform.localScale;
        transform.localScale = new Vector3(
            startingScale.x * scaleMultiplier,
            startingScale.y * scaleMultiplier,
            startingScale.z
        );

        SnapToLane();
    }

    private void LateUpdate()
    {
        // The existing cat move is still free to animate X. We only own Y/Z so
        // the cat cannot drift onto the magician/hat when mechanics overlap.
        if (enforceLane)
            SnapToLane();
    }

    private void SnapToLane()
    {
        Vector3 p = transform.position;
        p.y = laneY;
        p.z = 0f;
        transform.position = p;
    }
}
