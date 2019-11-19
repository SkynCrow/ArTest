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
    }
    private void Start()
    {
        MainCamera = GetComponentInChildren<Camera>(true);
        PlaneManager.planesChanged += PlaneManager_planesChanged;
    }
    private void Update()
    {
        foreach (var item in PlaneBeacon)
        {
            if (item.Value == null)
            {
                Debug.Log("Spameando");
                SpawnObjetive(item.Key);
            }
        }
    }
    private void SpawnObjetive(ARPlane x)
    {
        float mitadancho = (x.size.x*0.8f) / 2 ;
        float mitadlargo = (x.size.y*0.8f) / 2;
        int factorX = UnityEngine.Random.Range(-1, 1);
        int factorY = UnityEngine.Random.Range(-1, 1);
        Vector3 posicion;
        if (x.alignment.IsHorizontal())
            posicion = new Vector3(x.center.x + (mitadancho * factorX), x.center.y, x.center.z + (mitadlargo * factorY));
        else
            posicion = new Vector3(x.center.x + (mitadancho * factorX), x.center.y + (mitadlargo * factorY), x.center.z);
        Debug.Log("Instanciando objetivo");
        var go = Instantiate<GameObject>(ObjectivePrefab, posicion, Quaternion.identity);
        if (!PlaneBeacon.ContainsKey(x))
            PlaneBeacon.Add(x, go);
        else
            PlaneBeacon[x] = go;
    }
    private void PlaneManager_planesChanged(ARPlanesChangedEventArgs obj)
    {
        obj.added.ForEach((x) =>
        {
                SpawnObjetive(x);
        });
        obj.removed.ForEach((x) =>
        {
            if (!PlaneBeacon.ContainsKey(x))
                return;
            Destroy(PlaneBeacon[x]);
            PlaneBeacon.Remove(x);
        });
        obj.updated.ForEach((x) =>
        {
            //if (PlaneBeacon.ContainsKey(x))
            //    PlaneBeacon[x].transform.position = x.center;
        });
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