using System;

[AttributeUsage(AttributeTargets.Method)]
public class ConsoleCommandAttribute : Attribute
{
    public string Name {get;}
    public string Description {get;}

    public ConsoleCommandAttribute(string name, string description = "")
    {
        Name = name.ToLower();
        Description = description;
    }
}