using System.ComponentModel.DataAnnotations;

namespace GraduationProjectApi.Models;

public class Trade
{
    [Required]
    public string TradeId { get; set; } = Guid.NewGuid().ToString();

    [Required, MaxLength(100)]
    public required string TradeName { get; set; }
}