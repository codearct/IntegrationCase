using Integration.Service;

namespace Integration;

public abstract class Program
{
    public static void Main(string[] args)
    {
        SingleService();
        //DistributedService();
    }

    public static void SingleService()
    {
        var service = new ItemIntegrationService();

        ThreadPool.QueueUserWorkItem(async _ => { var result1 = await service.SaveItem("a"); Console.WriteLine($"Result 1 :{result1.Message}"); });
        ThreadPool.QueueUserWorkItem(async _ => { var result2 = await service.SaveItem("b"); Console.WriteLine($"Result 2 :{result2.Message}"); });
        ThreadPool.QueueUserWorkItem(async _ => { var result3 = await service.SaveItem("c"); Console.WriteLine($"Result 3 :{result3.Message}"); });

        Thread.Sleep(500);

        ThreadPool.QueueUserWorkItem(async _ => { var result4 = await service.SaveItem("a"); Console.WriteLine($"Result 4 :{result4.Message}"); });
        ThreadPool.QueueUserWorkItem(async _ => { var result5 = await service.SaveItem("b"); Console.WriteLine($"Result 5 :{result5.Message}"); });
        ThreadPool.QueueUserWorkItem(async _ => { var result6 = await service.SaveItem("c"); Console.WriteLine($"Result 6 :{result6.Message}"); });

        Thread.Sleep(5000);

        Console.WriteLine("Everything recorded:");

        service.GetAllItems().ForEach(Console.WriteLine);

        Console.ReadLine();
    }

    //Redis must be run with "redis-server" by terminal at port 6379
    public static void DistributedService()
    {
        var service = new ItemIntegrationServiceDistributed();

        ThreadPool.QueueUserWorkItem(async _ => { var result1 = await service.SaveItem("a"); Console.WriteLine($"Result 1 :{result1.Message}"); });
        ThreadPool.QueueUserWorkItem(async _ => { var result2 = await service.SaveItem("b"); Console.WriteLine($"Result 2 :{result2.Message}"); });
        ThreadPool.QueueUserWorkItem(async _ => { var result3 = await service.SaveItem("c"); Console.WriteLine($"Result 3 :{result3.Message}"); });

        Thread.Sleep(500);

        ThreadPool.QueueUserWorkItem(async _ => { var result4 = await service.SaveItem("a"); Console.WriteLine($"Result 4 :{result4.Message}"); });
        ThreadPool.QueueUserWorkItem(async _ => { var result5 = await service.SaveItem("b"); Console.WriteLine($"Result 5 :{result5.Message}"); });
        ThreadPool.QueueUserWorkItem(async _ => { var result6 = await service.SaveItem("c"); Console.WriteLine($"Result 6 :{result6.Message}"); });

        Thread.Sleep(5000);

        ThreadPool.QueueUserWorkItem(async _ => { var result4 = await service.SaveItem("a"); Console.WriteLine($"Result 4 :{result4.Message}"); });
        ThreadPool.QueueUserWorkItem(async _ => { var result5 = await service.SaveItem("b"); Console.WriteLine($"Result 5 :{result5.Message}"); });
        ThreadPool.QueueUserWorkItem(async _ => { var result6 = await service.SaveItem("c"); Console.WriteLine($"Result 6 :{result6.Message}"); });

        Thread.Sleep(5000);

        Console.WriteLine("Everything recorded:");

        service.GetAllItems().ForEach(Console.WriteLine);

        Console.ReadLine();
    }
}