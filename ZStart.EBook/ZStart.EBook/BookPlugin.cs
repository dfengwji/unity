using UnityEngine;
using ZStart.EBook;

public class BookPlugin
{
    public void Do(string method, object[] objs)
    {
        if (method == "simulator")
        {
            var obj = objs[0] as GameObject;
            obj.AddComponent<Simulator>();
        }
    }
}
