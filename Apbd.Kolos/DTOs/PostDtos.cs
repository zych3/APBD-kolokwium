namespace Apbd.Kolos.DTOs;


public class PostCustomerDto
{
    public PostCusomterDetailsDto Customer { get; set; }
    public List<PostPurchaseDto> Purchases { get; set; }
}

public class PostCusomterDetailsDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? PhoneNumber { get; set; }
}


public class PostPurchaseDto
{
    public int SeatNumber { get; set; }
    public string ConcertName { get; set; }
    public decimal Price { get; set; }
}