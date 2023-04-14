namespace Core.Enum
{
    public enum Whastapp
    {
        sim,
        nao
    }

    public static class WhastappExtensions
    {
        public static string GetString(this Whastapp me)
        {
            switch (me)
            {
                case Whastapp.sim:
                    return "S";
                case Whastapp.nao:
                default:
                    return "N";
            }
        }
    }
}
