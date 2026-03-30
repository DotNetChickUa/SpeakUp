//using AbstractApiExtensions;

//Console.WriteLine(await PhoneValidation.IsPhoneNumberValid("", "", CancellationToken.None));
await OfficeExtensions.OfficeCommands.WordToPdf("1.docx", "1.pdf");
OfficeExtensions.OfficeCommands.WordToPdf4("1.docx", "4.pdf");
OfficeExtensions.OfficeCommands.WordToPdf5("1.docx", "5.pdf");
