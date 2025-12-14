using Application.Services;

var service = new ApplicationService();

Console.Write("Podaj imie: ");
var name = Console.ReadLine() ?? string.Empty;

var application = service.Create(name!);

Console.WriteLine($"Utworzono wniosek: {application.Id}");
Console.WriteLine($"Status: {application.Status}");
Console.WriteLine($"Data utworzenia: {application.CreatedAt}");

Console.ReadKey();