using System.Formats.Asn1;
using BankingServiceCLI.Models;

namespace BankingServiceCLI.Extensions;

//методы расширения 
public static class CurrencyExtensions
{
    //константы захардкоженные
    private const decimal EurToRubRate = 90m;
    private const decimal UsdToRubRate = 80m;
    
    //метод расширения ToRub для decimal
    public static decimal ToRub(this decimal amount, Currency from)
    {

        //возвращаем свитч
        return from switch
        {
            Currency.RUB => amount,
            Currency.EUR => amount * EurToRubRate,
            Currency.USD => amount * UsdToRubRate,
            _ => amount
        };
    }

    //метод расширения FromRub для decimal
    public static decimal FromRub(this decimal amount, Currency to)
    {
        return to switch
        {
            Currency.RUB => amount,
            Currency.EUR => amount / EurToRubRate,
            Currency.USD => amount / UsdToRubRate,
            _ => amount
        };
    }
}