namespace  TelegramBotPGFRC
{
    public class CreatinineCystatinCCKiD : IEstimations
    {
        public decimal Calculate(PatientsAntropometrics antropometrics)
        {
           
            decimal Q = antropometrics.Gender == Gender.Male ? 1.099m : 1;

             decimal DecimalDoubleConverting(decimal value, double power)
            {
                return Convert.ToDecimal(Math.Pow(Convert.ToDouble(value), power));
            }

            decimal CreatininePart = DecimalDoubleConverting(antropometrics.Height / 10 / antropometrics.SCreatinine, 0.516);
            decimal CystatinPart = DecimalDoubleConverting(1.8m / antropometrics.SCystatinC, 0.294);
            decimal BUNPart = DecimalDoubleConverting(30 / antropometrics.BUN, 0.169);

            decimal result = 39.1m * CreatininePart * CystatinPart * BUNPart * Q * ((antropometrics.Height / 10) / 1.4m) * 0.188m;
            return Math.Round(result, 2);

        }
    }
}
