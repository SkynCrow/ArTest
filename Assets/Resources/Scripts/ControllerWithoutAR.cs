using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerWithoutAR : MonoBehaviour
{
    public bool SpawnSet { get; private set; }
    public int Touches { get; private set; }
    public int ObjectiveLife { get; private set; }
    public int Points;
    public float Timer = 180;
    private Camera MainCamera;
    public GameObject laser;
    public Text t, ol, p;
    public bool waiting;

    private void Awake()
    {
        MainCamera = FindObjectOfType<Camera>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnDisable()
    {
        laser.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (TryGetTouch(out Vector2 point))
        {
            Debug.Log("Rayo!!!");
            laser.SetActive(true);

            Vector3 tNear = Camera.main.ScreenToWorldPoint(new Vector3(point.x, point.y, MainCamera.nearClipPlane));
            Vector3 tFar = Camera.main.ScreenToWorldPoint(new Vector3(point.x, point.y, MainCamera.farClipPlane));


            Ray raycast = new Ray(tNear, tFar - tNear);

            Touches += 1;

            if (Physics.Raycast(raycast, out RaycastHit raycastHit))
            {
                laser.transform.GetChild(1).position = raycastHit.point;
                laser.transform.GetChild(0).GetComponent<LineRenderer>().SetPosition(0, MainCamera.transform.position - Vector3.up);
                laser.transform.GetChild(0).GetComponent<LineRenderer>().SetPosition(1, raycastHit.point);

                Debug.Log("Toque algo");
                if (raycastHit.collider.CompareTag("Object"))
                {
                    Debug.Log("AAAAYY!!!");
                    ObjectiveLife--;
                    if (ObjectiveLife <= 0)
                    {
                        Points += 1;
                    }
                }
            }
            else
            {
                laser.transform.GetChild(1).position = tFar - tNear;
                laser.transform.GetChild(0).GetComponent<LineRenderer>().SetPosition(0, MainCamera.transform.position - Vector3.up);
                laser.transform.GetChild(0).GetComponent<LineRenderer>().SetPosition(1, tFar - tNear);
            }
        }
        else
        {
            laser.SetActive(false);
        }
        
    }
    public bool TryGetTouch(out Vector2 point)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            point = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            return true;
        }
#endif
#if UNITY_ANDROID
        if (Input.touchCount > 0)
	{
            point = new Vector2(Input.GetTouch(0).position.x,Input.GetTouch(0).position.y);
            return true;
	}
#endif
        point = default;
        return false;

    }
}

