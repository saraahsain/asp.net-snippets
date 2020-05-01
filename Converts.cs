public class Converts
{
    public string ByteArrayToString(byte[] bytes)
    {
        return bytes != null ? System.Text.Encoding.UTF8.GetString(bytes) : null;
    }
    public string StringToByteArray(byte[] string)
    {
        return string != null ? Encoding.ASCII.GetBytes(string) : null;
    }
}