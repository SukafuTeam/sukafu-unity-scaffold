using System.Collections;
using JetBrains.Annotations;
using UnityEngine.EventSystems;

public interface IExampleController : IEventSystemHandler
{
    IEnumerable ParameterMethod(int value1 = -1, int value2 = -1);
    IEnumerable NoParameterMethod();
    bool? RequestMethod();
    [CanBeNull] string RequestStringMethod();
}
