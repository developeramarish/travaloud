using ApexCharts;
using Travaloud.Application.Catalog.Bookings.Dto;

namespace Travaloud.Application.Dashboard;

public class StatsDto
{
    public int BookingsCount { get; set; }
    public int TourBookingsCount { get; set; }
    public decimal TourBookingsRevenue { get; set; }
    public int PropertyBookingsCount { get; set; }
    public int GuestsCount { get; set; }
    public int ToursCount { get; set; }
    public int PropertiesCount { get; set; }
    public List<BookingExportDto>? TodaysTours { get; set; }
    
    public List<ChartSeries> DataEnterBarChart { get; set; } = new();
    public Dictionary<string, double>? ProductByBrandTypePieChart { get; set; }
    
    public IEnumerable<BookingItemDetailsDto>? PaidTourBookings { get; set; }
    public IEnumerable<BookingItemDetailsDto>? AllTourBookings { get; set; }

    public List<TourBookingsBarChartSummary> TourBookingsBarChartSummaries { get; set; } = [];
}

public class ChartSeries
{
    public string? Name { get; set; }
    public double[]? Data { get; set; }
}

public class TourBookingsBarChartSummary
{
    public string? TourName { get; set; }
    public List<MonthAmount> MonthlyAmounts { get; set; } = [];

    public class MonthAmount
    {
        public DateTime MonthYear { get; set; }
        public decimal Amount { get; set; }
        public int Count { get; set; }
    }
}