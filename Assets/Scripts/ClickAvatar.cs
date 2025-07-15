using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickAvatar : MonoBehaviour
{
    private Camera main_camera;
    private Renderer _renderer;
    private Ray ray;
    private RaycastHit hit;

    // public GameObject cube;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        main_camera = Camera.main;
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            ray = new Ray(main_camera.ScreenToWorldPoint(Input.mousePosition), main_camera.transform.forward);
            Debug.Log("AWD");
            if (Physics.Raycast(ray, out hit, 1000f)){
                Debug.Log("WOOOO");
                if (hit.transform == transform){
                    Debug.Log("CLOKD");
                    _renderer.material.color = _renderer.material.color == Color.red ? Color.blue : Color.red;
                }
            }
        }
        // if (Input.GetMouseButtonDown(0)){
        //     Debug.Log("CLOCKED");
        // }
        
        // if (Input.GetMouseButtonUp(0)){
        //     Debug.Log("UPOOED");
        // }
    }

    // private GameObject GetClickedObject(out RaycastHit hit){
    //     GameObject target = null;
    //     Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

    //     if (Physics.Raycast(ray.origin, ray.direction = 10, out hit)){
    //         if (!IsPointerOverObject()){
    //             target = hit.collider.gameObject;
    //         }
    //     }

    //     return target;
    // }

    // private bool IsPointerOverObject(){
    //     PointerEventData ped = new PointerEventData(EventSystem.current);
    //     ped.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    //     List<RaycastResult> results = new List<RaycastResult>();
    //     EventSystem.current.RaycastAll(ped, results);
    //     return results.Count > 0;
    // }
}
