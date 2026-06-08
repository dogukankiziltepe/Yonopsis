using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.Payments.DTOs;

public record PaymentItemDto(string Name, decimal Amount);

public record BulkCreatePaymentsDto(
    Guid? BuildingId,
    List<PaymentItemDto> Items,
    DateTime DueDate,
    string? Description);

public record CreatePaymentDto(
    Guid UnitId,
    List<PaymentItemDto> Items,
    DateTime DueDate,
    string? Description);

public record UpdatePaymentDto(
    List<PaymentItemDto> Items,
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
    List<PaymentItemDto> Items,
    DateTime DueDate,
    DateTime? PaidDate,
    PaymentStatus Status,
    string? Description,
    DateTime CreatedAt);

public record CreateCheckoutSessionDto(string SuccessUrl, string CancelUrl);
