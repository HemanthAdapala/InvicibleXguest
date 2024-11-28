using System;

public static class ControllerFactory
{
    public static IController CreateController(string controllerType)
    {
        return controllerType switch
        {
            "KeyObjectController" => new KeyObjectController(),
            _ => throw new ArgumentException("Invalid controller type")
        };
    }
}