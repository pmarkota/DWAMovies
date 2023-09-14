using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.DALModels;
using Microsoft.EntityFrameworkCore;

namespace DAL.BLModels;

[Table("VideoTag")]
public partial class BLVideoTag
{
    [Key]
    public int Id { get; set; }

    public int VideoId { get; set; }

    public int TagId { get; set; }

    [ForeignKey("TagId")]
    [InverseProperty("VideoTags")]
    public virtual BLTag Tag { get; set; } = null!;

    [ForeignKey("VideoId")]
    [InverseProperty("VideoTags")]
    public virtual BLVideo Video { get; set; } = null!;
}
