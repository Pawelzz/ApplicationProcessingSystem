using Application.Services;
using Domain.Entities;
using System;
using System.Numerics;

var service = new ApplicationService();

bool continueInput = true;

while (continueInput)
{   
    Console.WriteLine("\n=== System Zarządzania Wnioskami ===");
    Console.WriteLine("\nWybierz opcję:");
    Console.WriteLine("1. Dodaj wniosek");
    Console.WriteLine("2. Zmien status wniosku");
    Console.WriteLine("3. Pokaż wszystkie wnioski");
    Console.WriteLine("4. Wyjście");
    Console.Write("Twój wybór: ");

    var choice = Console.ReadLine();

    Console.WriteLine("\n=====================================");

    switch (choice)
    {
        case "1":
            Console.Write("Podaj imie: ");
            var name = Console.ReadLine() ?? string.Empty;

            var application = service.Create(name);

            Console.WriteLine($"Utworzono wniosek: {application.Id}");
            Console.WriteLine($"Status: {application.Status}");
            Console.WriteLine($"Data utworzenia: {application.CreatedAt}");

            break;

        case "2":
            var apps = service.GetAll();
            if (apps.Count == 0)
            {
                Console.WriteLine("Brak wniosków do zmiany statusu.");
                break;
            }

            PrintApplications(apps);
            Console.Write("Podaj ID wniosku do zmiany statusu: ");
            var idInput = Console.ReadLine();

            if (Guid.TryParse(idInput, out Guid appId))
            {
                Console.WriteLine("Wybierz nowy status:");
                Console.WriteLine("1. New");
                Console.WriteLine("2. InProgress");
                Console.WriteLine("3. Completed");
                Console.WriteLine("4. Rejected");
                var statusChoice = Console.ReadLine();
                Domain.Enums.ApplicationStatus newStatus;
                switch (statusChoice)
                {
                    case "1":
                        newStatus = Domain.Enums.ApplicationStatus.New;
                        break;
                    case "2":
                        newStatus = Domain.Enums.ApplicationStatus.InProgress;
                        break;
                    case "3":
                        newStatus = Domain.Enums.ApplicationStatus.Completed;
                        break;
                    case "4":
                        newStatus = Domain.Enums.ApplicationStatus.Rejected;
                        break;
                    default:
                        Console.WriteLine("Nieprawidłowy wybór statusu.");
                        continue;
                }
                bool statusChanged = service.ChangeStatus(appId, newStatus);
                if (statusChanged)
                {
                    Console.WriteLine("Status wniosku został zaktualizowany.");
                }
                else
                {
                    Console.WriteLine("Nie znaleziono wniosku o podanym ID.");
                }
            }
                else
                {
                    Console.WriteLine("Nieprawidłowy format ID.");
                }
    
            break;

        case "3":
            var allApps = service.GetAll();

            if (!allApps.Any())
            {
                Console.WriteLine("Brak wniosków.");
                break;
            }
         
            PrintApplications(allApps);

            break;

        case "4":
            continueInput = false;
            break;
        default:
            Console.WriteLine("Nieprawidłowy wybór. Spróbuj ponownie.");
            break;
    }
}


static void PrintApplications(IEnumerable<ApplicationRequest> applications)
{
    Console.WriteLine("\nWszystkie wnioski:");

    foreach (var app in applications)
    {
        Console.WriteLine($"ID: {app.Id}, Imie: {app.ApplicantName}, Status: {app.Status}, Data: {app.CreatedAt}");
    }
}