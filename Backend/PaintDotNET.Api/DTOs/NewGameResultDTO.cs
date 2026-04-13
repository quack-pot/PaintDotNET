using System.ComponentModel.DataAnnotations;

namespace PaintDotNET.Api.DTOs;

public record NewGameResultDTO(
    [Required] uint GameID,
    [Required] uint PlayerID,

    [Required] float InitialX,
    [Required] float InitialY,

    [Required] bool IsRedTeam
);
