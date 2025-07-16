using UnityEngine;

public class YouSelector : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (MainUIManager.state == MainUIManager.UIState.DEFAULT){
            Debug.Log(gameObject.name);
            You targetYou = gameObject.GetComponent<You>();
            CameraPositionControl.instance.MoveCamToPos(new Vector3 (targetYou.transform.position.x + 3.5f, targetYou.transform.position.y, targetYou.transform.position.z), 0.5f, 15.0f);
            MainUIManager.instance.InspectYou(targetYou);
        }

        else{
            CameraPositionControl.instance.MoveCamToPos(CameraPositionControl.instance.defaultPos, 0.5f);
            MainUIManager.instance.UnInspect();
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            CameraPositionControl.instance.MoveCamToPos(CameraPositionControl.instance.defaultPos, 0.5f);
            MainUIManager.instance.UnInspect();
        }
    }

    // public float distanceFromYou = 5;
    // public void Update(){

    //     if (Input.GetKeyDown(KeyCode.Mouse0)){
    //         Vector2 mousePos = Input.mousePosition;
    //         Debug.Log(mousePos);

    //         Vector3 point;
    //         point = Camera.main.ScreenToWorldPoint(
    //             new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane)
    //         );
    //         // Debug.Log(point);

    //         //,LayerMask.GetMask("YOU")
    //         RaycastHit hit;
    //         if( Physics.Raycast(new Ray(point, Camera.main.transform.forward), out hit, Mathf.Infinity)){

    //             You targetYou = hit.collider.gameObject.GetComponent<You>();
    //             CameraPositionControl.instance.MoveCamToPos(targetYou.transform.position, 0.5f, 4);
    //             MainUIManager.instance.InspectYou(targetYou);

    //             Debug.Log(targetYou.name);
    //         }

    //         else{
    //             Debug.Log(hit.distance);
    //         }

    //     }

    //     if (Input.GetKeyDown(KeyCode.Escape)){
    //         CameraPositionControl.instance.MoveCamToPos(CameraPositionControl.instance.defaultPos, 0.5f);
    //     }
    // }
}
