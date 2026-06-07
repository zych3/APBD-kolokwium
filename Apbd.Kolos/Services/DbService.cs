using Apbd.Kolos.Data;
using Apbd.Kolos.Data.Models;
using Apbd.Kolos.DTOs;
using Apbd.Kolos.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Apbd.Kolos.Services;

public class DbService(AppDbContext db) : IDbService
{
    public async Task<IEnumerable<GetCustomerDto>> GetAllAsync()
    {
        var dtos = await db.Customers
            .Select(c => new GetCustomerDto
            {
                FirstName = c.FirstName,
                LastName = c.LastName,
                PhoneNumber = c.PhoneNumber,
                Purchases = c.PurchasedTickets
                    .Select(pc => new GetPurchaseDto
                    {
                        Date = pc.PurchaseDate,
                        Price = pc.TicketConcert.Price,
                        Ticket = new GetTicketDto
                        {
                            Serial = pc.TicketConcert.Ticket.SerialNumber,
                            SeatNumber = pc.TicketConcert.Ticket.SeatNumber
                        },
                        Concert = new GetConcertDto
                        {
                            Name = pc.TicketConcert.Concert.Name,
                            Date = pc.TicketConcert.Concert.Date,
                        }
                    }).ToList()
            }).ToListAsync();
        return dtos;
    }

    public async Task<GetCustomerDto> GetByIdAsync(int id)
    {
        var customer = await db.Customers
            .Where(c => c.CustomerId == id)
            .Select(c => new GetCustomerDto
            {
                FirstName = c.FirstName,
                LastName = c.LastName,
                PhoneNumber = c.PhoneNumber,
                Purchases = c.PurchasedTickets
                    .Select(pc => new GetPurchaseDto
                    {
                        Date = pc.PurchaseDate,
                        Price = pc.TicketConcert.Price,
                        Ticket = new GetTicketDto
                        {
                            Serial = pc.TicketConcert.Ticket.SerialNumber,
                            SeatNumber = pc.TicketConcert.Ticket.SeatNumber
                        },
                        Concert = new GetConcertDto
                        {
                            Name = pc.TicketConcert.Concert.Name,
                            Date = pc.TicketConcert.Concert.Date,
                        }
                    }).ToList()
            }).FirstOrDefaultAsync();
        return customer ?? throw new NotFoundException($"No customer found for ID: {id}");
    }

    public async Task CreateAsync(PostCustomerDto dto)
    {
        if (dto.Purchases.GroupBy(p => p.ConcertName).Any(g => g.Count() > 5))
        {
            throw new TooManyTicketsException("Customer can buy max 5 tickets to one concert");
        }

        if (db.Customers.Any(c => c.CustomerId == dto.Customer.Id))
        {
            throw new AlreadyExistsException($"Customer with ID {dto.Customer.Id} already exists");
        }
        await using var transaction = await db.Database.BeginTransactionAsync();
        var customer = new Customer
        {
            FirstName = dto.Customer.FirstName,
            LastName = dto.Customer.LastName,
            PhoneNumber = dto.Customer.PhoneNumber,
        };
        foreach (var purchase in dto.Purchases)
        {
            var concert = await db.Concerts.FirstOrDefaultAsync(c => c.Name == purchase.ConcertName);
            if (concert is null)
            {
                await transaction.RollbackAsync();
                throw new NotFoundException($"No concert found for name: {purchase.ConcertName}");
            }

            var ticket = new Ticket
            {
                SeatNumber = purchase.SeatNumber,
                SerialNumber = Guid.NewGuid().ToString(),
            };

            var ticketConcert = new TicketConcert
            {
                Ticket = ticket,
                Concert = concert,
                Price = purchase.Price,
            };

            var purchasedTicket = new PurchasedTicket
            {
                Customer = customer,
                TicketConcert = ticketConcert,
                PurchaseDate = DateTime.UtcNow
            };
            customer.PurchasedTickets.Add(purchasedTicket);
        }
        await db.Customers.AddAsync(customer);
        await transaction.CommitAsync();
        await db.SaveChangesAsync();
    }
}