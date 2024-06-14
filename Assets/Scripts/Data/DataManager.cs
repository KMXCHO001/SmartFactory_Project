using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Firebase.Database;
using Firebase;
using System.Threading.Tasks;
using JetBrains.Annotations;

public class DataManager : MonoBehaviour
{
    public string dbURL = "https://smartfactory-6c1f5-default-rtdb.asia-southeast1.firebasedatabase.app/";
   [SerializeField] public BoxLoadingSysData data;
   [SerializeField] DatabaseReference dbRef;

    [Serializable]
    public class BoxLoadingSysData
    {
        public ProductionScheduleData scheduleData;
        public ProductionProcessData processData;

        public ConveyorStatusData conveyorData;

        public CylinderStatusData pushCylinderData;
        public SwitchSensorData[] pushCylinderSwitchDatas;

        public CylinderStatusData gateCylinderData;
        public SwitchSensorData[] gateCylinderSwitchDatas;

        public SensorData arriveSensorData;
        public SensorData alignSensorData;
        public SensorData closeSensorData;

        public CylinderStatusData X_TransferData;
        public SwitchSensorData[] X_TransferSwitchDatas;

        public CylinderStatusData LM_TransferData;
        public SwitchSensorData[] LM_TransferSwitchDatas;

        public CylinderStatusData Z_TransferData;
        public SwitchSensorData[] Z_TransferSwitchDatas;

        public CylinderStatusData LoadCylinderData;
        public SwitchSensorData[] LoadCylinderSwitchDatas;

        public BoxLoadingSysData()
        {
            scheduleData = new ProductionScheduleData(0, 0, DateTime.Now);
            processData = new ProductionProcessData(0, 0, 0, 0, "", false);
        }
    }

    void Start()
    {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new Uri(dbURL);
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        StartCoroutine(UpdateFirebaseDB());
        RequestData();
    }

    IEnumerator UpdateFirebaseDB() 
    {
        while (true)
        {
            yield return SendData();
        }
    }

    private int TryParseSnapshotValueInt(DataSnapshot snapshot)
    {
        int tempResult;
        int parsedValue;

        if (snapshot.Value is long longValue)
        {
            tempResult = (int)longValue;
        }
        else if (snapshot.Value is int intValue)
        {
            tempResult = intValue;
        }
        else if (snapshot.Value is string stringValue && int.TryParse(stringValue, out parsedValue))
        {
            tempResult = parsedValue;
        }
        else
        {
            throw new InvalidOperationException("Unexpected Type");
        }
        return tempResult;
    }

    IEnumerator SendData()
    {
        yield return new WaitForSeconds(1); //1�ʿ� �ѹ��� 
        //SimpleData data = new SimpleData();
       // string json = JsonUtility.ToJson(data);

        string result = InitData(data); //Ŭ���� �ʱ�ȭ

        Task setTask  = dbRef.SetRawJsonValueAsync(result); //�̴ֵ����͸� ���ε�
        // �񵿱�: ���ε�Ǵ� �ð��� �������
        yield return new WaitUntil(()=> setTask.IsCompleted);
    }

    public void RequestData()
    {
        dbRef.GetValueAsync().ContinueWith(LoadData);
        
        void LoadData(Task<DataSnapshot> task) 
        {
                if (task.IsFaulted)
                {
                    print("������ �ε� ����");
                }
                else if(task.IsCanceled)
                {
                    print("������ �ε� ���");
                }

                else if(task.IsCompleted)
                {
                    string json = "";
                    DataSnapshot snapshot = task.Result;

                    int count = 0;
                    List<EquipmentStatusData> equipmentStatuses = new List<EquipmentStatusData>();

                    foreach (var child in snapshot.Children)
                    {
                        json = child.GetRawJsonValue();

                        switch (count)
                        {
                            case 0:
                                //data.equipmentStatusDatas = JsonConvert.DeserializeObject<List<EquipmentStatusData>>(json);
                                break;
                                        
                            case 1:
                                data.processData = JsonConvert.DeserializeObject<ProductionProcessData>(json);
                                break;

                            case 2:
                                data.scheduleData = JsonConvert.DeserializeObject<ProductionScheduleData>(json);
                                break;
                         }

                        count++;
                        print(json);

                     }
                        print("������ �ε� ����");
                 }
            }
    }

    string InitData(BoxLoadingSysData data)
    {
        string json = JsonConvert.SerializeObject(data);

        return json;
    }

    public string processStatus = "";

    public string GetProcessStatus()
    {
        dbRef.Child("processData").Child("processStatus").GetValueAsync().ContinueWith(LoadData);

        void LoadData(Task<DataSnapshot> task)
        {

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;    

                foreach (var child in snapshot.Children)
                {
                    object result = child.GetValue(true); // Ȯ���ϸ� snapshot.Getvalue(true) �ؼ� �ٷ� �����͵� ��

                    processStatus = (string)result;

                }
            }
        }
        return processStatus;
    }
    public bool SetProcessStatus(string input)
    {
        Task task = dbRef.Child("processData").Child("processStatus").SetValueAsync(input);
        if(task.IsCompleted)
        {
            print("���ۿϷ�");
            return true;
        }
        else
        {
            print("���۽���");
            return false;
        }
    }
}
