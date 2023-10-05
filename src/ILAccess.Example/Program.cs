using System;

namespace ILAccess.Example
{
    public interface IService
    {
        int Property { get; }
        void Method();
    }

    public interface IService1 : IService
    {
        public const string Name = nameof(IService1) + "." + nameof(Method);
        int IService.Property => 1;
        void IService.Method() => Console.WriteLine(Name);
    }

    public interface IService2 : IService
    {
        public const string Name = nameof(IService2) + "." + nameof(Method);
        int IService.Property => 2;
        void IService.Method() => Console.WriteLine(Name);
    }

    public interface IService3
    {
        void Method<T>() => Console.WriteLine(typeof(T).Name);
    }

    public interface IService4<T>
    {
        void Method() => Console.WriteLine(typeof(T).Name);
    }

    public class Service : IService1, IService2, IService3, IService4<string>
    {
        public void Method() => throw new InvalidOperationException();
        public int Property => throw new InvalidOperationException();

        public void Invoke()
        {
            Console.WriteLine("Start invoking...");
            this.Base<IService1>().Method();
            this.Base<IService2>().Method();
            this.Base<IService3>().Method<int>();
            this.Base<IService4<string>>().Method();
            Console.WriteLine(this.Base<IService1>().Property);
            Console.WriteLine(this.Base<IService2>().Property);
            Console.WriteLine("End invoking.");
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var service = new Service();
            service.Invoke();
            Console.Read();
        }
    }
}
