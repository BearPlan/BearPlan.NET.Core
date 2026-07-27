using System;

namespace BearPlan.Core.DI;

public interface IDisposableContainer : IDisposable
{
    void AddDisposableObj(IDisposable disposableObj);
}
