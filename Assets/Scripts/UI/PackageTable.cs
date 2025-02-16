using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Package/PackageTable",fileName="PackageTable")]
public class PackageTable : ScriptableObject
{
    public List<PackageItem> DataList = new List<PackageItem>();
}
[System.Serializable]
public class PackageItem
{
    public int id;
    public int type;
    public int star;
    public int num;
    public string name;
    public string description;
    public string skillDescription;
    public string imagePath;


}
