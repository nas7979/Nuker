using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel;

public static class ControllerExtension
{
    public static void push(this GameSystem gs, IController item) =>
        gs.Controllers.Enqueue(item);
    public static IController pop(this GameSystem gs) =>
        gs.Controllers.Dequeue();
    public static void ClearQueue<T>(this GameSystem gs, in Queue<T> item) =>
        item.Clear();
    public static void ClearEvent(this GameSystem gs, out Action item) =>
        item = () => { };

    public static void Raise(this GameSystem gs, Action raiseAction)
        => raiseAction?.Invoke();

    public static Queue<Type> FindAttributeClass<T>(this GameSystem gs)
    {
        Queue<Type> _attr = new Queue<Type>();

        Assembly.GetExecutingAssembly().GetTypes().ToList().ForEach(t =>
        {
            var attributes = t.GetCustomAttributes().Where(attribute => attribute is T);
            attributes.ToList().ForEach(e => { _attr.Enqueue(t); });
        });
        return _attr;
    }
}

