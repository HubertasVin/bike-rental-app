namespace BikeRentalApp.Application.DTOs;

public class AuthDTO
{
    public AuthDTO(string token)
    {
        Token = token;
    }

    public string Token {get; set;}


}