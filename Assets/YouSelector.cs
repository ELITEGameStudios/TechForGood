using UnityEngine;

public class YouSelector : MonoBehaviour
{

    public float distanceFromYou = 5;
    public void FixedUpdate(){

        if (Input.GetKeyDown(KeyCode.Mouse0)){
            Vector2 mousePos = Input.mousePosition;
            Debug.Log(mousePos);

            Vector3 point;
            point = Camera.main.ScreenToWorldPoint(
                new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane)
            );
            Debug.Log(point);

            RaycastHit hit;
            if( Physics.Raycast(new Ray(point, Camera.main.transform.forward), out hit, Mathf.Infinity, LayerMask.GetMask("YOU"))){
                
                You targetYou = hit.collider.gameObject.GetComponent<You>();
                CameraPositionControl.instance.MoveCamToPos(targetYou.transform.position, 0.5f, 4);
                MainUIManager.instance.InspectYou(targetYou);

                Debug.Log(targetYou.name);
            }

        }

        if (Input.GetKeyDown(KeyCode.Escape)){
            CameraPositionControl.instance.MoveCamToPos(CameraPositionControl.instance.defaultPos, 0.5f);
        }
    }
}
