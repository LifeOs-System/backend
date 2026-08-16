using Domain.Entities.Habits;


namespace Application.Services.HabitRecords.GetLastWeek;

// ==================== RESPUESTA DEL ENDPOINT ====================
public class GetLastWeekResponse
{
    // ===== Header: "9 hábitos · 76% cumplimiento · 8 – 14 ago" =====
    public int TotalHabits { get; set; }
    public decimal OverallCompletionPercentage { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    // ===== Tarjetas resumen =====
    public HabitSummaryHighlight? StarHabit { get; set; }      // "Magnesio · 7 de 7 días"
    public HabitSummaryHighlight? AtRiskHabit { get; set; }    // "Gimnasio · 3 de 7 días"
    public WeakestDaySummary? WeakestDay { get; set; }         // "Miércoles · 50% cumplido"
    public WeekComparisonSummary WeekComparison { get; set; } = new(); // "+8% · 68% → 76%"

    // ===== Columnas: S 8 | D 9 | L 10 ... =====
    public List<WeekDayHeader> Days { get; set; } = new();

    // ===== Filas de la tabla =====
    public List<HabitWeekSummary> Habits { get; set; } = new();
}

// "Magnesio · 7 de 7 días"
public class HabitSummaryHighlight
{
    public Guid HabitId { get; set; }
    public string HabitName { get; set; } = string.Empty;
    public int CompletedDays { get; set; }
    public int TotalDays { get; set; }
}

// "Miércoles · 50% cumplido"
public class WeakestDaySummary
{
    public DateOnly Date { get; set; }
    public string DayName { get; set; } = string.Empty;
    public decimal CompletionPercentage { get; set; }
}

// "+8% · 68% → 76%"
public class WeekComparisonSummary
{
    public decimal PercentageDifference { get; set; }
    public decimal PreviousWeekPercentage { get; set; }
    public decimal CurrentWeekPercentage { get; set; }
}

// Cabecera de columna: "S 8"
public class WeekDayHeader
{
    public DateOnly Date { get; set; }
    public string DayLetter { get; set; } = string.Empty;
    public int DayNumber { get; set; }
}

// ==================== FILA: UN HÁBITO ====================
public class HabitWeekSummary
{
    public Guid HabitId { get; set; }
    public string Name { get; set; } = string.Empty;
    public HabitType Type { get; set; }
    public string? Unit { get; set; }                       // "g", "min", "lec"

    // "6 de 7 días · 86%"
    public int CompletedDays { get; set; }
    public int TotalDays { get; set; }

    // "750g / 840g · 89%" (solo CANTIDAD / TIEMPO)
    public decimal? WeekTotalValue { get; set; }
    public decimal? WeekTotalTarget { get; set; }
    public decimal? DailyTarget { get; set; }

    // Siempre: 86%, 89%, 114%...
    public decimal WeekCompletionPercentage { get; set; }

    public List<HabitDaySummary> Days { get; set; } = new();
}

// ==================== CELDA: UN DÍA ====================
public class HabitDaySummary
{
    public DateOnly Date { get; set; }
    public bool HasRecord { get; set; }         // false → pinta "—"
    public bool? IsCompleted { get; set; }      // BINARIO → ✓ / ✗
    public decimal? Value { get; set; }         // CANTIDAD/TIEMPO → número
    public bool MetDailyTarget { get; set; }    // true → efecto glow
}