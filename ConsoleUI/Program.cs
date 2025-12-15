using Application.Services;
using System.Numerics;

var service = new ApplicationService();

bool contuinueInput = true;

while (contuinueInput)
{

    Console.WriteLine("1. Dodaj wniosek");
    Console.WriteLine("2. Pokaż wszystkie wnioski");
    Console.WriteLine("3. Wyjście");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
        Console.Write("Podaj imie: ");
        var name = Console.ReadLine() ?? string.Empty;

        var application = service.Create(name!);

        Console.WriteLine($"Utworzono wniosek: {application.Id}");
        Console.WriteLine($"Status: {application.Status}");
        Console.WriteLine($"Data utworzenia: {application.CreatedAt}");

        break;

        case "2":
        var allApps = service.GetAll();

        Console.WriteLine("\nWszystkie wnioski:");

        foreach (var app in allApps)
        {
            Console.WriteLine($"ID: {app.Id}, Imie: {app.ApplicantName}, Status: {app.Status}, Data: {app.CreatedAt}");
        }

        break;

        case "3":
        contuinueInput = false;
        break;
        default:
        Console.WriteLine("Nieprawidłowy wybór. Spróbuj ponownie.");
        break;
    }
}