using Application.Services;
using Domain.Entities;
using Domain.Enums;
using System;
using System.IO;
using System.Numerics;
using System.Text.Json;

var service = new ApplicationService();
bool continueInput = true;
string filePath = "applications.json";

try
{
    LoadApplicationsFromFile(service, filePath);
}
catch (Exception ex)
{
    Console.WriteLine($"Błąd wczytywania pliku: {ex.Message}");
}

while (continueInput)
{   
    Console.WriteLine("\n=== System Zarządzania Wnioskami ===");
    Console.WriteLine("\nWybierz opcję:");
    Console.WriteLine("1. Dodaj wniosek");
    Console.WriteLine("2. Zmien status wniosku");
    Console.WriteLine("3. Pokaż wszystkie wnioski");
    Console.WriteLine("4. Pokaz wnioski wg statusu");
    Console.WriteLine("5. Wyjście i zapis");
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
            Console.WriteLine("Wybierz status wg ktorego chcesz filtrowac:");
            Console.WriteLine("1. New");
            Console.WriteLine("2. InProgress");
            Console.WriteLine("3. Completed");
            Console.WriteLine("4. Rejected");
            Console.Write("Twój wybór: ");

            string? filter = Console.ReadLine();

            if (!Enum.TryParse<ApplicationStatus>(filter, out var status) ||
                !Enum.IsDefined(typeof(ApplicationStatus), status))
            {
                Console.WriteLine("Nieprawidłowy wybór statusu.");
                break;
            }

            var filteredApps = service.FilterByStatus(status);

            foreach (var filtered_app in filteredApps)
            {
                Console.WriteLine(
                    $"ID: {filtered_app.Id}, " +
                    $"Imie: {filtered_app.ApplicantName}, " +
                    $"Status: {filtered_app.Status}, " +
                    $"Data: {filtered_app.CreatedAt}");
            }

            break;

        case "5":
            continueInput = false;

            var applicationsToSave = service.GetAll();
            SaveApplicationsToFile(applicationsToSave, filePath);
            Console.WriteLine($"Wnioski zostały zapisane do pliku: {filePath}");

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

static void SaveApplicationsToFile(IEnumerable<ApplicationRequest> applications, string filePath)
{
    var options = new JsonSerializerOptions { WriteIndented = true };
    var json = JsonSerializer.Serialize(applications, options);
    File.WriteAllText(filePath, json);
}

static void LoadApplicationsFromFile(ApplicationService service, string filePath)
{
    if (File.Exists(filePath))
    {
        var json = File.ReadAllText(filePath);
        var applications = JsonSerializer.Deserialize<List<ApplicationRequest>>(json);
        if (applications != null)
        {
            foreach (var app in applications)
            {
                // Assuming ApplicationService has a method to add existing applications
                service.AddExistingApplication(app);
            }
        }
    }
}