using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[Serializable]
public class SwitchSensorData
{
    public string name;                 //���� �̸�
    public string serialNumber;    //���� �ø���ѹ�
    public string manufacturer;    //���� ������
    public bool operationStatus;             //�۵�����
    public bool malfunctionStatus;         //��������
    public bool maintenanceStatus;       //�����ʿ�����
    public int usageCount=0;                      //���Ƚ��
    public string productionDate;    //���������


    public SwitchSensorData(string name, string serialNumber, string manufacturer, int usageCount, string productionDate)
    {
        this.name = name;
        this.serialNumber = serialNumber;
        this.manufacturer = manufacturer;
        this.usageCount = usageCount;
        this.productionDate = productionDate;
    }
}


