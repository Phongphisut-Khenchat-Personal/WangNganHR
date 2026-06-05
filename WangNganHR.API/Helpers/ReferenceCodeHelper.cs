namespace WangNganHR.API.Helpers;

public static class ReferenceCodeHelper
{
    public static string Generate()
    {
        var number = Random.Shared.Next(1000, 9999);
        var letter = (char)Random.Shared.Next('A', 'Z' + 1);
        return $"WNG-{letter}{number}";
    }
}