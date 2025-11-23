using System.Linq;
using Mindly.Api.Domain.Entities;
using Mindly.Api.Domain.Enums;

namespace Mindly.Api.Data;

public static class SeedData
{
    public static void EnsureSeeded(ApplicationDbContext context)
    {
        if (context.Users.Any())
        {
            return;
        }

        var users = new[]
        {
            new User("Ana Silva", "ana.silva@mindly.com"),
            new User("Carlos Santos", "carlos.santos@mindly.com"),
            new User("Maria Oliveira", "maria.oliveira@mindly.com")
        };

        foreach (var user in users)
        {
            context.Users.Add(user);
        }

        context.SaveChanges();

        var sessions = new[]
        {
            new FocusSession("Planejar sprint Mindly", 50, 10, users[0].Id, "Revisar backlog com o time remoto", enableIoT: true),
            new FocusSession("Estudo do método Pomodoro", 30, 5, users[1].Id, "Reforçar hábito e validar integrações", enableIoT: false),
            new FocusSession("Criação de conteúdo Mindly", 45, 10, users[2].Id, "Gravar roteiro sobre equilíbrio entre foco e descanso", enableIoT: true),
            new FocusSession("Desenvolvimento API", 60, 15, users[0].Id, "Implementar endpoints RESTful", enableIoT: true),
            new FocusSession("Revisão de código", 25, 5, users[1].Id, "Code review do PR #42", enableIoT: false)
        };

        foreach (var session in sessions)
        {
            session.Start();
            session.Pause();
            context.FocusSessions.Add(session);
        }

        context.SaveChanges();
    }
}
