namespace SEG.Core.Enum
{
    public enum Digital
    {
        sim,
        nao
    }


    public static class DigitalExtensions
    {
        public static string GetString(this Digital me)
        {
            switch (me)
            {
                case Digital.sim:
                    return "S";
                case Digital.nao:
                default:
                    return "N"; 
            }
        }
    }

}
