using System;

namespace ArguSzem.Services
{
    class NetworkException : Exception
    {
        public NetworkException( String message ) : base( message )
        {
        }

        public NetworkException( Exception innerException ) : base( "Exception occurred.", innerException )
        {
        }
    }
}
