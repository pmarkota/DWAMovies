using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DALModels;

[Table("VideoTag")]
public partial class VideoTag
{
    [Key]
    public int Id { get; set; }

    public int VideoId { get; set; }

    public int TagId { get; set; }

    [ForeignKey("TagId")]
    [InverseProperty("VideoTags")]
    public virtual Tag Tag { get; set; } = null!;

    [ForeignKey("VideoId")]
    [InverseProperty("VideoTags")]
    public virtual Video Video { get; set; } = null!;
}
