using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Util //new�� �ν��Ͻ��� �ʿ䰡 ���� �۷ι� ���� Ŭ����
{
    public static T CreateJsonForCylinder<T>(CylinderStatusData cylinderStatusData)
    {
        cylinderStatusData = new CylinderStatusData(cylinderStatusData.name, cylinderStatusData.serialNumber, cylinderStatusData.manufacturer, cylinderStatusData.solenoidStatus, cylinderStatusData.usageCount, cylinderStatusData.productionDate);
        string json = JsonUtility.ToJson(cylinderStatusData);

        FileStream fs = new FileStream("Assets/Scripts/Data/CylData/" + cylinderStatusData.name + "_data" + ".json", FileMode.Create); // ������ �����
        StreamWriter sw = new StreamWriter(fs); // ���� ������ ������ ����, ���ڵ� ó��
        sw.Write(json);
        sw.Close();
        fs.Close();

        return default(T);
    }

    public static T CreateJsonForSensor<T>(SensorData sensorData)
    {
        sensorData = new SensorData(sensorData.name, sensorData.serialNumber, sensorData.manufacturer, sensorData.usageCount, sensorData.productionDate);
        string json = JsonUtility.ToJson(sensorData);

        FileStream fs = new FileStream("Assets/Scripts/Data/SenData/" + sensorData.name + "_data" + ".json", FileMode.Create); // ������ �����
        StreamWriter sw = new StreamWriter(fs); // ���� ������ ������ ����, ���ڵ� ó��
        sw.Write(json);
        sw.Close();
        fs.Close();

        return default(T);
    }

    public static T CreateJsonForSwitchSensor<T>(SwitchSensorData switchSensorData)
    {
        switchSensorData = new SwitchSensorData(switchSensorData.name, switchSensorData.serialNumber, switchSensorData.manufacturer, switchSensorData.usageCount, switchSensorData.productionDate);
        string json = JsonUtility.ToJson(switchSensorData);

        FileStream fs = new FileStream("Assets/Scripts/Data/CylData/CylSwitchData/" + switchSensorData.name + "_data" + ".json", FileMode.Create); // ������ �����
        StreamWriter sw = new StreamWriter(fs); // ���� ������ ������ ����, ���ڵ� ó��
        sw.Write(json);
        sw.Close();
        fs.Close();

        return default(T);
    }

    public static T CreateJsonForConveyor<T>(ConveyorStatusData conveyorStatusData)
    {
        conveyorStatusData = new ConveyorStatusData(conveyorStatusData.name, conveyorStatusData.serialNumber, conveyorStatusData.manufacturer, conveyorStatusData.direction, conveyorStatusData.usageCount, conveyorStatusData.productionDate);
        string json = JsonUtility.ToJson(conveyorStatusData);

        FileStream fs = new FileStream("Assets/Scripts/Data/ConData/" + conveyorStatusData.name + "_data" + ".json", FileMode.Create); // ������ �����
        StreamWriter sw = new StreamWriter(fs); // ���� ������ ������ ����, ���ڵ� ó��
        sw.Write(json);
        sw.Close();
        fs.Close();

        return default(T);
    }

/*    public static T CreateJsonForMachine<T>(MachineData machineData)
    {
        machineData = new MachineData(machineData.name,machineData.serialNumber, machineData.manufacturer, machineData.usageCount, machineData.productionDate);
        string json = JsonUtility.ToJson(machineData);

        FileStream fs = new FileStream("Assets/" + machineData.name + "_data" + ".json", FileMode.Create); // ������ �����
        StreamWriter sw = new StreamWriter(fs); // ���� ������ ������ ����, ���ڵ� ó��
        sw.Write(json);
        sw.Close();
        fs.Close();

        return default(T);
    }*/

    public static T LoadJson<T>(string path)  //T - Ư�� ������ �������� �ʰ� �� �������� ������ �����
    {
        FileStream fs = new FileStream(path, FileMode.Open); // ������ ����->��Ʈ��ȭ
        StreamReader sr = new StreamReader(fs); // ��Ʈ�� ������ �б�
        string json = sr.ReadToEnd();

        T obj = JsonUtility.FromJson<T>(json); //T������ obj�� json���� �޾� �����Ѵ�.

        sr.Close(); //��Ʈ������ �ݱ�
        fs.Close(); //��Ʈ�� �ݱ�

        return obj;
    }
}
