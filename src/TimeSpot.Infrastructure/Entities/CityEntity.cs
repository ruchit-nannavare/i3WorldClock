using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeSpot.Infrastructure.Entities;

[Table("cities")]
public class CityEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("country")]
    public string Country { get; set; } = string.Empty;

    [Column("utc_offset")]
    public double UtcOffset { get; set; }
}
