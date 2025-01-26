namespace Application.Exceptions
{
    public class InvalidDataException : Exception
    {
        public InvalidDataException() : base("Dados inválidos") { }
    }
}
