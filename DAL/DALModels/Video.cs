using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DALModels;

[Table("Video")]
public partial class Video
{
    [Key]
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    [StringLength(256)]
    public string Name { get; set; } = null!;

    [StringLength(1024)]
    public string? Description { get; set; }

    public int GenreId { get; set; }

    public int TotalSeconds { get; set; }

    [StringLength(256)]
    public string? StreamingUrl { get; set; }

    public int? ImageId { get; set; }

    [ForeignKey("GenreId")]
    [InverseProperty("Videos")]
    public virtual Genre Genre { get; set; } = null!;

    [ForeignKey("ImageId")]
    [InverseProperty("Videos")]
    public virtual Image? Image { get; set; }

    [InverseProperty("Video")]
    public virtual ICollection<VideoTag> VideoTags { get; set; } = new List<VideoTag>();
}
