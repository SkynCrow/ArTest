using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rayo : MonoBehaviour
{
    public static Rayo Instance;
    public int Hits;
    private Camera MainCamera;
    private GameObject Laser;

    private void Awake()
    {
        Instance = this;
        Laser = transform.GetChild(0).gameObject;
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(() => FindObjectOfType<Camera>() != null);
        MainCamera = FindObjectOfType<Camera>();
    }
    private void OnDisable()
    {
        Laser.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (MainCamera == null)
            return;
        if (TryGetTouch(out Vector2 point))
        {
            //Debug.Log("Rayo!!!");
            Laser.SetActive(true);

            Vector3 tNear = MainCamera.ScreenToWorldPoint(new Vector3(point.x, point.y, MainCamera.nearClipPlane));
            Vector3 tFar = MainCamera.ScreenToWorldPoint(new Vector3(point.x, point.y, MainCamera.farClipPlane));


            Ray raycast = new Ray(tNear, tFar - tNear);


            if (Physics.Raycast(raycast, out RaycastHit raycastHit))
            {
                Laser.transform.GetChild(1).position = raycastHit.point;
                Laser.transform.GetChild(0).GetComponent<LineRenderer>().SetPosition(0, MainCamera.transform.position - Vector3.up);
                Laser.transform.GetChild(0).GetComponent<LineRenderer>().SetPosition(1, raycastHit.point);
                if (raycastHit.collider.CompareTag("Object"))
                {
                    var cube = raycastHit.collider.GetComponent<Cubo>();
                    if (cube != null)
                    {
                        cube.hits += 1;
                        if (cube.hits > 20)
                        {
                            Hits += 1;
                            Destroy(cube.gameObject);
                        }
                    }
                }
            }
            else
            {
                Laser.transform.GetChild(1).position = tFar - tNear;
                Laser.transform.GetChild(0).GetComponent<LineRenderer>().SetPosition(0, MainCamera.transform.position - Vector3.up);
                Laser.transform.GetChild(0).GetComponent<LineRenderer>().SetPosition(1, tFar - tNear);
            }
        }
        else
        {
            Laser.SetActive(false);
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
