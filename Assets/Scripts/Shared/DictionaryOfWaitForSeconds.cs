using System.Collections;
using System.Collections.Generic;

public static class DictionaryOfWaitForSeconds
{
    private static readonly Dictionary<float, WaitForSeconds> dictOfWaitForSeconds = new();

    public static WaitForSeconds GetWaitForSeconds(float time)
    {
        if (dictOfWaitForSeconds.TryGetValue(time, out WaitForSeconds result))
        {
            return result;
        }
        else
        {
            WaitForSeconds wait = new WaitForSeconds(time);
            dictOfWaitForSeconds.Add(time, wait);
            return wait;
        }
    }
}
