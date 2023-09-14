using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.DALModels;
using Microsoft.EntityFrameworkCore;

namespace DAL.BLModels;

[Table("Video")]
public partial class BLVideo
{
    [Key]
    public int Id { get; set; }

    [StringLength(256)]
    public string Name { get; set; } = null!;

    [StringLength(1024)]
    public string? Description { get; set; }

    public int GenreId { get; set; }

    public int TotalSeconds { get; set; }

    [StringLength(256)]
    public string? StreamingUrl { get; set; }

    public int? ImageId { get; set; }

    
}
