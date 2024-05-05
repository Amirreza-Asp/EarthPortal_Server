namespace Domain.Exceptions
{
    public class NotFoundException : Exception
    {

        public static void ThrowIfNull(dynamic? data)
        {
            if (data == null)
                throw new NotFoundException();
        }

    }
}
