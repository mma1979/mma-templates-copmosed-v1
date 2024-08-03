namespace WebApplication1.Common.Extensions
{
    public static class GuidExtensions
    {
        public static string ExtractPin(this Guid guid)
        {
            var hash = guid.GetHashCode().ToString();
            return hash.Substring(hash.Length - 6, 6);
        }

        public static string ExtractPin(this Guid guid, int pinLength)
        {
            var hash = guid.GetHashCode().ToString();
            return hash.Substring(hash.Length - pinLength, pinLength);
        }

        public static bool VirifyPin(this Guid guid, string pin)
        {
            var hash = guid.GetHashCode().ToString();
            var computedPin= hash.Substring(hash.Length - 6, 6);
            return pin==computedPin;
        }
    }
}
