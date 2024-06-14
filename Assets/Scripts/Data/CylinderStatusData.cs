using System;

[Serializable]
public class CylinderStatusData
{
    public string name;                             //�Ǹ��� �̸�
    public string serialNumber;                 //�Ǹ��� �ø���ѹ�
    public string manufacturer;                //�Ǹ��� ������
    public string productionDate;     //���������

    public int usageCount;                       //���Ƚ��
    
    public int[] solenoidStatus;                 //���� �ַ����̵� ��ġ(��, ��)

    public bool operationStatus;              //�۵�����
    public bool malfunctionStatus;          //��������
    public bool maintenanceStatus;        //�����ʿ�����



    // ������
    public CylinderStatusData( string name, string serialNumber, string manufacturer, int[] solenoidStatus, int usageCount, string productionDate)
    {
        this.name = name;
        this.serialNumber = serialNumber;
        this.manufacturer = manufacturer;
        this.solenoidStatus = solenoidStatus;
        this.usageCount = usageCount;
        this.productionDate = productionDate;
    }
}
