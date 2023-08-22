using System.Runtime.InteropServices;
using UnityEngine;

public class JsMappings : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void Save(string json);

    [DllImport("__Internal")]
    public static extern string Load();
}
