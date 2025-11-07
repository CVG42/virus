using UnityEngine;
using UnityEngine.Events;

namespace Virus
{
    public class LaserWall : MonoBehaviour
    {
        [Header("Laser Settings")]
        [SerializeField] private int beamCount = 4;
        [SerializeField] private float spacing = 0.3f;
        [SerializeField] private float laserDistance = 8f;
        [SerializeField] private LayerMask ignoreMask;
        [SerializeField] private Material laserMaterial;
        [SerializeField] private float laserWidth = 0.1f;

        private LineRenderer[] lasers;
        private RaycastHit rayHit;
        private Ray ray;

        private void Awake()
        {
            lasers = new LineRenderer[beamCount];

            for (int i = 0; i < beamCount; i++)
            {
                GameObject laserObj = new GameObject("Laser_" + i);
                laserObj.transform.parent = transform;
                laserObj.transform.localPosition = new Vector3(0, i * spacing, 0);

                LineRenderer lr = laserObj.AddComponent<LineRenderer>();
                lr.material = laserMaterial;
                lr.positionCount = 2;
                lr.startWidth = laserWidth;
                lr.endWidth = laserWidth;
                lr.useWorldSpace = true;

                lasers[i] = lr;
            }
        }

        private void Update()
        {
            for (int i = 0; i < beamCount; i++)
            {
                Vector3 origin = lasers[i].transform.position;
                ray = new Ray(origin, transform.forward);

                if (Physics.Raycast(ray, out rayHit, laserDistance, ~ignoreMask))
                {
                    lasers[i].SetPosition(0, origin);
                    lasers[i].SetPosition(1, rayHit.point);
                }
                else
                {
                    lasers[i].SetPosition(0, origin);
                    lasers[i].SetPosition(1, origin + transform.forward * laserDistance);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (lasers == null) return;

            Gizmos.color = Color.red;
            for (int i = 0; i < beamCount; i++)
            {
                Vector3 origin = transform.position + new Vector3(0, i * spacing, 0);
                Gizmos.DrawRay(origin, transform.forward * laserDistance);
            }
        }
    }
}
