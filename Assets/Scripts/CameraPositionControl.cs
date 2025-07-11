using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPositionControl : MonoBehaviour
{
    public Vector3 defaultPos;
    [SerializeField] private Vector3 targetPosition, lastPos;
    [SerializeField] private float time, currentTimer, targetZoom, lastZoom, defaultZoom;
    [SerializeField] private bool active {get{return currentTimer > 0;}}
    [SerializeField] private AnimationCurve defaultCurve, currentCurve;
    public static CameraPositionControl instance {get; private set;}

    void Awake()
    {
        if(instance == null){instance = this;}
        else if(instance != this){Destroy(this);}

        defaultPos = transform.position;
        defaultZoom = Camera.main.orthographicSize;
    }

    void Update()
    {
        if(active){
            transform.position = Vector3.Lerp(lastPos, targetPosition, currentCurve.Evaluate(1 - currentTimer/time));
            Camera.main.orthographicSize = Mathf.Lerp(lastZoom, targetZoom, currentCurve.Evaluate(1 - currentTimer/time));

            currentTimer -= Time.deltaTime;
        
            if(!active){
                transform.position = targetPosition;
                Camera.main.orthographicSize = targetZoom;
            }
        }
    }

    public void MoveCamToPos(Vector3 newPos, float time, float zoom = -1, AnimationCurve curve = null){
        Vector3 toLocal = transform.InverseTransformPoint(newPos);
        toLocal.z = 0;
        targetPosition = transform.TransformPoint(toLocal);
        if(time <= 0){
            transform.position = newPos;
            return;
        }

        this.time = time;
        currentTimer = time;

        lastPos = transform.position;
        currentCurve = curve == null ? defaultCurve : curve;

        targetZoom = zoom == -1 ? defaultZoom : zoom;
        lastZoom = Camera.main.orthographicSize;
    }
}
