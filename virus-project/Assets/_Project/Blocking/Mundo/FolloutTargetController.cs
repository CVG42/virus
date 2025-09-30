using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Animations.Rigging;
using System.Threading.Tasks;

public class FolloutTargetController : MonoBehaviour
{
    public Transform player;
    public Rig rig;
    //public Transform brother;
    //Transform startTransform;


    public float dist;
    public float inMin = 2f;
    public float inMax = 5f;
    public float outMin = 0f;
    public float outMax = 1f;
    public AnimationCurve remapLerp;


   /* void Start()
    {
        startTransform = new GameObject().transform;
        startTransform.position = transform.position;
        startTransform.rotation = transform.rotation;
    }*/
    void Update()
    {
        dist = Vector3.Distance(player.position, transform.position);
        float value = math.remap(inMin, inMax, outMin, outMax, dist);
        rig.weight = remapLerp.Evaluate(value);
    }

   /* public void ChangePosition()
    {
        if(transform.position == startTransform.position)
        {
            SomeAsyncMethod(startTransform, brother);
        }
        else
        {
            SomeAsyncMethod(brother, startTransform);
        }
    }

    async Task SomeAsyncMethod(Transform from, Transform to)
    {
        for (float = 0; time < 1; time += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(from.position, to.position, remapLerp.Evaluate(time));
            transform.rotation = Quaternion.Lerp(from.rotation, to.rotation, remapLerp.Evaluate(time));
            await Task.Vield();
        }
        transform.position = Vector3.Lerp(from.position, to.position, remapLerp.Evaluate(1));
        transform.rotation = Quaternion.Lerp(from.rotation, to.rotation, remapLerp.Evaluate(1));
    }*/
}
