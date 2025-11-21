using System.Linq;
using Mindly.Api.Domain.Entities;
using Mindly.Api.Domain.Enums;

namespace Mindly.Api.Data;

public static class SeedData
{
    public static void EnsureSeeded(ApplicationDbContext context)
    {
        if (context.FocusSessions.Any())
        {
            return;
        }

        var sessions = new[]
        {
            new FocusSession("Planejar sprint Mindly", 50, 10, "Revisar backlog com o time remoto", enableIoT: true),
            new FocusSession("Estudo do método Pomodoro", 30, 5, "Reforçar hábito e validar integrações", enableIoT: false),
            new FocusSession("Criação de conteúdo Mindly", 45, 10, "Gravar roteiro sobre equilíbrio entre foco e descanso", enableIoT: true)
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
