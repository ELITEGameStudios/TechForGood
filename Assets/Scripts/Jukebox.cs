using UnityEngine;

public class Jukebox : MonoBehaviour
{
    public Animator lid_animator;
    public Animator record_animator;
    public Animator needle_animator;
    private bool opened = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        record_animator.speed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (!opened){
            CameraPositionControl.instance.MoveCamToPos(new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), 0.5f, 5.0f);
            lid_animator.SetTrigger("Open");
            record_animator.speed = 1;
        }
        
        else{
            CameraPositionControl.instance.MoveCamToPos(CameraPositionControl.instance.defaultPos, 0.5f);
            lid_animator.SetTrigger("Close");
            record_animator.speed = 0;
        }
    }
}
