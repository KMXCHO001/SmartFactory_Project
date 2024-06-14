using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[Serializable]
public class SensorData
{
    public string name;                 //���� �̸�
    public string serialNumber;    //���� �ø���ѹ�
    public string manufacturer;    //���� ������
    public bool operationStatus;             //�۵�����
    public bool malfunctionStatus;         //��������
    public bool maintenanceStatus;       //�����ʿ�����
    public int usageCount;                      //���Ƚ��
    public string productionDate;    //���������

    public SensorData(string name, string serialNumber, string manufacturer, int usageCount, string productionDate)
    {
        this.name = name;
        this.serialNumber = serialNumber;
        this.manufacturer = manufacturer;
        this.usageCount = usageCount;
        this.productionDate = productionDate;
    }
}
