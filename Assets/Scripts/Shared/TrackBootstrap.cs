using UnityEngine;

public class TrackBootstrap : MonoBehaviour
{
    [SerializeField] private Transform finishLineAnchor;

    private void Awake()
    {
        // Auto-create TrackManager if not present in scene
        var trackManager = FindAnyObjectByType<TrackManager>();
        if (trackManager == null)
        {
            var go = new GameObject("TrackManager");
            trackManager = go.AddComponent<TrackManager>();
        }

        // Auto-create CheckPoint at anchor position if not assigned
        if (trackManager.LineaDeLlegada == null)
        {
            Vector3 pos = finishLineAnchor != null ? finishLineAnchor.position : new Vector3(0, 0.5f, 50);
            Quaternion rot = finishLineAnchor != null ? finishLineAnchor.rotation : Quaternion.identity;

            var cpGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cpGO.name = "FinishLine_CheckPoint";
            cpGO.transform.position = pos;
            cpGO.transform.rotation = rot;
            cpGO.transform.localScale = new Vector3(5f, 3f, 0.3f);

            // Remove mesh renderer (invisible trigger)
            var renderer = cpGO.GetComponent<MeshRenderer>();
            if (renderer != null) Destroy(renderer);

            // Ensure it's a trigger
            var collider = cpGO.GetComponent<BoxCollider>();
            if (collider != null) collider.isTrigger = true;

            var checkPoint = cpGO.AddComponent<CheckPoint>();
            trackManager.LineaDeLlegada = checkPoint;

            Debug.Log("[TrackBootstrap] CheckPoint created and assigned automatically.");
        }
    }
}
