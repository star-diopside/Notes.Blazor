﻿namespace Notes.Blazor.Data.Models;

public interface ITrackable
{
    DateTime? CreatedAt { get; set; }

    DateTime? UpdatedAt { get; set; }
}
