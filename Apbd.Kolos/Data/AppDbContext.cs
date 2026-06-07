using Apbd.Kolos.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Apbd.Kolos.Data;

public class AppDbContext(DbContextOptions opts) : DbContext(opts)
{
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Concert> Concerts { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<TicketConcert> TicketConcerts { get; set; }
    public DbSet<PurchasedTicket> PurchasedTickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Concert>(entity =>
        {
            entity.HasKey(e => e.ConcertId);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(e => e.Date)
                .IsRequired();
            entity.Property(e => e.AvailableTickets)
                .IsRequired();
            entity.ToTable("Concert");
            
            entity.HasMany(e => e.TicketConcerts)
                .WithOne(e => e.Concert)
                .HasForeignKey(e => e.ConcertId);

            entity.HasData([
                new Concert
                {
                    ConcertId = 1,
                    Name = "Concert 1",
                    AvailableTickets = 2137,
                    Date = new DateTime(2025, 06, 07, 09, 00, 00)
                },
                new Concert
                {
                    ConcertId = 14,
                    Name = "Concert 14",
                    AvailableTickets = 420,
                    Date = new DateTime(2025, 06, 10, 09, 00, 00)
                }
            ]);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId);
            entity.Property(e => e.SerialNumber)
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.SeatNumber)
                .IsRequired();

            entity.ToTable("Ticket");

            entity.HasMany(e => e.TicketConcerts)
                .WithOne(e => e.Ticket)
                .HasForeignKey(e => e.TicketId);

            entity.HasData([
                new Ticket
                {
                    TicketId = 1,
                    SerialNumber = "TK2034/S4531/12",
                    SeatNumber = 124,
                },
                new Ticket
                {
                    TicketId = 2,
                    SerialNumber = "TK2027/S4831/133",
                    SeatNumber = 330,
                }
            ]);
        });

        modelBuilder.Entity<TicketConcert>(entity =>
        {
            entity.HasKey(e => e.TicketConcertId);
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10,2)")
                .IsRequired();
            entity.ToTable("Ticket_Concert");

            entity.HasMany(e => e.PurchasedTickets)
                .WithOne(e => e.TicketConcert)
                .HasForeignKey(e => e.TicketConcertId);

            entity.HasData([
                new TicketConcert
                {
                    TicketConcertId = 1,
                    TicketId = 1,
                    ConcertId = 1,
                    Price = 33.4m
                },
                new TicketConcert
                {
                    TicketConcertId = 2,
                    TicketId = 2,
                    ConcertId = 14,
                    Price = 48.4m
                }
            ]);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(100)
                .IsRequired(false);

            entity.ToTable("Customer");

            entity.HasMany(e => e.PurchasedTickets)
                .WithOne(e => e.Customer)
                .HasForeignKey(e => e.CustomerId);

            entity.HasData([
                new Customer
                {
                    CustomerId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    PhoneNumber = null
                }
            ]);
        });

        modelBuilder.Entity<PurchasedTicket>(entity =>
        {
            entity.ToTable("Purchased_Ticket");
            entity.Property(e => e.PurchaseDate)
                .IsRequired();

            entity.HasKey(e => new { e.TicketConcertId, e.CustomerId });

            entity.HasData([
                new PurchasedTicket
                {

                    CustomerId = 1,
                    TicketConcertId = 1,
                    PurchaseDate = new DateTime(2025, 06, 03, 09, 00, 00)
                },
                new PurchasedTicket
                {
                    CustomerId = 1,
                    TicketConcertId = 2,
                    PurchaseDate = new DateTime(2025, 06, 03, 09, 00, 00)
                }
            ]);
        });
    }
}