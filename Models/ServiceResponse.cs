namespace dotnet_rpg.Models;

public class ServiceResponse<T>
{
    public T Data { get; set; } //RPG character
    public bool Success { get; set; } = true; //tell if everything went well
    public string Message { get; set; } = null; //message if error
}