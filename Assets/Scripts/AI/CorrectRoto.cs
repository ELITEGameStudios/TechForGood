using UnityEngine;

public class CorrectRoto : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.rotation = new Quaternion(gameObject.transform.rotation.x, 0, gameObject.transform.rotation.z, gameObject.transform.rotation.w);
    }
}
