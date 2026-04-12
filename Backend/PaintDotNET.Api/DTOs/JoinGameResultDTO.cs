using System.ComponentModel.DataAnnotations;

namespace PaintDotNET.Api.DTOs;

public record JoinGameResultDTO(
    [Required] uint PlayerID,

    [Required] float InitialX,
    [Required] float InitialY,

    [Required] bool IsRedTeam
);
