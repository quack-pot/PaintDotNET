using System.ComponentModel.DataAnnotations;

namespace PaintDotNET.Api.DTOs;

public record PlayerInputDTO(
    [Required] uint GameID,
    [Required] uint PlayerID,

    [Required] bool IsUpPressed,
    [Required] bool IsDownPressed,
    [Required] bool IsLeftPressed,
    [Required] bool IsRightPressed
);
