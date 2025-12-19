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
List<ApplicationRequest> _lastView = new();

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
    Console.WriteLine("3. Zmien imie we wniosku");
    Console.WriteLine("4. Pokaż wszystkie wnioski");
    Console.WriteLine("5. Filtruj wnioski");
    Console.WriteLine("6. Sortuj wnioski");
    Console.WriteLine("7. Usun wniosek");
    Console.WriteLine("8. Wyjście i zapis");
    Console.WriteLine("\n=====================================");
    Console.Write("Twój wybór: ");

    var choice = Console.ReadLine();

    Console.WriteLine("\n=====================================");

    switch (choice)
    {
        case "1":
            Console.Write("Podaj imie: ");
            var name = Console.ReadLine() ?? string.Empty;

            while (string.IsNullOrWhiteSpace(name))
            {
                Console.Write("Imie nie może być puste. Podaj imie: ");
                name = Console.ReadLine() ?? string.Empty;
            }

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
                ApplicationStatus newStatus;
                switch (statusChoice)
                {
                    case "1":
                        newStatus = ApplicationStatus.New;
                        break;
                    case "2":
                        newStatus = ApplicationStatus.InProgress;
                        break;
                    case "3":
                        newStatus = ApplicationStatus.Completed;
                        break;
                    case "4":
                        newStatus = ApplicationStatus.Rejected;
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
            var applicationsToEdit = service.GetAll();
            if (applicationsToEdit.Count == 0)
            {
                Console.WriteLine("Brak wniosków do edycji.");
                break;
            }
            PrintApplications(applicationsToEdit);
            Console.Write("Podaj ID wniosku do zmiany imienia: ");
            var idEditInput = Console.ReadLine();
            if (Guid.TryParse(idEditInput, out Guid appEditId))
            {
                Console.Write("Podaj nowe imie: ");
                var newName = Console.ReadLine() ?? string.Empty;

                while (string.IsNullOrWhiteSpace(newName))
                {
                    Console.Write("Imie nie może być puste. Podaj imie: ");
                    newName = Console.ReadLine() ?? string.Empty;
                }

                var editedApp = service.EditApplicantName(appEditId, newName);
                if (editedApp != null)
                {
                    Console.WriteLine("Imie we wniosku zostało zaktualizowane.");
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


        case "4":
            var allApps = service.GetAll();

            if (!allApps.Any())
            {
                Console.WriteLine("Brak wniosków.");
                break;
            }
            
            PrintApplications(allApps);

            break;

        case "5":

            Console.WriteLine("1. Filtruj wnioski według statusu");
            Console.WriteLine("2. Filtruj wnioski według daty");
            Console.Write("Twój wybór: ");
            var filterChoice = Console.ReadLine();

            if (filterChoice == "1")
            {
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
                PrintApplications(filteredApps);
               
                break;
            }

            else if (filterChoice == "2")
            {
                Console.Write("Podaj datę początkową (yyyy-MM-dd): ");
                var startDateInput = Console.ReadLine();
                Console.Write("Podaj datę końcową (yyyy-MM-dd): ");
                var endDateInput = Console.ReadLine();
                if (DateTime.TryParse(startDateInput, out DateTime startDate) &&
                    DateTime.TryParse(endDateInput, out DateTime endDate))
                {
                    var filteredByDateApps = service.FilterByDateRange(startDate, endDate);
                    foreach (var filtered_app in filteredByDateApps)
                    {
                        Console.WriteLine(
                            $"ID: {filtered_app.Id}, " +
                            $"Imie: {filtered_app.ApplicantName}, " +
                            $"Status: {filtered_app.Status}, " +
                            $"Data: {filtered_app.CreatedAt}");
                    }
                }
                else
                {
                    Console.WriteLine("Nieprawidłowy format daty.");
                }
                break;
            }

            break;

        case "6":
            Console.WriteLine("Wybierz sposób sortowania:");
            Console.WriteLine("1. Imie rosnąco");
            Console.WriteLine("2. Imie malejąco");
            Console.WriteLine("3. Data rosnąco");
            Console.WriteLine("4. Data malejąco");
            Console.Write("Twój wybór: ");

            var sortChoice = Console.ReadLine();
            SortBy sortBy;

            switch (sortChoice)
            {
                case "1":
                    sortBy = SortBy.NameAsc;
                    break;
                case "2":
                    sortBy = SortBy.NameDesc;
                    break;
                case "3":
                    sortBy = SortBy.DateAsc;
                    break;
                case "4":
                    sortBy = SortBy.DateDesc;
                    break;
                default:
                    Console.WriteLine("Nieprawidłowy wybór sortowania.");
                    continue;
            }
            var sortedApps = service.SortApplications(_lastView, sortBy);
            PrintApplications(sortedApps);
            break;

        case "7":
            var applications = service.GetAll();
            if (!applications.Any())
            {
                Console.WriteLine("Brak wniosków do usunięcia.");
                break;
            }

            PrintApplications(applications);

            Console.Write("Podaj ID wniosku do usunięcia: ");
            var idDelete = Console.ReadLine();

            if (Guid.TryParse(idDelete, out Guid appDeleteId))
            {
                bool deleted = service.Delete(appDeleteId);
                if (deleted)
                {
                    Console.WriteLine($"Wniosek {appDeleteId} został usunięty.");
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

        case "8":
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


void PrintApplications(IEnumerable<ApplicationRequest> applications)
{
    _lastView = applications.ToList();
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
                service.AddExistingApplication(app);
            }
        }
    }
}