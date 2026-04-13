using System.ComponentModel.DataAnnotations;

namespace PaintDotNET.Api.DTOs;

public record GameUpdateDTO(
    [Required][Range(0.0f, 3600.0f)] float GameTimeSecs
);
