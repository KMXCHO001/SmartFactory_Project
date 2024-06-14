using System;

[Serializable]
public class ConveyorStatusData
{
    public enum Direction
    {
        Forward,
        Reverse
    }
    public string name;                            //�����̾� �̸�
    public string serialNumber;                //�����̾� �ø���ѹ�
    public string manufacturer;               //�����̾� ������
    public bool operationStatus;             //�۵�����
    public Direction direction;                 //����
    public float speed;                             //�ӵ�
    public bool overloadStatus;              //����� ����
    public bool malfunctionStatus;         //��������
    public bool maintenanceStatus;       //�����ʿ�����
    public int usageCount;                      //���Ƚ��
    public string productionDate;    //���������

    // ������
    public ConveyorStatusData(string name, string serialNumber, string manufacturer,Direction direction, int usageCount, string productionDate)
    {
        this.name = name;
        this.serialNumber = serialNumber;
        this.manufacturer = manufacturer;
        this.direction = direction;
        this.usageCount = usageCount;
        this.productionDate = productionDate;
    }
}
