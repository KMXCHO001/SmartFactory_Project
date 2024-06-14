using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������¸� ��Ÿ���� ���� �����̳� Ŭ����
/// </summary>
public class EquipmentStatusData
{
    public string name;
    public bool operationStatus;
    public bool malfunctionStatus;
    public bool maintenanceStatus;

    //�ʱ�ȭ
    public EquipmentStatusData(string name, bool operationStatus, bool malfunctionStatus, bool maintenanceStatus)
    {
        this.name = name;
        this.operationStatus = operationStatus;
        this.malfunctionStatus = malfunctionStatus;
        this.maintenanceStatus = maintenanceStatus;
    }

    public bool GetoperationStatus()
    {
        return this.operationStatus;
    }
    public bool GetmalfunctionStatus()
    {
        return this.malfunctionStatus;
    }
    public bool GetmaintenanceStatus()
    {
        return this.malfunctionStatus;
    }
}
