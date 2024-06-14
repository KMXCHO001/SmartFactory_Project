using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Diagnostics;
using System.Xml.Linq;
using TMPro;
using mxComponent;
using System.IO;
using static UnityEditor.PlayerSettings;

public class Sensor : MonoBehaviour
{
    [Header("������")]
    [Tooltip("������ �� ������ ���� ���� ��θ� �����մϴ�.")]
    public SensorData sensorData;
    public string sensorDataPath;

    [Header("����")]
    public bool isObjectDetected = false;
    public ConveyorCylinder pushCylinder;
    public Image sensorImage;

    [Header("PLC")]
    public string plcAddress;
    public int plcInputValue;
    public string boxName;

    void Start()
    {
        Util.CreateJsonForSensor<DataManager>(sensorData);
        sensorImage.color = Color.white;
        sensorData = Util.LoadJson<SensorData>(sensorDataPath);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Box1" | other.gameObject.tag == "Box2")
        {
            isObjectDetected = true;
            print("[Sensor]" + other.gameObject.tag + "��(��) [" + this.name + "]�� �νĵ˴ϴ�.");
            int cnt = 1;
            do
            {
                sensorData.usageCount++;
                cnt++;
            } while (cnt == 1);
        }

        if (other.gameObject.tag == "Box1")
        {
            sensorImage.color = Color.green;
            plcInputValue = 1;
            MxComponent.instance.SetDevice(plcAddress, 1);
            sensorData.operationStatus = true;
            pushCylinder.maxRange = -0.25f;
            boxName = "Box1";
        }

        else if (other.gameObject.tag == "Box2")
        {
            sensorImage.color = Color.green;
            plcInputValue = 1;
            MxComponent.instance.SetDevice(plcAddress, 1);
            sensorData.operationStatus = true;
            pushCylinder.maxRange = -0.188f;
            boxName = "Box2";
        }
        pushCylinder.minPos = new Vector3(pushCylinder.transform.localPosition.x, pushCylinder.transform.localPosition.y, pushCylinder.minRange);
        pushCylinder.maxPos = new Vector3(pushCylinder.transform.localPosition.x, pushCylinder.transform.localPosition.y, pushCylinder.maxRange);

    }

    private void OnTriggerExit(Collider other)
    {
        sensorImage.color = Color.white;
        isObjectDetected = false;
        sensorData.operationStatus = false;
        plcInputValue = 0;
        MxComponent.instance.SetDevice(plcAddress, 0);
    }
}