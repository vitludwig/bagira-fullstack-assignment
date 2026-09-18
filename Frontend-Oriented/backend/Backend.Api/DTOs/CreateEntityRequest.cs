using System.ComponentModel.DataAnnotations;
using Backend.Domain.Models;

namespace Backend.Api.DTOs;

public class CreateEntityRequest
{
    [Required(ErrorMessage = "EntityType is required.")]
    [EnumDataType(typeof(EntityType), ErrorMessage = "Invalid EntityType value.")]
    public EntityType? Type { get; set; }

    [Required(ErrorMessage = "TaskForce is required.")]
    [EnumDataType(typeof(TaskForce), ErrorMessage = "Invalid TaskForce value.")]
    public TaskForce? TaskForce { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(200, ErrorMessage = "Name must be 200 characters or fewer.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Latitude is required.")]
    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
    public double? Latitude { get; set; }

    [Required(ErrorMessage = "Longitude is required.")]
    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
    public double? Longitude { get; set; }
}
