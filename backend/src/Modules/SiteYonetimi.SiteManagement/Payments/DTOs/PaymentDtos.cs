using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.Payments.DTOs;

public record BulkCreatePaymentsDto(
    Guid? BuildingId,
    decimal Amount,
    DateTime DueDate,
    string? Description);

public record CreatePaymentDto(
    Guid UnitId,
    decimal Amount,
    DateTime DueDate,
    string? Description);

public record UpdatePaymentDto(
    decimal Amount,
    DateTime DueDate,
    string? Description);

public record UpdatePaymentStatusDto(
    PaymentStatus Status,
    DateTime? PaidDate);

public record PaymentSummaryDto(
    Guid Id,
    Guid SiteId,
    Guid UnitId,
    string? UnitDoorNumber,
    decimal Amount,
    DateTime DueDate,
    DateTime? PaidDate,
    PaymentStatus Status,
    string? Description,
    DateTime CreatedAt);

public record CreateCheckoutSessionDto(string SuccessUrl, string CancelUrl);
