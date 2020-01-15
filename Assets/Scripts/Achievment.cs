using System;

[Serializable]
public class Achievment
{
    public string name;
    public bool unlocked;
    public string description;
    public string iconName;
    public string iconNameGreyscale;

    public Achievment(string achievmentName, bool lockedOrNot, string descriptionOfAchievment, string icoName, string gs) {
        name = achievmentName;
        unlocked = lockedOrNot;
        description = descriptionOfAchievment;
        iconName = icoName;
        iconNameGreyscale = gs;
    }

}
