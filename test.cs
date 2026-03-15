using System;
using System.Reflection;
using Bitbound.SimpleMessenger;

class Program {
    static void Main() {
        var t = typeof(RegistrationCallback<>);
        var method = t.GetMethod("Invoke");
        Console.WriteLine(method);
    }
}
