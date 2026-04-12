using System.ComponentModel.DataAnnotations;

namespace PaintDotNET.Api.DTOs;

public record GameOverDTO(
    [Required][Range(0.0f, 1.0f)] float RedTeamCoverage,
    [Required][Range(0.0f, 1.0f)] float BlueTeamCoverage
);
