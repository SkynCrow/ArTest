using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Controller : MonoBehaviour
{
    public GameObject ObjectivePrefab;
    public GameObject SpawnPrefab;

    public float selectionTimeInterval = 3;
    public float MaxDistance = 1;
    public float ObjectiveLife;
    public bool selectingHit;

    public Text p, t, ol;

    private float CurrentTime;
    private int Points;
    private int Touches;

    private GameObject Spawn;
    private GameObject Objective;

    private Dictionary<ARPlane, GameObject> PlaneBeacon;
    private ARPlaneManager PlaneManager;
    private List<ARRaycastHit> Hits;
    private ARRaycastManager RaycastManager;

    private Vector2 touchPosition;
    private Vector2 ScreenCenter;

    private Camera MainCamera;

    private bool spawnSet;
    public bool SpawnSet { get => spawnSet; set => spawnSet = value; }

    private void Awake()
    {
        PlaneManager = GetComponent<ARPlaneManager>();
        PlaneBeacon = new Dictionary<ARPlane, GameObject>();
        RaycastManager = GetComponent<ARRaycastManager>();
        Hits = new List<ARRaycastHit>();
        ScreenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        MainCamera = GetComponentInChildren<Camera>();
    }
    void Update()
    {
        if (SpawnSet)
        {
            if (Objective == null)
            {
                InvokeObjetive();
            }

            if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
            {
                Debug.Log("Rayo!!!");
                Vector3 tNear = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, MainCamera.nearClipPlane));
                Vector3 tFar = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, MainCamera.farClipPlane));
                Ray raycast = new Ray(tNear, tFar - tNear);
                
                Touches += 1;
                t.text = Touches.ToString();
                if (Physics.Raycast(raycast, out RaycastHit raycastHit))
                {
                    Debug.Log("Toque algo");
                    if (raycastHit.collider.CompareTag("Player"))
                    {
                        Debug.Log("AAAAYY!!!");
                        ObjectiveLife--;
                        ol.text = ObjectiveLife.ToString();
                        if (ObjectiveLife <= 0)
                        {
                            Destroy(Objective);
                            Points += 1;
                            p.text = Points.ToString();
                        }
                    }
                }
            }
        }
        else
        {
            if (Objective != null)
            {
                Destroy(Objective);
            }
            SettingSpawn();
        }
    }
    private void InvokeObjetive()
    {
        CurrentTime += Time.deltaTime;
        if (CurrentTime > selectionTimeInterval)
        {
            ObjectiveLife = 5;
            Objective = Instantiate(ObjectivePrefab, Spawn.transform,false);
            CurrentTime = 0;
        }
    }
    public void SettingSpawn()
    {
        if (RaycastManager.Raycast(ScreenCenter, Hits, TrackableType.PlaneWithinPolygon))
        {
            var currentPose = Hits[0].pose;

            if (Spawn == null)
            {
                Spawn = Instantiate(SpawnPrefab, currentPose.position, currentPose.rotation);
            }
            else
            {
                Spawn.transform.SetPositionAndRotation(currentPose.position, currentPose.rotation);
            }
            if (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Began))
            {
                Debug.Log("Spawn set in: " + currentPose.position.ToString());
                SpawnSet = true;
            }
        }
    }
}