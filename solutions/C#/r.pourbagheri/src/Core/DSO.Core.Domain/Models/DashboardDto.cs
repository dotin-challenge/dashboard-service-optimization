namespace DSO.Core.Domain.Models;

public record DashboardDto
(
    DateTime GeneratedAt,          // زمان تولید گزارش
    int TotalSales,                // مجموع فروش
    int TotalPurchases,            // مجموع خرید
    int InventoryCount,            // موجودی انبار
    decimal Revenue,               // درآمد کل
    decimal Expenses,              // هزینه‌ها
    decimal Profit,                // سود خالص
    int ActiveUsers,               // تعداد کاربران فعال
    int NewCustomers,              // مشتریان جدید
    int SupportTicketsOpen,        // تیکت‌های پشتیبانی باز
    int SupportTicketsClosed       // تیکت‌های بسته شده
);

