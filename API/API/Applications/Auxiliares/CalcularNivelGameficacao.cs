namespace API.Applications.Auxiliares
{
    public class CalcularNivelGameficacao
    {
        public static int CalcularNivel(int experiencia)
        {
            if (experiencia >= 6000)
                return 10;

            if (experiencia >= 4200)
                return 9;

            if (experiencia >= 3000)
                return 8;

            if (experiencia >= 2100)
                return 7;

            if (experiencia >= 1400)
                return 6;

            if (experiencia >= 900)
                return 5;

            if (experiencia >= 500)
                return 4;

            if (experiencia >= 250)
                return 3;

            if (experiencia >= 100)
                return 2;

            if (experiencia > 0)
                return 1;

            return 0;
        }

        public static decimal LimitarNota(decimal valor)
        {
            return Math.Clamp(valor, 0m, 100m);
        }
    }
}
