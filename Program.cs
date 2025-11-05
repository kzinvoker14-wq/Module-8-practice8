using System;

public interface IReport
{
    string Generate();
}

public class SalesReport : IReport
{
    public string Generate() => "Отчёт по продажам: товар A - 1000тг, товар B - 500тг.";
}

public class UserReport : IReport
{
    public string Generate() => "Отчёт по пользователям: Илья, Катя, Саша.";
}

public abstract class ReportDecorator : IReport
{
    protected IReport report;
    public ReportDecorator(IReport report) { this.report = report; }
    public virtual string Generate() => report.Generate();
}

public class DateFilterDecorator : ReportDecorator
{
    public DateFilterDecorator(IReport report) : base(report) { }
    public override string Generate() => report.Generate() + "\nФильтр: последние 7 дней.";
}

public class SortingDecorator : ReportDecorator
{
    public SortingDecorator(IReport report) : base(report) { }
    public override string Generate() => report.Generate() + "\nСортировка: по дате.";
}

public class WordExportDecorator : ReportDecorator
{
    public WordExportDecorator(IReport report) : base(report) { }
    public override string Generate() => report.Generate() + "\nЭкспорт: создан документ Word (.docx).";
}

public class PdfExportDecorator : ReportDecorator
{
    public PdfExportDecorator(IReport report) : base(report) { }
    public override string Generate() => report.Generate() + "\nЭкспорт: создан PDF файл.";
}

public interface IInternalDeliveryService
{
    void DeliverOrder(string orderId);
    string GetDeliveryStatus(string orderId);
}

public class InternalDeliveryService : IInternalDeliveryService
{
    public void DeliverOrder(string orderId) => Console.WriteLine($"Внутренняя доставка заказа {orderId} оформлена.");
    public string GetDeliveryStatus(string orderId) => "Статус: доставлено.";
}

public class YandexGoService
{
    public void SendOrder(string id) => Console.WriteLine($"Яндекс Go: заказ {id} принят и выехал курьер.");
    public string Track(string id) => "Яндекс Go: заказ в пути.";
}

public class KazPostService
{
    public void ShipPackage(string id) => Console.WriteLine($"Казпочта: посылка {id} принята на сортировку.");
    public string CheckStatus(string id) => "Казпочта: ожидает вручения.";
}

public class YandexGoAdapter : IInternalDeliveryService
{
    private readonly YandexGoService service = new YandexGoService();
    public void DeliverOrder(string orderId) => service.SendOrder(orderId);
    public string GetDeliveryStatus(string orderId) => service.Track(orderId);
}

public class KazPostAdapter : IInternalDeliveryService
{
    private readonly KazPostService service = new KazPostService();
    public void DeliverOrder(string orderId) => service.ShipPackage(orderId);
    public string GetDeliveryStatus(string orderId) => service.CheckStatus(orderId);
}

public static class DeliveryServiceFactory
{
    public static IInternalDeliveryService Create(string type)
    {
        return type.ToLower() switch
        {
            "internal" => new InternalDeliveryService(),
            "yandex" => new YandexGoAdapter(),
            "kazpost" => new KazPostAdapter(),
            _ => new InternalDeliveryService()
        };
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("ОТЧЁТЫ");
        IReport report = new SalesReport();
        report = new DateFilterDecorator(report);
        report = new SortingDecorator(report);
        report = new WordExportDecorator(report);
        Console.WriteLine(report.Generate());

        Console.WriteLine("\nДОСТАВКА");
        Console.WriteLine("Выберите службу доставки: internal / yandex / kazpost");
        string choice = Console.ReadLine() ?? "internal";

        var delivery = DeliveryServiceFactory.Create(choice);
        delivery.DeliverOrder("12345");
        Console.WriteLine(delivery.GetDeliveryStatus("12345"));
    }
}


