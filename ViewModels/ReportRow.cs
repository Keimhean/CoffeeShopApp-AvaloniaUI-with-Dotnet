using System;

namespace CoffeeShopApp.ViewModels;

public class ReportRow
{
    public DateTime DateUtc { get; set; }
    public DateTime DateLocal { get; set; }
    public string DateLocalString { get; set; } = "";
    public int Orders { get; set; }
    public decimal Revenue { get; set; }
}
