using System.ComponentModel.DataAnnotations;

namespace PaintDotNET.Api.DTOs;

public record GameStartedDTO(
    [Required] uint GridWidth,
    [Required] uint GridHeight
);
