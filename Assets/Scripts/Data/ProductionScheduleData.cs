using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������ ������ ���� �����̳� Ŭ����
/// </summary>
public class ProductionScheduleData
{
   public int productionTargetQuantity;
   public float productionRate;
   public DateTime materialSupplyDate;
    
    public ProductionScheduleData(int productionTargetQuantity, float productionRate, DateTime materialSupplyDate) 
    {
        productionTargetQuantity = this.productionTargetQuantity;
        productionRate = this.productionRate;
        materialSupplyDate = this.materialSupplyDate;
    }
}
