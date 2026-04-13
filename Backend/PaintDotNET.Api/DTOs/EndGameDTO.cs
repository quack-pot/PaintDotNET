using System.ComponentModel.DataAnnotations;

namespace PaintDotNET.Api.DTOs;

public record EndGameDTO(
    [Required] uint PlayerID
);
