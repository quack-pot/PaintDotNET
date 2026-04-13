using System.ComponentModel.DataAnnotations;
using PaintDotNET.Core.Meta;

namespace PaintDotNET.Api.DTOs;

public record GameStartedDTO(
    [Required] uint GridWidth,
    [Required] uint GridHeight,

    [Required] PlayerJoinData[] PlayerInitialValues
);
