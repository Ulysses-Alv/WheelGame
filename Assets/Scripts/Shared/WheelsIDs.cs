public struct WheelsIDs
{
    int F_LeftId;
    int F_RightId;
    int B_LeftId;
    int B_RightId;

    public WheelsIDs(int instanceID)
    {
        var instanceString = instanceID.ToString();

        F_LeftId = int.Parse(instanceString + 1.ToString());
        F_RightId = int.Parse(instanceString + 2.ToString());
        B_LeftId = int.Parse(instanceString + 3.ToString());
        B_RightId = int.Parse(instanceString + 4.ToString());
    }
}
