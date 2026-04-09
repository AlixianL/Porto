using System.Collections;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager: MonoBehaviour
{
    [SerializeField] private Camera _carCamera;
    public Camera _levelCamera; 

    /*
    private float testTime = 0f;
    private float startPosition = 0f;
    private float endPosition = -1f;
    */

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartCoroutine(SwitchPosition(targetPosition, 10f));
        Camera cam = GetComponent<Camera>();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeCamera()
    {
        if (_carCamera.enabled)
        {
            _levelCamera.enabled = true;
            _carCamera.enabled = false;
        }
        else
        {
            _carCamera.enabled = true;
            _levelCamera.enabled = false;
        }
    }

    /*
    public IEnumerator SwitchPosition(Transform targetNewPosition, float durationTime)
    {
        Transform initPosition = gameObject.transform;
        float timer = 0;
        while(timer <= 1)
        {
            // Timer
            timer += 1 / durationTime;
            //Position Lerp
            float Xresult = Mathf.Lerp(initPosition.position.x, targetNewPosition.position.x, timer);
            float Yresult = Mathf.Lerp(initPosition.position.y, targetNewPosition.position.y, timer);
            float Zresult = Mathf.Lerp(initPosition.position.z, targetNewPosition.position.y, timer);

            gameObject.transform.position = new Vector3 (Xresult, Yresult, Zresult);

            //Rotation lerp
            Xresult = Mathf.LerpAngle(initPosition.eulerAngles.x, targetNewPosition.eulerAngles.x, timer);
            Yresult = Mathf.LerpAngle(initPosition.eulerAngles.y, targetNewPosition.eulerAngles.y, timer);
            Zresult = Mathf.LerpAngle(initPosition.eulerAngles.z, targetNewPosition.eulerAngles.y, timer);

            gameObject.transform.eulerAngles = new Vector3(Xresult, Yresult, Zresult);
            
            yield return null;
        }
        print("end coroutine");
    }
    */
}
