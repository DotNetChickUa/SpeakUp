using AbstractApiExtensions;

Console.WriteLine(await PhoneValidation.IsPhoneNumberValid("", "", CancellationToken.None));
