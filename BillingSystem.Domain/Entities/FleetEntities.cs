namespace BillingSystem.Domain.Entities;
using System;
using System.Collections.Generic;

public class Vehicle : BaseEntity
{
    public string PlateNumber { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public decimal? Capacity { get; set; }
}

public class Driver : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string? Phone { get; set; }
}

public class DeliveryRoute : BaseEntity
{
    public DateTime Date { get; set; } = DateTime.Today;
    public int DriverId { get; set; }
    public Driver? Driver { get; set; }
    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public string Status { get; set; } = "PENDING";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public List<RouteStop> Stops { get; set; } = new();
}

public class RouteStop : BaseEntity
{
    public int DeliveryRouteId { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int StopOrder { get; set; }
    public string Status { get; set; } = "PENDING";
    public DateTime? EstimatedTime { get; set; }
}
