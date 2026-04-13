using System.ComponentModel.DataAnnotations;

namespace PaintDotNET.Api.DTOs;

public record StartGameDTO(
    [Required] uint PlayerID
);
