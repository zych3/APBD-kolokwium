namespace Apbd.Kolos.DTOs;

public class GetCustomerDto
{
    public string FirstName { get; set; }
    public string LastName {get; set;}
    public string? PhoneNumber { get; set; }
    public List<GetPurchaseDto> Purchases { get; set; } = [];
}



public class GetPurchaseDto
{
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    public GetTicketDto Ticket { get; set; }
    public GetConcertDto Concert { get; set; }
}

public class GetTicketDto
{
    public string Serial { get; set; }
    public int SeatNumber { get; set; }
}

public class GetConcertDto
{
    public string Name { get; set; }
    public DateTime Date { get; set; }
}