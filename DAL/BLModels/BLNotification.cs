using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.BLModels;

[Table("Notification")]
public partial class BLNotification
{
    //[Key]
    //public int Id { get; set; }

    //public DateTime CreatedAt { get; set; }

    [StringLength(256)]
    public string ReceiverEmail { get; set; } = null!;

    [StringLength(256)]
    public string? Subject { get; set; }

    [StringLength(1024)]
    public string Body { get; set; } = null!;

    //public DateTime? SentAt { get; set; }
}
