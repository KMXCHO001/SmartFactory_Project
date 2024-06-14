using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mxComponent;

public class CylinderSensor : MonoBehaviour
{
    [SerializeField]
    [Header("������")]
    public SwitchSensorData switchSensorData;

    [Header("����")]
    public Cylinder cylinder = Cylinder.Conveyor_PushCylinder;
    public enum Cylinder
    {
        Conveyor_PushCylinder,
        Conveyor_GateCylinder,
        LoadingSys_X_Transfer,
        LoadingSys_LM_Transfer,
        LoadingSys_Z_Transfer,
        LoadingSys_LoadCylinder,
    }

    public Position position = Position.��������;
    public enum Position
    {
        ��������,
        �߰�����,
        ��������
    }

    public bool isObjectDetected = false;

    public string plcAddress;
    public int plcInputValue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Slider"))
        {
            isObjectDetected = true;
            plcInputValue = 1;
            MxComponent.instance.SetDevice(plcAddress, plcInputValue);
            //�α�
            if (position == Position.��������)
            {
                print(this.cylinder + " - ���� �����Դϴ�");
            }
            else if (position == Position.��������)
            {
                print(this.cylinder + " - ���� �����Դϴ�");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Slider"))
        {
            isObjectDetected = false;
            plcInputValue = 0;
            MxComponent.instance.SetDevice(plcAddress, plcInputValue);
        }
    }
}