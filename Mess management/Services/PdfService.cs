using MessManagement.Interfaces;
using MessManagement.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MessManagement.Services;

public class PdfService : IPdfService
{
    public PdfService()
    {
        // Set the license type for QuestPDF (Community license for open source/personal use)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateDailyReport(DailyReportViewModel report)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(c => ComposeHeader(c, $"Daily Report - {report.Date:dd MMM yyyy}"));
                
                page.Content().Column(column =>
                {
                    column.Spacing(20);

                    // Summary Stats
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Element(c => StatCard(c, "Total Members", report.TotalMembers.ToString(), "#10b981"));
                        row.ConstantItem(10);
                        row.RelativeItem().Element(c => StatCard(c, "Present", report.PresentCount.ToString(), "#3b82f6"));
                        row.ConstantItem(10);
                        row.RelativeItem().Element(c => StatCard(c, "Absent", report.AbsentCount.ToString(), "#ef4444"));
                    });

                    // Collection Stats
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Element(c => StatCard(c, "Total Collection", $"Rs.{report.TotalCollection:N2}", "#8b5cf6"));
                        row.ConstantItem(10);
                        row.RelativeItem().Element(c => StatCard(c, "Cash", $"Rs.{report.CashCollection:N2}", "#06b6d4"));
                        row.ConstantItem(10);
                        row.RelativeItem().Element(c => StatCard(c, "Online", $"Rs.{report.OnlineCollection:N2}", "#f59e0b"));
                    });

                    // Attendance Table
                    if (report.AttendanceDetails?.Any() == true)
                    {
                        column.Item().Text("Attendance Details").FontSize(14).Bold();
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Member Name").Bold();
                                header.Cell().Element(CellStyle).Text("Room").Bold();
                                header.Cell().Element(CellStyle).Text("Status").Bold();
                            });

                            foreach (var item in report.AttendanceDetails)
                            {
                                table.Cell().Element(CellStyle).Text(item.MemberName);
                                table.Cell().Element(CellStyle).Text(item.RoomNumber);
                                table.Cell().Element(CellStyle).Text(item.Status);
                            }
                        });
                    }
                });

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateWeeklyReport(WeeklyReportViewModel report)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(c => ComposeHeader(c, $"Weekly Report ({report.StartDate:dd MMM} - {report.EndDate:dd MMM yyyy})"));
                
                page.Content().Column(column =>
                {
                    column.Spacing(20);

                    // Summary Stats
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Element(c => StatCard(c, "Total Collection", $"Rs.{report.TotalCollection:N2}", "#10b981"));
                        row.ConstantItem(10);
                        row.RelativeItem().Element(c => StatCard(c, "Avg Daily Attendance", $"{report.AverageAttendance:F1}", "#3b82f6"));
                        row.ConstantItem(10);
                        row.RelativeItem().Element(c => StatCard(c, "Total Payments", report.TotalPayments.ToString(), "#8b5cf6"));
                    });

                    // Daily Breakdown Table
                    if (report.DailyBreakdown?.Any() == true)
                    {
                        column.Item().Text("Daily Breakdown").FontSize(14).Bold();
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Date").Bold();
                                header.Cell().Element(CellStyle).Text("Present").Bold();
                                header.Cell().Element(CellStyle).Text("Absent").Bold();
                                header.Cell().Element(CellStyle).Text("Collection").Bold();
                            });

                            foreach (var day in report.DailyBreakdown)
                            {
                                table.Cell().Element(CellStyle).Text(day.Date.ToString("ddd, dd MMM"));
                                table.Cell().Element(CellStyle).Text(day.PresentCount.ToString());
                                table.Cell().Element(CellStyle).Text(day.AbsentCount.ToString());
                                table.Cell().Element(CellStyle).Text($"Rs.{day.Collection:N2}");
                            }
                        });
                    }
                });

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateMonthlyReport(MonthlyReportViewModel report)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(c => ComposeHeader(c, $"Monthly Report - {report.Month:MMMM yyyy}"));
                
                page.Content().Column(column =>
                {
                    column.Spacing(20);

                    // Financial Summary
                    column.Item().Text("Financial Summary").FontSize(14).Bold();
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Element(c => StatCard(c, "Total Revenue", $"Rs.{report.TotalRevenue:N2}", "#10b981"));
                        row.ConstantItem(10);
                        row.RelativeItem().Element(c => StatCard(c, "Cash", $"Rs.{report.CashCollection:N2}", "#3b82f6"));
                        row.ConstantItem(10);
                        row.RelativeItem().Element(c => StatCard(c, "Online", $"Rs.{report.OnlineCollection:N2}", "#8b5cf6"));
                    });

                    // Attendance Summary
                    column.Item().Text("Attendance Summary").FontSize(14).Bold();
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Element(c => StatCard(c, "Total Members", report.TotalMembers.ToString(), "#f59e0b"));
                        row.ConstantItem(10);
                        row.RelativeItem().Element(c => StatCard(c, "Avg Attendance", $"{report.AverageAttendance:F1}%", "#06b6d4"));
                        row.ConstantItem(10);
                        row.RelativeItem().Element(c => StatCard(c, "Working Days", report.WorkingDays.ToString(), "#ec4899"));
                    });

                    // Member-wise Summary
                    if (report.MemberSummaries?.Any() == true)
                    {
                        column.Item().Text("Member-wise Summary").FontSize(14).Bold();
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Member").Bold();
                                header.Cell().Element(CellStyle).Text("Days Present").Bold();
                                header.Cell().Element(CellStyle).Text("Amount Paid").Bold();
                                header.Cell().Element(CellStyle).Text("Balance").Bold();
                            });

                            foreach (var member in report.MemberSummaries)
                            {
                                table.Cell().Element(CellStyle).Text(member.MemberName);
                                table.Cell().Element(CellStyle).Text(member.DaysPresent.ToString());
                                table.Cell().Element(CellStyle).Text($"Rs.{member.TotalPaid:N2}");
                                table.Cell().Element(CellStyle).Text($"Rs.{member.Balance:N2}");
                            }
                        });
                    }
                });

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateMemberStatement(string memberName, IEnumerable<PaymentStatementItem> payments, decimal totalAmount)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(c => ComposeHeader(c, $"Payment Statement - {memberName}"));
                
                page.Content().Column(column =>
                {
                    column.Spacing(20);

                    column.Item().Element(c => StatCard(c, "Total Amount", $"Rs.{totalAmount:N2}", "#10b981"));

                    column.Item().Text("Transaction History").FontSize(14).Bold();
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Date").Bold();
                            header.Cell().Element(CellStyle).Text("Description").Bold();
                            header.Cell().Element(CellStyle).Text("Amount").Bold();
                            header.Cell().Element(CellStyle).Text("Status").Bold();
                        });

                        foreach (var payment in payments)
                        {
                            table.Cell().Element(CellStyle).Text(payment.Date.ToString("dd MMM yyyy"));
                            table.Cell().Element(CellStyle).Text(payment.Description);
                            table.Cell().Element(CellStyle).Text($"Rs.{payment.Amount:N2}");
                            table.Cell().Element(CellStyle).Text(payment.Status);
                        }
                    });
                });

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateMemberMonthlyBill(MemberMonthlyBillViewModel bill)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(c => ComposeBillHeader(c, bill));
                
                page.Content().Column(column =>
                {
                    column.Spacing(15);

                    // Member Info Card
                    column.Item().Border(1).BorderColor("#e5e7eb").Background("#f0fdf4").Padding(15).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(bill.MemberName).FontSize(16).Bold().FontColor("#065f46");
                            col.Item().Text($"Room: {bill.RoomNumber}").FontSize(11);
                            if (!string.IsNullOrEmpty(bill.Email))
                                col.Item().Text($"Email: {bill.Email}").FontSize(9).FontColor("#6b7280");
                            if (!string.IsNullOrEmpty(bill.Phone))
                                col.Item().Text($"Phone: {bill.Phone}").FontSize(9).FontColor("#6b7280");
                        });
                        row.ConstantItem(120).AlignRight().Column(col =>
                        {
                            col.Item().Text($"Bill Period").FontSize(9).FontColor("#6b7280");
                            col.Item().Text($"{bill.MonthName} {bill.Year}").FontSize(14).Bold().FontColor("#065f46");
                        });
                    });

                    // Attendance Summary
                    column.Item().Text("📅 Attendance Summary").FontSize(12).Bold();
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Element(c => MiniStatCard(c, "Total Days", bill.TotalDays.ToString(), "#3b82f6"));
                        row.ConstantItem(10);
                        row.RelativeItem().Element(c => MiniStatCard(c, "Present", bill.PresentDays.ToString(), "#10b981"));
                        row.ConstantItem(10);
                        row.RelativeItem().Element(c => MiniStatCard(c, "Absent", bill.AbsentDays.ToString(), "#ef4444"));
                    });

                    // Charges Breakdown
                    column.Item().Text("💰 Charges Breakdown").FontSize(12).Bold();
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Background("#f3f4f6").Text("Description").Bold();
                            header.Cell().Element(CellStyle).Background("#f3f4f6").AlignRight().Text("Qty").Bold();
                            header.Cell().Element(CellStyle).Background("#f3f4f6").AlignRight().Text("Rate").Bold();
                            header.Cell().Element(CellStyle).Background("#f3f4f6").AlignRight().Text("Amount").Bold();
                        });

                        // Meal Charges
                        table.Cell().Element(CellStyle).Text("Meal Charges");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{bill.PresentDays} days");
                        table.Cell().Element(CellStyle).AlignRight().Text($"Rs.{bill.MealRate:N0}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"Rs.{bill.TotalMealCharges:N0}");

                        // Water Charges
                        if (bill.WaterCount > 0)
                        {
                            table.Cell().Element(CellStyle).Text("Water Bottles");
                            table.Cell().Element(CellStyle).AlignRight().Text($"{bill.WaterCount}");
                            table.Cell().Element(CellStyle).AlignRight().Text($"Rs.{bill.WaterRate:N0}");
                            table.Cell().Element(CellStyle).AlignRight().Text($"Rs.{bill.WaterCharges:N0}");
                        }

                        // Tea Charges
                        if (bill.TeaCount > 0)
                        {
                            table.Cell().Element(CellStyle).Text("Tea/Coffee");
                            table.Cell().Element(CellStyle).AlignRight().Text($"{bill.TeaCount}");
                            table.Cell().Element(CellStyle).AlignRight().Text($"Rs.{bill.TeaRate:N0}");
                            table.Cell().Element(CellStyle).AlignRight().Text($"Rs.{bill.TeaCharges:N0}");
                        }

                        // Grand Total Row
                        table.Cell().ColumnSpan(3).Element(CellStyle).Background("#dcfce7").Text("Grand Total").Bold();
                        table.Cell().Element(CellStyle).Background("#dcfce7").AlignRight().Text($"Rs.{bill.GrandTotal:N0}").Bold().FontColor("#065f46");
                    });

                    // Payment Summary
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Element(c => MiniStatCard(c, "Amount Paid", $"Rs.{bill.AmountPaid:N0}", "#10b981"));
                        row.ConstantItem(10);
                        row.RelativeItem().Element(c => MiniStatCard(c, "Balance Due", $"Rs.{bill.Balance:N0}", bill.Balance > 0 ? "#ef4444" : "#10b981"));
                    });

                    // Payment History
                    if (bill.Payments.Any())
                    {
                        column.Item().Text("📋 Payment History").FontSize(12).Bold();
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Background("#f3f4f6").Text("Date").Bold();
                                header.Cell().Element(CellStyle).Background("#f3f4f6").Text("Description").Bold();
                                header.Cell().Element(CellStyle).Background("#f3f4f6").AlignRight().Text("Amount").Bold();
                                header.Cell().Element(CellStyle).Background("#f3f4f6").Text("Status").Bold();
                            });

                            foreach (var payment in bill.Payments)
                            {
                                table.Cell().Element(CellStyle).Text(payment.Date.ToString("dd MMM"));
                                table.Cell().Element(CellStyle).Text(payment.Description);
                                table.Cell().Element(CellStyle).AlignRight().Text($"Rs.{payment.Amount:N0}");
                                table.Cell().Element(CellStyle).Text(payment.Status);
                            }
                        });
                    }

                    // Footer Note
                    column.Item().PaddingTop(10).Border(1).BorderColor("#fbbf24").Background("#fef3c7").Padding(10).Text(text =>
                    {
                        text.Span("Note: ").Bold();
                        text.Span("Please clear any outstanding balance by the 5th of next month. For queries, contact the mess manager.");
                    });
                });

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeBillHeader(IContainer container, MemberMonthlyBillViewModel bill)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("🍽️ Mess Management System").FontSize(18).Bold().FontColor("#10b981");
                    col.Item().Text("Monthly Bill / Statement").FontSize(14).SemiBold();
                });
                row.ConstantItem(120).AlignRight().Column(col =>
                {
                    col.Item().Text($"Bill #: {bill.Year}{bill.Month:D2}-{bill.MemberName.GetHashCode():X4}").FontSize(9);
                    col.Item().Text($"Date: {DateTime.Now:dd MMM yyyy}").FontSize(9);
                });
            });
            column.Item().PaddingTop(10).LineHorizontal(2).LineColor("#10b981");
        });
    }

    private void MiniStatCard(IContainer container, string label, string value, string color)
    {
        container.Border(1).BorderColor("#e5e7eb").Background("#ffffff").Padding(10).Column(column =>
        {
            column.Item().Text(label).FontSize(9).FontColor("#6b7280");
            column.Item().Text(value).FontSize(14).Bold().FontColor(color);
        });
    }

    private void ComposeHeader(IContainer container, string title)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("🍽️ Mess Management System").FontSize(20).Bold().FontColor("#10b981");
                    col.Item().Text(title).FontSize(16).SemiBold();
                });
                row.ConstantItem(100).Text($"Generated: {DateTime.Now:dd MMM yyyy}").FontSize(9).AlignRight();
            });
            column.Item().PaddingTop(10).LineHorizontal(1).LineColor("#e5e7eb");
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().LineHorizontal(1).LineColor("#e5e7eb");
            column.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Text("Mess Management System - Confidential").FontSize(9).FontColor("#9ca3af");
                row.RelativeItem().AlignRight().Text(x =>
                {
                    x.Span("Page ").FontSize(9).FontColor("#9ca3af");
                    x.CurrentPageNumber().FontSize(9).FontColor("#9ca3af");
                    x.Span(" of ").FontSize(9).FontColor("#9ca3af");
                    x.TotalPages().FontSize(9).FontColor("#9ca3af");
                });
            });
        });
    }

    private void StatCard(IContainer container, string label, string value, string color)
    {
        container.Border(1).BorderColor("#e5e7eb").Background("#f9fafb").Padding(15).Column(column =>
        {
            column.Item().Text(label).FontSize(10).FontColor("#6b7280");
            column.Item().Text(value).FontSize(18).Bold().FontColor(color);
        });
    }

    private IContainer CellStyle(IContainer container)
    {
        return container.BorderBottom(1).BorderColor("#e5e7eb").Padding(5);
    }
}
